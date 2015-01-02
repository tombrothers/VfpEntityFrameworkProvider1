using System.Linq;
using VfpEntityFrameworkProvider.DbExpressions;
using VfpEntityFrameworkProvider.Visitors.Gatherers;

namespace VfpEntityFrameworkProvider.Visitors.Rewriters {
    internal class InvalidWhereExistsRewritter : DbExpressionVisitor {
        public static DbExpression Rewrite(DbExpression expression) {
            var rewriter = new InvalidWhereExistsRewritter();

            return rewriter.Visit(expression);
        }

        public override DbExpression Visit(DbFilterExpression expression) {
            var isEmptyExpression = expression.Predicate as DbIsEmptyExpression;

            if (isEmptyExpression == null) {
                return base.Visit(expression);
            }

            var projectExpression = isEmptyExpression.Argument as DbProjectExpression;

            if (projectExpression == null) {
                return base.Visit(expression);
            }

            var filterExpression = projectExpression.Input.Expression as DbFilterExpression;
            var variables = VariableReferenceGatherer.Gather(filterExpression);

            if (variables.All(x => expression.Input.VariableName != x.VariableName)) {
                return base.Visit(expression);
            }

            // TODO:  verify inner join
            
            //var joinExpression = DbExpression.Join(DbExpressionKind.InnerJoin, expression.ResultType, expression.Input, filterExpression.Input, filterExpression.Predicate);
            //var binding = DbExpression.Binding(joinExpression, DbExpression.VariableRef(PrimitiveTypeKind.String.ToTypeUsage(), "X" + (++_count)));

            //return base.Visit(binding);

            return base.Visit(expression);
        }
    }
}