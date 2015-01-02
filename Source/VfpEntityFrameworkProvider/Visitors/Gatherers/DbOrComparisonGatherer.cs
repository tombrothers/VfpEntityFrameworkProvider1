using System.Collections.Generic;
using System.Collections.ObjectModel;
using VfpEntityFrameworkProvider.DbExpressions;

namespace VfpEntityFrameworkProvider.Visitors.Gatherers {
    internal class DbOrComparisonGatherer : DbExpressionVisitor {
        private readonly List<DbComparisonExpression> _expressions = new List<DbComparisonExpression>();
        private bool _invalid;

        public static ReadOnlyCollection<DbComparisonExpression> Gather(DbOrExpression expression) {
            var visitor = new DbOrComparisonGatherer();

            visitor.Visit(expression);

            if (visitor._invalid) {
                visitor._expressions.Clear();
            }

            return visitor._expressions.AsReadOnly();
        }

        public override DbExpression Visit(DbAndExpression expression) {
            _invalid = true;

            return base.Visit(expression);
        }

        public override DbExpression Visit(DbComparisonExpression expression) {
            _expressions.Add(expression);

            return base.Visit(expression);
        }
    }
}