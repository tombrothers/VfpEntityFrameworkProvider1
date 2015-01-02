using DbExpressionVisualizer;
using Microsoft.VisualStudio.DebuggerVisualizers;
using VfpEntityFrameworkProvider.DbExpressions;

namespace DbExpressionTreeVisualizerTest {
    internal class Program {
        public static void Main(string[] args) {
            var left = DbExpression.Constant(0);
            var right = DbExpression.Constant(0);
            var binary = DbExpression.And(left, right);
            var host = new VisualizerDevelopmentHost(binary,
                                                     typeof(DbExpressionTreeVisualizer),
                                                     typeof(DbExpressionTreeVisualizerObjectSource));

            host.ShowVisualizer();
        }
    }
}