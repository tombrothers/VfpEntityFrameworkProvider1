namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbSetClause {
        public DbExpression Property { get; private set; }
        public DbExpression Value { get; private set; }

        internal DbSetClause(DbExpression property, DbExpression value) {
            Property = property;
            Value = value;
        }
    }
}