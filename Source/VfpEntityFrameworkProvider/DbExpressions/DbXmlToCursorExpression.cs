using System;
using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbXmlToCursorExpression : DbExpression {
        public DbExpression Property { get; private set; }
        public new DbExpression Parameter { get; private set; }
        public string CursorName { get; private set; }
        public Type ItemType { get; private set; }

        public DbXmlToCursorExpression(DbExpression property, DbExpression parameter, string cursorName, Type itemType)
            : base(DbExpressionKind.XmlToCursor, PrimitiveTypeKind.Boolean.ToTypeUsage()) {
            Property = property;
            Parameter = parameter;
            CursorName = cursorName;
            ItemType = itemType;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}