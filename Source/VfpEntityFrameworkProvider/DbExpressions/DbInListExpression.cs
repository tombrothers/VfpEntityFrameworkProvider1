using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbInListExpression : DbExpression {
        public DbExpression Property { get; private set; }
        public DbExpression Values { get; private set; }

        internal DbInListExpression(DbExpression property, DbExpression values)
            : base(DbExpressionKind.InList, PrimitiveTypeKind.Boolean.ToTypeUsage()) {
            Property = property;
            Values = values;
        }

        internal DbArrayExpression GetArrayExpression() {
            var values = Values;
            var notExpression = values as DbNotExpression;

            if (notExpression != null) {
                values = notExpression.Argument;
            }

            return values as DbArrayExpression;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}