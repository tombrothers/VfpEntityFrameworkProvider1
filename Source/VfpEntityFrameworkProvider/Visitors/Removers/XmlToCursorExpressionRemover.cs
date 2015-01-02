using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Linq;
using VfpEntityFrameworkProvider.DbExpressions;
using VfpEntityFrameworkProvider.Visitors.Gatherers;
using VfpEntityFrameworkProvider.Visitors.Replacers;

namespace VfpEntityFrameworkProvider.Visitors.Removers {
    internal class XmlToCursorExpressionRemover : DbExpressionVisitor {
        private readonly string _cursorName;

        public static DbExpression Remove(DbExpression expression, string cursorName) {
            ArgumentUtility.CheckNotNullOrEmpty("cursorName", cursorName);

            var visitor = new XmlToCursorExpressionRemover(cursorName);

            return visitor.Visit(expression);
        }

        private XmlToCursorExpressionRemover(string cursorName) {
            _cursorName = cursorName;
        }

        public override DbExpression Visit(DbJoinExpression expression) {
            var result = base.Visit(expression);

            expression = result as DbJoinExpression;

            if (expression == null) {
                return result;
            }

            var filterExpression = expression.Left.Expression as DbFilterExpression;

            if (IsZeroEqualsZero(filterExpression)) {
                expression = DbExpression.Join(expression.ExpressionKind, expression.ResultType, filterExpression.Input, expression.Right, expression.JoinCondition);
            }

            filterExpression = expression.Right.Expression as DbFilterExpression;

            if (IsZeroEqualsZero(filterExpression)) {
                expression = DbExpression.Join(expression.ExpressionKind, expression.ResultType, expression.Left, filterExpression.Input, expression.JoinCondition);
            }

            return expression;
        }

        public override DbExpression Visit(DbProjectExpression expression) {
            var result = base.Visit(expression);

            expression = result as DbProjectExpression;

            if (expression == null) {
                return result;
            }

            var filterExpression = expression.Input.Expression as DbFilterExpression;

            if (IsZeroEqualsZero(filterExpression)) {
                var projectionExpression = GetProjection(filterExpression, expression.Input.VariableName, expression.Projection);

                return DbExpression.Project(expression.ResultType, filterExpression.Input, projectionExpression);
            }

            return result;
        }

        private static DbExpression GetProjection(DbFilterExpression filterExpression, string filterVariableName, DbExpression projectionExpression) {
            var newInstanceExpression = projectionExpression as DbNewInstanceExpression;

            if (newInstanceExpression == null) {
                return projectionExpression;
            }

            var newInstanceVariableNames = VariableReferenceGatherer.Gather(newInstanceExpression)
                                                                    .Select(x => x.VariableName)
                                                                    .Distinct()
                                                                    .ToArray();

            if (newInstanceVariableNames.Length != 1) {
                return projectionExpression;
            }

            if (filterVariableName != newInstanceVariableNames.First()) {
                return projectionExpression;
            }

            return VariableReferenceReplacer.Replace(filterExpression.Input.Variable, newInstanceExpression);
        }

        private static bool IsZeroEqualsZero(DbFilterExpression expression) {
            return expression != null && IsZeroEqualsZero(expression.Predicate as DbComparisonExpression);
        }

        private static bool IsZeroEqualsZero(DbComparisonExpression expression) {
            return expression != null && expression.ExpressionKind == DbExpressionKind.Equals && IsZero(expression.Left as DbConstantExpression) && IsZero(expression.Right as DbConstantExpression);
        }

        private static bool IsZero(DbConstantExpression expression) {
            return expression != null && expression.Value != null && expression.Value.ToString() == "0";
        }

        public override DbExpression Visit(DbFilterExpression expression) {
            var result = base.Visit(expression);

            expression = result as DbFilterExpression;

            if (expression == null) {
                return result;
            }

            if (IsValidXmlToCursorExpression(expression.Predicate as DbXmlToCursorExpression)) {
                var predicate = DbExpression.Comparison(DbExpressionKind.Equals,
                                                        PrimitiveTypeKind.Boolean.ToTypeUsage(),
                                                        DbExpression.Constant(0),
                                                        DbExpression.Constant(0));

                return new DbFilterExpression(expression.ResultType, expression.Input, predicate);
            }

            return result;
        }

        public override DbExpression Visit(DbAndExpression expression) {
            var result = base.Visit(expression);

            expression = result as DbAndExpression;

            if (expression == null) {
                return result;
            }

            if (IsValidXmlToCursorExpression(expression.Left as DbXmlToCursorExpression)) {
                return expression.Right;
            }

            if (IsValidXmlToCursorExpression(expression.Right as DbXmlToCursorExpression)) {
                return expression.Left;
            }

            return result;
        }

        private bool IsValidXmlToCursorExpression(DbXmlToCursorExpression expression) {
            return expression != null && expression.CursorName == _cursorName;
        }
    }
}