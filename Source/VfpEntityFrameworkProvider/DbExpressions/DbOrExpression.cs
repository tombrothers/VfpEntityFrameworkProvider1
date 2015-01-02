using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbOrExpression : DbBinaryExpression {
        internal DbOrExpression(TypeUsage booleanResultType, DbExpression left, DbExpression right)
            : base(DbExpressionKind.Or, booleanResultType, left, right) {
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}