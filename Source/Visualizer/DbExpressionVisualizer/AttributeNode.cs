using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;
using System.Windows.Forms;
using VfpEntityFrameworkProvider.DbExpressions;

namespace DbExpressionVisualizer {
    [Serializable]
    public class AttributeNode : TreeNode {
        protected AttributeNode(SerializationInfo info, StreamingContext context)
            : base(info, context) {
        }

        public AttributeNode(object attribute, PropertyInfo propertyInfo, Color color) {
            ForeColor = color;
            Text = propertyInfo.Name + " : " + propertyInfo.PropertyType.ObtainOriginalName();
            ImageIndex = 3;
            SelectedImageIndex = 3;

            var value = propertyInfo.GetValue(attribute, null);

            if (value != null) {
                if (value.GetType().IsGenericType && value.GetType().GetGenericTypeDefinition() == typeof(ReadOnlyCollection<>)) {
                    if ((int)value.GetType().InvokeMember("get_Count", BindingFlags.InvokeMethod, null, value, null, CultureInfo.InvariantCulture) == 0) {
                        Text += " : Empty";
                    }
                    else {
                        foreach (object tree in (IEnumerable)value) {
                            if (tree is DbExpression) {
                                Nodes.Add(new DbExpressionTreeNode(tree, color));
                            }
                            else if (tree is DbSetClause) {
                                Nodes.Add(new DbExpressionTreeNode(tree, color));
                            }
                            else if (tree is MemberAssignment) {
                                Nodes.Add(new DbExpressionTreeNode(((MemberAssignment)tree).Expression, color));
                            }
                        }
                    }
                }
                else if (value is DbExpressionList) {
                    var expressionList = (DbExpressionList)value;

                    if (expressionList.Any()) {
                        foreach (object tree in (IEnumerable)value) {
                            if (tree is DbExpression) {
                                Nodes.Add(new DbExpressionTreeNode(tree, color));
                            }
                            else if (tree is MemberAssignment) {
                                Nodes.Add(new DbExpressionTreeNode(((MemberAssignment)tree).Expression, color));
                            }
                        }
                    }
                    else {
                        Text += " : Empty";
                    }
                }
                else if (value is DbExpression) {
                    Text += ((DbExpression)value).ExpressionKind;
                    Nodes.Add(new DbExpressionTreeNode(value, color));
                }
                else if (value is DbExpressionBinding) {
                    Text += "DbExpressionBinding";
                    Nodes.Add(new DbExpressionTreeNode(value, color));
                }
                else if (value is MethodInfo) {
                    MethodInfo minfo = value as MethodInfo;
                    Text += " : \"" + minfo.ObtainOriginalMethodName() + "\"";
                }
                else if (value is Type) {
                    Type minfo = value as Type;
                    Text += " : \"" + minfo.ObtainOriginalName() + "\"";
                }
                else {
                    Text += " : \"" + value.ToString() + "\"";
                }
            }
            else {
                Text += " : null";
            }
        }
    }
}