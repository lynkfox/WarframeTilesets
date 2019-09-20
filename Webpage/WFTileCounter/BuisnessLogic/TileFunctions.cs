using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
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
            fullDetailsOfTile.Variants = tile.TileDetail.VariantTiles;
            fullDetailsOfTile.Images = tile.TileImages.Where(x=>x.ViewName !="Map");

            fullDetailsOfTile.Map = _gf.GetMapImagePath(tile.Name);


            return fullDetailsOfTile;
        }
    }
}
