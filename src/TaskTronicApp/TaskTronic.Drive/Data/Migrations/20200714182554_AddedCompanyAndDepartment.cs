using Microsoft.EntityFrameworkCore.Migrations;

namespace TaskTronic.Drive.Data.Migrations
{
    public partial class AddedCompanyAndDepartment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Catalogs");

            migrationBuilder.DropColumn(
                name: "DepartmentId",
                table: "Catalogs");

            migrationBuilder.AddColumn<int>(
                name: "CompanyDepartmentsId",
                table: "Catalogs",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    CompanyId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Companies", x => x.CompanyId);
                });

            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    DepartmentId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.DepartmentId);
                });

            migrationBuilder.CreateTable(
                name: "CompanyDepartments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(nullable: false),
                    DepartmentId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyDepartments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CompanyDepartments_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CompanyDepartments_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "DepartmentId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Catalogs_CompanyDepartmentsId",
                table: "Catalogs",
                column: "CompanyDepartmentsId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyDepartments_CompanyId",
                table: "CompanyDepartments",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyDepartments_DepartmentId",
                table: "CompanyDepartments",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Catalogs_CompanyDepartments_CompanyDepartmentsId",
                table: "Catalogs",
                column: "CompanyDepartmentsId",
                principalTable: "CompanyDepartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Catalogs_CompanyDepartments_CompanyDepartmentsId",
                table: "Catalogs");

            migrationBuilder.DropTable(
                name: "CompanyDepartments");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Catalogs_CompanyDepartmentsId",
                table: "Catalogs");

            migrationBuilder.DropColumn(
                name: "CompanyDepartmentsId",
                table: "Catalogs");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Catalogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DepartmentId",
                table: "Catalogs",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
