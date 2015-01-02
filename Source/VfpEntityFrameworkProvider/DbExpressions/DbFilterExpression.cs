using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbFilterExpression : DbExpression {
        public DbExpressionBinding Input { get; private set; }
        public DbExpression Predicate { get; private set; }

        internal DbFilterExpression(TypeUsage resultType, DbExpressionBinding input, DbExpression predicate)
            : base(DbExpressionKind.Filter, resultType) {
            Input = input;
            Predicate = predicate;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}