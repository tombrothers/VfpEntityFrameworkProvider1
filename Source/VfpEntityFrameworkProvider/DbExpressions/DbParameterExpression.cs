using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbParameterExpression : DbExpression {
        public DbConstantExpression Value { get; private set; }
        public string Name { get; private set; }

        internal DbParameterExpression(TypeUsage type, string name, DbConstantExpression value)
            : base(DbExpressionKind.Parameter, type) {
            Name = name;
            Value = value;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}