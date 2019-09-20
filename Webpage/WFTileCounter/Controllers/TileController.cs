using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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