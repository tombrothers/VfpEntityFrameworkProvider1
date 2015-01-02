using System.Data.Metadata.Edm;
using System.Linq;
using System.Text;
using VfpClient.Utils;
using VfpEntityFrameworkProvider.DbExpressions;

namespace VfpEntityFrameworkProvider.Visitors.Rewriters {
    internal class XmlToCursorRewritter : DbExpressionVisitor {
        private int _count;

        public static DbExpression Rewrite(DbExpression expression) {
            return new XmlToCursorRewritter().Visit(expression);
        }

        public override DbExpression Visit(DbInListExpression expression) {
            const int convertToXmltocursorMintextLength = 200;

            var arrayExpression = expression.GetArrayExpression();

            if (arrayExpression == null) {
                return base.Visit(expression);
            }

            if (arrayExpression.Values.Any(x => x.ExpressionKind != DbExpressionKind.Constant)) {
                return base.Visit(expression);
            }

            var values = new StringBuilder(convertToXmltocursorMintextLength);
            var array = arrayExpression.Values.Cast<DbConstantExpression>().Select(x => x.Value).Distinct().ToArray();

            foreach (var item in array) {
                values.Append(item);

                if (values.Length > convertToXmltocursorMintextLength) {
                    break;
                }
            }

            if (values.Length > convertToXmltocursorMintextLength) {
                _count++;

                var arrayXmlToCursor = new ArrayXmlToCursor(array);
                var xml = DbExpression.Constant(arrayXmlToCursor.Xml);
                var parameter = DbExpression.Parameter(PrimitiveTypeKind.String.ToTypeUsage(), "@__XmlToCursor" + _count, xml);
                var cursorName = "curXml" + _count;
                var xmlToCursor = DbExpression.XmlToCursor(expression.Property, parameter, cursorName, arrayXmlToCursor.ItemType);

                return base.Visit(xmlToCursor);
            }

            return base.Visit(expression);
        }
    }
}