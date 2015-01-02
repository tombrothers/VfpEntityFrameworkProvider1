using System.Data.Metadata.Edm;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbExpressionBinding {
        public DbExpression Expression { get; private set; }
        public DbVariableReferenceExpression Variable { get; private set; }

        public string VariableName {
            get {
                return Variable.VariableName;
            }
        }

        public TypeUsage VariableType {
            get {
                return Variable.ResultType;
            }
        }

        internal DbExpressionBinding(DbExpression expression, DbVariableReferenceExpression variableReference) {
            Expression = expression;
            Variable = variableReference;
        }
    }
}