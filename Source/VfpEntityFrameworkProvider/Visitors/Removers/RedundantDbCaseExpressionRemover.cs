using System.Data.Metadata.Edm;
using VfpEntityFrameworkProvider.DbExpressions;

namespace VfpEntityFrameworkProvider.Visitors.Removers {
    /*
     * Example
     * Before:
     * (ICASE((LEN(Extent1.ContactName)) > 0,.T., NOT ((LEN(Extent1.ContactName)) > 0),.F.)
     * 
     * After:
     * LEN(Extent1.ContactName)) > 0
     * */
    internal class RedundantDbCaseExpressionRemover : DbExpressionVisitor {
        public static DbExpression Remove(DbExpression expression) {
            return new RedundantDbCaseExpressionRemover().Visit(expression);
        }

        public override DbExpression Visit(DbCaseExpression expression) {
            return IsExpectedCaseExpression(expression) ? base.Visit(expression.When[0]) : base.Visit(expression);
        }

        private static bool IsExpectedCaseExpression(DbCaseExpression expression) {
            if (expression == null || expression.When.Count != 2 || expression.Then.Count != 2) {
                return false;
            }

            var constant1 = expression.Then[0] as DbConstantExpression;

            if (constant1 == null || constant1.ConstantKind != PrimitiveTypeKind.Boolean) {
                return false;
            }

            var constant2 = expression.Then[1] as DbConstantExpression;

            if (constant2 == null || constant2.ConstantKind != PrimitiveTypeKind.Boolean) {
                return false;
            }

            if (!(bool)constant1.Value) {
                return false;
            }

            if ((bool)constant2.Value) {
                return false;
            }

            return true;
        }
    }
}