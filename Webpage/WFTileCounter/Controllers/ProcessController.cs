using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MetadataExtractor;
using System.IO;
using WFTileCounter.ModelsLogic;
using WFTileCounter.ModelsView;
using WFTileCounter.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;

namespace WFTileCounter.BuisnessLogic
{

    /*This Controller is mostly used at the moment for testing purposes. - It has everything needed to parse, display, and insert the data into the database.
     * 
     * However, future release iterations I want a lot of that more seperate, or more streamlined. When I proceed to actually taking Uploaded images, storing them,
     * and processsing their meta data, this controller will be depreciated for a more accurately named, and more streamlined version
     * 
     */


    public class ProcessController : Controller
    {

        private readonly DatabaseContext _db; //database context shortcut
        private IHostingEnvironment _env;


        public ProcessController(DatabaseContext context, IHostingEnvironment env)
        {
            _db = context;
            _env = env;
        }


        public async Task<IActionResult> DeveloperSkip()
        {

            /* Developer Environment Only Link that skips the filter check and will upload everything without checking the checkboxes*/

            var _gf = new GeneralFunctions(_db); // class that holds various methods for clean use.
            var _df = new DatabaseFunctions(_db); // class that holds various database methods for clean use

            List<ImgMetaData> metaList = new List<ImgMetaData>();
            var path = _gf.DeveloperAutoGrabGetPath();
            metaList = _gf.GetMetaList(path);

            List<InsertReadyData> insert = _df.ConvertToDatabase(metaList);

            ViewBag.newTiles  = await _df.InsertIntoDatabase(insert);

            return View("Success", insert);
        }

        public IActionResult ProcessUploadedFiles()
        {
            List<ImgMetaData> metaList = new List<ImgMetaData>();
            var _gf = new GeneralFunctions(_db); // class that holds various methods for clean use.
            var webRoot = _env.WebRootPath;
            
            string userId = _gf.GetUserId().ToString();
            string path = Path.Combine(webRoot, "temp_uploads", userId);

            List<string> newPaths =_gf.MoveFilesToMapIdDirectory(path);

            if (newPaths == null|| (newPaths.Count()==1 && string.IsNullOrEmpty(newPaths[0])))
            {
                return View("NoData");
            }

            foreach(var nPath in newPaths)
            {
                metaList.AddRange(_gf.GetMetaList(nPath));
            }
            


            return View("Review", metaList);
        }


        //depreciated
        public IActionResult ProcessFiles()
        {
            List<ImgMetaData> metaList = new List<ImgMetaData>();

            var _gf = new GeneralFunctions(_db); // class that holds various methods for clean use.

            var path = _gf.DeveloperAutoGrabGetPath();
            metaList = _gf.GetMetaList(path);


            return View("Review", metaList);
        }

        
        [HttpPost]
        public async Task<IActionResult> Keep(List<ImgMetaData> imagesThatHaveBeenReviewed)
        {
            
            var _df = new DatabaseFunctions(_db); // class that holds various database methods for clean use

            List<ImgMetaData> keepTheseTiles = ProcessImagesToKeepOrDelete(imagesThatHaveBeenReviewed);
            
            if (keepTheseTiles.Count() == 0 || keepTheseTiles is null)
            {
                return View("NoData");
            }
            else
            {
                List<InsertReadyData> insert = _df.ConvertToDatabase(keepTheseTiles);

                ViewBag.newTiles = await _df.InsertIntoDatabase(insert);

                return View("Success", insert);
            }
        }

        private List<ImgMetaData> ProcessImagesToKeepOrDelete(List<ImgMetaData> datas)
        {
            List<ImgMetaData> keepTheseTiles = new List<ImgMetaData>();

            foreach (var piece in datas)
            {

                if (piece.UnknownValue)
                {
                    piece.KeepThis = false;
                }
                    

                if (piece.KeepThis)
                {
                    keepTheseTiles.Add(piece);
                }
                else
                {
                    DeleteUnwantedTilePictureFromUserDirectory(piece);
                }

            }

            return keepTheseTiles;
        }

        private void DeleteUnwantedTilePictureFromUserDirectory(ImgMetaData piece)
        {
            var _gf = new GeneralFunctions(_db);
            var pathToDeleteFile = _gf.GetPath(piece);
            if (System.IO.File.Exists(pathToDeleteFile))
            {
                System.IO.File.Delete(pathToDeleteFile);
            }
        }
    }
}

