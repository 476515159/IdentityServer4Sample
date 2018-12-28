using Microsoft.EntityFrameworkCore.Migrations;

namespace Fate.Recommend.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RecommendProjects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    ProjectId = table.Column<int>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    Avatar = table.Column<string>(maxLength: 100, nullable: true),
                    Company = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecommendProjects", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RecommendProjects");
        }
    }
}
