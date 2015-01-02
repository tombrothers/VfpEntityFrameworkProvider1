using System;
using VfpEntityFrameworkProvider.DbExpressions;

namespace VfpEntityFrameworkProvider.SqlGeneration {
    internal class BinaryFragment : SqlFragmentBase {
        public DbExpressionKind Kind { get; private set; }
        public ISqlFragment Left { get; private set; }
        public ISqlFragment Right { get; private set; }

        public BinaryFragment(DbExpressionKind kind, ISqlFragment left, ISqlFragment right) : base(SqlFragmentType.Binary) {
            Kind = kind;
            Left = left;
            Right = right;
        }
                
        public override void WriteSql(SqlWriter writer, SqlVisitor visitor) {
            Left.WriteSql(writer, visitor);
            writer.Write(GetOperator(Kind));
            Right.WriteSql(writer, visitor);
        }

        public static string GetOperator(DbExpressionKind kind) {
            switch (kind) {
                case DbExpressionKind.And:
                    return " AND ";
                case DbExpressionKind.Divide:
                    return " / ";
                case DbExpressionKind.Equals:
                    return " = ";
                case DbExpressionKind.GreaterThan:
                    return " > ";
                case DbExpressionKind.GreaterThanOrEquals:
                    return " >= ";
                case DbExpressionKind.LessThan:
                    return " < ";
                case DbExpressionKind.LessThanOrEquals:
                    return " <= ";
                case DbExpressionKind.Minus:
                    return " - ";
                case DbExpressionKind.Modulo:
                    return " % ";
                case DbExpressionKind.Multiply:
                    return " * ";
                case DbExpressionKind.Or:
                    return " OR ";
                case DbExpressionKind.Plus:
                    return " + ";
                case DbExpressionKind.NotEquals:
                    return " <> ";
                default:
                    throw new InvalidOperationException("Invalid ExpressionKind:  " + kind.ToString());
            }
        }
    }
}