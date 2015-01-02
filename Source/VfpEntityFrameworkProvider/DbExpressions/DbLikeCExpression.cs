using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbLikeCExpression : DbExpression {
        public DbExpression Argument { get; private set; }
        public DbExpression Pattern { get; private set; }

        internal DbLikeCExpression(TypeUsage resultType, DbExpression argument, DbExpression pattern)
            : base(DbExpressionKind.LikeC, resultType) {
            Argument = ArgumentUtility.CheckNotNull("argument", argument);
            Pattern = ArgumentUtility.CheckNotNull("pattern", pattern);
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}