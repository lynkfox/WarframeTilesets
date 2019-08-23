using Microsoft.EntityFrameworkCore.Migrations;

namespace WFTileCounter.Migrations
{
    public partial class AlternateTilesetColumnToTiles : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AlternateTileset",
                table: "Tiles",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlternateTileset",
                table: "Tiles");
        }
    }
}
