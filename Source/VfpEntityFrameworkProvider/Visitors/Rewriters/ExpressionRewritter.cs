using VfpEntityFrameworkProvider.DbExpressions;
using VfpEntityFrameworkProvider.Visitors.Removers;

namespace VfpEntityFrameworkProvider.Visitors.Rewriters {
    internal class ExpressionRewritter {
        public static DbExpression Rewrite(VfpProviderManifest vfpManifest, DbExpression expression) {
            expression = RedundantDbCaseExpressionRemover.Remove(expression);

            expression = FlattenFilterRewritter.Rewrite(expression);
            expression = ApplyRewritter.Rewrite(expression);
            expression = InListRewritter.Rewrite(expression);
            expression = XmlToCursorRewritter.Rewrite(expression);
            expression = XmlToCursorMoveToInnerExpressionRewriter.Rewrite(expression);
            expression = XmlToCursorJoinRewriter.Rewrite(expression);

            expression = ComparisonRewritter.Rewrite(expression);
            expression = LikeRewritter.Rewrite(expression);
            expression = LikeCRewritter.Rewrite(expression);
            expression = CaseWithNullRewriter.Rewrite(expression);

            expression = SingleRowTableRewritter.Rewrite(expression);
            expression = MissingOrderByRewritter.Rewrite(expression);
            expression = VariableReferenceRewritter.Rewrite(expression);
            expression = ConstantToParameterRewritter.Rewrite(expression);
            expression = FilterProjectRewritter.Rewrite(expression);

            expression = DecimalPropertyRewritter.Rewrite(vfpManifest, expression);

            return expression;
        }
    }
}