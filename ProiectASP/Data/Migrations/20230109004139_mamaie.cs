using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProiectASP.Data.Migrations
{
    public partial class mamaie : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "Groups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subject",
                table: "Groups");
        }
    }
}
