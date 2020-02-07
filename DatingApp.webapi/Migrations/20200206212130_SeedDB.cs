using Microsoft.EntityFrameworkCore.Migrations;

namespace DatingApp.webapi.Migrations
{
    public partial class SeedDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("insert into [values](Name) values('Value1');" +
            "insert into [values](name) values('Value2');" +
            "insert into [values](name) values('Value3');" 
            );

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("truncate table [values]");

        }
    }
}
