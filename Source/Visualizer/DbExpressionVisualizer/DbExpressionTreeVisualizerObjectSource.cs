using System.IO;
using Microsoft.VisualStudio.DebuggerVisualizers;
using VfpEntityFrameworkProvider.DbExpressions;

namespace DbExpressionVisualizer {
    public class DbExpressionTreeVisualizerObjectSource : VisualizerObjectSource {
        public override void GetData(object target, Stream outgoingData) {
            var expr = (DbExpression)target;
            var browser = new DbExpressionTreeNode(expr);
            var container = new DbExpressionTreeContainer(browser);

            Serialize(outgoingData, container);
        }
    }
}