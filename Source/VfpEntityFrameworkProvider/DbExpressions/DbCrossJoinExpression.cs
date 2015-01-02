using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbCrossJoinExpression : DbExpression {
        public IList<DbExpressionBinding> Inputs { get; private set; }

        internal DbCrossJoinExpression(TypeUsage collectionOfRowResultType, ReadOnlyCollection<DbExpressionBinding> inputs)
            : base(DbExpressionKind.CrossJoin, collectionOfRowResultType) {
            Inputs = inputs;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}