using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NorthwindEFModel;

namespace VfpEntityFrameworkProvider.Tests {
    [TestClass]
    public class ObjectServicesTests : TestBase {
        [TestMethod]
        public void CRUDTest() {
            this.Create();
            this.Read();
            this.Update();
            this.Delete();
        }

        private void Delete() {
            var context = this.GetContext();

            foreach (Category c in context.Categories.Where(c => c.CategoryName == "X")) {
                context.DeleteObject(c);
            }
            context.SaveChanges();
        }

        private void Update() {
            var context = this.GetContext();

            Category c = (from o in context.Categories
                          where o.CategoryName == "X"
                          orderby o.CategoryID
                          select o).First();
            c.Description = "Some description " + DateTime.Now.ToString();
            context.SaveChanges();
        }

        private void Read() {
            var context = this.GetContext();

            Category c = (from o in context.Categories
                          where o.CategoryName == "X"
                          orderby o.CategoryID
                          select o).First();
        }

        private void Create() {
            var context = this.GetContext();
            Category c = new Category();

            Assert.AreEqual(0, c.CategoryID);
            c.CategoryName = "X";
            context.AddToCategories(c);
            context.SaveChanges();
            Assert.AreNotEqual(0, c.CategoryID);
        }
    }
}
