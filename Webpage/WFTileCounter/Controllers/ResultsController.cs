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

        [HttpGet]
        [Route("Results/{tileset}")]
        public IActionResult Tileset([FromRoute] string tileset)
        {
            
            var allData = GetAllDataForTileset(tileset);

            var notPartialRuns = RemovePartialRuns(allData);

            IQueryable<CleanedUpDatabaseData> cleanedUpData = notPartialRuns.Select(x => new CleanedUpDatabaseData { TileName = x.Tile.Name, RunId = x.Run.Id, MissionName = x.Run.Mission.Type });

            
            
            IQueryable<int> distinctRuns = GetDistinctRuns(cleanedUpData);

            
            List<TileDataPoint> allTilesDataList = GetTileCountsForEachMissionType(tileset, cleanedUpData);

            List<RunsPerMission> sortedMissionTotalList = SortMissionlList(cleanedUpData);

            List<TilesPerMission> sortedMissionTileList = SortedMissionTileNameList(cleanedUpData);


            var dataPointView = new TileDataPointsViewModel();

            dataPointView.TilesetName = tileset;
            dataPointView.TotalTilesPerMission = sortedMissionTileList;
            dataPointView.TotalRunsPerMissionList = sortedMissionTotalList;
            dataPointView.DataPoints = allTilesDataList;
            dataPointView.TotalRuns = distinctRuns.Count();

            return View(dataPointView);
        }

        private IQueryable<MapPoint> RemovePartialRuns(IQueryable<MapPoint> allData)
        {
            return allData.Where(x => x.Run.FullRun);
        }

        private IQueryable<MapPoint> GetAllDataForTileset(string tilesetName)
        {
            return _db.MapPoints.Where(x => x.Tile.Tileset.Name == tilesetName)
                                .Include(x => x.Tile).ThenInclude(x => x.Tileset)
                                .Include(x => x.Run).ThenInclude(x => x.Mission);
            
        }


        private static IQueryable<int> GetDistinctRuns(IQueryable<CleanedUpDatabaseData> allData)
        {
            return allData.Select(x => x.RunId).Distinct();
        }


        private List<string> ExtractAllTileNames(IQueryable<CleanedUpDatabaseData> allData)
        {
            return allData.Select(x => x.TileName).Distinct().OrderBy(x => x).ToList();
        }

        private List<string> ExtractAllMissionNames(IQueryable<CleanedUpDatabaseData> allData)
        {
            return allData.Select(x => x.MissionName).Distinct().OrderBy(x => x).ToList();
        }




        private List<TileDataPoint> GetTileCountsForEachMissionType(string tilesetName, IQueryable<CleanedUpDatabaseData> allData)
        {
            List<string> allMissionNames = ExtractAllMissionNames(allData);
            List<string> allTileNames = ExtractAllTileNames(allData);

            List<TileDataPoint> listT = new List<TileDataPoint>();

            foreach (var tileName in allTileNames)
            {
                
                var tile = new TileDataPoint();
                List<MissionAppearance> tileMissionAppearanceFullCountPerMission = new List<MissionAppearance>();

                tile.TileName = tileName.Replace(tilesetName, "");
                foreach (var missionName in allMissionNames)
                {
                    var missionSpecificData = allData.Where(x=>x.MissionName == missionName);
                    MissionAppearance individualTileInfoPerMission = CalculateTotalAppearancesCount(missionSpecificData, tileName);
                    individualTileInfoPerMission.PercentLikelyhoodOfAppearance = CalculateLikelyhoodOfAppearance(allData, tileName, missionName);


                    tileMissionAppearanceFullCountPerMission.Add(individualTileInfoPerMission);

                }

                double timesTileAppearsAtLeastOnce = allData.Where(x => x.TileName == tileName).Distinct().Count();
                double totalRuns = GetDistinctRuns(allData).Count();
                double percentage = (timesTileAppearsAtLeastOnce / totalRuns) * 100;

                tile.OverallPercentageToAppear = Math.Truncate(percentage);
                tile.MissionTileNumbers = tileMissionAppearanceFullCountPerMission;
                listT.Add(tile);
            }

            return listT;
        }

        private MissionAppearance CalculateTotalAppearancesCount(IQueryable<CleanedUpDatabaseData> missionData, string tileName)
        {
            MissionAppearance info = new MissionAppearance
            {
                MissionName = missionData.Select(x => x.MissionName).FirstOrDefault().ToString(),
                TotalAppearances = missionData.Where(x => x.TileName == tileName).Count()
            };

            return info;
        }

        private double CalculateLikelyhoodOfAppearance(IQueryable<CleanedUpDatabaseData> allData, string tileName, string mission)
        {
            var runsOfThisMissionType = allData.Where(x => x.MissionName == mission);
            double tileAppearedAtLeastOnceInThisManyRuns = runsOfThisMissionType.Where(x => x.TileName == tileName).Select(x => new { x.TileName, x.RunId }).Distinct().Count();
            double numberOfRunsOfThisMission = runsOfThisMissionType.Select(x => x.RunId).Distinct().Count();
           

            double percentage = (tileAppearedAtLeastOnceInThisManyRuns / numberOfRunsOfThisMission) * 100;

            return Math.Truncate(percentage);
        }

        private List<TilesPerMission> SortedMissionTileNameList(IQueryable<CleanedUpDatabaseData> allData)
        {
            var totalTilesPerMissionType = allData.GroupBy((x => x.MissionName), (key, elements) => new { key = key, count = elements.Distinct().Count() });
            List<TilesPerMission> listT = new List<TilesPerMission>();
            foreach (var item in totalTilesPerMissionType)
            {
                var singleMissionTileCount = new TilesPerMission();
                singleMissionTileCount.MissionName = item.key;
                singleMissionTileCount.TotalTilesForMission = item.count;


                listT.Add(singleMissionTileCount);
            }

            return listT;
        }

        private List<RunsPerMission> SortMissionlList(IQueryable<CleanedUpDatabaseData> allData)
        {
            List<RunsPerMission> listT = new List<RunsPerMission>();
            var totalMissionsOfEachType = allData.Select(x => new { x.MissionName, x.RunId }).GroupBy((x => x.MissionName), (key, elements) => new { key = key, count = elements.Distinct().Count() });


            foreach (var item in totalMissionsOfEachType)
            {
                var singleMissionRunCount = new RunsPerMission();
                singleMissionRunCount.MissionName = item.key;
                singleMissionRunCount.TotalRunsForMiss = item.count;
                listT.Add(singleMissionRunCount);
            }

            return listT;
        }
    }
}