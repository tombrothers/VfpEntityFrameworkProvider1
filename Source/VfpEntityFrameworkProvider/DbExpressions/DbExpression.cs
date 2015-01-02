using System;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Linq;
using VfpEntityFrameworkProvider.Visitors;

namespace VfpEntityFrameworkProvider.DbExpressions {
    public abstract class DbExpression {
        public DbExpressionKind ExpressionKind { get; private set; }
        public TypeUsage ResultType { get; private set; }

        internal DbExpression(DbExpressionKind expressionKind, TypeUsage resultType) {
            ExpressionKind = expressionKind;
            ResultType = resultType;
        }

        public abstract TResultType Accept<TResultType>(DbExpressionVisitorBase<TResultType> visitor);

        public static DbPropertyExpression Property(TypeUsage resultType, EdmMember property, DbExpression instance) {
            return new DbPropertyExpression(resultType, property, instance);
        }

        public static DbProjectExpression Project(TypeUsage resultType, DbExpressionBinding input, DbExpression projection) {
            return new DbProjectExpression(resultType, input, projection);
        }

        public static DbFilterExpression Filter(TypeUsage resultType, DbExpressionBinding input, DbExpression predicate) {
            return new DbFilterExpression(resultType, input, predicate);
        }

        public static DbCastExpression Cast(TypeUsage type, DbExpression expression) {
            return new DbCastExpression(type, expression);
        }

        public static DbVariableReferenceExpression VariableRef(TypeUsage type, string name) {
            return new DbVariableReferenceExpression(type, name);
        }

        public static DbJoinExpression Join(DbExpressionKind joinKind, TypeUsage collectionOfRowResultType, DbExpressionBinding left, DbExpressionBinding right, DbExpression condition) {
            return new DbJoinExpression(joinKind, collectionOfRowResultType, left, right, condition);
        }

        public static DbComparisonExpression Comparison(DbExpressionKind kind, TypeUsage booleanResultType, DbExpression left, DbExpression right) {
            return new DbComparisonExpression(kind, booleanResultType, left, right);
        }

        public static DbLikeExpression Like(TypeUsage resultType, DbExpression argument, DbExpression pattern, DbExpression escape) {
            return new DbLikeExpression(resultType, argument, pattern, escape);
        }

        public static DbParameterExpression Parameter(TypeUsage type, string name, DbConstantExpression value) {
            return new DbParameterExpression(type, name, value);
        }

        public static DbXmlToCursorPropertyExpression XmlToCursorProperty(TypeUsage type, DbExpression instance) {
            return new DbXmlToCursorPropertyExpression(type, instance);
        }

        public static DbXmlToCursorScanExpression XmlToCursorScan(DbExpression parameter, string cursorName) {
            return new DbXmlToCursorScanExpression(parameter, cursorName);
        }

        public static DbXmlToCursorExpression XmlToCursor(DbExpression property, DbExpression parameter, string cursorName, Type itemType) {
            return new DbXmlToCursorExpression(property, parameter, cursorName, itemType);
        }

        public static DbInListExpression InList(DbExpression property, DbExpression values) {
            return new DbInListExpression(property, values);
        }

        public static DbArrayExpression Array(IEnumerable<DbExpression> list) {
            return new DbArrayExpression(List(list.ToList()));
        }

        public static DbArrayExpression Array(DbExpressionList values) {
            return new DbArrayExpression(values);
        }

        public static DbExpressionList List(IList<DbExpression> list) {
            return new DbExpressionList(list);
        }

        public static DbExpressionBinding Binding(DbExpression expression, DbVariableReferenceExpression variableReference) {
            return new DbExpressionBinding(expression, variableReference);
        }

        public static DbBinaryExpression And(DbExpression left, DbExpression right) {
            return And(PrimitiveTypeKind.Boolean.ToTypeUsage(), left, right);
        }

        public static DbBinaryExpression And(TypeUsage resultType, DbExpression left, DbExpression right) {
            return new DbAndExpression(resultType, left, right);
        }

        public static DbConstantExpression Constant(object value) {
            if (value == null) {
                throw new ArgumentNullException("value");
            }

            return Constant(value, value.GetType());
        }

        public static DbConstantExpression Constant(object value, Type type) {
            return new DbConstantExpression(type.ToTypeUsage(), value);
        }
    }
}