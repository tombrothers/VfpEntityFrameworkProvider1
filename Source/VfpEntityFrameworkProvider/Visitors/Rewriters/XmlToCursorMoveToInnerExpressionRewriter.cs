using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Metadata.Edm;
using System.Linq;
using System.Text;
using VfpEntityFrameworkProvider.DbExpressions;
using VfpEntityFrameworkProvider.Visitors.Gatherers;
using VfpEntityFrameworkProvider.Visitors.Removers;

namespace VfpEntityFrameworkProvider.Visitors.Rewriters {
    /*
     * Moves the XmlToCursor statment to the table instead of the outer most join
     

Before Example: 
     
    SELECT E1.OrderID, E4.ProductID, CAST( J1.Freight AS n(20,4)) AS Freight ;
        FROM   OrderDetails E1 ;
        INNER JOIN  (SELECT E2.OrderID OrderID1, E2.CustomerID CustomerID, E2.EmployeeID EmployeeID, E2.OrderDate OrderDate, E2.RequiredDate RequiredDate, E2.ShippedDate ShippedDate, E2.Freight Freight, E2.ShipName ShipName, E2.ShipAddress ShipAddress, E2.ShipCity ShipCity, E2.ShipRegion ShipRegion, E2.ShipPostalCode ShipPostalCode, E2.ShipCountry ShipCountry, E3.OrderID OrderID2, E3.CustomsDescription CustomsDescription, E3.ExciseTax ExciseTax ;
	        FROM  Orders E2 ;
	        LEFT JOIN InternationalOrders E3 ON E2.OrderID = E3.OrderID ) J1 ON E1.OrderID = J1.OrderID1 ;
        INNER JOIN Products E4 ON E1.ProductID = E4.ProductID ;
        WHERE J1.CustomerID IN (SELECT Id FROM (iif(XmlToCursor(__vfpClient___XmlToCursor1, 'curXml1') > 0, 'curXml1', '')))
      
After Example:
    SELECT E1.OrderID, E4.ProductID, CAST( J1.Freight AS n(20,4)) AS Freight ;
        FROM   OrderDetails E1 ;
        INNER JOIN  (SELECT E2.OrderID OrderID1, E2.CustomerID CustomerID, E2.EmployeeID EmployeeID, E2.OrderDate OrderDate, E2.RequiredDate RequiredDate, E2.ShippedDate ShippedDate, E2.Freight Freight, E2.ShipName ShipName, E2.ShipAddress ShipAddress, E2.ShipCity ShipCity, E2.ShipRegion ShipRegion, E2.ShipPostalCode ShipPostalCode, E2.ShipCountry ShipCountry, E3.OrderID OrderID2, E3.CustomsDescription CustomsDescription, E3.ExciseTax ExciseTax ;
	                    FROM   (SELECT * ;
		                            FROM Orders MX0 ;
		                            WHERE MX0.CustomerID IN (SELECT Id FROM (iif(XmlToCursor(__vfpClient___XmlToCursor1, 'curXml1_x') > 0, 'curXml1_x', ''))) ) E2 ;
	                    LEFT JOIN InternationalOrders E3 ON E2.OrderID = E3.OrderID ) J1 ON E1.OrderID = J1.OrderID1 ;
        INNER JOIN Products E4 ON E1.ProductID = E4.ProductID
     
     */
    internal class XmlToCursorMoveToInnerExpressionRewriter : DbExpressionVisitor {
        public const string CursorNamePrefix = "MX";

        private int _count;
        private readonly IDictionary<string, List<XmlToCursorData>> _xmlToCursors;
        private readonly List<string> _xmlToCursorsToBeRemoved = new List<string>();

        private XmlToCursorMoveToInnerExpressionRewriter(DbExpression expression) {
            _xmlToCursors = GetXmlToCursorsThatHaveNestedProperties(expression).GroupBy(x => x.TableProperty.Property.Name, x => x)
                                                                               .ToDictionary(x => x.Key, x => x.ToList());
        }

        private ReadOnlyCollection<XmlToCursorData> GetXmlToCursorsThatHaveNestedProperties(DbExpression expression) {
            return XmlToCursorExpressionGatherer.Gather(expression)
                                                .Select(x => new XmlToCursorData(x))
                                                .Where(x => x.TableProperty != null)
                                                .Where(x => x.TableProperty.Instance is DbPropertyExpression)
                                                .Select(x => x).ToList().AsReadOnly();
        }

        internal static DbExpression Rewrite(DbExpression expression) {
            var rewriter = new XmlToCursorMoveToInnerExpressionRewriter(expression);

            if (!rewriter._xmlToCursors.Any()) {
                return expression;
            }

            expression = rewriter.Visit(expression);

            foreach (var cursorName in rewriter._xmlToCursorsToBeRemoved) {
                expression = XmlToCursorExpressionRemover.Remove(expression, cursorName);
            }

            return expression;
        }

        protected override DbExpressionBinding VisitDbExpressionBinding(DbExpressionBinding binding) {
            binding = base.VisitDbExpressionBinding(binding);

            if (!_xmlToCursors.ContainsKey(binding.VariableName)) {
                return binding;
            }

            var xmlToCursors = _xmlToCursors[binding.VariableName];

            if (!xmlToCursors.Any()) {
                return binding;
            }

            var scan = binding.Expression as DbScanExpression;

            if (scan == null) {
                return binding;
            }

            var scanVariable = DbExpression.VariableRef(binding.VariableType, CursorNamePrefix + (_count++));
            var scanBinding = DbExpression.Binding(scan, scanVariable);
            DbExpression predicate = null;

            foreach (var xmlToCursor in xmlToCursors) {
                var scanProperty = DbExpression.Property(xmlToCursor.ColumnProperty.ResultType, xmlToCursor.ColumnProperty.Property, scanVariable);
                var xmlToCursorExpression = DbExpression.XmlToCursor(scanProperty, xmlToCursor.XmlToCursor.Parameter, CursorNamePrefix + xmlToCursor.XmlToCursor.CursorName, xmlToCursor.XmlToCursor.ItemType);

                _xmlToCursorsToBeRemoved.Add(xmlToCursor.XmlToCursor.CursorName);

                if (predicate == null) {
                    predicate = xmlToCursorExpression;
                }
                else {
                    predicate = DbExpression.And(predicate, xmlToCursorExpression);
                }
            }

            var filter = DbExpression.Filter(binding.VariableType, scanBinding, predicate);
            var filterBinding = DbExpression.Binding(filter, binding.Variable);

            return filterBinding;
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