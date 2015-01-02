using System;
using System.Diagnostics;
using DbExpressionVisualizer;
using Microsoft.VisualStudio.DebuggerVisualizers;
using VfpEntityFrameworkProvider.DbExpressions;

[assembly: DebuggerVisualizer(typeof(DbExpressionTreeVisualizer), typeof(DbExpressionTreeVisualizerObjectSource), Target = typeof(DbExpression), Description = "DbExpression Tree Visualizer")]

namespace DbExpressionVisualizer {
    public class DbExpressionTreeVisualizer : DialogDebuggerVisualizer {
        private IDialogVisualizerService modalService;

        protected override void Show(IDialogVisualizerService windowService, IVisualizerObjectProvider objectProvider) {
            modalService = windowService;

            if (modalService == null) {
                throw new NotSupportedException("This debugger does not support modal visualizers");
            }

            var container = (DbExpressionTreeContainer)objectProvider.GetObject();
            var treeForm = new TreeWindow(container.Tree);

            modalService.ShowDialog(treeForm);
        }
    }
}