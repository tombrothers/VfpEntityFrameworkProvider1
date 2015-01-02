using System.Linq;
using VfpEntityFrameworkProvider.DbExpressions;
using VfpEntityFrameworkProvider.Visitors.Gatherers;

namespace VfpEntityFrameworkProvider.Visitors.Rewriters {
    /// <summary>
    /// The purpose of this rewriter is to ensure that vfp indexes are utilized.  The Entity Framework seems to prefer placing (constant) values on the left side of
    /// a comparison.  This is not ideal for vfp as vfp will only utilize an index if the index expression is on the left side of the comparison.
    /// </summary>
    /// <example>
    /// Before:
    ///         SELECT ;
    ///             Extent1.city;
    ///             FROM Customers Extent1;
    ///             WHERE ('USA' = (ALLTRIM(UPPER(Extent1.country)))) 
    ///             
    /// After:
    ///         SELECT ;
    ///             Extent1.city;
    ///             FROM Customers Extent1;
    ///             WHERE ((ALLTRIM(UPPER(Extent1.country))) = 'USA') 
    /// </example>
    internal class ComparisonRewritter : DbExpressionVisitor {
        public static DbExpression Rewrite(DbExpression expression) {
            var rewriter = new ComparisonRewritter();

            return rewriter.Visit(expression);
        }

        public override DbExpression Visit(DbComparisonExpression expression) {
            // Check the left expression to see if it contains a property expression.
            // Don't need to rewrite the comparison expression of the left side already contains a property expression.
            if (DbPropertyGatherer.Gather(expression.Left).Any()) {
                return expression;
            }

            // Check the right expression to see if it has a property expression.
            if (!DbPropertyGatherer.Gather(expression.Right).Any()) {
                return expression;
            }

            return DbExpression.Comparison(Reverse(expression.ExpressionKind),
                                           expression.ResultType,
                                           expression.Right,
                                           expression.Left);
        }

        private static DbExpressionKind Reverse(DbExpressionKind kind) {
            switch (kind) {
                case DbExpressionKind.GreaterThan:
                    return DbExpressionKind.LessThan;

                case DbExpressionKind.GreaterThanOrEquals:
                    return DbExpressionKind.LessThanOrEquals;

                case DbExpressionKind.LessThan:
                    return DbExpressionKind.GreaterThan;

                case DbExpressionKind.LessThanOrEquals:
                    return DbExpressionKind.GreaterThanOrEquals;

                default:
                    return kind;
            }
        }
    }
}