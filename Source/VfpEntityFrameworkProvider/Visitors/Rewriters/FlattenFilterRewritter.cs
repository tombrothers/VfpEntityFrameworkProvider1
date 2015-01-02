using System.Linq;
using VfpEntityFrameworkProvider.DbExpressions;
using VfpEntityFrameworkProvider.Visitors.Gatherers;
using VfpEntityFrameworkProvider.Visitors.Replacers;

namespace VfpEntityFrameworkProvider.Visitors.Rewriters {
    internal class FlattenFilterRewritter : DbExpressionVisitor {
        public static DbExpression Rewrite(DbExpression expression) {
            var rewriter = new FlattenFilterRewritter();

            return rewriter.Visit(expression);
        }

        public override DbExpression Visit(DbFilterExpression expression) {
            expression = (DbFilterExpression)base.Visit(expression);

            var innerFilter = expression.Input.Expression as DbFilterExpression;

            if (innerFilter == null) {
                return expression;
            }

            var variables = VariableReferenceGatherer.Gather(innerFilter.Predicate);

            if (!variables.Any() || variables.Select(x => x.VariableName).Distinct().Count() > 1) {
                return expression;
            }

            var predicate = VariableReferenceReplacer.Replace(variables.First(), expression.Predicate);

            return DbExpression.Filter(expression.ResultType, innerFilter.Input, DbExpression.And(predicate, innerFilter.Predicate));
        }
    }
}