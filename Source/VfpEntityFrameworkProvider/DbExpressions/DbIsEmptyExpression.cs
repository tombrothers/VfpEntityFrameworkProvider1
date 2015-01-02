using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbIsEmptyExpression : DbUnaryExpression {
        internal DbIsEmptyExpression(TypeUsage booleanResultType, DbExpression argument)
            : base(DbExpressionKind.IsEmpty, booleanResultType, argument) {
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}