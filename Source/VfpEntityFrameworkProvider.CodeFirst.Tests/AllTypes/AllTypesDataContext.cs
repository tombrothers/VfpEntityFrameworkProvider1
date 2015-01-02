using System.Data.Entity;

namespace VfpEntityFrameworkProvider.CodeFirst.Tests.AllTypes {
    public class AllTypesDataContext : DbContext {
        public IDbSet<AllTypesTable> AllTypes { get; set; }

        static AllTypesDataContext() {
            Database.SetInitializer<AllTypesDataContext>(null);
        }

        public AllTypesDataContext(VfpConnection connection)
            : base(connection, true) {
        }
    }
}