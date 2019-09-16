using Microsoft.EntityFrameworkCore.Migrations;

namespace WFTileCounter.Migrations
{
    public partial class RemoveAlternateTileset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlternateTileset",
                table: "Tiles");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AlternateTileset",
                table: "Tiles",
                maxLength: 100,
                nullable: true);
        }
    }
}
