using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbApplyExpression : DbExpression {
        public DbExpressionBinding Apply { get; private set; }
        public DbExpressionBinding Input { get; private set; }

        internal DbApplyExpression(DbExpressionKind applyKind, TypeUsage resultRowCollectionTypeUsage, DbExpressionBinding input, DbExpressionBinding apply)
            : base(applyKind, resultRowCollectionTypeUsage) {
            Input = input;
            Apply = apply;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}