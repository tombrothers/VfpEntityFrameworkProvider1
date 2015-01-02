using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbScanExpression : DbExpression {
        public EntitySetBase Target { get; private set; }

        internal DbScanExpression(TypeUsage collectionOfEntityType, EntitySetBase entitySet)
            : base(DbExpressionKind.Scan, collectionOfEntityType) {
            Target = entitySet;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}