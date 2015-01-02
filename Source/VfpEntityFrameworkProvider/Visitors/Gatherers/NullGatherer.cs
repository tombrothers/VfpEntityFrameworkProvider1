using System.Collections.Generic;
using System.Collections.ObjectModel;
using VfpEntityFrameworkProvider.DbExpressions;

namespace VfpEntityFrameworkProvider.Visitors.Gatherers {
    internal class NullGatherer : DbExpressionVisitor {
        private readonly List<DbNullExpression> _expressions = new List<DbNullExpression>();

        public static ReadOnlyCollection<DbNullExpression> Gather(DbExpression expression) {
            var visitor = new NullGatherer();

            visitor.Visit(expression);

            return visitor._expressions.AsReadOnly();
        }

        public override DbExpression Visit(DbNullExpression expression) {
            _expressions.Add(expression);

            return base.Visit(expression);
        }
    }
}