using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbIsNullExpression : DbUnaryExpression {
        internal DbIsNullExpression(TypeUsage booleanResultType, DbExpression argument)
            : base(DbExpressionKind.IsNull, booleanResultType, argument) {
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}