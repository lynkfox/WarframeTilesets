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

namespace WFTileCounter.ControllersProcessing
{
    public class DatabaseInsertController : Controller
    {
        private readonly DatabaseContext _db; //database context shortcut

        public DatabaseInsertController(DatabaseContext context)
        {
            _db = context;
        }


        public IActionResult Index()
        {
            List<MetaProcessed> metaList = TempData["metaList"] as List<MetaProcessed>;

            var _gf = new GeneralFunctions();

            IEnumerable<ProcessedData> readyForInsert = _gf.ConvertToDatabase(metaList);

            return View();
        }
    }
}