using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbLimitExpression : DbExpression {
        public DbExpression Argument { get; private set; }
        public DbExpression Limit { get; private set; }
        public bool WithTies { get; private set; }

        internal DbLimitExpression(TypeUsage resultType, DbExpression argument, DbExpression limit, bool withTies)
            : base(DbExpressionKind.Limit, resultType) {
            Argument = argument;
            Limit = limit;
            WithTies = withTies;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}