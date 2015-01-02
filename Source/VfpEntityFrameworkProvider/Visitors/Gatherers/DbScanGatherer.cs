using System.Collections.Generic;
using System.Collections.ObjectModel;
using VfpEntityFrameworkProvider.DbExpressions;

namespace VfpEntityFrameworkProvider.Visitors.Gatherers {
    internal class DbScanGatherer : DbExpressionVisitor {
        private readonly List<DbScanExpression> _expressions = new List<DbScanExpression>();

        public static ReadOnlyCollection<DbScanExpression> Gather(DbExpression expression) {
            var visitor = new DbScanGatherer();

            visitor.Visit(expression);

            return visitor._expressions.AsReadOnly();
        }

        public override DbExpression Visit(DbScanExpression expression) {
            _expressions.Add(expression);

            return base.Visit(expression);
        }
    }
}