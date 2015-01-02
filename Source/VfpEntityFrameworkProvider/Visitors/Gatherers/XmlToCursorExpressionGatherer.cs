using System.Collections.Generic;
using System.Collections.ObjectModel;
using VfpEntityFrameworkProvider.DbExpressions;

namespace VfpEntityFrameworkProvider.Visitors.Gatherers {
    internal class XmlToCursorExpressionGatherer : DbExpressionVisitor {
        private bool _canRewrite = true;

        private readonly List<DbXmlToCursorExpression> _expressions = new List<DbXmlToCursorExpression>();

        public static ReadOnlyCollection<DbXmlToCursorExpression> Gather(DbExpression expression) {
            var visitor = new XmlToCursorExpressionGatherer();

            visitor.Visit(expression);

            if (!visitor._canRewrite) {
                visitor._expressions.Clear();
            }

            return visitor._expressions.AsReadOnly();
        }

        public override DbExpression Visit(DbNotExpression expression) {
            if (expression.Argument is DbXmlToCursorExpression) {
                _canRewrite = false;
            }

            return base.Visit(expression);
        }

        public override DbExpression Visit(DbOrExpression expression) {
            _canRewrite = false;

            return base.Visit(expression);
        }

        public override DbExpression Visit(DbXmlToCursorExpression expression) {
            _expressions.Add(expression);

            return base.Visit(expression);
        }
    }
}