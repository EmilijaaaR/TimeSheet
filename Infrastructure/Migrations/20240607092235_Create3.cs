using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Create3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientUsers_Clients_ClientId",
                table: "ClientUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientUsers_Users_UserId",
                table: "ClientUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Leads_LeadId",
                table: "Projects");

            migrationBuilder.DropTable(
                name: "Leads");

            migrationBuilder.DropIndex(
                name: "IX_Projects_LeadId",
                table: "Projects");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ClientUsers",
                table: "ClientUsers");

            migrationBuilder.DropColumn(
                name: "LeadId",
                table: "Projects");

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

            migrationBuilder.CreateTable(
                name: "ProjectUser",
                columns: table => new
                {
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectUser", x => new { x.ProjectId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ProjectUser_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectUser_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUser_UserId",
                table: "ProjectUser",
                column: "UserId");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClientUser_Clients_ClientId",
                table: "ClientUser");

            migrationBuilder.DropForeignKey(
                name: "FK_ClientUser_Users_UserId",
                table: "ClientUser");

            migrationBuilder.DropTable(
                name: "ProjectUser");

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

            migrationBuilder.AddColumn<int>(
                name: "LeadId",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ClientUsers",
                table: "ClientUsers",
                columns: new[] { "ClientId", "UserId" });

            migrationBuilder.CreateTable(
                name: "Leads",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leads", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Leads_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_LeadId",
                table: "Projects",
                column: "LeadId");

            migrationBuilder.CreateIndex(
                name: "IX_Leads_UserId",
                table: "Leads",
                column: "UserId");

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

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Leads_LeadId",
                table: "Projects",
                column: "LeadId",
                principalTable: "Leads",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
