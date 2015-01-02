using System.Collections.Generic;
using System.Collections.ObjectModel;
using VfpEntityFrameworkProvider.DbExpressions;

namespace VfpEntityFrameworkProvider.Visitors.Gatherers {
    internal class VariableReferenceGatherer : DbExpressionVisitor {
        private readonly List<DbVariableReferenceExpression> _expressions = new List<DbVariableReferenceExpression>();

        public static ReadOnlyCollection<DbVariableReferenceExpression> Gather(DbExpression expression) {
            var visitor = new VariableReferenceGatherer();

            visitor.Visit(expression);

            return visitor._expressions.AsReadOnly();
        }

        public override DbExpression Visit(DbVariableReferenceExpression expression) {
            _expressions.Add(expression);

            return base.Visit(expression);
        }
    }
}