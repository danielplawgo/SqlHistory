namespace SqlHistory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHistoryToProduct2 : DbMigration
    {
        public override void Up()
        {
            //DropPrimaryKey("dbo.Products");
            //CreateTable(
            //    "dbo.ProductsHistory",
            //    c => new
            //        {
            //            Id = c.Int(nullable: false, identity: true),
            //            ValidFrom = c.DateTime(nullable: false),
            //            ValidTo = c.DateTime(nullable: false),
            //            Name = c.String(),
            //            CategoryId = c.Int(nullable: false),
            //            IsActive = c.Boolean(nullable: false),
            //        })
            //    .PrimaryKey(t => new { t.Id, t.ValidFrom, t.ValidTo })
            //    .ForeignKey("dbo.Categories", t => t.CategoryId, cascadeDelete: true)
            //    .Index(t => t.CategoryId);
            
            //AddColumn("dbo.Products", "ValidFrom", c => c.DateTime(nullable: false));
            //AddColumn("dbo.Products", "ValidTo", c => c.DateTime(nullable: false));
            //AddPrimaryKey("dbo.Products", new[] { "Id", "ValidFrom", "ValidTo" });
        }
        
        public override void Down()
        {
            //DropForeignKey("dbo.ProductsHistory", "CategoryId", "dbo.Categories");
            //DropIndex("dbo.ProductsHistory", new[] { "CategoryId" });
            //DropPrimaryKey("dbo.Products");
            //DropColumn("dbo.Products", "ValidTo");
            //DropColumn("dbo.Products", "ValidFrom");
            //DropTable("dbo.ProductsHistory");
            //AddPrimaryKey("dbo.Products", "Id");
        }
    }
}
