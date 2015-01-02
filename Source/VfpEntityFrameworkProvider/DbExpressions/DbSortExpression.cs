using System.Collections.ObjectModel;
using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbSortExpression : DbExpression {
        public DbExpressionBinding Input { get; private set; }
        public ReadOnlyCollection<DbSortClause> SortOrder { get; private set; }

        internal DbSortExpression(TypeUsage resultType, DbExpressionBinding input, ReadOnlyCollection<DbSortClause> sortOrder)
            : base(DbExpressionKind.Sort, resultType) {
            Input = input;
            SortOrder = sortOrder;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}