using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Metadata.Edm;
using System.Diagnostics;
using System.Linq;
using VfpEntityFrameworkProvider.DbExpressions;
using VfpEntityFrameworkProvider.VfpOleDb;
using VfpEntityFrameworkProvider.Visitors.Gatherers;
using VfpEntityFrameworkProvider.Visitors.Rewriters;

namespace VfpEntityFrameworkProvider.Visitors {
    internal class DmlSqlFormatter : SqlFormatter {
        private bool _usePrefix;

        internal static string GenerateFunctionSql(System.Data.Common.CommandTrees.DbFunctionCommandTree xcommandTree, out CommandType commandType) {
            var function = xcommandTree.EdmFunction;
            var userCommandText = (string)function.MetadataProperties["CommandTextAttribute"].Value;
            var userFuncName = (string)function.MetadataProperties["StoreFunctionNameAttribute"].Value;

            if (string.IsNullOrEmpty(userCommandText)) {
                // build a quoted description of the function
                commandType = CommandType.StoredProcedure;

                // if the function store name is not explicitly given, it is assumed to be the metadata name
                var functionName = String.IsNullOrEmpty(userFuncName) ? function.Name : userFuncName;

                return functionName;
            }
            // if the user has specified the command text, pass it through verbatim and choose CommandType.Text
            commandType = CommandType.Text;

            return userCommandText;
        }

        internal static string GenerateUpdateSql(VfpProviderManifest vfpManifest, System.Data.Common.CommandTrees.DbUpdateCommandTree updateCommandTree, out List<DbParameter> parameters) {
            var commandTree = GetCommandTreeExpression<DbUpdateCommandTree>(vfpManifest, updateCommandTree);
            var formatter = new DmlSqlFormatter();

            formatter.Write("UPDATE x");
            formatter.WriteLine(Indentation.Same);

            // set c1 = ..., c2 = ..., ...
            var first = true;

            formatter.Write("SET ");

            foreach (var setClause in commandTree.SetClauses) {
                if (first) {
                    first = false;
                }
                else {
                    formatter.Write(", ");
                }

                formatter.Write("x.");
                setClause.Property.Accept(formatter);
                formatter.Write(" = ");
                setClause.Value.Accept(formatter);
            }

            if (first) {
                // If first is still true, it indicates there were no set
                // clauses. Introduce a fake set clause so that:
                // - we acquire the appropriate locks
                // - server-gen columns (e.g. timestamp) get recomputed
                //
                // We use the following pattern:
                //
                //  update Foo
                //  set @i = 0
                //  where ...
                var parameter = formatter.CreateParameter(default(Int32), DbType.Int32);

                formatter.Write(parameter.ParameterName);
                formatter.Write(" = 0");
            }

            formatter.WriteLine(Indentation.Same);

            formatter.Write("FROM ");
            commandTree.Target.Expression.Accept(formatter);
            formatter.Write(" AS x");
            formatter.WriteLine(Indentation.Same);

            // where c1 = ..., c2 = ...
            formatter.Write("WHERE ");

            formatter._usePrefix = true;
            commandTree.Predicate.Accept(formatter);
            formatter._usePrefix = false;

            formatter.WriteLine(Indentation.Same);

            // generate returning sql
            GenerateReturningSql(vfpManifest, commandTree, formatter, commandTree.Returning);

            parameters = GetParameters(commandTree);

            return formatter.ToString();
        }

        public static string GenerateInsertSql(VfpProviderManifest vfpManifest, System.Data.Common.CommandTrees.DbInsertCommandTree insertCommandTree, out List<DbParameter> parameters) {
            var commandTree = GetCommandTreeExpression<DbInsertCommandTree>(vfpManifest, insertCommandTree);
            var formatter = new DmlSqlFormatter();

            formatter.Write("INSERT INTO ");
            commandTree.Target.Expression.Accept(formatter);

            // (c1, c2, c3, ...)
            formatter.Write("(");

            var setClauses = commandTree.SetClauses.Where(x => x.Value.ExpressionKind != DbExpressionKind.Null).ToArray();

            var first = true;

            foreach (var setClause in setClauses) {
                if (first) {
                    first = false;
                }
                else {
                    formatter.Write(", ");
                }

                setClause.Property.Accept(formatter);
            }

            formatter.Write(")");
            formatter.WriteLine(Indentation.Same);

            // values c1, c2, ...
            first = true;
            formatter.Write("values (");

            foreach (var setClause in setClauses) {
                if (first) {
                    first = false;
                }
                else {
                    formatter.Write(", ");
                }

                setClause.Value.Accept(formatter);
                //translator.RegisterMemberValue(setClause.Property, setClause.Value);
            }

            formatter.Write(")");
            formatter.WriteLine(Indentation.Same);

            // generate returning sql
            GenerateReturningSql(vfpManifest, commandTree, formatter, commandTree.Returning);

            parameters = GetParameters(commandTree);

            return formatter.ToString();
        }

        /// <summary>
        /// Call this method to register a property value pair so the translator "remembers"
        /// the values for members of the row being modified. These values can then be used
        /// to form a predicate for server-generation (based on the key of the row)
        /// </summary>
        /// <param name="propertyExpression">DbExpression containing the column reference (property expression).</param>
        /// <param name="value">DbExpression containing the value of the column.</param>
        internal void RegisterMemberValue(DbExpression propertyExpression, DbExpression value) {
            if (memberValues == null) {
                return;
            }

            // register the value for this property
            Debug.Assert(propertyExpression.ExpressionKind == DbExpressionKind.Property, "DML predicates and setters must be of the form property = value");

            // get name of left property 
            var property = ((DbPropertyExpression)propertyExpression).Property;

            // don't track null values
            if (value.ExpressionKind == DbExpressionKind.Null) {
                return;
            }

            Debug.Assert(value.ExpressionKind == DbExpressionKind.Constant, "value must either constant or null");
            // retrieve the last parameter added (which describes the parameter)
            memberValues[property] = parameters[parameters.Count - 1];
        }

        private static void GenerateReturningSql(VfpProviderManifest vfpManifest, DbModificationCommandTree commandTree, DmlSqlFormatter formatter, DbExpression returning) {
            if (returning == null) {
                return;
            }

            formatter.WriteLine(Indentation.Same);
            formatter.Write(VfpCommand.SplitCommandsToken);
            formatter.WriteLine(Indentation.Same);

            // select
            formatter.Write("SELECT ");
            returning.Accept(formatter);
            formatter.WriteLine(Indentation.Same);

            // from
            formatter.Write("FROM ");
            commandTree.Target.Expression.Accept(formatter);
            formatter.WriteLine(Indentation.Same);

            // where
            formatter.Write("WHERE ");

            var table = ((DbScanExpression)commandTree.Target.Expression).Target;
            var identity = false;
            var first = true;

            foreach (var keyMember in table.ElementType.KeyMembers) {
                if (first) {
                    first = false;
                }
                else {
                    formatter.Write(" and ");
                }

                formatter.Write(keyMember);

                // retrieve member value sql. the translator remembers member values
                // as it constructs the DML statement (which precedes the "returning"
                // SQL)
                DbParameter value;
                if (formatter.memberValues.TryGetValue(keyMember, out value)) {
                    formatter.Write(" = ");
                    formatter.Write(value.ParameterName);
                }
                else {
                    if (identity) {
                        throw new NotSupportedException(string.Format("Server generated keys are only supported for identity columns. More than one key column is marked as server generated in table '{0}'.", table.Name));
                    }

                    formatter.Write(" = ");
                    formatter.Write(VfpCommand.ExecuteScalarBeginDelimiter);
                    formatter.Write("=");
                    formatter.Write(GetTableName(commandTree));
                    formatter.Write(".");
                    formatter.Write(keyMember.Name);
                    formatter.Write(VfpCommand.ExecuteScalarEndDelimiter);
                    formatter.Write(" ");

                    identity = true;
                }
            }
        }

        private static string GetTableName(DbModificationCommandTree modificationCommandTree) {
            var formatter = new DmlSqlFormatter();

            modificationCommandTree.Target.Expression.Accept(formatter);

            return formatter.ToString().Trim();
        }

        internal static string GenerateDeleteSql(VfpProviderManifest vfpManifest, System.Data.Common.CommandTrees.DbDeleteCommandTree deleteCommandTree, out List<DbParameter> parameters) {
            var formatter = new DmlSqlFormatter();
            var commandTree = GetCommandTreeExpression<DbDeleteCommandTree>(vfpManifest, deleteCommandTree);

            formatter.Write("DELETE FROM ");
            commandTree.Target.Expression.Accept(formatter);

            formatter.Write(" WHERE ");
            commandTree.Predicate.Accept(formatter);

            parameters = GetParameters(commandTree);

            return formatter.ToString();
        }

        private static List<DbParameter> GetParameters(DbExpression expression) {
            var parameterExpressions = DbParameterGatherer.Gather(expression);

            if (parameterExpressions.Any()) {
                var parameterHelper = new VfpParameterHelper();

                return parameterExpressions.Select(x => parameterHelper.CreateVfpParameter(x.Name, x.ResultType, ParameterMode.In, x.Value.Value)).Cast<DbParameter>().ToList();
            }

            return new List<DbParameter>();
        }

        public override DbExpression Visit(DbParameterExpression expression) {
            Write(expression.Name);

            return expression;
        }

        public override DbExpression Visit(DbPropertyExpression expression) {
            if (_usePrefix) {
                Write("x.");
            }

            Write(expression.Property.Name);

            return expression;
        }

        private static T GetCommandTreeExpression<T>(VfpProviderManifest vfpManifest, System.Data.Common.CommandTrees.DbCommandTree commandTree) where T : DbCommandTree {
            var visitor = new ExpressionConverterVisitor();
            var queryCommandTree = visitor.Visit(commandTree);

            return (T)ExpressionRewritter.Rewrite(vfpManifest, queryCommandTree);
        }
    }
}