using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbUpdateCommandTree : DbModificationCommandTree {
        public IList<DbSetClause> SetClauses { get; private set; }
        public DbExpression Predicate { get; private set; }
        public DbExpression Returning { get; private set; }

        public override DbCommandTreeKind CommandTreeKind {
            get {
                return DbCommandTreeKind.Update;
            }
        }

        public DbUpdateCommandTree(DbExpressionBinding target,
                                   ReadOnlyCollection<DbSetClause> setClauses,
                                   DbExpression predicate,
                                   DbExpression returning,
                                   IEnumerable<KeyValuePair<string, TypeUsage>> parameters)
            : base(target.Expression.ResultType, target, parameters) {
            SetClauses = setClauses;
            Predicate = predicate;
            Returning = returning;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}