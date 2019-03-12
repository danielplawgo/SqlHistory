namespace SqlHistory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddOrder : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Number = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.OrderBaseProducts",
                c => new
                    {
                        Order_Id = c.Int(nullable: false),
                        BaseProduct_Id = c.Int(nullable: false),
                        //BaseProduct_ValidFrom = c.DateTime(nullable: false),
                        //BaseProduct_ValidTo = c.DateTime(nullable: false),
                    })
                //.PrimaryKey(t => new { t.Order_Id, t.BaseProduct_Id, t.BaseProduct_ValidFrom, t.BaseProduct_ValidTo })
                .PrimaryKey(t => new { t.Order_Id, t.BaseProduct_Id })
                .ForeignKey("dbo.Orders", t => t.Order_Id, cascadeDelete: true)
                .Index(t => t.Order_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrderBaseProducts", "Order_Id", "dbo.Orders");
            DropIndex("dbo.OrderBaseProducts", new[] { "Order_Id" });
            DropTable("dbo.OrderBaseProducts");
            DropTable("dbo.Orders");
        }
    }
}
