namespace SqlHistory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDescriptionToProduct : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "Description", c => c.String());
            //AddColumn("dbo.ProductsHistory", "Description", c => c.String());
        }
        
        public override void Down()
        {
            //DropColumn("dbo.ProductsHistory", "Description");
            DropColumn("dbo.Products", "Description");
        }
    }
}
