using VfpEntityFrameworkProvider.DbExpressions;

namespace VfpEntityFrameworkProvider {
    internal static class DbExpressionExtensions {
        public static bool IsPropertyOverVarRef(this DbExpression expression) {
            var propertyExpression = expression as DbPropertyExpression;

            if (propertyExpression == null) {
                return false;
            }

            var varRefExpression = propertyExpression.Instance as DbVariableReferenceExpression;

            return varRefExpression != null;
        }

        public static bool IsJoinExpression(this DbExpression e) {
            return DbExpressionKind.CrossJoin == e.ExpressionKind ||
                   DbExpressionKind.FullOuterJoin == e.ExpressionKind ||
                   DbExpressionKind.InnerJoin == e.ExpressionKind ||
                   DbExpressionKind.LeftOuterJoin == e.ExpressionKind;
        }

        public static bool IsApplyExpression(this DbExpression expression) {
            return expression.ExpressionKind == DbExpressionKind.CrossApply || expression.ExpressionKind == DbExpressionKind.OuterApply;
        }

        public static bool IsComplexExpression(this DbExpression expression) {
            return !(expression.ExpressionKind == DbExpressionKind.Constant || 
                     expression.ExpressionKind == DbExpressionKind.ParameterReference || 
                     expression.ExpressionKind == DbExpressionKind.Property);
        }
    }
}