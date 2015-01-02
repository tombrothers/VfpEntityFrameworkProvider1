using System.Collections.Generic;
using System.Collections.ObjectModel;
using VfpEntityFrameworkProvider.DbExpressions;

namespace VfpEntityFrameworkProvider.Visitors.Gatherers {
    internal class DbPropertyGatherer : DbExpressionVisitor {
        private readonly List<DbPropertyExpression> _expressions = new List<DbPropertyExpression>();

        public static ReadOnlyCollection<DbPropertyExpression> Gather(DbExpression expression) {
            var visitor = new DbPropertyGatherer();

            visitor.Visit(expression);

            return visitor._expressions.AsReadOnly();
        }

        public override DbExpression Visit(DbPropertyExpression expression) {
            _expressions.Add(expression);

            return base.Visit(expression);
        }
    }
}