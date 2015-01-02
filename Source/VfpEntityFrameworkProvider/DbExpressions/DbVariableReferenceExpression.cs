using System.Data.Metadata.Edm;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    [DebuggerDisplay("{DebuggerDisplay(),nq}")]
    public class DbVariableReferenceExpression : DbExpression {
        public string VariableName { get; private set; }

        internal DbVariableReferenceExpression(TypeUsage type, string name)
            : base(DbExpressionKind.VariableReference, type) {
            VariableName = name;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }

        [ExcludeFromCodeCoverage]
        private string DebuggerDisplay() {
            return string.Format("VariableName={0} | EdmType={1}", VariableName, ResultType.EdmType);
        }
    }
}