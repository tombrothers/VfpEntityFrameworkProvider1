using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbDeleteCommandTree : DbModificationCommandTree {
        public DbExpression Predicate { get; private set; }

        public override DbCommandTreeKind CommandTreeKind {
            get {
                return DbCommandTreeKind.Delete;
            }
        }

        public DbDeleteCommandTree(DbExpressionBinding target, DbExpression predicate, IEnumerable<KeyValuePair<string, TypeUsage>> parameters)
            : base(target.Expression.ResultType, target, parameters) {
            Predicate = predicate;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}