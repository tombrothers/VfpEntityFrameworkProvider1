using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbParameterReferenceExpression : DbExpression {
        public string ParameterName { get; private set; }

        internal DbParameterReferenceExpression(TypeUsage type, string name)
            : base(DbExpressionKind.ParameterReference, type) {
            ParameterName = name;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}