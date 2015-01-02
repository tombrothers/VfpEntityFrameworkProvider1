using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbOfTypeExpression : DbUnaryExpression {
        public TypeUsage OfType { get; private set; }

        internal DbOfTypeExpression(DbExpressionKind expressionKind, TypeUsage collectionResultType, DbExpression argument, TypeUsage type)
            : base(expressionKind, collectionResultType, argument) {
            OfType = type;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}