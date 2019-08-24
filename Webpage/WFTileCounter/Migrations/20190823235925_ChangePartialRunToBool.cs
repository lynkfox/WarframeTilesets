using Microsoft.EntityFrameworkCore.Migrations;

namespace WFTileCounter.Migrations
{
    public partial class ChangePartialRunToBool : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PartialRun",
                table: "Runs");

            migrationBuilder.AddColumn<bool>(
                name: "FullRun",
                table: "Runs",
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
