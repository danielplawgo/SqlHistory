namespace SqlHistory.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHistoryToProduct : DbMigration
    {
        public override void Up()
        {
            Sql(TemporalTableQueryBuilder.GetCreateSql("dbo.Products"));
        }
        
        public override void Down()
        {
            Sql(TemporalTableQueryBuilder.GetDropSql("dbo.Products"));
        }
    }
}
