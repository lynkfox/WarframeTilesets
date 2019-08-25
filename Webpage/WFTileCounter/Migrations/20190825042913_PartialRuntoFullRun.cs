using Microsoft.EntityFrameworkCore.Migrations;

namespace WFTileCounter.Migrations
{
    public partial class PartialRuntoFullRun : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PartialRun",
                table: "Runs");

            migrationBuilder.AddColumn<bool>(
                name: "FullRun",
                table: "Runs",
                maxLength: 1,
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullRun",
                table: "Runs");

            migrationBuilder.AddColumn<string>(
                name: "PartialRun",
                table: "Runs",
                maxLength: 1,
                nullable: true);
        }
    }
}
