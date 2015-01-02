using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbJoinExpression : DbExpression {
        public DbExpressionBinding Left { get; private set; }
        public DbExpressionBinding Right { get; private set; }
        public DbExpression JoinCondition { get; private set; }

        internal DbJoinExpression(DbExpressionKind joinKind, TypeUsage collectionOfRowResultType, DbExpressionBinding left, DbExpressionBinding right, DbExpression condition)
            : base(joinKind, collectionOfRowResultType) {
            Left = left;
            Right = right;
            JoinCondition = condition;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}