using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using VfpEntityFrameworkProvider.DbExpressions;

namespace VfpEntityFrameworkProvider.Visitors {
    internal class DbExpressionTextGenerator : DbExpressionVisitor {
        private readonly StringBuilder _text = new StringBuilder();

        public static string GetText(DbExpression expression) {
            var visitor = new DbExpressionTextGenerator();

            visitor.Visit(expression);

            return visitor._text.ToString();
        }

        public override DbExpression Visit(DbParameterExpression expression) {
            Write(expression, x => {
                _text.Append("(Name=");
                _text.Append(expression.Name);
                _text.Append("|Value=");

                expression.Value.Accept(this);

                _text.Append(")");
            });

            return expression;
        }

        public override DbExpression Visit(DbCrossJoinExpression expression) {
            Write(expression, x => base.Visit(expression));

            return expression;
        }

        public override DbExpression Visit(DbDerefExpression expression) {
            Write(expression, x => base.Visit(expression));

            return expression;
        }

        public override DbExpression Visit(DbDistinctExpression expression) {
            Write(expression, x => base.Visit(expression));

            return expression;
        }

        public override DbExpression Visit(DbElementExpression expression) {
            Write(expression, x => base.Visit(expression));

            return expression;
        }

        public override DbExpression Visit(DbEntityRefExpression expression) {
            Write(expression, x => base.Visit(expression));

            return expression;
        }

        public override DbExpression Visit(DbExceptExpression expression) {
            Write(expression, x => base.Visit(expression));

            return expression;
        }

        public override DbExpression Visit(DbXmlToCursorPropertyExpression expression) {
            Write(expression, x => base.Visit(expression));

            return expression;
        }

        public override DbExpression Visit(DbXmlToCursorScanExpression expression) {
            Write(expression, x => {
                _text.Append("(CursorName=");
                _text.Append(expression.CursorName);
                _text.Append("|Parameter=");

                expression.Parameter.Accept(this);

                _text.Append(")");
            });

            return expression;
        }

        public override DbExpression Visit(DbFilterExpression expression) {
            Write(expression, x => base.Visit(expression));

            return expression;
        }

        public override DbExpression Visit(DbFunctionExpression expression) {
            Write(expression, x => base.Visit(expression));

            return expression;
        }

        public override DbExpression Visit(DbGroupByExpression expression) {
            Write(expression, x => base.Visit(expression));

            return expression;
        }

        public override DbExpression Visit(DbIntersectExpression expression) {
            Write(expression, x => base.Visit(expression));

            return expression;
        }

        public override DbExpression Visit(DbIsEmptyExpression expression) {
            Write(expression, x => base.Visit(expression));

            return expression;
        }

        public override DbExpression Visit(DbIsNullExpression expression) {
            Write(expression, x => base.Visit(expression));

            return expression;
        }

        public override DbExpression Visit(DbIsOfExpression expression) {
            Write(expression, x => {
                _text.Append("(Argument=");

                expression.Argument.Accept(this);

                _text.Append("|OfType=");
                _text.Append(expression.OfType);
                _text.Append(")");
            });

            return expression;
        }

        public override DbExpression Visit(DbJoinExpression expression) {
            Write(expression, x => base.Visit(expression));

            return expression;
        }

        public override DbExpression Visit(DbLikeExpression expression) {
            Write(expression, x => base.Visit(expression));

            return expression;
        }

        public override DbExpression Visit(DbLimitExpression expression) {
            Write(expression, x => base.Visit(expression));

            return expression;
        }

        public override DbExpression Visit(DbNewInstanceExpression expression) {
            Write(expression, x => base.Visit(expression));

            return expression;
        }

        public override DbExpression Visit(DbNotExpression expression) {
            Write(expression, x => base.Visit(expression));

            return expression;
        }

        public override DbExpression Visit(DbNullExpression expression) {
            Write(expression, x => base.Visit(expression));

            return expression;
        }

        public override DbExpression Visit(DbOfTypeExpression expression) {
            Write(expression, x => {
                _text.Append("(Argument=");

                expression.Argument.Accept(this);

                _text.Append("|OfType=");
                _text.Append(expression.OfType);
                _text.Append(")");
            });

            return expression;
        }

        public override DbExpression Visit(DbOrExpression expression) {
            Write(expression, x => base.Visit(expression));

            return expression;
        }

        public override DbExpression Visit(DbParameterReferenceExpression expression) {
            Write(expression, x => {
                _text.Append("(ParameterName=");
                _text.Append(expression.ParameterName);
                _text.Append(")");
            });

            return expression;
        }

        public override DbExpression Visit(DbProjectExpression expression) {
            Write(expression, x => base.Visit(expression));

            return expression;
        }

        public override DbExpression Visit(DbPropertyExpression expression) {
            Write(expression, x => {
                _text.Append("(Property=");
                _text.Append(expression.Property.Name);
                _text.Append("|Instance=");

                expression.Instance.Accept(this);

                _text.Append(")");
            });

            return expression;
        }

        public override DbExpression Visit(DbQuantifierExpression expression) {
            Write(expression, x => base.Visit(expression));

            return expression;
        }

        public override DbExpression Visit(DbRefExpression expression) {
            Write(expression, x => {
                _text.Append("(EntitySet=");
                _text.Append(expression.EntitySet);
                _text.Append("|Argument=");

                expression.Argument.Accept(this);

                _text.Append(")");
            });

            return expression;
        }

        public override DbExpression Visit(DbRefKeyExpression expression) {
            Write(expression, x => base.Visit(expression));

            return expression;
        }

        public override DbExpression Visit(DbRelationshipNavigationExpression expression) {
            Write(expression, x => {
                _text.Append("(Relationship=");
                _text.Append(expression.Relationship.Name);
                _text.Append("|NavigateTo=");
                _text.Append(expression.NavigateTo.Name);
                _text.Append("|NavigateFrom=");
                _text.Append(expression.NavigateFrom.Name);
                _text.Append("|NavigationSource=");

                expression.NavigationSource.Accept(this);

                _text.Append(")");
            });

            return expression;
        }

        public override DbExpression Visit(DbScanExpression expression) {
            Write(expression, x => {
                _text.Append("(Target=");
                _text.Append(expression.Target);
                _text.Append(")");
            });

            return expression;
        }

        public override DbExpression Visit(DbSkipExpression expression) {
            Write(expression, x => base.Visit(expression));

            return expression;
        }

        public override DbExpression Visit(DbSortExpression expression) {
            Write(expression, x => base.Visit(expression));

            return expression;
        }

        public override DbExpression Visit(DbTreatExpression expression) {
            Write(expression, x => base.Visit(expression));

            return expression;
        }

        public override DbExpression Visit(DbUnionAllExpression expression) {
            Write(expression, x => base.Visit(expression));

            return expression;
        }

        public override DbExpression Visit(DbVariableReferenceExpression expression) {
            Write(expression, x => {
                _text.Append("(VariableName=");
                _text.Append(expression.VariableName);
                _text.Append(")");
            });

            return expression;
        }

        protected override DbExpressionBinding VisitDbExpressionBinding(DbExpressionBinding binding) {
            _text.AppendLine("(DbExpressionBinding:");

            binding.Expression.Accept(this);
            binding.Variable.Accept(this);

            _text.AppendLine(")");

            return binding;
        }

        protected override ReadOnlyCollection<DbAggregate> CreateDbAggregates(IList<DbAggregate> aggregates) {
            _text.AppendLine("(DbAggregate:");

            var results = base.CreateDbAggregates(aggregates);

            _text.AppendLine(")");

            return results;
        }

        protected virtual DbFunctionAggregate CreateDbFunctionAggregate(DbFunctionAggregate functionAggregate) {
            _text.AppendLine("(DbFunctionAggregate:");
            _text.Append("(Function=");
            _text.Append(functionAggregate.Function.Name);
            _text.Append("|Distinct=");
            _text.Append(functionAggregate.Distinct);
            _text.Append("|Arguments=");

            functionAggregate = base.CreateDbFunctionAggregate(functionAggregate);

            _text.Append(")");

            return functionAggregate;
        }

        protected override DbExpressionList VisitDbExpressionList(IList<DbExpression> expressions) {
            var list = base.VisitDbExpressionList(expressions);

            for (int index = 0, total = list.Count; index < total; index++) {
                if (index > 0) {
                    _text.Append("|");
                }

                _text.Append("Item");
                _text.Append(index);
                _text.Append("=");

                list[index].Accept(this);
            }

            return list;
        }

        protected override ReadOnlyCollection<DbSortClause> CreateDbSortClauses(IList<DbSortClause> sortClause) {
            var list = base.CreateDbSortClauses(sortClause);

            _text.AppendLine("(DbSortClause:");

            foreach (var item in list) {
                _text.Append("(DbSortClauseItem:");
                _text.Append("(Ascending=");
                _text.Append(item.Ascending);
                _text.Append("|Collation=");
                _text.Append(item.Collation);
                _text.Append("|Expression=");

                item.Expression.Accept(this);

                _text.AppendLine(")");
            }

            _text.AppendLine(")");

            return list;
        }

        protected override ReadOnlyCollection<DbRelatedEntityRef> CreateDbRelatedEntityRefList(ReadOnlyCollection<DbRelatedEntityRef> relationships) {
            var list = base.CreateDbRelatedEntityRefList(relationships);

            _text.AppendLine("(DbRelatedEntityRef:");

            if (relationships == null) {
                _text.AppendLine("null");
            }
            else {
                foreach (var item in list) {
                    _text.Append("(DbRelatedEntityRefItem:");
                    _text.Append("(SourceEnd=");
                    _text.Append(item.SourceEnd.Name);
                    _text.Append("|TargetEnd=");
                    _text.Append(item.TargetEnd.Name);
                    _text.Append("|TargetEntityRef=");

                    item.TargetEntityRef.Accept(this);

                    _text.AppendLine(")");
                }
            }

            return list;
        }
        private void Write<T>(T expression, Action<T> action) where T : DbExpression {
            WriteStart(expression);
            action(expression);
            WriteEnd(expression);
        }

        private void WriteEnd(DbExpression expression) {
            _text.Append(":");
            _text.Append(expression.ResultType);
            _text.AppendLine(")");
        }

        private void WriteStart(DbExpression expression) {
            _text.Append("(Kind:");
            _text.Append(expression.ExpressionKind);
        }
    }
}
