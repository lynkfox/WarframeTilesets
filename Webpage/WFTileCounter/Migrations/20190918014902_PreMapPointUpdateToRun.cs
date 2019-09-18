using Microsoft.EntityFrameworkCore.Migrations;

namespace WFTileCounter.Migrations
{
    public partial class PreMapPointUpdateToRun : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PreMapPoint",
                table: "Runs",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AlterColumn<string>(
                name: "Spawnable",
                table: "MapPoints",
                nullable: false,
                defaultValue:"Nothing",
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Scanables",
                table: "MapPoints",
                nullable: false,
                defaultValue: "Nothing",
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Objectives",
                table: "MapPoints",
                nullable: false,
                defaultValue: "Nothing",
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Collectibles",
                table: "MapPoints",
                nullable: false,
                defaultValue: "Nothing",
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PreMapPoint",
                table: "Runs");

            migrationBuilder.AlterColumn<string>(
                name: "Spawnable",
                table: "MapPoints",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Scanables",
                table: "MapPoints",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Objectives",
                table: "MapPoints",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "Collectibles",
                table: "MapPoints",
                nullable: true,
                oldClrType: typeof(string));
        }
    }
}
