using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Create1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Leads",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ClientUser",
                columns: table => new
                {
                    ClientId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientUser", x => new { x.ClientId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ClientUser_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientUser_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Leads_UserId",
                table: "Leads",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientUser_UserId",
                table: "ClientUser",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Leads_Users_UserId",
                table: "Leads",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Leads_Users_UserId",
                table: "Leads");

            migrationBuilder.DropTable(
                name: "ClientUser");

            migrationBuilder.DropIndex(
                name: "IX_Leads_UserId",
                table: "Leads");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Leads");
        }
    }
}
