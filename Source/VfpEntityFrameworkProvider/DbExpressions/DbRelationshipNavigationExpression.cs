using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbRelationshipNavigationExpression : DbExpression {
        public DbExpression NavigationSource { get; private set; }
        public RelationshipType Relationship { get; private set; }
        public RelationshipEndMember NavigateTo { get; private set; }
        public RelationshipEndMember NavigateFrom { get; private set; }

        internal DbRelationshipNavigationExpression(TypeUsage resultType, RelationshipType relType, RelationshipEndMember fromEnd, RelationshipEndMember toEnd, DbExpression navigateFrom)
            : base(DbExpressionKind.RelationshipNavigation, resultType) {
            Relationship = relType;
            NavigateFrom = fromEnd;
            NavigateTo = toEnd;
            NavigationSource = navigateFrom;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}