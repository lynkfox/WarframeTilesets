using Microsoft.EntityFrameworkCore.Migrations;

namespace WFTileCounter.Migrations
{
    public partial class TileDetailAdjustments : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LockerBanks",
                schema: "website",
                table: "TileDetails");

            migrationBuilder.RenameColumn(
                name: "TotalLockers",
                schema: "website",
                table: "TileDetails",
                newName: "Lockers");

            migrationBuilder.AddColumn<string>(
                name: "HazardDescription",
                schema: "website",
                table: "TileDetails",
                type: "ntext",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SecretDescription",
                schema: "website",
                table: "TileDetails",
                type: "ntext",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HazardDescription",
                schema: "website",
                table: "TileDetails");

            migrationBuilder.DropColumn(
                name: "SecretDescription",
                schema: "website",
                table: "TileDetails");

            migrationBuilder.RenameColumn(
                name: "Lockers",
                schema: "website",
                table: "TileDetails",
                newName: "TotalLockers");

            migrationBuilder.AddColumn<int>(
                name: "LockerBanks",
                schema: "website",
                table: "TileDetails",
                nullable: true);
        }
    }
}
