using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbExpressionList : ReadOnlyCollection<DbExpression> {
        internal DbExpressionList(IList<DbExpression> elements)
            : base(elements) {
        }
    }
}