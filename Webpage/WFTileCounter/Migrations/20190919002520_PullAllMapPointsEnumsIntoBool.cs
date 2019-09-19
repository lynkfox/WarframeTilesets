using Microsoft.EntityFrameworkCore.Migrations;

namespace WFTileCounter.Migrations
{
    public partial class PullAllMapPointsEnumsIntoBool : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Collectibles",
                table: "MapPoints");

            migrationBuilder.DropColumn(
                name: "Scanables",
                table: "MapPoints");

            migrationBuilder.DropColumn(
                name: "Spawnable",
                table: "MapPoints");

            migrationBuilder.AddColumn<bool>(
                name: "Ayatan",
                table: "MapPoints",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CaptureSpawn",
                table: "MapPoints",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Cephalon",
                table: "MapPoints",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FrameFighter",
                table: "MapPoints",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Medallion",
                table: "MapPoints",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RareContainer",
                table: "MapPoints",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SimarisSpawn",
                table: "MapPoints",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Somachord",
                table: "MapPoints",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ayatan",
                table: "MapPoints");

            migrationBuilder.DropColumn(
                name: "CaptureSpawn",
                table: "MapPoints");

            migrationBuilder.DropColumn(
                name: "Cephalon",
                table: "MapPoints");

            migrationBuilder.DropColumn(
                name: "FrameFighter",
                table: "MapPoints");

            migrationBuilder.DropColumn(
                name: "Medallion",
                table: "MapPoints");

            migrationBuilder.DropColumn(
                name: "RareContainer",
                table: "MapPoints");

            migrationBuilder.DropColumn(
                name: "SimarisSpawn",
                table: "MapPoints");

            migrationBuilder.DropColumn(
                name: "Somachord",
                table: "MapPoints");

            migrationBuilder.AddColumn<string>(
                name: "Collectibles",
                table: "MapPoints",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Scanables",
                table: "MapPoints",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Spawnable",
                table: "MapPoints",
                nullable: false,
                defaultValue: "");
        }
    }
}
