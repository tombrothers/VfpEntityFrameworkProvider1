using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbXmlToCursorScanExpression : DbExpression {
        public new DbExpression Parameter { get; private set; }
        public string CursorName { get; private set; }

        internal DbXmlToCursorScanExpression(DbExpression parameter, string cursorName)
            : base(DbExpressionKind.XmlToCursorScan, PrimitiveTypeKind.Boolean.ToTypeUsage()) {
            Parameter = parameter;
            CursorName = cursorName;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}