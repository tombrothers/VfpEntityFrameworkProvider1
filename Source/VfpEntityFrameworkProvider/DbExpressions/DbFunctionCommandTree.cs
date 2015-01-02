using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Diagnostics;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbFunctionCommandTree : DbCommandTree {
        public EdmFunction EdmFunction { get; private set; }

        public override DbCommandTreeKind CommandTreeKind {
            get {
                return DbCommandTreeKind.Function;
            }
        }

        public DbFunctionCommandTree(EdmFunction edmFunction, TypeUsage resultType, IEnumerable<KeyValuePair<string, TypeUsage>> parameters)
            : base(resultType, parameters) {
            EdmFunction = edmFunction;
        }

        [DebuggerStepThrough]
        public override TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor) {
            return ArgumentUtility.CheckNotNull("visitor", visitor).Visit(this);
        }
    }
}