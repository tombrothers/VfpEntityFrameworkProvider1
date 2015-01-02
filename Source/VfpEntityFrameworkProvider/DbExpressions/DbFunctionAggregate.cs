using System.Data.Metadata.Edm;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbFunctionAggregate : DbAggregate {
        public EdmFunction Function { get; private set; }
        public bool Distinct { get; private set; }

        internal DbFunctionAggregate(TypeUsage resultType, DbExpressionList arguments, EdmFunction function, bool isDistinct)
            : base(resultType, arguments) {
            Function = function;
            Distinct = isDistinct;
        }
    }
}