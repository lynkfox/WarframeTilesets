using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WFTileCounter.Models;
using WFTileCounter.Models.FrequencyModels;

namespace WFTileCounter.Controllers
{
    public class ResultsController : Controller
    {
        private readonly DatabaseContext _db; //database context shortcut
        private IHostingEnvironment _env;


        public ResultsController(DatabaseContext context, IHostingEnvironment env)
        {
            _db = context;
            _env = env;
        }
        public IActionResult Index()
        {
            string tilesetName = "CorpusShip";
            var allData = _db.MapPoints.Where(x => x.Tile.Tileset.Name == tilesetName).Include(x => x.Tile).ThenInclude(x => x.Tileset).Include(x => x.Run).ThenInclude( x=>x.Mission);


            List<string> allMissionNames = allData.Select(x => x.Run.Mission.Type).Distinct().OrderBy(x => x).ToList();
            List<string> allTileNames = allData.Select(x => x.Tile.Name).Distinct().OrderBy(x => x).ToList();
            List<string> allTilesetNames = allData.Select(x => x.Tile.Tileset.Name).Distinct().ToList();
            var distinctRuns = allData.Select(x => x.RunId).Distinct();
            var totalTilesPerMissionType = allData.GroupBy((x => x.Run.Mission.Type), (key, elements) => new { key = key, count = elements.Distinct().Count() });

            var totalMissionsOfEachType = allData.Select(x => new { x.Run.Mission.Type, x.RunId }).GroupBy((x => x.Type), (key, elements) => new {  key = key, count = elements.Distinct().Count() });


            List <TileDataPoint> allTilesDataList = new List<TileDataPoint>();

            foreach(var tileName in allTileNames)
            {
                var tile = new TileDataPoint();
                List<MissionAppearance> tileMisAppList = new List<MissionAppearance>();

                tile.TileName = tileName.Replace(tilesetName,"");
                foreach(var missionName in allMissionNames)
                {
                    var missApp = new MissionAppearance();

                    missApp.MissionName = missionName;
                    missApp.Appearances = allData.Where(x => x.Tile.Name == tileName && x.Run.Mission.Type == missionName).Count();
                    tileMisAppList.Add(missApp);

                }

                tile.MissionTileNumbers = tileMisAppList;
                allTilesDataList.Add(tile);
            }


            List<RunsPerMission> sortedMissionTotalList = new List<RunsPerMission>();
            foreach(var item in totalMissionsOfEachType)
            {
                var singleMissionRunCount = new RunsPerMission();
                singleMissionRunCount.MissionName = item.key;
                singleMissionRunCount.TotalRunsForMiss = item.count;
                sortedMissionTotalList.Add(singleMissionRunCount)
                    ;
            }








            List<TilesPerMission> sortedMissionTileList = new List<TilesPerMission>();
            
            foreach(var item in totalTilesPerMissionType)
            {
                var singleMissionTileCount = new TilesPerMission();
                singleMissionTileCount.MissionName = item.key;
                singleMissionTileCount.TotalTilesForMission = item.count;


                sortedMissionTileList.Add(singleMissionTileCount);
            }






            var dataPointView = new TileDataPointsViewModel();

            dataPointView.TilesetName = allTilesetNames.First(); //hack. fix
            dataPointView.TotalTilesPerMission = sortedMissionTileList;
            dataPointView.TotalRunsPerMissionList = sortedMissionTotalList;
            dataPointView.DataPoints = allTilesDataList;
            dataPointView.TotalRuns = distinctRuns.Count();

            return View(dataPointView);
        }
    }
}