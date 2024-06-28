using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Create2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientUser_Clients_ClientId",
                table: "ClientUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientUser_Users_UserId",
                table: "ClientUser");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientUser",
                table: "ClientUser");

            migrationBuilder.RenameTable(
                name: "ClientUser",
                newName: "ClientUsers");

            migrationBuilder.RenameIndex(
                name: "IX_ClientUser_UserId",
                table: "ClientUsers",
                newName: "IX_ClientUsers_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientUsers",
                table: "ClientUsers",
                columns: new[] { "ClientId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ClientUsers_Clients_ClientId",
                table: "ClientUsers",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientUsers_Users_UserId",
                table: "ClientUsers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientUsers_Clients_ClientId",
                table: "ClientUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientUsers_Users_UserId",
                table: "ClientUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientUsers",
                table: "ClientUsers");

            migrationBuilder.RenameTable(
                name: "ClientUsers",
                newName: "ClientUser");

            migrationBuilder.RenameIndex(
                name: "IX_ClientUsers_UserId",
                table: "ClientUser",
                newName: "IX_ClientUser_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientUser",
                table: "ClientUser",
                columns: new[] { "ClientId", "UserId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ClientUser_Clients_ClientId",
                table: "ClientUser",
                column: "ClientId",
                principalTable: "Clients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ClientUser_Users_UserId",
                table: "ClientUser",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
