using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WFTileCounter.Models;
using WFTileCounter.ModelsView;

namespace WFTileCounter.BuisnessLogic
{
    public class TileFunctions
    {

        private readonly DatabaseContext _db; //database context shortcut



        public TileFunctions(DatabaseContext context)
        {
            _db = context;
        }


        public TileDetailsViewModel GetFullTileInformation(string tileName)
        {
            var _gf = new GeneralFunctions(_db);
            var fullDetailsOfTile = new TileDetailsViewModel();
            var tile = _db.Tiles.Where(x => x.Name == tileName)
                .Include(x => x.TileDetail).ThenInclude(x => x.VariantTiles)
                .Include(x => x.TileImages).FirstOrDefault();

            

            

            if (tile is null)
            {
                tile = new Tile();
            }
            if( tile.TileDetail is null)
            {
                tile.TileDetail = new TileDetail();
            }
            if( tile.TileDetail.VariantTiles is null)
            {
                tile.TileDetail.VariantTiles = new List<VariantTile>();
            }
            if (tile.TileImages is null)
            {
                tile.TileImages = new List<TileImage>();
            }

            fullDetailsOfTile.Tile = tile;
            fullDetailsOfTile.Details = tile.TileDetail;
            fullDetailsOfTile.Variants = tile.TileDetail.VariantTiles.ToList();
            fullDetailsOfTile.Images = tile.TileImages.Where(x=>x.ViewName !="Map");


            while(fullDetailsOfTile.Variants.Count()<5)
            {
                fullDetailsOfTile.Variants.Add(new VariantTile { Details = fullDetailsOfTile.Tile.TileDetail, VariantTileName = "" });
            }



            var mapPointCollectibles = _db.MapPoints.Where(x => x.Tile.Name == tileName);

            if(mapPointCollectibles.Where(x => x.Ayatan).Any() || tile.TileDetail.Ayatan)
            {
                fullDetailsOfTile.Details.Ayatan = true;
            }
            if (mapPointCollectibles.Where(x => x.CaptureSpawn).Any() || tile.TileDetail.CaptureSpawn)
            {
                fullDetailsOfTile.Details.CaptureSpawn = true;
            }
            if (mapPointCollectibles.Where(x => x.FrameFighter).Any() || tile.TileDetail.FrameFighter)
            {
                fullDetailsOfTile.Details.FrameFighter = true;
            }
            if (mapPointCollectibles.Where(x => x.Medallion).Any() || tile.TileDetail.Medallion)
            {
                fullDetailsOfTile.Details.Medallion= true;
            }
            if (mapPointCollectibles.Where(x => x.RareContainer).Any() || tile.TileDetail.RareContainer)
            {
                fullDetailsOfTile.Details.RareContainer = true;
            }
            if (mapPointCollectibles.Where(x => x.SimarisSpawn).Any() || tile.TileDetail.SimarisSpawn)
            {
                fullDetailsOfTile.Details.SimarisSpawn = true;
            }
            if (mapPointCollectibles.Where(x => x.Somachord).Any() || tile.TileDetail.Somachord)
            {
                fullDetailsOfTile.Details.Somachord = true;
            }
            if (mapPointCollectibles.Where(x => x.Cache).Any() || tile.TileDetail.Cache)
            {
                fullDetailsOfTile.Details.Cache = true;
            }

            foreach (var img in fullDetailsOfTile.Images)
            {
                img.ImagePath = GetImagePath(img.ImageName, tile.Name);
            }

            fullDetailsOfTile.Map = _gf.GetMapImagePath(tile.Name);


            return fullDetailsOfTile;
        }

        private string GetImagePath(string imageName, string tileName)
        {
            var tileInformation =_db.Tiles.Where(x => x.Name == tileName).Include(x => x.Tileset).FirstOrDefault();


            return Path.Combine(tileInformation.Tileset.Name, tileInformation.Name, imageName);

        }
    }
}
