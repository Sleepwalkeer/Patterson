namespace Patterson.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createDataBaseTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.element",
                c => new
                    {
                        id = c.Guid(nullable: false, identity: true),
                        name = c.String(maxLength: 255),
                        deltar = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.experiment",
                c => new
                    {
                        id = c.Guid(nullable: false),
                        datetime = c.DateTime(nullable: false),
                        description = c.String(),
                        elem_id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.element", t => t.elem_id, cascadeDelete: true)
                .Index(t => t.elem_id);
            
            CreateTable(
                "dbo.patterson_peak",
                c => new
                    {
                        exp_id = c.Guid(nullable: false),
                        is_uv_exposed = c.Boolean(nullable: false),
                        u = c.Double(nullable: false),
                        pu = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.exp_id, t.is_uv_exposed, t.u, t.pu })
                .ForeignKey("dbo.experiment", t => t.exp_id, cascadeDelete: true)
                .Index(t => t.exp_id);
            
            CreateTable(
                "dbo.peak_data",
                c => new
                    {
                        experiment_id = c.Guid(nullable: false),
                        is_uv_exposed = c.Boolean(nullable: false),
                        peak_id = c.Int(nullable: false),
                        intensity = c.Double(nullable: false),
                        double_theta = c.Double(nullable: false),
                        plg = c.Double(nullable: false),
                        f_squared = c.Double(nullable: false),
                        d_over_n = c.Double(nullable: false),
                        one_over_d = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.experiment_id, t.is_uv_exposed, t.peak_id })
                .ForeignKey("dbo.experiment", t => t.experiment_id, cascadeDelete: true)
                .Index(t => t.experiment_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.peak_data", "experiment_id", "dbo.experiment");
            DropForeignKey("dbo.patterson_peak", "exp_id", "dbo.experiment");
            DropForeignKey("dbo.experiment", "elem_id", "dbo.element");
            DropIndex("dbo.peak_data", new[] { "experiment_id" });
            DropIndex("dbo.patterson_peak", new[] { "exp_id" });
            DropIndex("dbo.experiment", new[] { "elem_id" });
            DropTable("dbo.peak_data");
            DropTable("dbo.patterson_peak");
            DropTable("dbo.experiment");
            DropTable("dbo.element");
        }
    }
}
