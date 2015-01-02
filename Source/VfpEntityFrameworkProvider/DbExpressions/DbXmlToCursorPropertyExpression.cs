using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbXmlToCursorPropertyExpression : DbExpression {
        public DbExpression Instance { get; private set; }

        internal DbXmlToCursorPropertyExpression(TypeUsage resultType, DbExpression instance)
            : base(DbExpressionKind.XmlToCursorProperty, resultType) {
            Instance = instance;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}