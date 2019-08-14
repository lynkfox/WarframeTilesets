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

namespace WFTileCounter.ControllersProcessing
{

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

        




        
    }
}

