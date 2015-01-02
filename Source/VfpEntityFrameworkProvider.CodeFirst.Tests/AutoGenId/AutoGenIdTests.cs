using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VfpEntityFrameworkProvider.CodeFirst.Tests.AutoGenId {
    [TestClass]
    [DeploymentItem(@"AutoGenId\Data\", @"AutoGenId\Data\")]
    public class AutoGenIdTests : TestBase {
        [TestMethod]
        public void AutoGenIdTests_CRUDTest() {
            Create();
            Read();
            Update();
            Delete();
        }

        private static void Delete() {
            var context = GetContext();
            var entity = context.AutoGens.FirstOrDefault(x => x.Value == "Y");

            context.AutoGens.Remove(entity);
            context.SaveChanges();

            Assert.IsNull(GetContext().AutoGens.FirstOrDefault(x => x.Value == "Y"));
        }

        private static void Update() {
            var context = GetContext();
            var entity = context.AutoGens.FirstOrDefault(x => x.Value == "X");

            entity.Value = "Y";
            context.SaveChanges();

            Assert.IsNotNull(GetContext().AutoGens.FirstOrDefault(x => x.Value == "Y"));
        }

        private static void Read() {
            var context = GetContext();

            Assert.IsNotNull(context.AutoGens.FirstOrDefault(x => x.Value == "X"));
        }

        private static void Create() {
            var context = GetContext();
            var entity = new AutoGen();

            Assert.IsNull(entity.Id);

            entity.Value = "X";
            context.AutoGens.Add(entity);
            context.SaveChanges();

            Assert.IsNotNull(entity.Id);
        }

        private static new AutoGenDataContext GetContext() {
            return new AutoGenDataContext();
        }
    }
}