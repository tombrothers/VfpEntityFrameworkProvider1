using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbProjectExpression : DbExpression {
        public DbExpressionBinding Input { get; private set; }
        public DbExpression Projection { get; private set; }

        internal DbProjectExpression(TypeUsage resultType, DbExpressionBinding input, DbExpression projection)
            : base(DbExpressionKind.Project, resultType) {
            Input = input;
            Projection = projection;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}