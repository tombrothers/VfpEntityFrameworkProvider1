using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbRefExpression : DbUnaryExpression {
        public EntitySet EntitySet { get; private set; }

        internal DbRefExpression(TypeUsage refResultType, EntitySet entitySet, DbExpression refKeys)
            : base(DbExpressionKind.Ref, refResultType, refKeys) {
            EntitySet = entitySet;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}