using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProiectASP.Data.Migrations
{
    public partial class chat3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chat_AspNetUsers_UserId",
                table: "Chat");

            migrationBuilder.DropForeignKey(
                name: "FK_Chat_Groups_GroupId",
                table: "Chat");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Chat",
                table: "Chat");

            migrationBuilder.RenameTable(
                name: "Chat",
                newName: "Chats");

            migrationBuilder.RenameIndex(
                name: "IX_Chat_UserId",
                table: "Chats",
                newName: "IX_Chats_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Chat_GroupId",
                table: "Chats",
                newName: "IX_Chats_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Chats",
                table: "Chats",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_AspNetUsers_UserId",
                table: "Chats",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Groups_GroupId",
                table: "Chats",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_AspNetUsers_UserId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Groups_GroupId",
                table: "Chats");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Chats",
                table: "Chats");

            migrationBuilder.RenameTable(
                name: "Chats",
                newName: "Chat");

            migrationBuilder.RenameIndex(
                name: "IX_Chats_UserId",
                table: "Chat",
                newName: "IX_Chat_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Chats_GroupId",
                table: "Chat",
                newName: "IX_Chat_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Chat",
                table: "Chat",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Chat_AspNetUsers_UserId",
                table: "Chat",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Chat_Groups_GroupId",
                table: "Chat",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }
    }
}
