using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Fate.API.Migrations
{
    public partial class newusers820 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Avatar",
                table: "UserInfo",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Company",
                table: "UserInfo",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "UserInfo",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "UserInfo",
                maxLength: 100,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserTags",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Tag = table.Column<string>(maxLength: 20, nullable: false),
                    datetime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTags", x => new { x.UserId, x.Tag });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserTags");

            migrationBuilder.DropColumn(
                name: "Avatar",
                table: "UserInfo");

            migrationBuilder.DropColumn(
                name: "Company",
                table: "UserInfo");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "UserInfo");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "UserInfo");
        }
    }
}
