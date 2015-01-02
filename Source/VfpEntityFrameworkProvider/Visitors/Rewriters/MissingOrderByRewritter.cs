using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Linq;
using VfpEntityFrameworkProvider.DbExpressions;
using VfpEntityFrameworkProvider.Visitors.Gatherers;

namespace VfpEntityFrameworkProvider.Visitors.Rewriters {
    /// <summary>
    /// The purpose of this class is to ensure that a "Select Top X" statement includes an "Order By" clause.
    /// </summary>
    internal class MissingOrderByRewritter : DbExpressionVisitor {
        private DbExpression _rootExpression;

        public static DbExpression Rewrite(DbExpression expression) {
            var rewriter = new MissingOrderByRewritter { _rootExpression = expression };

            return rewriter.Visit(expression);
        }

        public override DbExpression Visit(DbLimitExpression expression) {
            var sortExpression = expression.Argument as DbSortExpression;

            if (sortExpression == null) {
                var newLimitExpression = GetDbLimitExpression(expression, expression.Argument as DbProjectExpression) ??
                                         GetDbLimitExpression(expression, expression.Argument as DbFilterExpression) ??
                                         GetDbLimitExpression(expression, expression.Argument as DbScanExpression);

                if (newLimitExpression != null) {
                    return base.Visit(newLimitExpression);
                }
            }

            return base.Visit(expression);
        }

        private DbLimitExpression GetDbLimitExpression(DbLimitExpression limitExpression, DbScanExpression scanExpression) {
            if (scanExpression == null) {
                return null;
            }

            var keyMembers = GetKeyMembers(scanExpression.Target.ElementType.KeyMembers);

            if (!keyMembers.Any()) {
                return null;
            }

            var variableReference = new DbVariableReferenceExpression(scanExpression.ResultType, GetUniqueVariableName());
            var expressionBinding = new DbExpressionBinding(scanExpression, variableReference);
            var sortExpression = GetSortExpression(scanExpression.ResultType, keyMembers, expressionBinding);

            if (sortExpression == null) {
                return null;
            }

            return new DbLimitExpression(limitExpression.ResultType,
                                         sortExpression,
                                         limitExpression.Limit,
                                         limitExpression.WithTies);
        }

        private string GetUniqueVariableName() {
            var list = GetVariableNames();
            var attempt = list.Count(x => x.StartsWith("t"));
            var variableName = "t" + attempt;

            while (list.Contains(variableName)) {
                attempt += 1;
                variableName = "t" + attempt;
            }

            return variableName;
        }

        private IList<string> GetVariableNames() {
            var list = VariableReferenceGatherer.Gather(_rootExpression);

            return list.Select(x => x.VariableName).ToList();
        }

        private static DbLimitExpression GetDbLimitExpression(DbLimitExpression limitExpression, DbFilterExpression filterExpression) {
            if (filterExpression == null) {
                return null;
            }

            var keyMembers = GetKeyMembers(filterExpression.Input);

            if (!keyMembers.Any()) {
                return null;
            }

            var sortExpression = GetSortExpression(filterExpression.ResultType, keyMembers, filterExpression.Input);

            if (sortExpression == null) {
                return null;
            }

            var newFilterExpression = new DbFilterExpression(filterExpression.ResultType,
                                                             new DbExpressionBinding(sortExpression, filterExpression.Input.Variable),
                                                             filterExpression.Predicate);

            return new DbLimitExpression(limitExpression.ResultType,
                                         newFilterExpression,
                                         limitExpression.Limit,
                                         limitExpression.WithTies);
        }

        private static DbLimitExpression GetDbLimitExpression(DbLimitExpression limitExpression, DbProjectExpression projectExpression) {
            if (projectExpression == null) {
                return null;
            }

            var keyMembers = GetKeyMembers(projectExpression.Input);

            if (!keyMembers.Any()) {
                return null;
            }

            var sortExpression = GetSortExpression(projectExpression.ResultType, keyMembers, projectExpression.Input);

            if (sortExpression == null) {
                return null;
            }

            return new DbLimitExpression(limitExpression.ResultType,
                                         sortExpression,
                                         limitExpression.Limit,
                                         limitExpression.WithTies);
        }

        private static DbSortExpression GetSortExpression(TypeUsage typeUsage, IEnumerable<EdmMember> keyMembers, DbExpressionBinding expressionBinding) {
            var list = keyMembers.Select(x => new DbSortClause(new DbPropertyExpression(x.TypeUsage, x, expressionBinding.Variable), true, string.Empty)).ToList();

            return new DbSortExpression(typeUsage, expressionBinding, list.AsReadOnly());
        }

        private static IEnumerable<EdmMember> GetKeyMembers(DbExpressionBinding expressionBinding) {
            return GetKeyMembers(expressionBinding.VariableType.EdmType);
        }

        private static IEnumerable<EdmMember> GetKeyMembers(EdmType edmType) {
            var entityType = edmType as EntityType;

            if (entityType == null) {
                return new EdmMember[] { };
            }

            return GetKeyMembers(entityType.MetadataProperties);
        }

        private static IEnumerable<EdmMember> GetKeyMembers(IEnumerable<MetadataProperty> properties) {
            if (properties == null) {
                return new EdmMember[] { };
            }

            return properties.Where(x => x.Name == "KeyMembers").SelectMany(x => GetKeyMembers(x.Value as ReadOnlyMetadataCollection<EdmMember>));
        }

        private static IEnumerable<EdmMember> GetKeyMembers(ReadOnlyCollection<EdmMember> properties) {
            if (properties == null) {
                return new EdmMember[] { };
            }

            return properties;
        }
    }
}