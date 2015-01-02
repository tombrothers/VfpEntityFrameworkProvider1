using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Metadata.Edm;
using System.Data.OleDb;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using VfpEntityFrameworkProvider.DbExpressions;
using VfpEntityFrameworkProvider.Visitors.Rewriters;

namespace VfpEntityFrameworkProvider.Visitors {
    internal class SqlFormatter : DbExpressionVisitor {
        protected readonly StringBuilder sb = new StringBuilder();
        protected int indent = 2;
        protected int depth;
        protected readonly List<DbParameter> parameters = new List<DbParameter>();
        protected readonly Dictionary<EdmMember, DbParameter> memberValues = new Dictionary<EdmMember, DbParameter>();
        protected int parameterNameCount = 0;


        public static string Format(VfpProviderManifest vfpManifest, System.Data.Common.CommandTrees.DbQueryCommandTree dbQueryCommandTree, out List<DbParameter> parameters) {
            var visitor = new ExpressionConverterVisitor();
            var dbExpression = visitor.Visit(dbQueryCommandTree.Query);

            return Format(vfpManifest, dbExpression, out parameters);
        }

        public static string Format(VfpProviderManifest vfpManifest, DbExpression expression, out List<DbParameter> parameters) {
            expression = ExpressionRewritter.Rewrite(vfpManifest, expression);

            var formatter = new SqlFormatter();

            formatter.Visit(expression);
            parameters = formatter.parameters;

            return formatter.ToString();
        }

        public override string ToString() {
            return sb.ToString();
        }

        #region Visit

        public override DbExpression Visit(DbAndExpression expression) {
            return Visit((DbBinaryExpression)expression);
        }

        public override DbExpression Visit(DbApplyExpression expression) {
            throw new NotSupportedException("APPLY joins are not supported");
        }

        public override DbExpression Visit(DbArithmeticExpression expression) {
            switch (expression.ExpressionKind) {
                case DbExpressionKind.Divide:
                case DbExpressionKind.Minus:
                case DbExpressionKind.Modulo:
                case DbExpressionKind.Multiply:
                case DbExpressionKind.Plus:
                    return Visit(new DbBinaryExpression(expression.ExpressionKind,
                                                        expression.ResultType,
                                                        expression.Arguments[0],
                                                        expression.Arguments[1]));
                case DbExpressionKind.UnaryMinus:
                    Write(" -(");
                    expression.Arguments[0].Accept(this);
                    Write(")");
                    break;

                default:
                    throw new InvalidOperationException(expression.ExpressionKind.ToString());
            }

            return expression;
        }

        public DbExpression Visit(DbBinaryExpression expression) {
            expression.Left.Accept(this);
            Write(GetOperator(expression));
            expression.Right.Accept(this);

            return expression;
        }

        public override DbExpression Visit(DbCaseExpression expression) {
            Debug.Assert(expression.When.Count == expression.Then.Count);

            Write("ICASE(");
            for (var index = 0; index < expression.When.Count; index++) {
                if (index > 0) {
                    Write(",");
                }

                expression.When[index].Accept(this);
                Write(",");
                expression.Then[index].Accept(this);
            }
            if (expression.Else != null && !(expression.Else is DbNullExpression)) {
                Write(",");
                expression.Else.Accept(this);
            }

            Write(")");

            return expression;
        }

        public override DbExpression Visit(DbCastExpression expression) {
            Write(" CAST( ");
            expression.Argument.Accept(this);
            Write(" AS ");
            Write(GetSqlPrimitiveType(expression.ResultType));
            Write(")");

            return expression;
        }

        public override DbExpression Visit(DbComparisonExpression expression) {
            return Visit((DbBinaryExpression)expression);
        }

        public override DbExpression Visit(DbConstantExpression expression) {
            WriteValue(expression.Value);

            return expression;
        }

        public override DbExpression Visit(DbCrossJoinExpression expression) {
            throw new NotImplementedException();
        }

        public override DbExpression Visit(DbDerefExpression expression) {
            throw new NotImplementedException();
        }

        public override DbExpression Visit(DbDistinctExpression expression) {
            expression.Argument.Accept(this);

            return expression;
        }

        public override DbExpression Visit(DbElementExpression expression) {
            Write("(");
            expression.Argument.Accept(this);
            Write(")");

            return expression;
        }

        public override DbExpression Visit(DbEntityRefExpression expression) {
            throw new NotImplementedException();
        }

        public override DbExpression Visit(DbExceptExpression expression) {
            return Visit((DbBinaryExpression)expression);
        }

        public override DbExpression Visit(DbFilterExpression expression) {
            VisitDbExpressionBinding(expression.Input);
            WriteLine(Indentation.Same);
            Write("WHERE ");
            expression.Predicate.Accept(this);

            return expression;
        }

        public override DbExpression Visit(DbFunctionExpression expression) {
            Write(expression.Function.Name);
            Write("(");
            Write(expression.Arguments);
            Write(")");

            return expression;
        }

        public override DbExpression Visit(DbGroupByExpression expression) {
            throw new NotImplementedException();
        }

        public override DbExpression Visit(DbIntersectExpression expression) {
            throw new NotImplementedException();
        }

        public override DbExpression Visit(DbIsEmptyExpression expression) {
            Write(" NOT EXISTS (");
            expression.Argument.Accept(this);
            Write(")");

            return expression;
        }

        public override DbExpression Visit(DbIsNullExpression expression) {
            throw new NotImplementedException();
        }

        public override DbExpression Visit(DbIsOfExpression expression) {
            throw new NotImplementedException();
        }

        public override DbExpression Visit(DbJoinExpression expression) {
            throw new NotImplementedException();
        }

        public override DbExpression Visit(DbLikeExpression expression) {
            expression.Argument.Accept(this);
            Write(" LIKE ");
            expression.Pattern.Accept(this);

            if (expression.Escape.ExpressionKind != DbExpressionKind.Null) {
                Write(" ESCAPE ");
                expression.Escape.Accept(this);
            }

            return expression;
        }

        //public override DbExpression Visit(DbLimitExpression expression) {
        //    this.WriteMethodName(MethodInfo.GetCurrentMethod());

        //    throw new NotImplementedException();
        //}

        public override DbExpression Visit(DbNewInstanceExpression expression) {
            Write(expression.Arguments);

            return expression;
        }

        public override DbExpression Visit(DbNotExpression expression) {
            throw new NotImplementedException();
        }

        public override DbExpression Visit(DbNullExpression expression) {
            Write("CAST(NULL AS ");
            Write(GetSqlPrimitiveType(expression.ResultType));
            Write(")");

            return expression;
        }

        public override DbExpression Visit(DbOfTypeExpression expression) {
            throw new NotImplementedException();
        }

        public override DbExpression Visit(DbOrExpression expression) {
            return Visit((DbBinaryExpression)expression);
        }

        public override DbExpression Visit(DbParameterReferenceExpression expression) {
            Write("?" + expression.ParameterName);

            return expression;
        }

        public override DbExpression Visit(DbProjectExpression expression) {
            Write(" SELECT ");

            expression.Projection.Accept(this);
            VisitDbExpressionBinding(expression.Input);

            return expression;
        }

        public override DbExpression Visit(DbPropertyExpression expression) {
            expression.Instance.Accept(this);
            Write(".");
            Write(expression.Property.Name);

            return expression;
        }

        public override DbExpression Visit(DbQuantifierExpression e) {
            throw new NotImplementedException();
        }

        public override DbExpression Visit(DbRefExpression expression) {
            throw new NotImplementedException();
        }

        public override DbExpression Visit(DbRefKeyExpression expression) {
            throw new NotImplementedException();
        }

        public override DbExpression Visit(DbRelationshipNavigationExpression expression) {
            throw new NotImplementedException();
        }

        public override DbExpression Visit(DbScanExpression expression) {
            WriteLine(Indentation.Inner);
            //this.Write(" FROM ");

            var definingQuery = expression.Target.TryGetValueForMetadataProperty<string>("DefiningQuery");

            if (!string.IsNullOrEmpty(definingQuery)) {
                Write("(");
                Write(definingQuery);
                Write(")");
            }
            else {
                var tableName = expression.Target.TryGetValueForMetadataProperty<string>("Table");

                Write(string.IsNullOrEmpty(tableName) ? expression.Target.Name : tableName);
            }

            return expression;
        }

        public override DbExpression Visit(DbSkipExpression expression) {
            throw new NotImplementedException();
        }

        public override DbExpression Visit(DbSortExpression expression) {
            // This method should not be called.  Sorting is handled in VisitSortClauses.
            throw new NotImplementedException();
        }

        public override DbExpression Visit(DbTreatExpression expression) {
            throw new NotImplementedException();
        }

        public override DbExpression Visit(DbUnionAllExpression expression) {
            throw new NotImplementedException();
        }

        public override DbExpression Visit(DbVariableReferenceExpression expression) {
            Write(" ");
            Write(expression.VariableName);

            return expression;
        }

        #endregion

        protected DbExpression VisitJoin(DbJoinExpression join) {
            VisitSource(join.Left);
            WriteLine(Indentation.Same);

            switch (join.ExpressionKind) {
                case DbExpressionKind.CrossJoin:
                    Write(", ");
                    break;
                case DbExpressionKind.InnerJoin:
                    Write("INNER JOIN ");
                    break;
                case DbExpressionKind.LeftOuterJoin:
                    Write("LEFT OUTER JOIN ");
                    break;
                default:
                    throw new NotSupportedException(join.ExpressionKind.ToString());
            }

            VisitSource(join.Right);
            if (join.JoinCondition != null) {
                WriteLine(Indentation.Inner);
                Write("ON ");
                VisitPredicate(join.JoinCondition);
                Indent(Indentation.Outer);
            }

            return join;
        }

        protected void VisitSource(DbExpressionBinding source) {
            //bool saveIsNested = this.isNested;
            //this.isNested = true;
            //switch ((DbExpressionType)source.NodeType) {
            //    case DbExpressionType.Table:
            //        TableExpression table = (TableExpression)source;
            //        this.WriteTableName(table.Name);
            //        if (!this.HideTableAliases) {
            //            this.Write(" ");
            //            this.WriteAsAliasName(GetAliasName(table.Alias));
            //        }
            //        break;
            //    case DbExpressionType.Select:
            //        SelectExpression select = (SelectExpression)source;
            //        this.Write("(");
            //        this.WriteLine(Indentation.Inner);
            //        this.Visit(select);
            //        this.WriteLine(Indentation.Same);
            //        this.Write(") ");
            //        this.WriteAsAliasName(GetAliasName(select.Alias));
            //        this.Indent(Indentation.Outer);
            //        break;
            //    case DbExpressionType.Join:
            //        this.VisitJoin((JoinExpression)source);
            //        break;
            //    default:
            //        throw new InvalidOperationException("Select source is not valid type");
            //}
            //this.isNested = saveIsNested;

        }

        protected virtual string GetOperator(DbBinaryExpression expression) {
            switch (expression.ExpressionKind) {
                case DbExpressionKind.And:
                    return (expression.ResultType.IsBoolean() ? " AND " : "&");
                case DbExpressionKind.Or:
                    return (expression.ResultType.IsBoolean() ? " OR " : "|");
                case DbExpressionKind.Equals:
                    return "=";
                case DbExpressionKind.NotEquals:
                    return "<>";
                case DbExpressionKind.LessThan:
                    return "<";
                case DbExpressionKind.LessThanOrEquals:
                    return "<=";
                case DbExpressionKind.GreaterThan:
                    return ">";
                case DbExpressionKind.GreaterThanOrEquals:
                    return ">=";
                case DbExpressionKind.Plus:
                    return "+";
                case DbExpressionKind.Minus:
                    return "-";
                case DbExpressionKind.Multiply:
                    return "*";
                case DbExpressionKind.Divide:
                    return "/";
                case DbExpressionKind.Modulo:
                    return "%";
                default:
                    throw new NotSupportedException(expression.ExpressionKind.ToString());
            }
        }

        protected virtual DbExpression VisitPredicate(DbExpression expr) {
            Visit(expr);

            if (!IsPredicate(expr)) {
                Write(" <> 0");
            }

            return expr;
        }

        protected virtual bool IsPredicate(DbExpression expr) {
            switch (expr.ExpressionKind) {
                case DbExpressionKind.And:
                case DbExpressionKind.Or:
                    return ((DbBinaryExpression)expr).ResultType.IsBoolean();
                case DbExpressionKind.Not:
                    return ((DbUnaryExpression)expr).ResultType.IsBoolean();
                case DbExpressionKind.Equals:
                case DbExpressionKind.NotEquals:
                case DbExpressionKind.LessThan:
                case DbExpressionKind.LessThanOrEquals:
                case DbExpressionKind.GreaterThan:
                case DbExpressionKind.GreaterThanOrEquals:
                case DbExpressionKind.IsNull:
                    return true;
                default:
                    return false;
            }
        }

        #region Write

        protected void WriteParameterName(string name) {
            Write(name);
        }

        protected void WriteValue(DbConstantExpression c) {
            if (c != null) {
                WriteValue(c.Value);
            }
        }

        protected void WriteValue(object value) {
            if (value != null && value.GetType() == typeof(char[])) {
                var valueArray = (char[])value;

                for (int index = 0, total = valueArray.Length; index < total; index++) {
                    Write(",'");
                    Write(valueArray[index]);
                    Write("'");
                }
            }
            else {
                if (value == null) {
                    Write("NULL");
                }
                else if (value.GetType().IsEnum) {
                    Write(Convert.ChangeType(value, Enum.GetUnderlyingType(value.GetType())));
                }
                else {
                    var valueType = value.GetType();

                    if (valueType == typeof(Guid)) {
                        value = value.ToString();
                    }

                    if (valueType == typeof(byte[])) {
                        throw new NotSupportedException("Modifying Blob data is not supported at the moment.");
                        //value = BitConverter.ToString((byte[])value);
                        //this.Write("0h" + value);
                        //return;
                    }

                    switch (Type.GetTypeCode(value.GetType())) {
                        case TypeCode.Boolean:
                            Write(((bool)value) ? ".t." : ".f.");
                            break;
                        case TypeCode.String:
                            Write("'");
                            Write(value.ToString().Replace("'", "' + CHR(39) + '"));
                            Write("'");
                            break;
                        case TypeCode.Single:
                        case TypeCode.Double:
                        case TypeCode.Decimal:
                            var str = string.Format(CultureInfo.InvariantCulture, "{0}", value);

                            if (!str.Contains('.')) {
                                str += ".0";
                            }

                            Write(str);
                            break;
                        case TypeCode.DateTime:
                            Write(string.Format("CTOT('{0:yyyy-MM-dd}T{0:HH:mm:ss}')", value));
                            break;
                        case TypeCode.Object:
                            throw new NotSupportedException(string.Format("The constant for '{0}' is not supported", value));
                        default:
                            Write(value);
                            break;
                    }
                }
            }
        }

        protected void WriteLine(Indentation style) {
            sb.AppendLine();
            Indent(style);

            for (int index = 0, total = depth * indent; index < total; index++) {
                Write(" ");
            }
        }

        protected void Write(object value) {
            sb.Append(value);
        }

        public void Write(DbExpressionList list) {
            if (list == null) {
                return;
            }

            for (int index = 0, total = list.Count; index < total; index++) {
                if (index > 0) {
                    Write(",");
                }

                list[index].Accept(this);
            }
        }

        #endregion

        protected void Indent(Indentation style) {
            if (style == Indentation.Inner) {
                depth++;
            }
            else if (style == Indentation.Outer) {
                depth--;
                Debug.Assert(this.depth >= 0);
            }
        }

        protected enum Indentation {
            Same,
            Inner,
            Outer
        }

        /// <summary>
        /// Returns the sql primitive/native type name. 
        /// It will include size, precision or scale depending on type information present in the 
        /// type facets
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private string GetSqlPrimitiveType(TypeUsage type) {
            var primitiveType = type.GetEdmType<PrimitiveType>();

            string typeName = primitiveType.Name;
            bool isFixedLength = false;
            int maxLength = 0;
            string length = "max";
            bool preserveSeconds = true;
            byte decimalPrecision = 0;
            byte decimalScale = 0;

            switch (primitiveType.PrimitiveTypeKind) {
                case PrimitiveTypeKind.Binary:
                    //maxLength = type.GetFacetValueOrDefault<int>(FacetInfo.MaxLengthFacetName, FacetInfo.BinaryMaxMaxLength);
                    //if (maxLength == FacetInfo.BinaryMaxMaxLength) {
                    //    length = "max";
                    //}
                    //else {
                    //    length = maxLength.ToString(CultureInfo.InvariantCulture);
                    //}
                    //isFixedLength = type.GetFacetValueOrDefault<bool>(FacetInfo.FixedLengthFacetName, false);
                    //typeName = (isFixedLength ? "binary(" : "varbinary(") + length + ")";
                    typeName = "w";
                    break;
                case PrimitiveTypeKind.Guid:
                    typeName = "c(38)";
                    break;
                case PrimitiveTypeKind.String:
                    isFixedLength = type.GetFacetValueOrDefault<bool>(FacetInfo.FixedLengthFacetName, false);
                    maxLength = type.GetFacetValueOrDefault<int>(FacetInfo.MaxLengthFacetName, Int32.MinValue);

                    if (maxLength == Int32.MinValue) {
                        length = "254";
                    }
                    else {
                        length = maxLength.ToString(CultureInfo.InvariantCulture);
                    }

                    if (isFixedLength) {
                        typeName = "c(" + length + ")";
                    }
                    else {
                        typeName = "v(" + length + ")";
                    }

                    // Using the Unicode Facet to determine if the field is binary...
                    var binary = type.GetFacetValueOrDefault<bool>(FacetInfo.UnicodeFacetName, false);

                    if (binary) {
                        typeName += " NOCPTRANS";
                    }

                    break;

                case PrimitiveTypeKind.DateTime:
                    typeName = "t";
                    break;

                case PrimitiveTypeKind.Time:
                    typeName = "time";
                    break;

                //case PrimitiveTypeKind.DateTimeOffset:
                //    typeName = "datetimeoffset";
                //    break;

                case PrimitiveTypeKind.Single:
                    decimalPrecision = type.GetFacetValueOrDefault<byte>(FacetInfo.PrecisionFacetName, 18);
                    Debug.Assert(decimalPrecision > 0, "decimal precision must be greater than zero");
                    decimalScale = type.GetFacetValueOrDefault<byte>(FacetInfo.ScaleFacetName, 0);
                    Debug.Assert(decimalPrecision >= decimalScale, "decimalPrecision must be greater or equal to decimalScale");
                    Debug.Assert(decimalPrecision <= 38, "decimalPrecision must be less than or equal to 38");
                    typeName = "f(" + decimalPrecision + "," + decimalScale + ")";
                    break;
                case PrimitiveTypeKind.Double:
                    decimalPrecision = type.GetFacetValueOrDefault<byte>(FacetInfo.PrecisionFacetName, 18);
                    Debug.Assert(decimalPrecision > 0, "decimal precision must be greater than zero");
                    typeName = "b";
                    break;
                case PrimitiveTypeKind.Decimal:
                    decimalPrecision = type.GetFacetValueOrDefault<byte>(FacetInfo.PrecisionFacetName, 18);
                    Debug.Assert(decimalPrecision > 0, "decimal precision must be greater than zero");
                    decimalScale = type.GetFacetValueOrDefault<byte>(FacetInfo.ScaleFacetName, 0);
                    Debug.Assert(decimalPrecision >= decimalScale, "decimalPrecision must be greater or equal to decimalScale");
                    Debug.Assert(decimalPrecision <= 38, "decimalPrecision must be less than or equal to 38");
                    typeName = "n(" + decimalPrecision + "," + decimalScale + ")";
                    break;
                case PrimitiveTypeKind.Int64:
                    typeName = "n(20)";
                    break;
                case PrimitiveTypeKind.Byte:
                case PrimitiveTypeKind.Int16:
                case PrimitiveTypeKind.Int32:
                    typeName = "i";
                    break;

                case PrimitiveTypeKind.Boolean:
                    typeName = "l";
                    break;

                //case PrimitiveTypeKind.Guid:
                //    typeName = "uniqueidentifier";
                //    break;

                default:
                    throw new NotSupportedException("Unsupported EdmType: " + primitiveType.PrimitiveTypeKind);
            }

            return typeName;
        }

        // Creates a new parameter for a value in this expression translator
        internal OleDbParameter CreateParameter(object value, DbType dbType) {
            var parameterName = string.Concat("@p", this.parameterNameCount.ToString(CultureInfo.InvariantCulture));

            parameterNameCount++;

            var parameter = new OleDbParameter(parameterName, value);

            parameter.DbType = dbType;

            parameters.Add(parameter);

            return parameter;
        }
    }
}