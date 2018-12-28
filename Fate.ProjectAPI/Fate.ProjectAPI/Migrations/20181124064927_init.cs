using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Fate.ProjectAPI.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    UserId = table.Column<int>(nullable: true),
                    Avatar = table.Column<string>(maxLength: 100, nullable: true),
                    Company = table.Column<string>(maxLength: 100, nullable: true),
                    OriginBPFile = table.Column<string>(maxLength: 100, nullable: true),
                    FormatBPFile = table.Column<string>(maxLength: 100, nullable: true),
                    ShowSecurityInfo = table.Column<short>(nullable: true),
                    ProvinceId = table.Column<int>(nullable: true),
                    Province = table.Column<string>(maxLength: 50, nullable: true),
                    CityId = table.Column<int>(nullable: true),
                    City = table.Column<string>(maxLength: 50, nullable: true),
                    AreaId = table.Column<int>(nullable: true),
                    AreaName = table.Column<string>(maxLength: 50, nullable: true),
                    RegisterTime = table.Column<DateTime>(nullable: true),
                    Introduction = table.Column<string>(maxLength: 500, nullable: true),
                    FinPercentage = table.Column<string>(maxLength: 50, nullable: true),
                    FinStage = table.Column<string>(maxLength: 50, nullable: true),
                    FinMoney = table.Column<int>(nullable: true),
                    Income = table.Column<int>(nullable: true),
                    Revenue = table.Column<int>(nullable: true),
                    Valuation = table.Column<int>(nullable: true),
                    BrokerageOptions = table.Column<int>(nullable: true),
                    OnPlatform = table.Column<short>(nullable: true),
                    SourceId = table.Column<int>(nullable: true),
                    ReferenceId = table.Column<int>(nullable: true),
                    Tags = table.Column<string>(maxLength: 500, nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: true),
                    UpdateTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PrjectVisibleRule",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    ProjectId = table.Column<int>(nullable: false),
                    Visible = table.Column<short>(nullable: true),
                    Tags = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PrjectVisibleRule", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PrjectVisibleRule_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectContributor",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    ProjectId = table.Column<int>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    Avatar = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: true),
                    IsCloser = table.Column<short>(nullable: true),
                    ContributorType = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectContributor", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectContributor_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectProperty",
                columns: table => new
                {
                    ProjectId = table.Column<int>(nullable: false),
                    Key = table.Column<string>(maxLength: 100, nullable: false),
                    Text = table.Column<string>(nullable: true),
                    Value = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectProperty", x => new { x.Key, x.Value, x.ProjectId });
                    table.ForeignKey(
                        name: "FK_ProjectProperty_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectViewer",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySQL:AutoIncrement", true),
                    ProjectId = table.Column<int>(nullable: true),
                    UserId = table.Column<int>(nullable: true),
                    UserName = table.Column<string>(nullable: true),
                    Avatar = table.Column<string>(nullable: true),
                    CreatedTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectViewer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectViewer_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PrjectVisibleRule_ProjectId",
                table: "PrjectVisibleRule",
                column: "ProjectId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectContributor_ProjectId",
                table: "ProjectContributor",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectProperty_ProjectId",
                table: "ProjectProperty",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectViewer_ProjectId",
                table: "ProjectViewer",
                column: "ProjectId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PrjectVisibleRule");

            migrationBuilder.DropTable(
                name: "ProjectContributor");

            migrationBuilder.DropTable(
                name: "ProjectProperty");

            migrationBuilder.DropTable(
                name: "ProjectViewer");

            migrationBuilder.DropTable(
                name: "Project");
        }
    }
}
