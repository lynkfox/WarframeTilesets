using Microsoft.EntityFrameworkCore.Migrations;

namespace WFTileCounter.Migrations
{
    public partial class SmallTileImgChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TileImages_Tilesets_TilesetName",
                schema: "website",
                table: "TileImages");

            migrationBuilder.DropIndex(
                name: "IX_TileImages_TilesetName",
                schema: "website",
                table: "TileImages");

            migrationBuilder.DropColumn(
                name: "ImgName",
                schema: "website",
                table: "TileImages");

            migrationBuilder.DropColumn(
                name: "TilesetName",
                schema: "website",
                table: "TileImages");

            migrationBuilder.AddColumn<string>(
                name: "AltText",
                schema: "website",
                table: "TileImages",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                schema: "website",
                table: "TileImages",
                maxLength: 250,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AltText",
                schema: "website",
                table: "TileImages");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                schema: "website",
                table: "TileImages");

            migrationBuilder.AddColumn<string>(
                name: "ImgName",
                schema: "website",
                table: "TileImages",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TilesetName",
                schema: "website",
                table: "TileImages",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TileImages_TilesetName",
                schema: "website",
                table: "TileImages",
                column: "TilesetName");

            migrationBuilder.AddForeignKey(
                name: "FK_TileImages_Tilesets_TilesetName",
                schema: "website",
                table: "TileImages",
                column: "TilesetName",
                principalTable: "Tilesets",
                principalColumn: "Name",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
