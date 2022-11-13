using Microsoft.EntityFrameworkCore.Migrations;

namespace TaskTronic.Drive.Data.Migrations
{
    public partial class RemoveCompanyForeignKeyForEmployee : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Employees_CompanyDepartments_CompanyDepartmentsId",
                table: "Employees");

            migrationBuilder.DropIndex(
                name: "IX_Employees_CompanyDepartmentsId",
                table: "Employees");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
    }
}
