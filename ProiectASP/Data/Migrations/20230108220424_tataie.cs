using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProiectASP.Data.Migrations
{
    public partial class tataie : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Profiles");

            migrationBuilder.AddColumn<int>(
                name: "Profile_Status",
                table: "Profiles",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Profile_Status",
                table: "Profiles");

            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Profiles",
                type: "bit",
                nullable: true);
        }
    }
}
