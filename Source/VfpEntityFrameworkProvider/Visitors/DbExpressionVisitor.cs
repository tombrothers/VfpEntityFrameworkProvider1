using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using VfpEntityFrameworkProvider.DbExpressions;

namespace VfpEntityFrameworkProvider.Visitors {
    internal class DbExpressionVisitor : DbExpressionVisitorBase<DbExpression> {
        public override DbExpression Visit(DbCommandTree expression) {
            switch (expression.CommandTreeKind) {
                case DbCommandTreeKind.Delete:
                    return Visit((DbDeleteCommandTree)expression);
                case DbCommandTreeKind.Function:
                    return Visit((DbFunctionCommandTree)expression);
                case DbCommandTreeKind.Insert:
                    return Visit((DbInsertCommandTree)expression);
                case DbCommandTreeKind.Query:
                    return Visit((DbQueryCommandTree)expression);
                case DbCommandTreeKind.Update:
                    return Visit((DbUpdateCommandTree)expression);
                default:
                    throw new NotImplementedException(expression.CommandTreeKind.ToString());
            }
        }

        public override DbExpression Visit(DbLikeCExpression expression) {
            return new DbLikeCExpression(expression.ResultType,
                                         expression.Argument.Accept(this),
                                         expression.Pattern.Accept(this));
        }

        public override DbExpression Visit(DbFunctionCommandTree expression) {
            return new DbFunctionCommandTree(expression.EdmFunction, expression.ResultType, expression.Parameters);
        }

        public override DbExpression Visit(DbQueryCommandTree expression) {
            return new DbQueryCommandTree(expression.Query.Accept(this),
                                          expression.Parameters);
        }

        public override DbExpression Visit(DbDeleteCommandTree expression) {
            return new DbDeleteCommandTree(VisitDbExpressionBinding(expression.Target),
                                           expression.Predicate.Accept(this),
                                           expression.Parameters);
        }

        public override DbExpression Visit(DbUpdateCommandTree expression) {
            return new DbUpdateCommandTree(VisitDbExpressionBinding(expression.Target),
                                           CreateDbSetClauses(expression.SetClauses),
                                           expression.Predicate,
                                           expression.Returning == null ? null : expression.Returning.Accept(this),
                                           expression.Parameters);
        }

        public override DbExpression Visit(DbInsertCommandTree expression) {
            return new DbInsertCommandTree(VisitDbExpressionBinding(expression.Target),
                                           CreateDbSetClauses(expression.SetClauses),
                                           expression.Parameters,
                                           expression.Returning == null ? null : expression.Returning.Accept(this));
        }

        public override DbExpression Visit(DbParameterExpression expression) {
            return new DbParameterExpression(expression.ResultType, expression.Name, (DbConstantExpression)expression.Value.Accept(this));
        }

        public override DbExpression Visit(DbXmlToCursorExpression expression) {
            return new DbXmlToCursorExpression(expression.Property.Accept(this),
                                               expression.Parameter.Accept(this),
                                               expression.CursorName,
                                               expression.ItemType);
        }

        public override DbExpression Visit(DbInListExpression expression) {
            return new DbInListExpression(expression.Property.Accept(this),
                                          expression.Values.Accept(this));
        }

        public override DbExpression Visit(DbArrayExpression expression) {
            return new DbArrayExpression(expression.Values);
        }

        public override DbExpression Visit(DbAndExpression expression) {
            return new DbAndExpression(expression.ResultType,
                                       expression.Left.Accept(this),
                                       expression.Right.Accept(this));
        }

        public override DbExpression Visit(DbApplyExpression expression) {
            return new DbApplyExpression(expression.ExpressionKind,
                                         expression.ResultType,
                                         VisitDbExpressionBinding(expression.Input),
                                         VisitDbExpressionBinding(expression.Apply));
        }

        public override DbExpression Visit(DbArithmeticExpression expression) {
            return new DbArithmeticExpression(expression.ExpressionKind,
                                              expression.ResultType,
                                              VisitDbExpressionList(expression.Arguments));

        }

        public override DbExpression Visit(DbCaseExpression expression) {
            return new DbCaseExpression(expression.ResultType,
                                        VisitDbExpressionList(expression.When),
                                        VisitDbExpressionList(expression.Then),
                                        expression.Else.Accept(this));
        }

        public override DbExpression Visit(DbCastExpression expression) {
            return new DbCastExpression(expression.ResultType, expression.Argument.Accept(this));
        }

        public override DbExpression Visit(DbComparisonExpression expression) {
            return new DbComparisonExpression(expression.ExpressionKind,
                                                expression.ResultType,
                                                expression.Left.Accept(this),
                                                expression.Right.Accept(this));
        }

        public override DbExpression Visit(DbConstantExpression expression) {
            return expression;
        }

        public override DbExpression Visit(DbCrossJoinExpression expression) {
            return new DbCrossJoinExpression(expression.ResultType, CreateDbExpressionBindings(expression.Inputs));
        }

        public override DbExpression Visit(DbDerefExpression expression) {
            return new DbDerefExpression(expression.ResultType, expression.Argument.Accept(this));
        }

        public override DbExpression Visit(DbDistinctExpression expression) {
            return new DbDistinctExpression(expression.ResultType, expression.Argument.Accept(this));
        }

        public override DbExpression Visit(DbElementExpression expression) {
            return new DbElementExpression(expression.ResultType, expression.Argument.Accept(this));
        }

        public override DbExpression Visit(DbEntityRefExpression expression) {
            return new DbEntityRefExpression(expression.ResultType, expression.Argument.Accept(this));
        }

        public override DbExpression Visit(DbExceptExpression expression) {
            return new DbExceptExpression(expression.ResultType, expression.Left.Accept(this), expression.Right.Accept(this));
        }

        public override DbExpression Visit(DbExpression expression) {
            if (expression == null) {
                return null;
            }

            switch (expression.ExpressionKind) {
                case DbExpressionKind.Parameter:
                    return Visit((DbParameterExpression)expression);
                case DbExpressionKind.Array:
                    return Visit((DbArrayExpression)expression);
                case DbExpressionKind.InList:
                    return Visit((DbInListExpression)expression);
                case DbExpressionKind.And:
                    return Visit((DbAndExpression)expression);
                //case DbExpressionKind.Any:
                //    break;
                case DbExpressionKind.Case:
                    return Visit((DbCaseExpression)expression);
                case DbExpressionKind.Cast:
                    return Visit((DbCastExpression)expression);
                case DbExpressionKind.Constant:
                    return Visit((DbConstantExpression)expression);
                case DbExpressionKind.CrossJoin:
                    return Visit((DbCrossJoinExpression)expression);
                case DbExpressionKind.Deref:
                    return Visit((DbDerefExpression)expression);
                case DbExpressionKind.Distinct:
                    return Visit((DbDistinctExpression)expression);
                case DbExpressionKind.Element:
                    return Visit((DbElementExpression)expression);
                case DbExpressionKind.EntityRef:
                    return Visit((DbEntityRefExpression)expression);
                case DbExpressionKind.Except:
                    return Visit((DbExceptExpression)expression);
                case DbExpressionKind.Filter:
                    return Visit((DbFilterExpression)expression);
                case DbExpressionKind.Function:
                    return Visit((DbFunctionExpression)expression);
                case DbExpressionKind.GroupBy:
                    return Visit((DbGroupByExpression)expression);
                case DbExpressionKind.Intersect:
                    return Visit((DbIntersectExpression)expression);
                case DbExpressionKind.IsEmpty:
                    return Visit((DbIsEmptyExpression)expression);
                case DbExpressionKind.IsNull:
                    return Visit((DbIsNullExpression)expression);
                case DbExpressionKind.IsOf:
                case DbExpressionKind.IsOfOnly:
                    return Visit((DbIsOfExpression)expression);
                case DbExpressionKind.FullOuterJoin:
                case DbExpressionKind.InnerJoin:
                case DbExpressionKind.LeftOuterJoin:
                    return Visit((DbJoinExpression)expression);
                case DbExpressionKind.Like:
                    return Visit((DbLikeExpression)expression);
                case DbExpressionKind.Limit:
                    return Visit((DbLimitExpression)expression);
                case DbExpressionKind.Divide:
                case DbExpressionKind.Minus:
                case DbExpressionKind.Modulo:
                case DbExpressionKind.Multiply:
                case DbExpressionKind.Plus:
                    return Visit((DbArithmeticExpression)expression);
                case DbExpressionKind.Equals:
                case DbExpressionKind.GreaterThan:
                case DbExpressionKind.GreaterThanOrEquals:
                case DbExpressionKind.LessThan:
                case DbExpressionKind.LessThanOrEquals:
                case DbExpressionKind.NotEquals:
                    return Visit((DbComparisonExpression)expression);
                case DbExpressionKind.NewInstance:
                    return Visit((DbNewInstanceExpression)expression);
                case DbExpressionKind.Not:
                    return Visit((DbNotExpression)expression);
                case DbExpressionKind.Null:
                    return Visit((DbNullExpression)expression);
                case DbExpressionKind.OfType:
                case DbExpressionKind.OfTypeOnly:
                    return Visit((DbOfTypeExpression)expression);
                case DbExpressionKind.Or:
                    return Visit((DbOrExpression)expression);
                case DbExpressionKind.CrossApply:
                case DbExpressionKind.OuterApply:
                    return Visit((DbApplyExpression)expression);
                case DbExpressionKind.ParameterReference:
                    return Visit((DbParameterReferenceExpression)expression);
                case DbExpressionKind.Project:
                    return Visit((DbProjectExpression)expression);
                case DbExpressionKind.Property:
                    return Visit((DbPropertyExpression)expression);
                case DbExpressionKind.Ref:
                    return Visit((DbRefExpression)expression);
                case DbExpressionKind.RefKey:
                    return Visit((DbRefKeyExpression)expression);
                case DbExpressionKind.RelationshipNavigation:
                    return Visit((DbRelationshipNavigationExpression)expression);
                case DbExpressionKind.Scan:
                    return Visit((DbScanExpression)expression);
                case DbExpressionKind.Skip:
                    return Visit((DbSkipExpression)expression);
                case DbExpressionKind.Sort:
                    return Visit((DbSortExpression)expression);
                case DbExpressionKind.Treat:
                    return Visit((DbTreatExpression)expression);
                case DbExpressionKind.UnaryMinus:
                    return Visit((DbUnaryExpression)expression);
                case DbExpressionKind.UnionAll:
                    return Visit((DbUnionAllExpression)expression);
                case DbExpressionKind.VariableReference:
                    return Visit((DbVariableReferenceExpression)expression);
                case DbExpressionKind.CommandTree:
                    return Visit((DbCommandTree)expression);

                case DbExpressionKind.XmlToCursor:
                    return Visit((DbXmlToCursorExpression)expression);
                case DbExpressionKind.XmlToCursorScan:
                    return Visit((DbXmlToCursorScanExpression)expression);
                case DbExpressionKind.XmlToCursorProperty:
                    return Visit((DbXmlToCursorPropertyExpression)expression);
                default:
                    throw new NotImplementedException(expression.ExpressionKind.ToString());
            }
        }

        public override DbExpression Visit(DbXmlToCursorPropertyExpression expression) {
            return new DbXmlToCursorPropertyExpression(expression.ResultType, expression.Instance.Accept(this));
        }

        public override DbExpression Visit(DbXmlToCursorScanExpression expression) {
            return new DbXmlToCursorScanExpression(expression.Parameter.Accept(this), expression.CursorName);
        }

        public override DbExpression Visit(DbFilterExpression expression) {
            return new DbFilterExpression(expression.ResultType, VisitDbExpressionBinding(expression.Input), expression.Predicate.Accept(this));
        }

        public override DbExpression Visit(DbFunctionExpression expression) {
            return new DbFunctionExpression(expression.ResultType, expression.Function, VisitDbExpressionList(expression.Arguments));
        }

        public override DbExpression Visit(DbGroupByExpression expression) {
            return new DbGroupByExpression(expression.ResultType,
                                           CreateDbGroupExpressionBinding(expression.Input),
                                           VisitDbExpressionList(expression.Keys),
                                           CreateDbAggregates(expression.Aggregates));
        }

        public override DbExpression Visit(DbIntersectExpression expression) {
            return new DbIntersectExpression(expression.ResultType,
                                             expression.Left.Accept(this),
                                             expression.Right.Accept(this));
        }

        public override DbExpression Visit(DbIsEmptyExpression expression) {
            return new DbIsEmptyExpression(expression.ResultType, expression.Argument.Accept(this));
        }

        public override DbExpression Visit(DbIsNullExpression expression) {
            return new DbIsNullExpression(expression.ResultType, expression.Argument.Accept(this));
        }

        public override DbExpression Visit(DbIsOfExpression expression) {
            return new DbIsOfExpression(expression.ExpressionKind,
                                        expression.ResultType,
                                        expression.Argument.Accept(this),
                                        expression.OfType);
        }

        public override DbExpression Visit(DbJoinExpression expression) {
            return new DbJoinExpression(expression.ExpressionKind,
                                        expression.ResultType,
                                        VisitDbExpressionBinding(expression.Left),
                                        VisitDbExpressionBinding(expression.Right),
                                        expression.JoinCondition.Accept(this));
        }

        public override DbExpression Visit(DbLikeExpression expression) {
            return new DbLikeExpression(expression.ResultType,
                                        expression.Argument.Accept(this),
                                        expression.Pattern.Accept(this),
                                        expression.Escape.Accept(this));
        }

        public override DbExpression Visit(DbLimitExpression expression) {
            return new DbLimitExpression(expression.ResultType,
                                         expression.Argument.Accept(this),
                                         expression.Limit.Accept(this),
                                         expression.WithTies);
        }

        public override DbExpression Visit(DbNewInstanceExpression expression) {
            return new DbNewInstanceExpression(expression.ResultType,
                                               VisitDbExpressionList(expression.Arguments),
                                               CreateDbRelatedEntityRefList(expression.Relationships));
        }

        public override DbExpression Visit(DbNotExpression expression) {
            return new DbNotExpression(expression.ResultType, expression.Argument.Accept(this));
        }

        public override DbExpression Visit(DbNullExpression expression) {
            return expression;
        }

        public override DbExpression Visit(DbOfTypeExpression expression) {
            return new DbOfTypeExpression(expression.ExpressionKind,
                                            expression.ResultType,
                                            expression.Argument.Accept(this),
                                            expression.OfType);
        }

        public override DbExpression Visit(DbOrExpression expression) {
            return new DbOrExpression(expression.ResultType,
                                      expression.Left.Accept(this),
                                      expression.Right.Accept(this));
        }

        public override DbExpression Visit(DbParameterReferenceExpression expression) {
            return new DbParameterReferenceExpression(expression.ResultType, expression.ParameterName);
        }

        public override DbExpression Visit(DbProjectExpression expression) {
            var projection = expression.Projection.Accept(this);
            var input = VisitDbExpressionBinding(expression.Input);

            return new DbProjectExpression(expression.ResultType, input, projection);
        }

        public override DbExpression Visit(DbPropertyExpression expression) {
            return new DbPropertyExpression(expression.ResultType,
                                            expression.Property,
                                            expression.Instance.Accept(this));
        }

        public override DbExpression Visit(DbQuantifierExpression expression) {
            return new DbQuantifierExpression(expression.ExpressionKind,
                                              expression.ResultType,
                                              VisitDbExpressionBinding(expression.Input),
                                              expression.Predicate.Accept(this));
        }

        public override DbExpression Visit(DbRefExpression expression) {
            return new DbRefExpression(expression.ResultType, expression.EntitySet, expression.Argument.Accept(this));
        }

        public override DbExpression Visit(DbRefKeyExpression expression) {
            return new DbRefKeyExpression(expression.ResultType, expression.Argument.Accept(this));
        }

        public override DbExpression Visit(DbRelationshipNavigationExpression expression) {
            return new DbRelationshipNavigationExpression(expression.ResultType,
                                                          expression.Relationship,
                                                          expression.NavigateFrom,
                                                          expression.NavigateTo,
                                                          expression.NavigationSource.Accept(this));
        }

        public override DbExpression Visit(DbScanExpression expression) {
            return expression;
        }

        public override DbExpression Visit(DbSkipExpression expression) {
            return new DbSkipExpression(expression.ResultType,
                                        VisitDbExpressionBinding(expression.Input),
                                        CreateDbSortClauses(expression.SortOrder),
                                        expression.Count.Accept(this));
        }

        public override DbExpression Visit(DbSortExpression expression) {
            return new DbSortExpression(expression.ResultType,
                                        VisitDbExpressionBinding(expression.Input),
                                        CreateDbSortClauses(expression.SortOrder));
        }

        public override DbExpression Visit(DbTreatExpression expression) {
            return new DbTreatExpression(expression.ResultType, expression.Argument.Accept(this));
        }

        public override DbExpression Visit(DbUnionAllExpression expression) {
            return new DbUnionAllExpression(expression.ResultType, expression.Left.Accept(this), expression.Right.Accept(this));
        }

        public override DbExpression Visit(DbVariableReferenceExpression expression) {
            return new DbVariableReferenceExpression(expression.ResultType, expression.VariableName);
        }

        protected ReadOnlyCollection<DbSortClause> VisitNullHandler(ReadOnlyCollection<DbSortClause> sortClauses) {
            return sortClauses == null ? null : VisitSortClauses(sortClauses);
        }

        protected virtual ReadOnlyCollection<DbSortClause> VisitSortClauses(ReadOnlyCollection<DbSortClause> list) {
            var sortClauses = new List<DbSortClause>();

            for (int index = 0, total = list.Count; index < total; index++) {
                var item = list[index];

                item = new DbSortClause(item.Expression.Accept(this), item.Ascending, item.Collation);

                sortClauses.Add(item);
            }

            return sortClauses.AsReadOnly();
        }

        protected DbVariableReferenceExpression VisitNullHandler(DbVariableReferenceExpression expression) {
            return expression;
        }

        protected DbExpressionList VisitNullHandler(DbExpressionList expression) {
            return expression == null ? null : VisitDbExpressionList(expression);
        }

        protected DbExpression VisitNullHandler(DbExpression expression) {
            return expression == null ? null : expression.Accept(this);
        }

        protected ReadOnlyCollection<DbExpressionBinding> CreateDbExpressionBindings(IList<DbExpressionBinding> bindings) {
            var list = bindings.Select(VisitDbExpressionBinding).ToList();

            return list.AsReadOnly();
        }

        protected DbGroupExpressionBinding CreateDbGroupExpressionBinding(DbGroupExpressionBinding binding) {
            return new DbGroupExpressionBinding(binding.Expression.Accept(this),
                                                (DbVariableReferenceExpression)binding.Variable.Accept(this),
                                                (DbVariableReferenceExpression)binding.GroupVariable.Accept(this));
        }

        protected virtual DbExpressionBinding VisitDbExpressionBinding(DbExpressionBinding binding) {
            return new DbExpressionBinding(binding.Expression.Accept(this), (DbVariableReferenceExpression)binding.Variable.Accept(this));
        }

        protected virtual ReadOnlyCollection<DbAggregate> CreateDbAggregates(IList<DbAggregate> aggregates) {
            var list = aggregates.Select(CreateDbAggregate).ToList();

            return new ReadOnlyCollection<DbAggregate>(list);
        }

        protected DbAggregate CreateDbAggregate(DbAggregate aggregate) {
            var functionAggregate = aggregate as DbFunctionAggregate;

            if (functionAggregate != null) {
                return CreateDbFunctionAggregate(functionAggregate);
            }

            throw new NotImplementedException(aggregate.GetType().ToString());
        }

        protected virtual DbFunctionAggregate CreateDbFunctionAggregate(DbFunctionAggregate functionAggregate) {
            return new DbFunctionAggregate(functionAggregate.ResultType,
                                           VisitDbExpressionList(functionAggregate.Arguments),
                                           functionAggregate.Function,
                                           functionAggregate.Distinct);
        }

        protected virtual DbExpressionList VisitDbExpressionList(IList<DbExpression> list) {
            var expressions = new List<DbExpression>();

            for (int index = 0, total = list.Count; index < total; index++) {
                var item = list[index].Accept(this);

                expressions.Add(item);
            }

            return new DbExpressionList(expressions);
        }

        protected virtual ReadOnlyCollection<DbSortClause> CreateDbSortClauses(IList<DbSortClause> sortClause) {
            var list = sortClause.Select(item => new DbSortClause(item.Expression.Accept(this), item.Ascending, item.Collation)).ToList();

            return new ReadOnlyCollection<DbSortClause>(list);
        }

        protected virtual ReadOnlyCollection<DbRelatedEntityRef> CreateDbRelatedEntityRefList(ReadOnlyCollection<DbRelatedEntityRef> relationships) {
            if (relationships == null) {
                return null;
            }

            var list = relationships.Select(item => new DbRelatedEntityRef(item.SourceEnd, item.TargetEnd, item.TargetEntityRef.Accept(this))).ToList();

            return new ReadOnlyCollection<DbRelatedEntityRef>(list);
        }

        protected ReadOnlyCollection<DbSetClause> CreateDbSetClauses(IEnumerable<DbSetClause> setClauses) {
            var list = setClauses.Select(item => new DbSetClause(item.Property.Accept(this), item.Value.Accept(this))).ToList();

            return new ReadOnlyCollection<DbSetClause>(list);
        }
    }
}