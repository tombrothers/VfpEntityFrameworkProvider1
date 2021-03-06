using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbExceptExpression : DbBinaryExpression {
        internal DbExceptExpression(TypeUsage resultType, DbExpression left, DbExpression right)
            : base(DbExpressionKind.Except, resultType, left, right) {
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}