using Microsoft.EntityFrameworkCore.Migrations;

namespace WFTileCounter.Migrations
{
    public partial class MapPointsEnumCorrection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AlterColumn<string>(
                name: "Spawnable",
                table: "MapPoints",
                nullable: false,
                defaultValue: "Nothing",
                oldClrType: typeof(string),
                oldNullable: true) ;

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
