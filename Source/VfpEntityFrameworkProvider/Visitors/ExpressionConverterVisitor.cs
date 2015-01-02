using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using VfpEntityFrameworkProvider.DbExpressions;

namespace VfpEntityFrameworkProvider.Visitors {
    // Converts System.Data.Common.CommandTrees.DbExpression to VfpEntityFrameworkProvider.DbExpressions for the purpose of rewriting the expression tree.
    internal class ExpressionConverterVisitor : System.Data.Common.CommandTrees.DbExpressionVisitor<DbExpression> {
        public DbExpression Visit(System.Data.Common.CommandTrees.DbCommandTree commandTree) {
            var queryCommandTree = commandTree as System.Data.Common.CommandTrees.DbQueryCommandTree;

            if (queryCommandTree != null) {
                return Visit(queryCommandTree);
            }

            var updateCommandTree = commandTree as System.Data.Common.CommandTrees.DbUpdateCommandTree;

            if (updateCommandTree != null) {
                return Visit(updateCommandTree);
            }

            var deleteCommandTree = commandTree as System.Data.Common.CommandTrees.DbDeleteCommandTree;

            if (deleteCommandTree != null) {
                return Visit(deleteCommandTree);
            }

            var insertCommandTree = commandTree as System.Data.Common.CommandTrees.DbInsertCommandTree;

            if (insertCommandTree != null) {
                return Visit(insertCommandTree);
            }

            var functionCommandTree = commandTree as System.Data.Common.CommandTrees.DbFunctionCommandTree;

            if (functionCommandTree != null) {
                return Visit(functionCommandTree);
            }

            throw new NotImplementedException(commandTree.GetType().FullName);
        }

        public DbExpression Visit(System.Data.Common.CommandTrees.DbFunctionCommandTree commandTree) {
            return new DbFunctionCommandTree(commandTree.EdmFunction, commandTree.ResultType, commandTree.Parameters);
        }

        public DbExpression Visit(System.Data.Common.CommandTrees.DbQueryCommandTree commandTree) {
            return new DbQueryCommandTree(commandTree.Query.Accept(this),
                                          commandTree.Parameters);
        }

        public DbExpression Visit(System.Data.Common.CommandTrees.DbDeleteCommandTree commandTree) {
            return new DbDeleteCommandTree(CreateDbExpressionBinding(commandTree.Target),
                                           commandTree.Predicate.Accept(this),
                                           commandTree.Parameters);
        }

        public DbExpression Visit(System.Data.Common.CommandTrees.DbUpdateCommandTree commandTree) {
            return new DbUpdateCommandTree(CreateDbExpressionBinding(commandTree.Target),
                                           CreateDbSetClauses(commandTree.SetClauses),
                                           commandTree.Predicate.Accept(this),
                                           commandTree.Returning == null ? null : commandTree.Returning.Accept(this),
                                           commandTree.Parameters);
        }

        public DbExpression Visit(System.Data.Common.CommandTrees.DbInsertCommandTree commandTree) {
            return new DbInsertCommandTree(CreateDbExpressionBinding(commandTree.Target),
                                           CreateDbSetClauses(commandTree.SetClauses),
                                           commandTree.Parameters,
                                           commandTree.Returning == null ? null : commandTree.Returning.Accept(this));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbAndExpression expression) {
            return new DbAndExpression(expression.ResultType,
                                       expression.Left.Accept(this),
                                       expression.Right.Accept(this));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbApplyExpression expression) {
            return new DbApplyExpression((DbExpressionKind)expression.ExpressionKind,
                                         expression.ResultType,
                                         CreateDbExpressionBinding(expression.Input),
                                         CreateDbExpressionBinding(expression.Apply));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbArithmeticExpression expression) {
            return new DbArithmeticExpression((DbExpressionKind)expression.ExpressionKind,
                                              expression.ResultType,
                                              CreateDbExpressionList(expression.Arguments));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbCaseExpression expression) {
            return new DbCaseExpression(expression.ResultType,
                                        CreateDbExpressionList(expression.When),
                                        CreateDbExpressionList(expression.Then),
                                        expression.Else.Accept(this));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbCastExpression expression) {
            return new DbCastExpression(expression.ResultType, expression.Argument.Accept(this));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbComparisonExpression expression) {
            return new DbComparisonExpression((DbExpressionKind)expression.ExpressionKind,
                                              expression.ResultType,
                                              expression.Left.Accept(this),
                                              expression.Right.Accept(this));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbConstantExpression expression) {
            return new DbConstantExpression(expression.ResultType, expression.Value);
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbCrossJoinExpression expression) {
            return new DbCrossJoinExpression(expression.ResultType, CreateDbExpressionBindings(expression.Inputs));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbDerefExpression expression) {
            return new DbDerefExpression(expression.ResultType, expression.Argument.Accept(this));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbDistinctExpression expression) {
            return new DbDistinctExpression(expression.ResultType, expression.Argument.Accept(this));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbElementExpression expression) {
            return new DbElementExpression(expression.ResultType, expression.Argument.Accept(this));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbEntityRefExpression expression) {
            return new DbEntityRefExpression(expression.ResultType, expression.Argument.Accept(this));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbExceptExpression expression) {
            return new DbExceptExpression(expression.ResultType, expression.Left.Accept(this), expression.Right.Accept(this));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbExpression expression) {
            switch (expression.ExpressionKind) {
                case System.Data.Common.CommandTrees.DbExpressionKind.And:
                    return Visit((System.Data.Common.CommandTrees.DbAndExpression)expression);
                //case System.Data.Common.CommandTrees.DbExpressionKind.Any:
                //    break;
                case System.Data.Common.CommandTrees.DbExpressionKind.Case:
                    return Visit((System.Data.Common.CommandTrees.DbCaseExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.Cast:
                    return Visit((System.Data.Common.CommandTrees.DbCastExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.Constant:
                    return Visit((System.Data.Common.CommandTrees.DbConstantExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.CrossJoin:
                    return Visit((System.Data.Common.CommandTrees.DbCrossJoinExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.Deref:
                    return Visit((System.Data.Common.CommandTrees.DbDerefExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.Distinct:
                    return Visit((System.Data.Common.CommandTrees.DbDistinctExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.Element:
                    return Visit((System.Data.Common.CommandTrees.DbElementExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.EntityRef:
                    return Visit((System.Data.Common.CommandTrees.DbEntityRefExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.Except:
                    return Visit((System.Data.Common.CommandTrees.DbExceptExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.Filter:
                    return Visit((System.Data.Common.CommandTrees.DbFilterExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.Function:
                    return Visit((System.Data.Common.CommandTrees.DbFunctionExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.GroupBy:
                    return Visit((System.Data.Common.CommandTrees.DbGroupByExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.Intersect:
                    return Visit((System.Data.Common.CommandTrees.DbIntersectExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.IsEmpty:
                    return Visit((System.Data.Common.CommandTrees.DbIsEmptyExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.IsNull:
                    return Visit((System.Data.Common.CommandTrees.DbIsNullExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.IsOf:
                case System.Data.Common.CommandTrees.DbExpressionKind.IsOfOnly:
                    return Visit((System.Data.Common.CommandTrees.DbIsOfExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.FullOuterJoin:
                case System.Data.Common.CommandTrees.DbExpressionKind.InnerJoin:
                case System.Data.Common.CommandTrees.DbExpressionKind.LeftOuterJoin:
                    return Visit((System.Data.Common.CommandTrees.DbJoinExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.Like:
                    return Visit((System.Data.Common.CommandTrees.DbLikeExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.Limit:
                    return Visit((System.Data.Common.CommandTrees.DbLimitExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.Divide:
                case System.Data.Common.CommandTrees.DbExpressionKind.Minus:
                case System.Data.Common.CommandTrees.DbExpressionKind.Modulo:
                case System.Data.Common.CommandTrees.DbExpressionKind.Multiply:
                case System.Data.Common.CommandTrees.DbExpressionKind.Plus:
                    return Visit((System.Data.Common.CommandTrees.DbArithmeticExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.Equals:
                case System.Data.Common.CommandTrees.DbExpressionKind.GreaterThan:
                case System.Data.Common.CommandTrees.DbExpressionKind.GreaterThanOrEquals:
                case System.Data.Common.CommandTrees.DbExpressionKind.LessThan:
                case System.Data.Common.CommandTrees.DbExpressionKind.LessThanOrEquals:
                case System.Data.Common.CommandTrees.DbExpressionKind.NotEquals:
                    return Visit((System.Data.Common.CommandTrees.DbComparisonExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.NewInstance:
                    return Visit((System.Data.Common.CommandTrees.DbNewInstanceExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.Not:
                    return Visit((System.Data.Common.CommandTrees.DbNotExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.Null:
                    return Visit((System.Data.Common.CommandTrees.DbNullExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.OfType:
                case System.Data.Common.CommandTrees.DbExpressionKind.OfTypeOnly:
                    return Visit((System.Data.Common.CommandTrees.DbOfTypeExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.Or:
                    return Visit((System.Data.Common.CommandTrees.DbOrExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.CrossApply:
                case System.Data.Common.CommandTrees.DbExpressionKind.OuterApply:
                    return Visit((System.Data.Common.CommandTrees.DbApplyExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.ParameterReference:
                    return Visit((System.Data.Common.CommandTrees.DbParameterReferenceExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.Project:
                    return Visit((System.Data.Common.CommandTrees.DbProjectExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.Property:
                    return Visit((System.Data.Common.CommandTrees.DbPropertyExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.Ref:
                    return Visit((System.Data.Common.CommandTrees.DbRefExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.RefKey:
                    return Visit((System.Data.Common.CommandTrees.DbRefKeyExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.RelationshipNavigation:
                    return Visit((System.Data.Common.CommandTrees.DbRelationshipNavigationExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.Scan:
                    return Visit((System.Data.Common.CommandTrees.DbScanExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.Skip:
                    return Visit((System.Data.Common.CommandTrees.DbSkipExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.Sort:
                    return Visit((System.Data.Common.CommandTrees.DbSortExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.Treat:
                    return Visit((System.Data.Common.CommandTrees.DbTreatExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.UnaryMinus:
                    return Visit((System.Data.Common.CommandTrees.DbUnaryExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.UnionAll:
                    return Visit((System.Data.Common.CommandTrees.DbUnionAllExpression)expression);
                case System.Data.Common.CommandTrees.DbExpressionKind.VariableReference:
                    return Visit((System.Data.Common.CommandTrees.DbVariableReferenceExpression)expression);
                default:
                    throw new NotImplementedException(expression.ExpressionKind.ToString());
            }
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbFilterExpression expression) {
            return new DbFilterExpression(expression.ResultType, CreateDbExpressionBinding(expression.Input), expression.Predicate.Accept(this));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbFunctionExpression expression) {
            return new DbFunctionExpression(expression.ResultType, expression.Function, CreateDbExpressionList(expression.Arguments));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbGroupByExpression expression) {
            return new DbGroupByExpression(expression.ResultType,
                                           CreateDbGroupExpressionBinding(expression.Input),
                                           CreateDbExpressionList(expression.Keys),
                                           CreateDbAggregates(expression.Aggregates));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbIntersectExpression expression) {
            return new DbIntersectExpression(expression.ResultType,
                                             expression.Left.Accept(this),
                                             expression.Right.Accept(this));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbIsEmptyExpression expression) {
            return new DbIsEmptyExpression(expression.ResultType, expression.Argument.Accept(this));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbIsNullExpression expression) {
            return new DbIsNullExpression(expression.ResultType, expression.Argument.Accept(this));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbIsOfExpression expression) {
            return new DbIsOfExpression((DbExpressionKind)expression.ExpressionKind,
                                        expression.ResultType,
                                        expression.Argument.Accept(this),
                                        expression.OfType);
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbJoinExpression expression) {
            return new DbJoinExpression((DbExpressionKind)expression.ExpressionKind,
                                        expression.ResultType,
                                        CreateDbExpressionBinding(expression.Left),
                                        CreateDbExpressionBinding(expression.Right),
                                        expression.JoinCondition.Accept(this));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbLikeExpression expression) {
            return new DbLikeExpression(expression.ResultType,
                                        expression.Argument.Accept(this),
                                        expression.Pattern.Accept(this),
                                        expression.Escape.Accept(this));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbLimitExpression expression) {
            return new DbLimitExpression(expression.ResultType,
                                         expression.Argument.Accept(this),
                                         expression.Limit.Accept(this),
                                         expression.WithTies);
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbNewInstanceExpression expression) {
            ReadOnlyCollection<DbRelatedEntityRef> readOnlyRelationships = null;

            dynamic list = expression.GetType()
                                     .GetProperty("RelatedEntityReferences", BindingFlags.NonPublic | BindingFlags.Instance)
                                     .GetValue(expression, null);

            if (list != null) {
                var relationships = new List<DbRelatedEntityRef>();

                foreach (dynamic item in list) {
                    relationships.Add(new DbRelatedEntityRef(item.SourceEnd, item.TargetEnd, item.TargetEntityRef));
                }

                readOnlyRelationships = new ReadOnlyCollection<DbRelatedEntityRef>(relationships);
            }

            return new DbNewInstanceExpression(expression.ResultType,
                                               CreateDbExpressionList(expression.Arguments),
                                               readOnlyRelationships);
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbNotExpression expression) {
            return new DbNotExpression(expression.ResultType, expression.Argument.Accept(this));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbNullExpression expression) {
            return new DbNullExpression(expression.ResultType);
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbOfTypeExpression expression) {
            return new DbOfTypeExpression((DbExpressionKind)expression.ExpressionKind,
                                          expression.ResultType,
                                          expression.Argument.Accept(this),
                                          expression.OfType);
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbOrExpression expression) {
            return new DbOrExpression(expression.ResultType,
                                      expression.Left.Accept(this),
                                      expression.Right.Accept(this));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbParameterReferenceExpression expression) {
            return new DbParameterReferenceExpression(expression.ResultType, expression.ParameterName);
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbProjectExpression expression) {
            return new DbProjectExpression(expression.ResultType,
                                           CreateDbExpressionBinding(expression.Input),
                                           expression.Projection.Accept(this));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbPropertyExpression expression) {
            return new DbPropertyExpression(expression.ResultType,
                                            expression.Property,
                                            expression.Instance.Accept(this));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbQuantifierExpression expression) {
            return new DbQuantifierExpression((DbExpressionKind)expression.ExpressionKind,
                                              expression.ResultType,
                                              CreateDbExpressionBinding(expression.Input),
                                              expression.Predicate.Accept(this));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbRefExpression expression) {
            return new DbRefExpression(expression.ResultType, expression.EntitySet, expression.Argument.Accept(this));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbRefKeyExpression expression) {
            return new DbRefKeyExpression(expression.ResultType, expression.Argument.Accept(this));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbRelationshipNavigationExpression expression) {
            return new DbRelationshipNavigationExpression(expression.ResultType,
                                                          expression.Relationship,
                                                          expression.NavigateFrom,
                                                          expression.NavigateTo,
                                                          expression.NavigationSource.Accept(this));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbScanExpression expression) {
            return new DbScanExpression(expression.ResultType, expression.Target);
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbSkipExpression expression) {
            return new DbSkipExpression(expression.ResultType,
                                        CreateDbExpressionBinding(expression.Input),
                                        CreateDbSortClauses(expression.SortOrder),
                                        expression.Count.Accept(this));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbSortExpression expression) {
            return new DbSortExpression(expression.ResultType,
                                        CreateDbExpressionBinding(expression.Input),
                                        CreateDbSortClauses(expression.SortOrder));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbTreatExpression expression) {
            return new DbTreatExpression(expression.ResultType, expression.Argument.Accept(this));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbUnionAllExpression expression) {
            return new DbUnionAllExpression(expression.ResultType, expression.Left.Accept(this), expression.Right.Accept(this));
        }

        public override DbExpression Visit(System.Data.Common.CommandTrees.DbVariableReferenceExpression expression) {
            return new DbVariableReferenceExpression(expression.ResultType, expression.VariableName);
        }

        private ReadOnlyCollection<DbExpressionBinding> CreateDbExpressionBindings(IEnumerable<System.Data.Common.CommandTrees.DbExpressionBinding> bindings) {
            var list = bindings.Select(CreateDbExpressionBinding).ToList();

            return new ReadOnlyCollection<DbExpressionBinding>(list);
        }

        private DbGroupExpressionBinding CreateDbGroupExpressionBinding(System.Data.Common.CommandTrees.DbGroupExpressionBinding binding) {
            return new DbGroupExpressionBinding(binding.Expression.Accept(this),
                                                (DbVariableReferenceExpression)binding.Variable.Accept(this),
                                                (DbVariableReferenceExpression)binding.GroupVariable.Accept(this));
        }

        private DbExpressionBinding CreateDbExpressionBinding(System.Data.Common.CommandTrees.DbExpressionBinding binding) {
            return new DbExpressionBinding(binding.Expression.Accept(this), (DbVariableReferenceExpression)binding.Variable.Accept(this));
        }

        private ReadOnlyCollection<DbAggregate> CreateDbAggregates(IEnumerable<System.Data.Common.CommandTrees.DbAggregate> aggregates) {
            var list = aggregates.Select(CreateDbAggregate).ToList();

            return new ReadOnlyCollection<DbAggregate>(list);
        }

        private DbAggregate CreateDbAggregate(System.Data.Common.CommandTrees.DbAggregate aggregate) {
            var functionAggregate = aggregate as System.Data.Common.CommandTrees.DbFunctionAggregate;

            if (functionAggregate != null) {
                return new DbFunctionAggregate(functionAggregate.ResultType,
                                               CreateDbExpressionList(functionAggregate.Arguments),
                                               functionAggregate.Function,
                                               functionAggregate.Distinct);
            }

            throw new NotImplementedException(aggregate.GetType().ToString());
        }

        private DbExpressionList CreateDbExpressionList(IList<System.Data.Common.CommandTrees.DbExpression> list) {
            var expressions = list.Select(argument => argument.Accept(this)).ToList();

            return new DbExpressionList(expressions);
        }

        private ReadOnlyCollection<DbSortClause> CreateDbSortClauses(IList<System.Data.Common.CommandTrees.DbSortClause> sortClause) {
            var list = sortClause.Select(item => new DbSortClause(item.Expression.Accept(this), item.Ascending, item.Collation)).ToList();

            return new ReadOnlyCollection<DbSortClause>(list);
        }

        private ReadOnlyCollection<DbSetClause> CreateDbSetClauses(IEnumerable<System.Data.Common.CommandTrees.DbModificationClause> setClauses) {
            return CreateDbSetClauses(setClauses.Cast<System.Data.Common.CommandTrees.DbSetClause>());
        }

        private ReadOnlyCollection<DbSetClause> CreateDbSetClauses(IEnumerable<System.Data.Common.CommandTrees.DbSetClause> setClauses) {
            var list = setClauses.Select(item => new DbSetClause(item.Property.Accept(this), item.Value.Accept(this))).ToList();

            return new ReadOnlyCollection<DbSetClause>(list);
        }
    }
}