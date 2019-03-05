using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure.Interception;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlHistory
{
    public class DataContext : DbContext
    {
        static DataContext()
        {
            //DbInterception.Add(new TemporalTableCommandTreeInterceptor());
        }

        public DataContext()
            : base("Name=DefaultConnection")
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Configurations.AddFromAssembly(this.GetType().Assembly);
        }

        public DbSet<Category> Categories { get; set; }

        public DbSet<BaseProduct> Products { get; set; }

        //public DbSet<Order> Orders { get; set; }
    }
}
