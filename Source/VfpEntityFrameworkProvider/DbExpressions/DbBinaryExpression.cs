using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbBinaryExpression : DbExpression {
        public DbExpression Left { get; private set; }
        public DbExpression Right { get; private set; }

        internal DbBinaryExpression(DbExpressionKind kind, TypeUsage type, DbExpression left, DbExpression right)
            : base(kind, type) {
            Left = left;
            Right = right;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}