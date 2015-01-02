using System.Data.Entity;

namespace VfpEntityFrameworkProvider.CodeFirst.Tests {
    public class CodeFirstContext : DbContext {
        public IDbSet<Artist> Artists { get; set; }
        public IDbSet<Album> Album { get; set; }
        public IDbSet<User> Users { get; set; }

        public CodeFirstContext(VfpConnection connection)
            : base(connection, true) {
        }
    }
}