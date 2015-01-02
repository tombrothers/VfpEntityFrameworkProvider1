using System.Collections.Generic;
using System.Data.Metadata.Edm;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public abstract class DbModificationCommandTree : DbCommandTree {
        public DbExpressionBinding Target { get; private set; }

        protected DbModificationCommandTree(TypeUsage resultType, DbExpressionBinding target, IEnumerable<KeyValuePair<string, TypeUsage>> parameters)
            : base(resultType, parameters) {
            Target = target;
        }
    }
}