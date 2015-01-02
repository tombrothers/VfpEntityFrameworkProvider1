using System.Collections.Generic;
using System.Data.Metadata.Edm;
using VfpEntityFrameworkProvider.DbExpressions;

namespace VfpEntityFrameworkProvider.Visitors.Rewriters {
    internal class SingleRowTableRewritter : DbExpressionVisitor {
        public static DbExpression Rewrite(DbExpression expression) {
            var rewriter = new SingleRowTableRewritter();

            return rewriter.Visit(expression);
        }

        public override DbExpression Visit(DbProjectExpression expression) {
            DbProjectExpression innerProjectionExpression;
            bool hasNotExpression;

            if (GetInnerProjectionExpression(expression, out innerProjectionExpression, out hasNotExpression)) {
                var innerNewInstanceExpression = innerProjectionExpression.Projection as DbNewInstanceExpression;

                if (innerNewInstanceExpression != null) {
                    var innerConstExpression = innerNewInstanceExpression.Arguments[0] as DbConstantExpression;

                    if (innerConstExpression != null) {
                        var countExpression = new DbConstantExpression(innerConstExpression.ResultType, "COUNT(*)");

                        innerNewInstanceExpression = new DbNewInstanceExpression(innerNewInstanceExpression.ResultType, new DbExpressionList(new List<DbExpression> { countExpression }));
                        innerProjectionExpression = new DbProjectExpression(innerProjectionExpression.ResultType, innerProjectionExpression.Input, innerNewInstanceExpression);

                        DbExpression comparison = new DbComparisonExpression(DbExpressionKind.LessThan,
                                                                             PrimitiveTypeKind.Boolean.ToTypeUsage(),
                                                                             new DbConstantExpression(PrimitiveTypeKind.Int32.ToTypeUsage(), 0),
                                                                             innerProjectionExpression);

                        if (!hasNotExpression) {
                            comparison = new DbNotExpression(comparison.ResultType, comparison);
                        }

                        innerNewInstanceExpression = new DbNewInstanceExpression(innerNewInstanceExpression.ResultType, new DbExpressionList(new List<DbExpression> { comparison }));
                        innerProjectionExpression = new DbProjectExpression(expression.ResultType, expression.Input, innerNewInstanceExpression);

                        return innerProjectionExpression;
                    }
                }
            }

            return base.Visit(expression);
        }

        private static bool GetInnerProjectionExpression(DbProjectExpression expression, out DbProjectExpression innerProjectionExpression, out bool hasNotExpression) {
            innerProjectionExpression = null;
            hasNotExpression = false;

            if (!expression.Input.VariableName.StartsWith("SingleRowTable")) {
                return false;
            }

            var newInstanceExpression = expression.Projection as DbNewInstanceExpression;

            if (newInstanceExpression == null) {
                return false;
            }

            var notExpression = newInstanceExpression.Arguments[0] as DbNotExpression;
            DbIsEmptyExpression isEmptyExpression = null;

            if (notExpression == null) {
                isEmptyExpression = newInstanceExpression.Arguments[0] as DbIsEmptyExpression;
            }
            else {
                isEmptyExpression = notExpression.Argument as DbIsEmptyExpression;
                hasNotExpression = true;
            }

            if (isEmptyExpression == null) {
                return false;
            }

            innerProjectionExpression = isEmptyExpression.Argument as DbProjectExpression;

            return innerProjectionExpression != null;
        }
    }
}