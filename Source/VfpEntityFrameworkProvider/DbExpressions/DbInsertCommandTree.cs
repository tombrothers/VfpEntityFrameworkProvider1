using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbInsertCommandTree : DbModificationCommandTree {
        public IList<DbSetClause> SetClauses { get; private set; }
        public DbExpression Returning { get; private set; }

        public override DbCommandTreeKind CommandTreeKind {
            get {
                return DbCommandTreeKind.Insert;
            }
        }

        public DbInsertCommandTree(DbExpressionBinding target, ReadOnlyCollection<DbSetClause> setClauses, IEnumerable<KeyValuePair<string, TypeUsage>> parameters, DbExpression returing)
            : base(target.Expression.ResultType, target, parameters) {
            SetClauses = setClauses;
            Returning = returing;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}