using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WFTileCounter.Migrations
{
    public partial class first : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "website");

            migrationBuilder.CreateTable(
                name: "Missions",
                columns: table => new
                {
                    Type = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Missions", x => x.Type);
                });

            migrationBuilder.CreateTable(
                name: "Tilesets",
                columns: table => new
                {
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Faction = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tilesets", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Username = table.Column<string>(maxLength: 150, nullable: false),
                    email = table.Column<string>(maxLength: 150, nullable: false),
                    RunsUploaded = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tiles",
                columns: table => new
                {
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    PopularName = table.Column<string>(maxLength: 100, nullable: true),
                    TilesetName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tiles", x => x.Name);
                    table.ForeignKey(
                        name: "FK_Tiles_Tilesets_TilesetName",
                        column: x => x.TilesetName,
                        principalTable: "Tilesets",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Runs",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RunDate = table.Column<DateTime>(nullable: false),
                    MissionType = table.Column<string>(nullable: true),
                    UserID = table.Column<int>(nullable: false),
                    IdentityString = table.Column<string>(maxLength: 250, nullable: false),
                    LogRange = table.Column<string>(maxLength: 250, nullable: true),
                    TotalTiles = table.Column<int>(nullable: false),
                    UniqueTiles = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Runs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Runs_Missions_MissionType",
                        column: x => x.MissionType,
                        principalTable: "Missions",
                        principalColumn: "Type",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Runs_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TileImages",
                schema: "website",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TileName = table.Column<string>(nullable: false),
                    TilesetName = table.Column<string>(nullable: true),
                    ViewName = table.Column<string>(maxLength: 50, nullable: true),
                    ImgName = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TileImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TileImages_Tiles_TileName",
                        column: x => x.TileName,
                        principalTable: "Tiles",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TileImages_Tilesets_TilesetName",
                        column: x => x.TilesetName,
                        principalTable: "Tilesets",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MapPoints",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RunId = table.Column<int>(nullable: false),
                    TileName = table.Column<string>(nullable: false),
                    CoordsTaken = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MapPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MapPoints_Runs_RunId",
                        column: x => x.RunId,
                        principalTable: "Runs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MapPoints_Tiles_TileName",
                        column: x => x.TileName,
                        principalTable: "Tiles",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MapPoints_RunId",
                table: "MapPoints",
                column: "RunId");

            migrationBuilder.CreateIndex(
                name: "IX_MapPoints_TileName",
                table: "MapPoints",
                column: "TileName");

            migrationBuilder.CreateIndex(
                name: "IX_Runs_MissionType",
                table: "Runs",
                column: "MissionType");

            migrationBuilder.CreateIndex(
                name: "IX_Runs_UserID",
                table: "Runs",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Tiles_TilesetName",
                table: "Tiles",
                column: "TilesetName");

            migrationBuilder.CreateIndex(
                name: "IX_TileImages_TileName",
                schema: "website",
                table: "TileImages",
                column: "TileName");

            migrationBuilder.CreateIndex(
                name: "IX_TileImages_TilesetName",
                schema: "website",
                table: "TileImages",
                column: "TilesetName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MapPoints");

            migrationBuilder.DropTable(
                name: "TileImages",
                schema: "website");

            migrationBuilder.DropTable(
                name: "Runs");

            migrationBuilder.DropTable(
                name: "Tiles");

            migrationBuilder.DropTable(
                name: "Missions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Tilesets");
        }
    }
}
