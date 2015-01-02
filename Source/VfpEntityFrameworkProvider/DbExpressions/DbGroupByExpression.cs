using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbGroupByExpression : DbExpression {
        public DbGroupExpressionBinding Input { get; private set; }
        public DbExpressionList Keys { get; private set; }
        public IList<DbAggregate> Aggregates { get; private set; }

        internal DbGroupByExpression(TypeUsage collectionOfRowResultType, DbGroupExpressionBinding input, DbExpressionList groupKeys, ReadOnlyCollection<DbAggregate> aggregates)
            : base(DbExpressionKind.GroupBy, collectionOfRowResultType) {
            Input = input;
            Keys = groupKeys;
            Aggregates = aggregates;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}