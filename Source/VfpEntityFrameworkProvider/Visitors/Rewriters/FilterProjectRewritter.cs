using System.Collections.ObjectModel;
using System.Linq;
using VfpEntityFrameworkProvider.DbExpressions;

namespace VfpEntityFrameworkProvider.Visitors.Rewriters {
    internal class FilterProjectRewritter : DbExpressionVisitor {
        public static DbExpression Rewrite(DbExpression expression) {
            var rewriter = new FilterProjectRewritter();

            return rewriter.Visit(expression);
        }

        protected override DbExpressionBinding VisitDbExpressionBinding(DbExpressionBinding binding) {
            binding = base.VisitDbExpressionBinding(binding);

            var filterExpression = binding.Expression as DbFilterExpression;

            if (filterExpression == null) {
                return binding;
            }

            var project = filterExpression.Input.Expression as DbProjectExpression;

            if (project == null) {
                return binding;
            }

            var scan = project.Input.Expression as DbScanExpression;

            if (scan == null) {
                return binding;
            }

            var newInstance = project.Projection as DbNewInstanceExpression;

            if (newInstance == null) {
                return binding;
            }

            var properties = newInstance.Arguments.OfType<DbPropertyExpression>().ToList().AsReadOnly();

            if (!properties.Any()) {
                return binding;
            }

            if (!DbPropertyExpressionChecker.CanRewrite(filterExpression.Predicate)) {
                return binding;
            }

            var newFilterPredicate = DbPropertyExpressionRewriter.Rewrite(filterExpression.Predicate, properties);
            var newFilterExpression = DbExpression.Filter(filterExpression.ResultType, project.Input, newFilterPredicate);
            var newFilterBinding = DbExpression.Binding(newFilterExpression, binding.Variable);
            var newProjection = DbVariableReferenceExpressionRewriter.Rewrite(project.Projection, binding.Variable);
            var newProjectExpression = DbExpression.Project(project.ResultType, newFilterBinding, newProjection);
            var newBinding = DbExpression.Binding(newProjectExpression, binding.Variable);

            return newBinding;
        }

        private class DbVariableReferenceExpressionRewriter : DbExpressionVisitor {
            private readonly DbVariableReferenceExpression _variableReference;

            public static DbExpression Rewrite(DbExpression expression, DbVariableReferenceExpression variableReference) {
                var rewriter = new DbVariableReferenceExpressionRewriter(variableReference);

                return rewriter.Visit(expression);
            }

            public DbVariableReferenceExpressionRewriter(DbVariableReferenceExpression variableReference) {
                _variableReference = variableReference;
            }

            public override DbExpression Visit(DbVariableReferenceExpression expression) {
                return new DbVariableReferenceExpression(_variableReference.ResultType, _variableReference.VariableName);
            }
        }

        private class DbPropertyExpressionRewriter : DbExpressionVisitor {
            private readonly ReadOnlyCollection<DbPropertyExpression> _properties;

            public static DbExpression Rewrite(DbExpression expression, ReadOnlyCollection<DbPropertyExpression> properties) {
                var rewriter = new DbPropertyExpressionRewriter(properties);

                return rewriter.Visit(expression);
            }

            public DbPropertyExpressionRewriter(ReadOnlyCollection<DbPropertyExpression> properties) {
                _properties = properties;
            }

            public override DbExpression Visit(DbPropertyExpression expression) {
                if (!expression.Property.Name.StartsWith("C")) {
                    return expression;
                }

                var index = int.Parse(expression.Property.Name.Substring(1)) - 1;
                var property = _properties[index];

                return DbExpression.Property(property.ResultType, property.Property, property.Instance);
            }
        }

        private class DbPropertyExpressionChecker : DbExpressionVisitor {
            private bool _canRewrite = true;

            public static bool CanRewrite(DbExpression expression) {
                var rewriter = new DbPropertyExpressionChecker();

                rewriter.Visit(expression);

                return rewriter._canRewrite;
            }

            public override DbExpression Visit(DbPropertyExpression expression) {
                int index;

                if (!expression.Property.Name.StartsWith("C") || !int.TryParse(expression.Property.Name.Substring(1), out index)) {
                    _canRewrite = false;

                    return expression;
                }

                return expression;
            }
        }
    }
}