using System;

namespace DbExpressionVisualizer {
    [Serializable]
    public class DbExpressionTreeContainer {
        public DbExpressionTreeNode Tree { get; private set; }

        public DbExpressionTreeContainer(DbExpressionTreeNode tree) {
            Tree = tree;
        }
    }
}