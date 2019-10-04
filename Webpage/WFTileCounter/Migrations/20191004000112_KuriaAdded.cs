using Microsoft.EntityFrameworkCore.Migrations;

namespace WFTileCounter.Migrations
{
    public partial class KuriaAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Kuria",
                schema: "website",
                table: "TileDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "KuvaSiphon",
                schema: "website",
                table: "TileDetails",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Kuria",
                schema: "website",
                table: "TileDetails");

            migrationBuilder.DropColumn(
                name: "KuvaSiphon",
                schema: "website",
                table: "TileDetails");
        }
    }
}
