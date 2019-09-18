using Microsoft.EntityFrameworkCore.Migrations;

namespace WFTileCounter.Migrations
{
    public partial class ChangeColNameInRuns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PreMapPoint",
                table: "Runs",
                newName: "MapPointsUsed");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "MapPointsUsed",
                table: "Runs",
                newName: "PreMapPoint");
        }
    }
}
