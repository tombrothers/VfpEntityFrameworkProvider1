using System.Data.Metadata.Edm;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbGroupExpressionBinding {
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

        public DbVariableReferenceExpression GroupVariable { get; private set; }

        public string GroupVariableName {
            get {
                return GroupVariable.VariableName;
            }
        }

        public TypeUsage GroupVariableType {
            get {
                return GroupVariable.ResultType;
            }
        }

        internal DbGroupExpressionBinding(DbExpression expression, DbVariableReferenceExpression variable, DbVariableReferenceExpression groupVariable) {
            Expression = expression;
            Variable = variable;
            GroupVariable = groupVariable;
        }
    }
}