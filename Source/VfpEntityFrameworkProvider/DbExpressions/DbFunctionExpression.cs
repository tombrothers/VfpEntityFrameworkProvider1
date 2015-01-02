using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbFunctionExpression : DbExpression {
        public EdmFunction Function { get; private set; }
        public DbExpressionList Arguments { get; private set; }

        internal DbFunctionExpression(TypeUsage resultType, EdmFunction function, DbExpressionList arguments)
            : base(DbExpressionKind.Function, resultType) {
            Function = function;
            Arguments = arguments;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}