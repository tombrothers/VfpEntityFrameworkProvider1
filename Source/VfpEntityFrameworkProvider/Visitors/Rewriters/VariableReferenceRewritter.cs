using VfpEntityFrameworkProvider.DbExpressions;
using VfpEntityFrameworkProvider.SqlGeneration;

namespace VfpEntityFrameworkProvider.Visitors.Rewriters {
    internal class VariableReferenceRewritter : DbExpressionVisitor {
        public static DbExpression Rewrite(DbExpression expression) {
            var rewriter = new VariableReferenceRewritter();

            return rewriter.Visit(expression);
        }

        public override DbExpression Visit(DbVariableReferenceExpression expression) {
            foreach (var shortNames in SqlVisitor.AliasNames) {
                if (expression.VariableName.StartsWith(shortNames)) {
                    return new DbVariableReferenceExpression(expression.ResultType, expression.VariableName.Replace(shortNames, shortNames.Substring(0, 1)));
                }
            }

            return base.Visit(expression);
        }
    }
}