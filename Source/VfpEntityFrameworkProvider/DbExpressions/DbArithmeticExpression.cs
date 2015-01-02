using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbArithmeticExpression : DbExpression {
        public DbExpressionList Arguments { get; private set; }

        internal DbArithmeticExpression(DbExpressionKind kind, TypeUsage numericResultType, DbExpressionList arguments)
            : base(kind, numericResultType) {
            Arguments = arguments;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}