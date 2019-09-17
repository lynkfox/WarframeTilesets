using Microsoft.EntityFrameworkCore.Migrations;

namespace WFTileCounter.Migrations
{
    public partial class newMapPointOptions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Collectibles",
                table: "MapPoints",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Objectives",
                table: "MapPoints",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Scanables",
                table: "MapPoints",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Collectibles",
                table: "MapPoints");

            migrationBuilder.DropColumn(
                name: "Objectives",
                table: "MapPoints");

            migrationBuilder.DropColumn(
                name: "Scanables",
                table: "MapPoints");
        }
    }
}
