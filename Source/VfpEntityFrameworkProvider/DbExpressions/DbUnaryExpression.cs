using System.Data.Metadata.Edm;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public abstract class DbUnaryExpression : DbExpression {
        public DbExpression Argument { get; private set; }

        protected DbUnaryExpression(DbExpressionKind kind, TypeUsage resultType, DbExpression argument)
            : base(kind, resultType) {
            Argument = argument;
        }
    }
}