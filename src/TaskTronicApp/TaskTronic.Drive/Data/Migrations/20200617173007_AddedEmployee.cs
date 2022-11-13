using Microsoft.EntityFrameworkCore.Migrations;

namespace TaskTronic.Drive.Data.Migrations
{
    public partial class AddedEmployee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Folders");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Blobsdata");

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "Permissions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "Folders",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "Files",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EmployeeId",
                table: "Blobsdata",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Employees",
                columns: table => new
                {
                    EmployeeId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Employees", x => x.EmployeeId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_EmployeeId",
                table: "Permissions",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_EmployeeId",
                table: "Folders",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_Files_EmployeeId",
                table: "Files",
                column: "EmployeeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Permissions_EmployeeId",
                table: "Permissions");

            migrationBuilder.DropIndex(
                name: "IX_Folders_EmployeeId",
                table: "Folders");

            migrationBuilder.DropIndex(
                name: "IX_Files_EmployeeId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Permissions");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Folders");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "EmployeeId",
                table: "Blobsdata");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Permissions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Folders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Files",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Blobsdata",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
