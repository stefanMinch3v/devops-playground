using Microsoft.EntityFrameworkCore.Migrations;

namespace TaskTronic.Drive.Data.Migrations
{
    public partial class EmployeeInCompanyDepartment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CompanyDepartmentsId",
                table: "Employees",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Employees_CompanyDepartmentsId",
                table: "Employees",
                column: "CompanyDepartmentsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_CompanyDepartments_CompanyDepartmentsId",
                table: "Employees",
                column: "CompanyDepartmentsId",
                principalTable: "CompanyDepartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_CompanyDepartments_CompanyDepartmentsId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_CompanyDepartmentsId",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "CompanyDepartmentsId",
                table: "Employees");
        }
    }
}
