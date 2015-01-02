using System.Data.Metadata.Edm;
using System.Linq;
using VfpEntityFrameworkProvider.DbExpressions;

namespace VfpEntityFrameworkProvider.Visitors.Rewriters {
    internal class DecimalPropertyRewritter : DbExpressionVisitor {
        private VfpProviderManifest _vfpManifest;

        public DecimalPropertyRewritter(VfpProviderManifest vfpManifest) {
            _vfpManifest = vfpManifest;
        }

        public static DbExpression Rewrite(VfpProviderManifest vfpManifest, DbExpression expression) {
            var rewriter = new DecimalPropertyRewritter(vfpManifest);

            return rewriter.Visit(expression);
        }

        public override DbExpression Visit(DbNewInstanceExpression expression) {
            var arguments = expression.Arguments.Select(GetDbExpression).ToList();

            return new DbNewInstanceExpression(expression.ResultType,
                                               VisitDbExpressionList(arguments),
                                               CreateDbRelatedEntityRefList(expression.Relationships));
        }

        private DbExpression GetDbExpression(DbExpression expression) {
            if (!IsDecimalPropertyExpression(expression)) {
                return expression;
            }

            var scale = GetScale(expression.ResultType);

            if (scale == 0) {
                return expression;
            }

            var castTypeUsage = _vfpManifest.GetDecimalTypeUsage(20, scale);

            return DbExpression.Cast(castTypeUsage, expression);
        }

        private static bool IsDecimalPropertyExpression(DbExpression expression) {
            if (expression == null || expression.ExpressionKind != DbExpressionKind.Property) {
                return false;
            }

            return expression.ResultType.IsPrimitiveType() && expression.ResultType.ToPrimitiveTypeKind() == PrimitiveTypeKind.Decimal;
        }

        private static byte GetScale(TypeUsage typeUsage) {
            byte scale;

            if (!typeUsage.TryGetScale(out scale)) {
                scale = 0;
            }

            return scale;
        }
    }
}