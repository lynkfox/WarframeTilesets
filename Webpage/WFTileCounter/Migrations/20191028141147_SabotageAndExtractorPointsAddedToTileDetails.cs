using Microsoft.EntityFrameworkCore.Migrations;

namespace WFTileCounter.Migrations
{
    public partial class SabotageAndExtractorPointsAddedToTileDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Extractor",
                schema: "website",
                table: "TileDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Sabotage",
                schema: "website",
                table: "TileDetails",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Extractor",
                schema: "website",
                table: "TileDetails");

            migrationBuilder.DropColumn(
                name: "Sabotage",
                schema: "website",
                table: "TileDetails");
        }
    }
}
