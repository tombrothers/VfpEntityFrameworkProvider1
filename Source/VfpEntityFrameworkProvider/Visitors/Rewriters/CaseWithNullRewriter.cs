using System.Linq;
using VfpEntityFrameworkProvider.DbExpressions;
using VfpEntityFrameworkProvider.Visitors.Gatherers;

namespace VfpEntityFrameworkProvider.Visitors.Rewriters {
    internal class CaseWithNullRewriter : DbExpressionVisitor {
        private bool _canRewrite;

        public static DbExpression Rewrite(DbExpression expression) {
            var rewriter = new CaseWithNullRewriter();

            return rewriter.Visit(expression);
        }

        public override DbExpression Visit(DbNewInstanceExpression expression) {
            _canRewrite = true;

            var result = base.Visit(expression);

            _canRewrite = false;

            return result;
        }

        public override DbExpression Visit(DbCaseExpression expression) {
            if (!_canRewrite || !NullGatherer.Gather(expression).Any()) {
                return expression;
            }

            return new DbCastExpression(expression.ResultType, expression);
        }
    }
}