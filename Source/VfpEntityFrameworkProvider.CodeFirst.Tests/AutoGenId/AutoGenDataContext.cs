using System.Data.Entity;

namespace VfpEntityFrameworkProvider.CodeFirst.Tests.AutoGenId {
    public class AutoGenDataContext : DbContext {
        public IDbSet<AutoGen> AutoGens { get; set; }

        static AutoGenDataContext() {
            Database.SetInitializer<AutoGenDataContext>(null);
        }
    }
}