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
            return View("Index");
        }

        [HttpGet]
        [Route("Results/{tileset}/Collectibles")]
        public IActionResult Collectible([FromRoute] string tileset)
        {
            var allData = GetAllDataForTileset(tileset);
            var onlyMapCollectibleRuns = RemoveNotCollectibleRuns(allData);

            CollectiblesViewModel collectibleResultsList = CreateViewModelOfCollectibles(onlyMapCollectibleRuns);

            return View("Collectible", collectibleResultsList);
        }

        private CollectiblesViewModel CreateViewModelOfCollectibles(IQueryable<MapPoint> tileInformation)
        {
            
            double totalRuns = tileInformation.Select(x => x.RunId).Distinct().Count();

            var cleanedUpCollectibleList = CleanUpCollectibles(tileInformation);

            var viewOfCollectibles = new CollectiblesViewModel()
                            {
                                TileCollectiblesList = GenerateListOfCollectiblesPerTile(cleanedUpCollectibleList, totalRuns),
                                Tileset = tileInformation.Select(x=>x.Tile.Tileset.Name).FirstOrDefault().ToString(),
                                AyatanTotal = cleanedUpCollectibleList.Where(x => x.Ayatan).Count(),
                                MedallionTotal = cleanedUpCollectibleList.Where(x => x.Medallion).Count(),
                                CephalonTotal = cleanedUpCollectibleList.Where(x => x.Cephalon).Count(),
                                SomachordTotal = cleanedUpCollectibleList.Where(x => x.Somachord).Count(),
                                FrameFighterTotal = cleanedUpCollectibleList.Where(x => x.FrameFighter).Count(),
                                RareContainerTotal = cleanedUpCollectibleList.Where(x => x.RareContainer).Count(),
                                SabotageCacheTotal = cleanedUpCollectibleList.Where(x => x.SabotageCache).Count()
            };

            viewOfCollectibles.AyatanPercentage = GetPercentageOfCollectiblePerRun(viewOfCollectibles.AyatanTotal, totalRuns);
            viewOfCollectibles.MedalionPercentage = GetPercentageOfCollectiblePerRun(viewOfCollectibles.MedallionTotal, totalRuns);
            viewOfCollectibles.CephalonPercentage = GetPercentageOfCollectiblePerRun(viewOfCollectibles.CephalonTotal, totalRuns);
            viewOfCollectibles.SomachordPercentage = GetPercentageOfCollectiblePerRun(viewOfCollectibles.SomachordTotal, totalRuns);
            viewOfCollectibles.FrameFighterPercentage = GetPercentageOfCollectiblePerRun(viewOfCollectibles.FrameFighterTotal, totalRuns);
            viewOfCollectibles.RareContainerPercentage = GetPercentageOfCollectiblePerRun(viewOfCollectibles.RareContainerTotal, totalRuns);
            viewOfCollectibles.SabotageCachePercentage = GetPercentageOfCollectiblePerRun(viewOfCollectibles.SabotageCacheTotal, totalRuns);

            return viewOfCollectibles;
        }

        private List<CollectibleFrequencePerTile> GenerateListOfCollectiblesPerTile(List<CleanedUpCollectibleDatabaseInfo> cleanedUpCollectibleList, double totalRuns)
        {
            List<string> tileNames = cleanedUpCollectibleList.Select(x => x.Tilename).Distinct().OrderBy(x=>x).ToList();
            var collectibleFrequenceList = new List<CollectibleFrequencePerTile>();

            foreach (var tile in tileNames)
            {
                CollectibleFrequencePerTile tileCollectibles = new CollectibleFrequencePerTile()
                {
                    Tilename = tile,
                    AyatanCount = cleanedUpCollectibleList.Where(x => x.Tilename == tile && x.Ayatan).Count(),
                    MedallionCount = cleanedUpCollectibleList.Where(x => x.Tilename == tile && x.Medallion).Count(),
                    CephalonCount = cleanedUpCollectibleList.Where(x => x.Tilename == tile && x.Cephalon).Count(),
                    SomachordCount = cleanedUpCollectibleList.Where(x => x.Tilename == tile && x.Somachord).Count(),
                    FrameFighterCount = cleanedUpCollectibleList.Where(x => x.Tilename == tile && x.FrameFighter).Count(),
                    RareContainerCount = cleanedUpCollectibleList.Where(x => x.Tilename == tile && x.RareContainer).Count(),
                    SabotageCacheCount = cleanedUpCollectibleList.Where(x => x.Tilename == tile && x.SabotageCache).Count(),
                };

                tileCollectibles.AyatanPercentagePerRun = GetPercentageOfCollectiblePerRun(tileCollectibles.AyatanCount, totalRuns);
                tileCollectibles.MedalionPercentagePerRun = GetPercentageOfCollectiblePerRun(tileCollectibles.MedallionCount, totalRuns);
                tileCollectibles.CephalonPercentagePerRun = GetPercentageOfCollectiblePerRun(tileCollectibles.CephalonCount, totalRuns);
                tileCollectibles.SomachordPercentagePerRun = GetPercentageOfCollectiblePerRun(tileCollectibles.SomachordCount, totalRuns);
                tileCollectibles.FrameFighterPercentagePerRun = GetPercentageOfCollectiblePerRun(tileCollectibles.FrameFighterCount, totalRuns);
                tileCollectibles.RareContainerPercentagePerRun = GetPercentageOfCollectiblePerRun(tileCollectibles.RareContainerCount, totalRuns);
                tileCollectibles.SabotageCachePercentagePerRun = GetPercentageOfCollectiblePerRun(tileCollectibles.SabotageCacheCount, totalRuns);


                if(tileCollectibles.AreThereAnyCollectiblesInThisTile())
                {
                    collectibleFrequenceList.Add(tileCollectibles);
                }
                
            }

            return collectibleFrequenceList;
        }


        private double GetPercentageOfCollectiblePerRun(double collectibleCount, double totalRuns)
        {
            double percentage = (collectibleCount / totalRuns) * 100;
            return Math.Truncate(percentage);
        }

        private List<CleanedUpCollectibleDatabaseInfo> CleanUpCollectibles(IQueryable<MapPoint> tileInformation)
        {
            return tileInformation.
                Select(x => new CleanedUpCollectibleDatabaseInfo
                                {
                                    Tilename = x.Tile.Name,
                                    Ayatan = x.Ayatan,
                                    Medallion = x.Medallion,
                                    Cephalon = x.Cephalon,
                                    Somachord = x.Somachord,
                                    FrameFighter = x.FrameFighter,
                                    RareContainer = x.RareContainer,
                                    SabotageCache = x.Cache

                                }).ToList();
        }

        private IQueryable<MapPoint> RemoveNotCollectibleRuns(IQueryable<MapPoint> allData)
        {
            return allData.Where(x => x.Run.MapPointsUsed);
        }

        [HttpGet]
        [Route("Results/{tileset}/TileCount")]
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
                                .Include(x => x.Run).ThenInclude(x => x.Mission)
                                .Include(x=>x.Tile.TileDetail);
            
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