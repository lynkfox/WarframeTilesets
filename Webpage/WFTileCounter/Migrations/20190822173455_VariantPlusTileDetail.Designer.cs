﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using WFTileCounter.Models;

namespace WFTileCounter.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20190822173455_VariantPlusTileDetail")]
    partial class VariantPlusTileDetail
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.6-servicing-10079")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("WFTileCounter.Models.MapPoint", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("CoordsTaken")
                        .HasMaxLength(50);

                    b.Property<int>("RunId");

                    b.Property<string>("TileName")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RunId");

                    b.HasIndex("TileName");

                    b.ToTable("MapPoints");
                });

            modelBuilder.Entity("WFTileCounter.Models.Mission", b =>
                {
                    b.Property<string>("Type")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(100);

                    b.HasKey("Type");

                    b.ToTable("Missions");
                });

            modelBuilder.Entity("WFTileCounter.Models.Run", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("IdentityString")
                        .IsRequired()
                        .HasMaxLength(250);

                    b.Property<string>("LogRange")
                        .HasMaxLength(250);

                    b.Property<string>("MissionType");

                    b.Property<DateTime>("RunDate");

                    b.Property<int>("TotalTiles");

                    b.Property<int>("UniqueTiles");

                    b.Property<int>("UserID");

                    b.HasKey("Id");

                    b.HasIndex("MissionType");

                    b.HasIndex("UserID");

                    b.ToTable("Runs");
                });

            modelBuilder.Entity("WFTileCounter.Models.Tile", b =>
                {
                    b.Property<string>("Name")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(100);

                    b.Property<string>("TilesetName");

                    b.HasKey("Name");

                    b.HasIndex("TilesetName");

                    b.ToTable("Tiles");
                });

            modelBuilder.Entity("WFTileCounter.Models.TileDetail", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("BehindName")
                        .HasColumnType("ntext");

                    b.Property<int?>("Consoles");

                    b.Property<string>("Description")
                        .HasColumnType("ntext");

                    b.Property<int?>("Exits");

                    b.Property<int?>("Hazards");

                    b.Property<int?>("LockerBanks");

                    b.Property<int?>("LootRooms");

                    b.Property<string>("Objectives")
                        .HasMaxLength(50);

                    b.Property<string>("PopularName")
                        .HasMaxLength(100);

                    b.Property<int?>("Secrets");

                    b.Property<string>("TileName")
                        .IsRequired();

                    b.Property<int?>("TotalLockers");

                    b.HasKey("Id");

                    b.HasIndex("TileName")
                        .IsUnique();

                    b.ToTable("TileDetails","website");
                });

            modelBuilder.Entity("WFTileCounter.Models.TileImage", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("ImgName")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<string>("TileName")
                        .IsRequired();

                    b.Property<string>("TilesetName");

                    b.Property<string>("ViewName")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.HasIndex("TileName");

                    b.HasIndex("TilesetName");

                    b.ToTable("TileImages","website");
                });

            modelBuilder.Entity("WFTileCounter.Models.Tileset", b =>
                {
                    b.Property<string>("Name")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(100);

                    b.Property<string>("Faction")
                        .HasMaxLength(50);

                    b.HasKey("Name");

                    b.ToTable("Tilesets");
                });

            modelBuilder.Entity("WFTileCounter.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("RunsUploaded");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(150);

                    b.Property<string>("email")
                        .IsRequired()
                        .HasMaxLength(150);

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("WFTileCounter.Models.VariantTile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("TileName");

                    b.Property<string>("VariantTileName")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.HasIndex("TileName");

                    b.ToTable("VariantTiles","website");
                });

            modelBuilder.Entity("WFTileCounter.Models.MapPoint", b =>
                {
                    b.HasOne("WFTileCounter.Models.Run", "Run")
                        .WithMany("MapPoints")
                        .HasForeignKey("RunId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WFTileCounter.Models.Tile", "Tile")
                        .WithMany("MapPoints")
                        .HasForeignKey("TileName")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WFTileCounter.Models.Run", b =>
                {
                    b.HasOne("WFTileCounter.Models.Mission", "Mission")
                        .WithMany("Runs")
                        .HasForeignKey("MissionType");

                    b.HasOne("WFTileCounter.Models.User", "User")
                        .WithMany("Runs")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WFTileCounter.Models.Tile", b =>
                {
                    b.HasOne("WFTileCounter.Models.Tileset", "Tileset")
                        .WithMany("Tiles")
                        .HasForeignKey("TilesetName");
                });

            modelBuilder.Entity("WFTileCounter.Models.TileDetail", b =>
                {
                    b.HasOne("WFTileCounter.Models.Tile", "Tile")
                        .WithOne("TileDetail")
                        .HasForeignKey("WFTileCounter.Models.TileDetail", "TileName")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("WFTileCounter.Models.TileImage", b =>
                {
                    b.HasOne("WFTileCounter.Models.Tile", "Tile")
                        .WithMany("TileImages")
                        .HasForeignKey("TileName")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("WFTileCounter.Models.Tileset", "Tileset")
                        .WithMany("Images")
                        .HasForeignKey("TilesetName");
                });

            modelBuilder.Entity("WFTileCounter.Models.VariantTile", b =>
                {
                    b.HasOne("WFTileCounter.Models.TileDetail", "Details")
                        .WithMany("VariantTiles")
                        .HasForeignKey("TileName")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
