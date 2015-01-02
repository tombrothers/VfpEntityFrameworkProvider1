using System.Collections.Generic;
using System.Data.Metadata.Edm;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public abstract class DbCommandTree : DbExpression {
        public IEnumerable<KeyValuePair<string, TypeUsage>> Parameters { get; private set; }
        public abstract DbCommandTreeKind CommandTreeKind { get; }

        internal DbCommandTree(TypeUsage resultType, IEnumerable<KeyValuePair<string, TypeUsage>> parameters)
            : base(DbExpressionKind.CommandTree, resultType) {
            Parameters = parameters;
        }
    }
}