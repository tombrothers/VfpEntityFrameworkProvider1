using System.Collections.Generic;
using System.Collections.ObjectModel;
using VfpEntityFrameworkProvider.DbExpressions;

namespace VfpEntityFrameworkProvider.Visitors.Gatherers {
    internal class DbParameterGatherer : DbExpressionVisitor {
        private readonly List<DbParameterExpression> _expressions = new List<DbParameterExpression>();

        public static ReadOnlyCollection<DbParameterExpression> Gather(DbExpression expression) {
            var visitor = new DbParameterGatherer();

            visitor.Visit(expression);

            return visitor._expressions.AsReadOnly();
        }

        public override DbExpression Visit(DbParameterExpression expression) {
            _expressions.Add(expression);

            return base.Visit(expression);
        }
    }
}