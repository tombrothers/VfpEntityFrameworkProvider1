using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbConstantExpression : DbExpression {
        public PrimitiveTypeKind ConstantKind { get; private set; }

        private object value;

        public object Value {
            get {
                if (ConstantKind == PrimitiveTypeKind.Binary && value != null) {
                    return ((byte[])value).Clone();
                }

                return value;
            }
            private set {
                this.value = value;
            }
        }

        internal DbConstantExpression(TypeUsage resultType, object value)
            : base(DbExpressionKind.Constant, resultType) {
            ConstantKind = resultType.ToPrimitiveTypeKind();

            if (ConstantKind == PrimitiveTypeKind.Binary && value != null) {
                Value = ((byte[])value).Clone();
            }
            else {
                Value = value;
            }
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}