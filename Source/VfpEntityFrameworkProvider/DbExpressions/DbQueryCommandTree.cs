using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbQueryCommandTree : DbCommandTree {
        public DbExpression Query { get; private set; }

        public override DbCommandTreeKind CommandTreeKind {
            get {
                return DbCommandTreeKind.Query;
            }
        }

        public DbQueryCommandTree(DbExpression query, IEnumerable<KeyValuePair<string, TypeUsage>> parameters)
            : base(query.ResultType, parameters) {
            Query = query;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}