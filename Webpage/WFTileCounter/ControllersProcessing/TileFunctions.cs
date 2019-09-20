using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WFTileCounter.Models;
using WFTileCounter.ModelsView;

namespace WFTileCounter.ControllersProcessing
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
            var fullDetailsOfTile = new TileDetailsViewModel();
            var tile = _db.Tiles.Where(x => x.Name == tileName)
                .Include(x => x.TileDetail).ThenInclude(x => x.VariantTiles)
                .Include(x => x.TileImages).FirstOrDefault();
                
            if (tile is null)
            {
                return null;
            }

            fullDetailsOfTile.Tile = tile;
            fullDetailsOfTile.Details = tile.TileDetail;
            fullDetailsOfTile.Variants = tile.TileDetail.VariantTiles;
            fullDetailsOfTile.Images = tile.TileImages;

            //pull out the map image from the ienumerable returned by EF
            var mapImage = tile.TileImages.Where(x => x.ImageName == "Map").FirstOrDefault();
            if(mapImage is null) // if it doesn't exist, create the default
            {
                fullDetailsOfTile.Map = new TileImage { ImagePath = "LotusFlower.png", AltText = "No Map Image Uploaded Yet", ImageName = "Map" };
                
            }else
            {
                fullDetailsOfTile.Map = mapImage; // pull it out for seperate use in the View

                //get the images again but this time without any titled "Map"
                fullDetailsOfTile.Images = fullDetailsOfTile.Images.Where(x => x.ImageName != "Map"); 

            }


            return fullDetailsOfTile;
        }
    }
}
