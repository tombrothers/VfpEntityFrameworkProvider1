using System.Linq;
using VfpEntityFrameworkProvider.DbExpressions;
using VfpEntityFrameworkProvider.Visitors.Gatherers;

namespace VfpEntityFrameworkProvider.Visitors.Rewriters {
    internal class InListRewritter : DbExpressionVisitor {
        public static DbExpression Rewrite(DbExpression expression) {
            var rewriter = new InListRewritter();

            return rewriter.Visit(expression);
        }

        public override DbExpression Visit(DbOrExpression expression) {
            var list = DbOrComparisonGatherer.Gather(expression);

            if (list.Any()) {
                var comparison = list.Select(x => new {
                    x.ExpressionKind,
                    LeftExpression = x.Left,
                    LeftDbConstantExpression = DbConstantGatherer.Gather(x.Left).FirstOrDefault(),
                    RightExpression = x.Right,
                    RightDbPropertyExpression = DbPropertyGatherer.Gather(x.Right).FirstOrDefault(),
                    RightDbVariableReferenceExpression = VariableReferenceGatherer.Gather(x.Right).FirstOrDefault()
                }).First();

                if (comparison.ExpressionKind != DbExpressionKind.Equals && comparison.ExpressionKind != DbExpressionKind.NotEquals) {
                    return base.Visit(expression);
                }

                if (comparison.LeftDbConstantExpression == null || comparison.RightDbPropertyExpression == null || comparison.RightDbVariableReferenceExpression == null) {
                    return base.Visit(expression);
                }

                var expressions = list.Select(x => new {
                    x.ExpressionKind,
                    LeftExpression = x.Left,
                    LeftDbConstantExpression = DbConstantGatherer.Gather(x.Left).FirstOrDefault(),
                    RightExpression = x.Right,
                    RightDbPropertyExpression = DbPropertyGatherer.Gather(x.Right).FirstOrDefault(),
                    RightDbVariableReferenceExpression = VariableReferenceGatherer.Gather(x.Right).FirstOrDefault()
                }).Where(x => x.LeftDbConstantExpression != null)
                  .Where(x => x.RightDbPropertyExpression != null)
                  .Where(x => x.RightDbVariableReferenceExpression != null)
                  .ToList();

                if (list.Count != expressions.Count) {
                    return base.Visit(expression);
                }

                if (!expressions.All(x => x.LeftDbConstantExpression.ConstantKind == comparison.LeftDbConstantExpression.ConstantKind &&
                                          x.LeftDbConstantExpression.ResultType == comparison.LeftDbConstantExpression.ResultType &&
                                          x.RightDbPropertyExpression.Property == comparison.RightDbPropertyExpression.Property &&
                                          x.RightDbVariableReferenceExpression.VariableName == comparison.RightDbVariableReferenceExpression.VariableName)) {
                    return base.Visit(expression);
                }

                var inListExpression = DbExpression.InList(comparison.RightExpression, DbExpression.Array(list.Select(x => x.Left)));

                return base.Visit(inListExpression);
            }

            return base.Visit(expression);
        }
    }
}