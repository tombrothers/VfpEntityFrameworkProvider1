using System.Data.Metadata.Edm;
using VfpEntityFrameworkProvider.DbExpressions;

namespace VfpEntityFrameworkProvider.Visitors.Rewriters {
    internal class ApplyRewritter : DbExpressionVisitor {
        public static DbExpression Rewrite(DbExpression expression) {
            var rewriter = new ApplyRewritter();

            return rewriter.Visit(expression);
        }

        public override DbExpression Visit(DbApplyExpression expression) {
            if (expression.ExpressionKind == DbExpressionKind.OuterApply) {
                if (IsSingleRowTable(expression.Input)) {
                    return CreateJoin(expression.Apply, expression.Input);
                }
            }

            return expression;
        }

        private static DbJoinExpression CreateJoin(DbExpressionBinding left, DbExpressionBinding right) {
            var comparison = DbExpression.Comparison(DbExpressionKind.Equals,
                                                     PrimitiveTypeKind.Boolean.ToTypeUsage(),
                                                     DbExpression.Constant(1),
                                                     DbExpression.Constant(1));

            return DbExpression.Join(DbExpressionKind.LeftOuterJoin, right.VariableType, left, right, comparison);
        }

        private static bool IsSingleRowTable(DbExpressionBinding expression) {
            return expression.VariableName.StartsWith("SingleRowTable");
        }
    }
}