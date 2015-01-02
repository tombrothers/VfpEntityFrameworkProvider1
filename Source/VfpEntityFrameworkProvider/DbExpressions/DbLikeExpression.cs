using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbLikeExpression : DbExpression {
        public DbExpression Argument { get; private set; }
        public DbExpression Pattern { get; private set; }
        public DbExpression Escape { get; private set; }

        internal DbLikeExpression(TypeUsage resultType, DbExpression argument, DbExpression pattern, DbExpression escape)
            : base(DbExpressionKind.Like, resultType) {
            Argument = argument;
            Pattern = pattern;
            Escape = escape;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}