using System.Collections.ObjectModel;
using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbNewInstanceExpression : DbExpression {
        public DbExpressionList Arguments { get; private set; }
        public ReadOnlyCollection<DbRelatedEntityRef> Relationships { get; private set; }

        internal DbNewInstanceExpression(TypeUsage type, DbExpressionList arguments, ReadOnlyCollection<DbRelatedEntityRef> relationships = null)
            : base(DbExpressionKind.NewInstance, type) {
            Arguments = arguments;

            if (relationships != null && relationships.Count > 0) {
                Relationships = relationships;
            }
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}