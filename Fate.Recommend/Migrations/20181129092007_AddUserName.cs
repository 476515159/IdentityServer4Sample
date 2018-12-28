using Microsoft.EntityFrameworkCore.Migrations;

namespace Fate.Recommend.Migrations
{
    public partial class AddUserName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FromUserId",
                table: "RecommendProjects",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FromUserName",
                table: "RecommendProjects",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "RecommendProjects",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FromUserId",
                table: "RecommendProjects");

            migrationBuilder.DropColumn(
                name: "FromUserName",
                table: "RecommendProjects");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "RecommendProjects");
        }
    }
}
