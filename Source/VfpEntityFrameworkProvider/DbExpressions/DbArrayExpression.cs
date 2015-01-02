using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbArrayExpression : DbExpression {
        public DbExpressionList Values { get; private set; }

        public DbArrayExpression(DbExpressionList values)
            : base(DbExpressionKind.InList, PrimitiveTypeKind.Boolean.ToTypeUsage()) {
            Values = values;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}