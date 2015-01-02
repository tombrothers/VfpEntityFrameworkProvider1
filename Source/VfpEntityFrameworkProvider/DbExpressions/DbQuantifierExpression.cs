using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbQuantifierExpression : DbExpression {
        public DbExpressionBinding Input { get; private set; }
        public DbExpression Predicate { get; private set; }

        internal DbQuantifierExpression(DbExpressionKind kind, TypeUsage booleanResultType, DbExpressionBinding input, DbExpression predicate)
            : base(kind, booleanResultType) {
            Input = input;
            Predicate = predicate;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}