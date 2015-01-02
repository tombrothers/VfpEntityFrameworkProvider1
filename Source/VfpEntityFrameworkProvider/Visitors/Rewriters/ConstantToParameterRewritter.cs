using System.Data.Metadata.Edm;
using VfpEntityFrameworkProvider.DbExpressions;

namespace VfpEntityFrameworkProvider.Visitors.Rewriters {
    internal class ConstantToParameterRewritter : DbExpressionVisitor {
        private int _count;

        public static DbExpression Rewrite(DbExpression expression) {
            var rewriter = new ConstantToParameterRewritter();

            return rewriter.Visit(expression);
        }

        public override DbExpression Visit(DbParameterExpression expression) {
            return expression;
        }

        public override DbExpression Visit(DbConstantExpression expression) {
            switch (expression.ResultType.ToPrimitiveTypeKind()) {
                case PrimitiveTypeKind.Binary:
                case PrimitiveTypeKind.Decimal:
                case PrimitiveTypeKind.Double:
                case PrimitiveTypeKind.Single:
                case PrimitiveTypeKind.String:
                    return new DbParameterExpression(expression.ResultType, GetParameterName(), expression);
                default:
                    return base.Visit(expression);
            }
        }

        private string GetParameterName() {
            return "@__C2P__" + (++_count);
        }
    }
}