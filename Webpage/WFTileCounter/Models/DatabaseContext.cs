using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WFTileCounter.Models;
using WFTileCounter.ModelsLogic;

namespace WFTileCounter.Models
{
    //Entity Frame Work Database Context class. 
    public class DatabaseContext : DbContext
    {
       
      
        public DatabaseContext(string connectionString) : base(GetOptions(connectionString))
        {

        }

        public DatabaseContext(DbContextOptions options) : base(options)
        {

        }

        private static DbContextOptions GetOptions(string cnn)
        {
            var modelBuilder = new DbContextOptionsBuilder();
            return modelBuilder.UseSqlServer(cnn).Options;

        }


        public DbSet<Mission> Missions { get; set; }
        public DbSet<Tile> Tiles { get; set; }
        public DbSet<Run> Runs { get; set; }
        public DbSet<MapPoint> MapPoints { get; set; }
        public DbSet<Tileset> Tilesets { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<TileImage> TileImages { get; set; }
        public DbSet<VariantTile> VariantTiles { get; set; }
        public DbSet<TileDetail> TileDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MapPoint>()
                .Property(c => c.Collectibles)
                .HasConversion<string>();
            modelBuilder.Entity<MapPoint>()
                .Property(c => c.Objectives)
                .HasConversion<string>();
            modelBuilder.Entity<MapPoint>()
                .Property(c => c.Scanables)
                .HasConversion<string>();
        }

    }
}