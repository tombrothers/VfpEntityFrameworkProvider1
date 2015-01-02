using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbCaseExpression : DbExpression {
        public DbExpression Else { get; private set; }
        public DbExpressionList Then { get; private set; }
        public DbExpressionList When { get; private set; }

        internal DbCaseExpression(TypeUsage commonResultType, DbExpressionList whenExpressions, DbExpressionList thenExpressions, DbExpression elseExpression)
            : base(DbExpressionKind.Case, commonResultType) {
            When = whenExpressions;
            Then = thenExpressions;
            Else = elseExpression;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}