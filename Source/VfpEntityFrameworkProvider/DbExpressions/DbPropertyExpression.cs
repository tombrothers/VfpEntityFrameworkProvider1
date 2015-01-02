using System.Data.Metadata.Edm;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    [DebuggerDisplay("{DebuggerDisplay(),nq}")]
    public class DbPropertyExpression : DbExpression {
        public EdmMember Property { get; private set; }
        public DbExpression Instance { get; private set; }

        internal DbPropertyExpression(TypeUsage resultType, EdmMember property, DbExpression instance)
            : base(DbExpressionKind.Property, resultType) {
            Property = property;
            Instance = instance;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }

        [ExcludeFromCodeCoverage]
        private string DebuggerDisplay() {
            return string.Format("PropertyName={0} | DeclaringTypeName={1}", this.Property.Name, this.Property.DeclaringType.Name);
        }
    }
}