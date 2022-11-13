using Microsoft.EntityFrameworkCore.Migrations;

namespace TaskTronic.Statistics.Data.Migrations
{
    public partial class InitialDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FolderViews",
                columns: table => new
                {
                    FolderViewId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FolderId = table.Column<int>(nullable: false),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FolderViews", x => x.FolderViewId);
                });

            migrationBuilder.CreateTable(
                name: "Statistics",
                columns: table => new
                {
                    StatisticsId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalFolders = table.Column<int>(nullable: false),
                    TotalFiles = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistics", x => x.StatisticsId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FolderViews_FolderId",
                table: "FolderViews",
                column: "FolderId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FolderViews");

            migrationBuilder.DropTable(
                name: "Statistics");
        }
    }
}
