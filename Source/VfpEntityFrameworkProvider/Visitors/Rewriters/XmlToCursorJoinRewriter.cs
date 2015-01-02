using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Metadata.Edm;
using System.Linq;
using VfpEntityFrameworkProvider.DbExpressions;
using VfpEntityFrameworkProvider.Visitors.Gatherers;
using VfpEntityFrameworkProvider.Visitors.Removers;
using VfpEntityFrameworkProvider.Visitors.Replacers;

namespace VfpEntityFrameworkProvider.Visitors.Rewriters {
    internal class XmlToCursorJoinRewriter : DbExpressionVisitor {
        private int _count;
        private readonly Stack<DbExpressionBinding> _filterBindings = new Stack<DbExpressionBinding>();
        private readonly List<string> _xmlToCursorsToBeRemoved = new List<string>();
        private readonly IDictionary<string, List<XmlToCursorData>> _xmlToCursors;

        private XmlToCursorJoinRewriter(DbExpression expression) {
            _xmlToCursors = GetXmlToCursors(expression).GroupBy(x => x.TableProperty.Property.Name, x => x)
                                                       .ToDictionary(x => x.Key, x => x.ToList());
        }

        private ReadOnlyCollection<XmlToCursorData> GetXmlToCursors(DbExpression expression) {
            return XmlToCursorExpressionGatherer.Gather(expression)
                                                .Where(x => !x.CursorName.StartsWith(XmlToCursorMoveToInnerExpressionRewriter.CursorNamePrefix)) // TODO:  figure out how to rewrite these as inner joins
                                                .Select(x => new XmlToCursorData(x))
                                                .Where(x => x.TableProperty != null)
                                                .Select(x => x).ToList().AsReadOnly();
        }

        internal static DbExpression Rewrite(DbExpression expression) {
            var rewriter = new XmlToCursorJoinRewriter(expression);

            expression = rewriter.Visit(expression);

            return expression;
        }

        protected override DbExpressionBinding VisitDbExpressionBinding(DbExpressionBinding binding) {
            var isFilterBinding = binding.Expression is DbFilterExpression;

            if (isFilterBinding) {
                _filterBindings.Push(binding);
            }

            binding = base.VisitDbExpressionBinding(binding);

            if (isFilterBinding) {
                var expression = binding.Expression;

                foreach (var cursorName in _xmlToCursorsToBeRemoved) {
                    expression = XmlToCursorExpressionRemover.Remove(expression, cursorName);
                }

                _xmlToCursorsToBeRemoved.Clear();
                _filterBindings.Pop();

                binding = DbExpression.Binding(expression, binding.Variable);
            }

            if (!_filterBindings.Any()) {
                return binding;
            }

            var filterBinding = _filterBindings.Peek();

            if (filterBinding == null) {
                return binding;
            }

            var scan = binding.Expression as DbScanExpression;

            if (scan == null) {
                return binding;
            }

            if (!_xmlToCursors.ContainsKey(binding.VariableName)) {
                return binding;
            }

            var xmlToCursors = _xmlToCursors[binding.VariableName];

            if (!xmlToCursors.Any()) {
                return binding;
            }

            foreach (var xmlToCursor in xmlToCursors) {
                var xmlToCursorExpression = xmlToCursor.XmlToCursor;

                _count++;

                var variableReference = DbExpression.VariableRef(xmlToCursorExpression.ItemType.ToTypeUsage(), "Xml" + _count);
                var xmlToCursorScan = DbExpression.XmlToCursorScan(xmlToCursorExpression.Parameter, xmlToCursorExpression.CursorName + "_j");
                var xmlToCursorBinding = DbExpression.Binding(xmlToCursorScan, variableReference);
                var xmlToCursorProperty = DbExpression.XmlToCursorProperty(variableReference.ResultType, variableReference);
                var scanProperty = GetScanProperty(binding.Variable, xmlToCursorExpression.Property);
                var comparison = DbExpression.Comparison(DbExpressionKind.Equals,
                                                         PrimitiveTypeKind.Boolean.ToTypeUsage(),
                                                         scanProperty,
                                                         xmlToCursorProperty);

                var joinVariableReference = DbExpression.VariableRef(binding.VariableType, binding.VariableName);
                var joinExpression = DbExpression.Join(DbExpressionKind.InnerJoin, scan.ResultType, binding, xmlToCursorBinding, comparison);

                _xmlToCursorsToBeRemoved.Add(xmlToCursorExpression.CursorName);

                binding = DbExpression.Binding(joinExpression, joinVariableReference);
            }

            return binding;
        }

        private static DbExpression GetScanProperty(DbVariableReferenceExpression variable, DbExpression property) {
            var expression = VariableReferenceReplacer.Replace(variable, property);
            var scanProperty = expression as DbPropertyExpression;

            if (scanProperty == null) {
                return expression;
            }

            var variables = VariableReferenceGatherer.Gather(expression);

            if (!variables.Any()) {
                return expression;
            }

            var scanVariable = variables.First();

            return DbExpression.Property(scanProperty.ResultType, scanProperty.Property, scanVariable);
        }

        private class XmlToCursorData {
            public DbXmlToCursorExpression XmlToCursor { get; private set; }
            public DbPropertyExpression ColumnProperty { get; private set; }
            public DbPropertyExpression TableProperty { get; private set; }

            public XmlToCursorData(DbXmlToCursorExpression expression) {
                ArgumentUtility.CheckNotNull("expression", expression);

                XmlToCursor = expression;
                ColumnProperty = expression.Property as DbPropertyExpression;

                if (ColumnProperty == null) {
                    return;
                }

                TableProperty = ColumnProperty.Instance as DbPropertyExpression;
            }
        }
    }
}