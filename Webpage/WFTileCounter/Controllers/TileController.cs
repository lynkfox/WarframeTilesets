using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WFTileCounter.ControllersProcessing;
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


        private readonly TileFunctions _tf;

        public IActionResult Index()
        {
            return View();
        }

        [Route("Tile/{tileset}/{tilename}/Edit")]
        public IActionResult EditTile([FromRoute] string tileName,[FromRoute] string tileset)
        {
            //if(GetUser()== Approved Admin ID)
            
            string fullTileName = tileset + tileName;
            var details = _tf.GetFullTileInformation(fullTileName);

            if(details is null)
            {
                return View("Index");
            }

            return View("EditTile", details);

            //else return 404
        }
    }
}