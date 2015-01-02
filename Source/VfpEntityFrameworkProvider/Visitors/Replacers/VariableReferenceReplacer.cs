using VfpEntityFrameworkProvider.DbExpressions;

namespace VfpEntityFrameworkProvider.Visitors.Replacers {
    internal class VariableReferenceReplacer : DbExpressionVisitor {
        private readonly DbVariableReferenceExpression _variableReferenceExpression;

        public static DbExpression Replace(DbVariableReferenceExpression variableReferenceExpression, DbExpression expression) {
            var rewriter = new VariableReferenceReplacer(variableReferenceExpression);

            return rewriter.Visit(expression);
        }

        public VariableReferenceReplacer(DbVariableReferenceExpression variableReferenceExpression) {
            _variableReferenceExpression = variableReferenceExpression;
        }


        public override DbExpression Visit(DbVariableReferenceExpression expression) {
            return _variableReferenceExpression;
        }
    }
}