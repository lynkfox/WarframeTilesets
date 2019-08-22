using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WFTileCounter.Migrations
{
    public partial class VariantPlusTileDetail : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PopularName",
                table: "Tiles");

            migrationBuilder.CreateTable(
                name: "TileDetails",
                schema: "website",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TileName = table.Column<string>(nullable: false),
                    PopularName = table.Column<string>(maxLength: 100, nullable: true),
                    Exits = table.Column<int>(nullable: true),
                    Consoles = table.Column<int>(nullable: true),
                    LockerBanks = table.Column<int>(nullable: true),
                    TotalLockers = table.Column<int>(nullable: true),
                    Secrets = table.Column<int>(nullable: true),
                    LootRooms = table.Column<int>(nullable: true),
                    Hazards = table.Column<int>(nullable: true),
                    Objectives = table.Column<string>(maxLength: 50, nullable: true),
                    Description = table.Column<string>(type: "ntext", nullable: true),
                    BehindName = table.Column<string>(type: "ntext", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TileDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TileDetails_Tiles_TileName",
                        column: x => x.TileName,
                        principalTable: "Tiles",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VariantTiles",
                schema: "website",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TileName = table.Column<int>(nullable: false),
                    VariantTileName = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VariantTiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VariantTiles_TileDetails_TileName",
                        column: x => x.TileName,
                        principalSchema: "website",
                        principalTable: "TileDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TileDetails_TileName",
                schema: "website",
                table: "TileDetails",
                column: "TileName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VariantTiles_TileName",
                schema: "website",
                table: "VariantTiles",
                column: "TileName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VariantTiles",
                schema: "website");

            migrationBuilder.DropTable(
                name: "TileDetails",
                schema: "website");

            migrationBuilder.AddColumn<string>(
                name: "PopularName",
                table: "Tiles",
                maxLength: 100,
                nullable: true);
        }
    }
}
