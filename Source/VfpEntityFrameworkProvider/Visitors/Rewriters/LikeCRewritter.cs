using VfpEntityFrameworkProvider.DbExpressions;

namespace VfpEntityFrameworkProvider.Visitors.Rewriters {
    internal class LikeCRewritter : DbExpressionVisitor {
        private bool _canRewrite;

        public static DbExpression Rewrite(DbExpression expression) {
            var rewriter = new LikeCRewritter();

            return rewriter.Visit(expression);
        }

        public override DbExpression Visit(DbNewInstanceExpression expression) {
            _canRewrite = true;

            var result = base.Visit(expression);

            _canRewrite = false;

            return result;
        }

        public override DbExpression Visit(DbLikeExpression expression) {
            if (!_canRewrite) {
                return expression;
            }

            return new DbLikeCExpression(expression.ResultType, expression.Argument, expression.Pattern);
        }
    }
}