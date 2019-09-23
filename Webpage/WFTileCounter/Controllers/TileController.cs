using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WFTileCounter.BuisnessLogic;
using WFTileCounter.Models;
using WFTileCounter.ModelsView;

namespace WFTileCounter.Controllers
{
    public class TileController : Controller
    {

        private readonly DatabaseContext _db; //database context shortcut



        public TileController(DatabaseContext context)
        {
            _db = context;
        }


       

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Route("Tile/{tileset}/{tilename}/Edit")]
        public IActionResult EditTile([FromRoute] string tileName,[FromRoute] string tileset)
        {

            var _tf = new TileFunctions(_db);

            //if(GetUser()== Approved Admin ID)


            string fullTileName = tileset + tileName;
            var details = _tf.GetFullTileInformation(fullTileName);

            

            if(details is null)
            {
                return View("Index");
            }

            details.Numbers = GenerateNumbers();

            return View("EditTile", details);

            //else return 404
        }

        [HttpPost]
        public async Task<IActionResult> Update(TileDetailsViewModel tileDetails)
        {


            //if(GetUser()== Approved Admin ID)


            var tile = _db.Tiles.Where(x => x.Name == tileDetails.Tile.Name).Include(x => x.TileDetail).ThenInclude(x => x.VariantTiles).Include(x => x.TileImages).FirstOrDefault();

            //Tile Edit can only pull from tiles already in the datbaase, so it should damn well exist.

            //clean up to make things look better:

            var tileDetailsAlreadyInDb = tile.TileDetail;
            var tileDetailsInInsert = tileDetails.Tile.TileDetail;

            var tileName = tile.Name.ToString();




            //Check to see if the TileDetails exist for given tile. If not, add them, if so, update them.
            if (tileDetailsAlreadyInDb is null)
            {
                tileDetailsInInsert.Tile = tile;
                _db.TileDetails.Add(tileDetailsInInsert);
            }
            else
            {
                tileDetailsInInsert.Tile = tile;
                tileDetailsAlreadyInDb = tileDetailsInInsert;
                _db.TileDetails.Update(tileDetailsAlreadyInDb);
            }


            /*
             * 
             * var variantsAlreadyInDb = tile.TileDetail.VariantTiles;
            var varientsInInsert = tileDetails.Tile.TileDetail.VariantTiles;

            var imagesAlreadyInDb = tile.TileImages;
            var imagesInInsert = tileDetails.Tile.TileImages;

            //Check to see if there is already a list of Variant Tiles in the db. If not, add each varient listed one by one.
           if (variantsAlreadyInDb is null)
            {
                foreach(var varient in varientsInInsert)
                {
                    _db.VariantTiles.Add(varient);
                }
            }
            else // if there already is a list, find out if the variant suggested is already in the list. If it is, ignore because it is just a single col. Otherwise, add it new.
            {
                var listOfVariant
                foreach (var varient in varientsInInsert)
                {
                    string varTileName = varient.VariantTileName.ToString();
                    if(!variantsAlreadyInDb.Contains(varTileName))
                    {
                        _db.VariantTiles.Add(varient);
                    }
                }

            }

        */

            await _db.SaveChangesAsync();

            return View("Index");

            //else return 404
        }



        private static List<SelectListItem> GenerateNumbers()
        {
            var numbers = (from p in Enumerable.Range(0, 10)
                           select new SelectListItem
                           {
                               Text = p.ToString(),
                               Value = p.ToString()
                           });
            return numbers.ToList();
        }





    }








}