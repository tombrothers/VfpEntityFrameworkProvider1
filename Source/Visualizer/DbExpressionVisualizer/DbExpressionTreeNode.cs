using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;
using VfpEntityFrameworkProvider.DbExpressions;

namespace DbExpressionVisualizer {
    [Serializable]
    public class DbExpressionTreeNode : TreeNode {
        private readonly string _namespace = typeof(DbExpression).Namespace;

        protected DbExpressionTreeNode(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }

        public DbExpressionTreeNode(object value, Color? color = null) {
            var dbExpression = value as DbExpression;
            color = color ?? Color.Black;

            if (dbExpression != null) {
                switch (dbExpression.ExpressionKind) {
                    case DbExpressionKind.XmlToCursor:
                        color = Color.Blue;
                        break;
                    case DbExpressionKind.Constant:
                        color = Color.Crimson;
                        break;
                    case DbExpressionKind.Property:
                        color = Color.Purple;
                        break;
                    case DbExpressionKind.Scan:
                        color = Color.SeaGreen;
                        break;
                    case DbExpressionKind.VariableReference:
                        color = Color.Violet;
                        break;
                }
            }

            ForeColor = color.Value;

            var type = value.GetType();
            Text = type.ObtainOriginalName();

            if (type.Namespace == _namespace) {
                foreach (var propertyInfo in GetProperties(type)) {
                    Nodes.Add(new AttributeNode(value, propertyInfo, color.Value));
                }
            }
            else {
                Text = "\"" + value + "\"";
            }
        }

        private IEnumerable<PropertyInfo> GetProperties(Type type) {
            return type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                       .Where(x => x.Name != "ExpressionKind");
        }
    }
}