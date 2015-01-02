using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbDerefExpression : DbUnaryExpression {
        internal DbDerefExpression(TypeUsage entityResultType, DbExpression refExpr)
            : base(DbExpressionKind.Deref, entityResultType, refExpr) {
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}