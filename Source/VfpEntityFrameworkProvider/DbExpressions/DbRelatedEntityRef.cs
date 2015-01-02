using System.Data.Metadata.Edm;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbRelatedEntityRef {
        public RelationshipEndMember SourceEnd { get; private set; }
        public RelationshipEndMember TargetEnd { get; private set; }
        public DbExpression TargetEntityRef { get; private set; }

        internal DbRelatedEntityRef(RelationshipEndMember sourceEnd, RelationshipEndMember targetEnd, DbExpression targetEntityRef) {
            TargetEntityRef = targetEntityRef;
            TargetEnd = targetEnd;
            SourceEnd = sourceEnd;
        }
    }
}