using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SqlHistory
{
    class Program
    {
        static void Main(string[] args)
        {
            HibernatingRhinos.Profiler.Appender.EntityFramework.EntityFrameworkProfiler.Initialize();

            //AddAndUpdateProductWithoutHistory();

            int productId = 0;

            productId = AddAndUpdateWithHistory();

            ShowHistoryUsingSql(productId);

            GetProductById(productId);

            ShowHistoryUsingLinq(productId);

            ShowProductInCategoryLazyLoadingProblem();

            ShowProductInCategoryLinq();

            int orderId = AddProductsToOrder();

            RemoveProductFromOrder(orderId, productId);
        }

        private static int AddProductsToOrder()
        {
            using (DataContext db = new DataContext())
            {
                var products = db.Products.OfType<Product>();

                var order = new Order()
                {
                    Number = "order number",
                    Products = products.Cast<BaseProduct>().ToList()
                };

                db.Orders.Add(order);
                db.SaveChanges();

                return order.Id;
            }
        }

        private static void RemoveProductFromOrder(int orderId, int productId)
        {
            using (DataContext db = new DataContext())
            {
                var order = db.Orders.FirstOrDefault(o => o.Id == orderId);

                var product = order.Products.FirstOrDefault(p => p.Id == productId);

                order.Products.Remove(product);

                db.SaveChanges();
            }
        }

        private static void ShowProductInCategoryLinq()
        {
            Console.WriteLine("ShowProductInCategoryLinq:");

            using (DataContext db = new DataContext())
            {
                var category = db.Categories.FirstOrDefault();

                foreach (var item in db.Products.OfType<Product>().Where(p => p.CategoryId == category.Id))
                {
                    Console.WriteLine($"{item.Name}, {item.ValidFrom}, {item.ValidTo}");
                }
            }
        }

        private static void ShowProductInCategoryLazyLoadingProblem()
        {
            Console.WriteLine("ShowProductInCategoryLazyLoadingProblem:");

            using (DataContext db = new DataContext())
            {
                var category = db.Categories.FirstOrDefault();

                foreach (var item in category.Products)
                {
                    Console.WriteLine($"{item.Name}, {item.ValidFrom}, {item.ValidTo}");
                }
            }
        }

        private static void ShowHistoryUsingLinq(int productId)
        {
            Console.WriteLine("ShowHistoryUsingLinq:");

            using (DataContext db = new DataContext())
            {
                var products = db.Products.Where(p => p.Id == productId)
                    .OrderByDescending(p => p.ValidFrom);

                foreach (var item in products)
                {
                    Console.WriteLine($"{item.Name}, {item.ValidFrom}, {item.ValidTo}");
                }
            }
        }

        private static void GetProductById(int productId)
        {
            Console.WriteLine("GetProductById:");

            using (DataContext db = new DataContext())
            {
                BaseProduct product = db.Products.OfType<Product>().FirstOrDefault(p => p.Id == productId);

                Console.WriteLine($"{product.Name}, {product.ValidFrom}, {product.ValidTo}");
            }
        }

        private static void ShowHistoryUsingSql(int productId)
        {
            Console.WriteLine("ShowHistoryUsingSql:");

            using (DataContext db = new DataContext())
            {
                var query = $"SELECT * FROM dbo.Products FOR SYSTEM_TIME ALL WHERE Id = {productId}";

                var products =
                    db.Database.SqlQuery<Product>(query)
                        .ToList();

                foreach (var item in products)
                {
                    Console.WriteLine($"{item.Name}");
                }
            }
        }

        private static int AddAndUpdateWithHistory()
        {
            int productId;
            using (DataContext db = new DataContext())
            {
                var category = db.Categories.FirstOrDefault();

                var product = new Product()
                {
                    Name = "Product",
                    Category = category
                };

                db.Products.Add(product);
                db.SaveChanges();

                Thread.Sleep(1 * 1000);

                productId = product.Id;

                product.Name = "New Product";

                db.SaveChanges();
            }

            return productId;
        }

        private static void AddAndUpdateProductWithoutHistory()
        {
            using (DataContext db = new DataContext())
            {
                var category = db.Categories.FirstOrDefault();

                var product = new Product()
                {
                    Name = "Product",
                    Category = category
                };

                db.Products.Add(product);
                db.SaveChanges();

                product.Name = "New Product";

                db.SaveChanges();
            }
        }
    }
}
