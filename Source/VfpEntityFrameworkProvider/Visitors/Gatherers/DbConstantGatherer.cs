using System.Collections.Generic;
using System.Collections.ObjectModel;
using VfpEntityFrameworkProvider.DbExpressions;

namespace VfpEntityFrameworkProvider.Visitors.Gatherers {
    internal class DbConstantGatherer : DbExpressionVisitor {
        private readonly List<DbConstantExpression> _expressions = new List<DbConstantExpression>();

        public static ReadOnlyCollection<DbConstantExpression> Gather(DbExpression expression) {
            var visitor = new DbConstantGatherer();

            visitor.Visit(expression);

            return visitor._expressions.AsReadOnly();
        }

        public override DbExpression Visit(DbConstantExpression expression) {
            _expressions.Add(expression);

            return base.Visit(expression);
        }
    }
}