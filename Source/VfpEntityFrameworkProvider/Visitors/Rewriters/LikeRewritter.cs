using System.Linq;
using VfpEntityFrameworkProvider.DbExpressions;
using VfpEntityFrameworkProvider.Visitors.Gatherers;

namespace VfpEntityFrameworkProvider.Visitors.Rewriters {
    internal class LikeRewritter : DbExpressionVisitor {
        public static DbExpression Rewrite(DbExpression expression) {
            var rewriter = new LikeRewritter();

            return rewriter.Visit(expression);
        }

        public override DbExpression Visit(DbLikeExpression expression) {
            // Check the argument expression to see if it contains a property expression.
            // Don't need to rewrite the expression of the left side already contains a property expression.
            if (DbPropertyGatherer.Gather(expression.Argument).Any()) {
                return expression;
            }

            // Check the pattern expression to see if it has a property expression.
            if (!DbPropertyGatherer.Gather(expression.Pattern).Any()) {
                return expression;
            }

            return DbExpression.Like(expression.ResultType,
                                     expression.Pattern,
                                     expression.Argument,
                                     expression.Escape);
        }
    }
}