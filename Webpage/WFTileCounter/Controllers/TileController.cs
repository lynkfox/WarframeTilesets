using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
        private IHostingEnvironment _env;


        public TileController(DatabaseContext context, IHostingEnvironment env)
        {
            _db = context;
            _env = env;
        }


       

        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        [Route("Tile/{tileset}/{tileName}")]
        public IActionResult ViewTile([FromRoute] string tileName,[FromRoute] string tileset)
        {

            var _tf = new TileFunctions(_db);

            tileset = AccountForTilesetVariations(tileset);

            string fullTileName = tileset + tileName;
            var details = _tf.GetFullTileInformation(fullTileName);

            

            if(details.ShortTileName == "Does Not Exist")
            {
                return View("NoTile");
            }else if (details.Tile.TileDetail is null)
            {
                details.Tile.TileDetail = new TileDetail();
            }

            details.Variants = details.Variants.Where(x => !string.IsNullOrEmpty(x.VariantTileName)).ToList();

            details.ShortTileName = tileName;
            details.TilesetName = tileset;

            return View("View", details);

        }

        

        [HttpGet]
        [Route("Tile/{tileset}/{tilename}/Edit")]
        public IActionResult EditTile([FromRoute] string tileName, [FromRoute] string tileset)
        {

            var _tf = new TileFunctions(_db);

            //if(GetUser()== Approved Admin ID)

            tileset = AccountForTilesetVariations(tileset);
            string fullTileName = tileset + tileName;
            var details = _tf.GetFullTileInformation(fullTileName);



            if (details.ShortTileName == "Does Not Exist")
            {
                return View("NoTile");
            }

            details.Numbers = GenerateNumbers();

            return View("EditTile", details);

            //else return 404
        }

        [HttpPost]
        public async Task<IActionResult> Update(TileDetailsViewModel tileDetails)
        {


            //if(GetUser()== Approved Admin ID)


            Tile tile = GetDatabaseTileInformationAsTile(tileDetails);

            if (tile is null)
            {
                return View("NoTile");
            }

            //clean up to make things look better:

            var tileDetailsAlreadyInDb = tile.TileDetail;
            var tileDetailsInInsert = tileDetails.Tile.TileDetail;
            string tileName = GetTileName(tile);

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


            // deal with the Variant Tile List

            var tileVariantsInDb = tile.TileDetail.VariantTiles;
            var tileVariantsInInsert = tileDetails.Variants.Where(x => !string.IsNullOrEmpty(x.VariantTileName)).ToList();


            if (tileVariantsInDb is null)
            {
                for (int i = 0; i < tileVariantsInInsert.Count(); i++)
                {
                    var variant = tileVariantsInInsert[i];

                    var tileActuallyInDbForVariant = _db.Tiles.Where(x => x.Name == variant.VariantTileName).FirstOrDefault();
                    if (!(tileActuallyInDbForVariant is null)) //if the tilename properly exists
                    {
                        if (tileDetailsAlreadyInDb is null)
                        {
                            variant.Details = tileDetailsInInsert;
                        }
                        else
                        {
                            variant.Details = tileDetailsAlreadyInDb;
                        }
                        _db.VariantTiles.Add(variant);
                    } //else (throw error about variant Tile Name


                }
            }
            else
            {
                foreach (var variant in tileVariantsInInsert)
                {
                    string variantName = variant.VariantTileName;
                    int variantId = variant.Id;



                    //if the Variant already has an ID, which should mean we pulled it from the database to display it...
                    if (tileVariantsInDb.Where(x => x.Id == variantId).Any())
                    {
                        var variantDB = tileVariantsInDb.Where(x => x.Id == variantId).FirstOrDefault();
                        variantDB = variant;
                        _db.VariantTiles.Update(variantDB);
                    }
                    else //don't have an id and so not yet in the db
                    {
                        var tileActuallyInDbForVariant = _db.Tiles.Where(x => x.Name == variant.VariantTileName).FirstOrDefault();
                        if (!(tileActuallyInDbForVariant is null)) //if the tilename properly exists
                        {
                            //if the VairantName isn't already listed for this particular tile
                            if (!tileVariantsInDb.Where(x => x.VariantTileName == variantName).Any())
                            {
                                _db.Add(variant);
                            }
                        } // else throw error about variant Tile Name not existing
                    }
                }
            }




            await _db.SaveChangesAsync();

            var webRoot = _env.WebRootPath;
            string tilesetName = tile.Tileset.Name.ToString();
            string directoryPath = Path.Combine(webRoot, "img", "tilesets", tilesetName, tileName);



            if (tileDetails.ImageFiles is null)
            {

            }
            else //(tileDetails.ImageFiles.Count()!=0)
            {
                List<TileImage> tileImagesList = new List<TileImage>();
                foreach (var file in tileDetails.ImageFiles)
                {
                    if (file == null || file.Length == 0)
                    {
                        continue;
                    }



                    var imagePath = Path.Combine(directoryPath,
                                 file.FileName);

                    Directory.CreateDirectory(directoryPath);

                    var tileImage = new TileImage();

                    if (tileDetails.ImageFiles.First() == file)
                    {
                        tileImage.TilesAlreadyInDatabase = _db.TileImages.Where(x => x.Tile.Name == tileDetails.Tile.Name).ToList();
                    }

                    tileImage.ImageName = file.FileName;
                    tileImage.ImagePath = Path.Combine(tilesetName, tileName, file.FileName);
                    tileImage.Tile = tile;
                    tileImage.TileName = tile.Name; // for later use when passing this model to the next controller


                    tileImage.AlreadyExists = await MoveImageToTileDirectory(imagePath, file);



                    tileImagesList.Add(tileImage);

                }

                //can I route this with  [Route("Tile/{tileset}/{tilename}/Edit")] and then be able to pullt he tilename dynamically? 
                return View("ImageDetails", tileImagesList);
            }


            string smallTileName = tileName.Replace(tilesetName, "");


            return RedirectToAction("ViewTile", new { tileset = tilesetName, tilename = smallTileName });

            //else return 404
        }

        private static string GetTileName(Tile tile)
        {
            return tile.Name.ToString();
        }

        private Tile GetDatabaseTileInformationAsTile(TileDetailsViewModel tileDetails)
        {
            return _db.Tiles.Where(x => x.Name == tileDetails.Tile.Name)
                            .Include(x => x.TileDetail).ThenInclude(x => x.VariantTiles)
                            .Include(x => x.TileImages)
                            .Include(x => x.Tileset)
                            .FirstOrDefault();
        }

        private async Task<bool> MoveImageToTileDirectory(string imagePath, IFormFile file)
        {
            if (System.IO.File.Exists(imagePath))
            {
                
                //need overwrite option?
                System.IO.File.Delete(imagePath);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);

                }
                return true;
            }
            else
            {
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);

                }
                return false;
            }
        }

        public async Task<IActionResult> ImageEdit(List<TileImage> images)
        {

            string tileName ="", tilesetName ="";
            foreach(var img in images)
            {
                
               
                img.Tile = _db.Tiles.Where(x => x.Name == img.TileName).Include(x=>x.Tileset).FirstOrDefault();

                var imgInDb = _db.TileImages.Where(x => x.ImageName == img.ImageName && x.TileName == img.TileName).FirstOrDefault();
                if(imgInDb is null)
                {
                    _db.Add(img);
                }
                else
                {
                    imgInDb.ViewName = img.ViewName;
                    imgInDb.AltText = img.AltText;
                    _db.TileImages.Update(imgInDb);

                }

                if (images.First() == img)
                {
                    tilesetName = img.Tile.Tileset.Name;
                    tileName = img.Tile.Name;
                    tileName = tileName.Replace(tilesetName, "");
                }
            }
            await _db.SaveChangesAsync();

            return RedirectToAction("ViewTile", new { tileset = tilesetName, tilename = tileName });
        }




        //Internal private Methods that probably should be cleaned up on a seperate object.



        private static List<SelectListItem> GenerateNumbers()
        {
            var numbers = (from p in Enumerable.Range(0, 26)
                           select new SelectListItem
                           {
                               Text = p.ToString(),
                               Value = p.ToString()
                           });
            return numbers.ToList();
        }


        private string AccountForTilesetVariations(string tileset)
        {
            tileset = tileset.ToLower();

            return FixAllVariationsOfTilesets(tileset);

        }

        private string FixAllVariationsOfTilesets(string tileset)
        {
            return GrineerTilesetNameFixes(tileset);
        }

        private string GrineerTilesetNameFixes(string tileset)
        {
            if (tileset == "grineerasteroid" || tileset == "asteroid" || tileset == "grn")
            {
                return "GrineerAsteroid";
            }
            else if (tileset == "cmp" || tileset == "grineersettlement" || tileset == "settlement" || tileset == "mars")
            {
                return "GrineerSettlement";
            }
            else if (tileset == "galleon" || tileset == "grineergalleon" || tileset == "grineership")
            {
                return "GrineerGalleon";
            }
            else if (tileset == "grineerforest" || tileset == "forest" || tileset == "gft" || tileset == "gftremastered" || tileset == "earth")
            {
                return "GrineerForest";
            }
            else if (tileset == "grineerocean" || tileset == "sharkwing" || tileset == "oceanlab" || tileset == "ocean" || tileset == "sealab" || tileset == "uranus")
            {
                return "GrineerOcean";
            }
            else if (tileset == "grineershipyards" || tileset == "shipyards")
            {
                return "GrineerShipyards";
            }
            else
            {
                return OrokinTilesetNameFixes(tileset);
            }

        }

        private string OrokinTilesetNameFixes(string tileset)
        {
            if (tileset == "orokinmoon" || tileset == "moon" || tileset == "orokinlua" || tileset == "lua")
            {
                return "OrokinMoon";
            }
            else if (tileset == "orokintower" || tileset == "tower" || tileset == "void")
            {
                return "OrokinTower";
            }
            else
            {
                return InfestedTilesetNameFixes(tileset);
            }
        }

        private string InfestedTilesetNameFixes(string tileset)
        {
            if (tileset == "infestedcorpusship" || tileset == "infestedcorpus" || tileset == "infestedship" || tileset == "infested" || tileset == "eris")
            {
                return "Infested";
            }
            else if (tileset == "orokintowerderelict" || tileset == "derelict" || tileset == "orokinderelict" || tileset == "towerderelict")
            {
                return "OrokinDerelict";
            }
            else
            {
                return CorpusTilesetNameFixes(tileset);
            }
        }

        private string CorpusTilesetNameFixes(string tileset)
        {
            if (tileset == "corpusgascity" || tileset == "gascity" || tileset == "corpusgas" || tileset == "gas")
            {
                return "CorpusGas";
            }
            else if (tileset == "corpuscceplanet" || tileset == "corpusice" || tileset == "iceplanet" || tileset == "corpusplanet" || tileset == "ice")
            {
                return "CorpusIce";
            }
            else if (tileset == "corpusship" || tileset == "guild" || tileset == "corpus" || tileset == "ship")
            {
                return "CorpusShip";
            }
            else if (tileset == "corpusarchwing" || tileset == "crparchwing" || tileset == "tr" || tileset == "spacebattles")
            {
                return "CorpusArchwing";
            } else
            {
                return InvasionTilesetNameFixes(tileset);
            }
        }

        private string InvasionTilesetNameFixes(string tileset)
        {
            if (tileset == "grineertocorpus" || tileset == "invasiong2c" || tileset == "g2c")
            {
                return "InvasionG2C";
            }
            else if (tileset == "corpustogrineer" || tileset == "invasionc2g" || tileset == "c2g")
            {
                return "InvasionC2G";
            }
            else
            {
                return tileset;
            }
        }
    }








}