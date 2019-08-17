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

namespace WFTileCounter.ControllersProcessing
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


        public ProcessController(DatabaseContext context)
        {
            _db = context;
        }


        public async Task<IActionResult> Index()
        {

            var _gf = new GeneralFunctions(_db); // class that holds various methods for clean use.
            var _df = new DatabaseFunctions(_db); // class that holds various database methods for clean use

            List<ImgMetaData> metaList = new List<ImgMetaData>();
            var path = _gf.GetPath();
            metaList = _gf.GetMetaList(path);

            List<InsertReadyData> insert = _df.ConvertToDatabase(metaList);

            ViewBag.newTiles  = await _df.InsertIntoDatabase(insert);

             
            

            return View(insert);
        }


        public IActionResult ProcessFiles()
        {
            List<ImgMetaData> metaList = new List<ImgMetaData>();

            var _gf = new GeneralFunctions(_db); // class that holds various methods for clean use.

            var path = _gf.GetPath();
            metaList = _gf.GetMetaList(path);


            return View("ProcessFiles", metaList);
        }

        
        [HttpPost]
        public IActionResult Keep(List<ImgMetaData> datas)
        {
            foreach(var piece in datas)
            {
                if (piece.UnknownValue == true) //If we get the UnknownValue flag set to true, then there is bad data in the process. Make sure this tile is NOT processed
                    piece.KeepThis = false;

                /* To do: Write the code for removing the unchecked files (KeepThis==False) and only passing the checked files on to be coverted
                 * 
                 */
                Debug.WriteLine("Tile Name: " + piece.FileName + " Keep? : " + piece.KeepThis);
            }
            return View("ProcessFiles2", datas);
        }




        
    }
}

