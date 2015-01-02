namespace VfpEntityFrameworkProvider.DbExpressions {
    public class DbSortClause {
        public bool Ascending { get; private set; }
        public string Collation { get; private set; }
        public DbExpression Expression { get; private set; }

        public DbSortClause(DbExpression key, bool ascending, string collation) {
            Expression = key;
            Ascending = ascending;
            Collation = collation;
        }
    }
}