using System.Data.Metadata.Edm;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public abstract class DbAggregate {
        public TypeUsage ResultType { get; private set; }
        public DbExpressionList Arguments { get; private set; }

        internal DbAggregate(TypeUsage resultType, DbExpressionList arguments) {
            ResultType = resultType;
            Arguments = arguments;
        }
    }
}