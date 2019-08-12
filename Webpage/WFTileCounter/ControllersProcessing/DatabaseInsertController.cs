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


        public async Task<IActionResult> Index()
        {
            List<MetaProcessed> metaList = TempData["metaList"] as List<MetaProcessed>;

            var _gf = new GeneralFunctions();

            IEnumerable<ProcessedData> readyForInsert = _gf.ConvertToDatabase(metaList);

            foreach(var data in readyForInsert)
            {
                var miss = _db.Missions.Where(x => x.Type == data.Mission.Type);
                if(miss is null)
                {
                    _db.Missions.Add(data.Mission);
                   
                }
                else
                {
                    data.Mission = miss.FirstOrDefault();
                }

                var tSet = _db.Tilesets.Where(x => x.Name == data.Tileset.Name);
                if (miss is null)
                {
                    _db.Tilesets.Add(data.Tileset);
                }
                else
                {
                    data.Tileset = tSet.FirstOrDefault();
                }

                var run = _db.Runs.Where(x => x.IndentityString == data.Run.IndentityString);
                if (run is null)
                {
                    _db.Runs.Add(data.Run);

                }
                else
                {
                    data.Run = run.FirstOrDefault();
                }

                List<MapPoint> map = new List<MapPoint>();
                foreach (var tile in data.Tiles)
                {
                    var mapPoint = new MapPoint();

                    var t = _db.Tiles.Where(x => x.Name == tile.Name);
                    mapPoint.Run = data.Run;
                    if (t is null)
                    {
                        _db.Tiles.Add(tile);
                        mapPoint.Tile = tile;
                    } else
                    {
                        mapPoint.Tile = t.FirstOrDefault();
                    }

                    map.Add(mapPoint);

                }

                // This needs to be updated to be dynamic.
                data.User = _db.Users.Find(1);
            }

            await _db.SaveChangesAsync();

            return View(readyForInsert);
        }
    }
}