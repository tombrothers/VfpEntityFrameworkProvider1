using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using VfpEntityFrameworkProvider;

namespace EFCodeFirstWalkthrough {
    class Program {
        static void Main(string[] args) {
            VfpProviderFactory.Register();

            using (var db = new ProductContext()) {
                // Use Find to locate the Food category 
                var food = db.Categories.Find("FOOD");
                if (food == null) {
                    food = new Category { CategoryId = "FOOD", Name = "Foods" };
                    db.Categories.Add(food);
                }
                // Create a new Food product 
                Console.Write("Please enter a name for a new food: ");
                var productName = Console.ReadLine();
                var product = new Product { Name = productName, Category = food };
                db.Products.Add(product);
                int recordsAffected = db.SaveChanges();
                Console.WriteLine("Saved {0} entities to the database.", recordsAffected);
                // Query for all Food products using LINQ 
                var allFoods = from p in db.Products
                               where p.CategoryId == "FOOD"
                               orderby p.Name
                               select p;
                Console.WriteLine("All foods in database:");
                foreach (var item in allFoods) {
                    Console.WriteLine(" - {0}", item.Name);
                }
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
        }
    }

    public class ProductContext : DbContext {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
    }

    public class Product {
        public int ProductId { get; set; }
        [MaxLength(50)]
        public string Name { get; set; }
        [MaxLength(50)]
        public string CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }

    public class Category {
        [MaxLength(50)]
        public string CategoryId { get; set; }
        public string Name { get; set; }
        public virtual ICollection<Product> Products { get; set; }
    }
}
