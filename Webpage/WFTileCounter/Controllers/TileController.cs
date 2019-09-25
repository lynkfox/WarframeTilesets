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


            var tile = _db.Tiles.Where(x => x.Name == tileDetails.Tile.Name).Include(x => x.TileDetail).ThenInclude(x => x.VariantTiles).Include(x => x.TileImages).Include(x=>x.Tileset).FirstOrDefault();

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

            
            var webRoot = _env.WebRootPath;
            string tilesetName = tile.Tileset.Name.ToString();
            string directoryPath = Path.Combine(webRoot,"img","tilesets",tilesetName, tileName);
            await _db.SaveChangesAsync();
            if(tileDetails.ImageFiles is null)
            {

            }else //(tileDetails.ImageFiles.Count()!=0)
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

                    tileImage.ImageName = file.FileName;
                    tileImage.ImagePath = imagePath;
                    tileImage.Tile = tile;
                    tileImage.TileName = tile.Name; // for later use when passing this model to the next controller

                    tileImagesList.Add(tileImage);
                    if (System.IO.File.Exists(imagePath))
                    {
                        //need overwrite option?
                        System.IO.File.Delete(imagePath);
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);

                        }
                    }
                    else
                    {
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);

                        }

                    }

                }

                //can I route this with  [Route("Tile/{tileset}/{tilename}/Edit")] and then be able to pullt he tilename dynamically? 
                return View("ImageDetails", tileImagesList);
            }

            

            return View("Index");

            //else return 404
        }


        public async Task<IActionResult> ImageEdit(List<TileImage> images)
        {
            

            foreach(var img in images)
            {
               
                img.Tile = _db.Tiles.Where(x => x.Name == img.TileName).FirstOrDefault();
                _db.Add(img);
            }
            await _db.SaveChangesAsync();

            return View("Index");
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