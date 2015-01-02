using System.Collections.ObjectModel;
using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbSkipExpression : DbExpression {
        public DbExpression Count { get; private set; }
        public DbExpressionBinding Input { get; private set; }
        public ReadOnlyCollection<DbSortClause> SortOrder { get; private set; }

        internal DbSkipExpression(TypeUsage resultType, DbExpressionBinding input, ReadOnlyCollection<DbSortClause> sortOrder, DbExpression count)
            : base(DbExpressionKind.Skip, resultType) {
            Input = input;
            SortOrder = sortOrder;
            Count = count;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}