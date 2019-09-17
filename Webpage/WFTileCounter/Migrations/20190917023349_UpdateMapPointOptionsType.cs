using Microsoft.EntityFrameworkCore.Migrations;

namespace WFTileCounter.Migrations
{
    public partial class UpdateMapPointOptionsType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Scanables",
                table: "MapPoints",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Objectives",
                table: "MapPoints",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Collectibles",
                table: "MapPoints",
                nullable: true,
                oldClrType: typeof(int),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Scanables",
                table: "MapPoints",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Objectives",
                table: "MapPoints",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Collectibles",
                table: "MapPoints",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
