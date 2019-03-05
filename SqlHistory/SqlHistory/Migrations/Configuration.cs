using Bogus;

namespace SqlHistory.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<SqlHistory.DataContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(SqlHistory.DataContext context)
        {
            if (context.Categories.Any() == false)
            {
                var categories = new Faker<Category>()
                    .RuleFor(c => c.Name, (f, c) => f.Commerce.Categories(1)[0])
                    .Generate(1);

                context.Categories.AddRange(categories);

                context.SaveChanges();
            }
        }
    }
}
