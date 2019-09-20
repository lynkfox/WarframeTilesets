using Microsoft.EntityFrameworkCore.Migrations;

namespace WFTileCounter.Migrations
{
    public partial class AddSpawnableToMapPoints : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {/*
            migrationBuilder.AddColumn<string>(
                name: "Spawnable",
                table: "MapPoints",
                nullable: true);
                */
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Spawnable",
                table: "MapPoints");
        }
    }
}
