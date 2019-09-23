using Microsoft.EntityFrameworkCore.Migrations;

namespace WFTileCounter.Migrations
{
    public partial class MoreColInTIleDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Objectives",
                schema: "website",
                table: "TileDetails");

            migrationBuilder.AddColumn<bool>(
                name: "Ayatan",
                schema: "website",
                table: "TileDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Cache",
                schema: "website",
                table: "TileDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CaptureSpawn",
                schema: "website",
                table: "TileDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Cephalon",
                schema: "website",
                table: "TileDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DefectionRest",
                schema: "website",
                table: "TileDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DefectionSpawn",
                schema: "website",
                table: "TileDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FrameFighter",
                schema: "website",
                table: "TileDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Medallion",
                schema: "website",
                table: "TileDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "MobileDefense",
                schema: "website",
                table: "TileDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PlayerExtract",
                schema: "website",
                table: "TileDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "PlayerSpawn",
                schema: "website",
                table: "TileDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RareContainer",
                schema: "website",
                table: "TileDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SimarisSpawn",
                schema: "website",
                table: "TileDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Somachord",
                schema: "website",
                table: "TileDetails",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SurvivalPylon",
                schema: "website",
                table: "TileDetails",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ayatan",
                schema: "website",
                table: "TileDetails");

            migrationBuilder.DropColumn(
                name: "Cache",
                schema: "website",
                table: "TileDetails");

            migrationBuilder.DropColumn(
                name: "CaptureSpawn",
                schema: "website",
                table: "TileDetails");

            migrationBuilder.DropColumn(
                name: "Cephalon",
                schema: "website",
                table: "TileDetails");

            migrationBuilder.DropColumn(
                name: "DefectionRest",
                schema: "website",
                table: "TileDetails");

            migrationBuilder.DropColumn(
                name: "DefectionSpawn",
                schema: "website",
                table: "TileDetails");

            migrationBuilder.DropColumn(
                name: "FrameFighter",
                schema: "website",
                table: "TileDetails");

            migrationBuilder.DropColumn(
                name: "Medallion",
                schema: "website",
                table: "TileDetails");

            migrationBuilder.DropColumn(
                name: "MobileDefense",
                schema: "website",
                table: "TileDetails");

            migrationBuilder.DropColumn(
                name: "PlayerExtract",
                schema: "website",
                table: "TileDetails");

            migrationBuilder.DropColumn(
                name: "PlayerSpawn",
                schema: "website",
                table: "TileDetails");

            migrationBuilder.DropColumn(
                name: "RareContainer",
                schema: "website",
                table: "TileDetails");

            migrationBuilder.DropColumn(
                name: "SimarisSpawn",
                schema: "website",
                table: "TileDetails");

            migrationBuilder.DropColumn(
                name: "Somachord",
                schema: "website",
                table: "TileDetails");

            migrationBuilder.DropColumn(
                name: "SurvivalPylon",
                schema: "website",
                table: "TileDetails");

            migrationBuilder.AddColumn<string>(
                name: "Objectives",
                schema: "website",
                table: "TileDetails",
                maxLength: 50,
                nullable: true);
        }
    }
}
