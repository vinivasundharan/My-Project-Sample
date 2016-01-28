namespace TEDALS_Ver01.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FromSAP : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FromSAP",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        OptionName = c.String(),
                        OptionValue = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FromSAP");
        }
    }
}
