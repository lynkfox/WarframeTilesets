﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WFTileCounter.Models;
using WFTileCounter.ModelsLogic;
using WFTileCounter.ModelsView;
using static WFTileCounter.Models.MapPoint;

namespace WFTileCounter.BuisnessLogic
{
    public class DatabaseFunctions
    {


        /* This class contains functions involving converting the data to proper EF models, and then inersting them into the database.
         * 
         */


        private readonly DatabaseContext _db; //database context shortcut

       

        public DatabaseFunctions(DatabaseContext context)
        {
            _db = context;
        }


        /* Take in a list of data that has been converted from the viewModel (ImgMetaData) into the various models for the database, but contained in
             * the single model of InsertReadyData
             * 
             * using the single Model that contains all the various Database Models for easy passing into functions, and if need be passing to View.
             * 
             * this may be slightly ineffecient - parsing data from the Meta into one Model, then transfering it into a global model containing each individual DB model. 
             * Could possibly refactor to combine the two functions?
             * 
             * I understand it better this way, am able to process it in my head easier - but that may not be the best way to do it.
        */

        public async Task<List<int>> InsertIntoDatabase(List<InsertReadyData> processed)
        {
            
            List<int> newTilesPerRun = new List<int>(); //list of new tiles added for each run - for use in View

            

            foreach (var data in processed)
            {
                int newTiles = 0; //for ViewBagfor view return

                var run = _db.Runs.Where(x => x.IdentityString == data.Run.IdentityString ).FirstOrDefault();
                if (run is null || data.Mission.Type == "Defense" || data.Mission.Type == "Interception")
                {
                    //finally found a situation where there might be the same map identifier string - Defense missions, which only have 2 tiles. (interception probably too)
                    // so we have to check for the Same IdString and if its a Def or Interception.

                    data.AlreadyProcessed = false;

                    var miss = _db.Missions.Where(x => x.Type == data.Mission.Type).FirstOrDefault();
                    if (miss is null)
                    {
                        //_db.Missions.Add(data.Mission);  -- oh entity framework. Left this here as a reminder to myself, when you use an object as a property
                        // in an EF Model that is a FK, you don't need to add that model to the DB if you have already set it to the property of its fk relation.
                        data.Run.Mission = data.Mission;
                    }
                    else
                    {
                        data.Run.Mission = miss;
                    }

                    var tSet = _db.Tilesets.Where(x => x.Name == data.Tileset.Name).FirstOrDefault();
                    if (tSet is null)
                    {
                        _db.Tilesets.Add(data.Tileset);

                    }
                    else
                    {
                        data.Tileset = tSet;
                    }

                    /*  To Code: User Compliance
                     *  
                    
                    Add Code to:

                    if (User exists)
                        fetch existing user from dbcontext
                        attach user to dbcontext for the run
                    else 
                        keep using new user on run (will be created when run is added, due to fkey compliance)
                    */

                    _db.Runs.Add(data.Run);


                    /* Map Points are a list of the unique tiles that were in the Run. This forloop pushes through each map point to find out if we need to add a 
                     * new tile or if the tile already exists in the db. 
                     * 
                     * it also records the number of new tiles added to the database for adding to the list, to display in a view if/when multiple runs are processed at once.
                     */

                    List<MapPoint> map = new List<MapPoint>();
                    foreach (var tile in data.CompleteTileList)
                    {
                        var mapPoint = new MapPoint();

                        var t = CheckTileAlreadyExists(tile.Name);

                        mapPoint.Run = data.Run;
                        mapPoint.CoordsTaken = tile.Coords;

                        mapPoint.Objectives = tile.Objectives;
                        mapPoint.Ayatan = tile.Ayatan;
                        mapPoint.Medallion = tile.Medallion;
                        mapPoint.RareContainer = tile.RareContainer;
                        mapPoint.Cephalon = tile.Cephalon;
                        mapPoint.Somachord = tile.Somachord;
                        mapPoint.FrameFighter = tile.FrameFighter;
                        mapPoint.CaptureSpawn = tile.CaptureSpawn;
                        mapPoint.SimarisSpawn = tile.SimarisSpawn;
                        mapPoint.Cache = tile.Cache;
                            






                        if (t is null)
                        {
                            tile.Tileset = data.Tileset;
                            mapPoint.Tile = tile;
                            newTiles++;

                        }
                        else
                        {
                            t.Tileset = _db.Tiles.Where(x => x.Name == t.Name).Include(x => x.Tileset).FirstOrDefault().Tileset;
                            

                            mapPoint.Tile = t;

                        }

                        _db.MapPoints.Add(mapPoint);


                    }

                }
                else
                {
                    /* If the Run was already found (Ie: A Unique Map ID or same mapId + different date
                     * then figure out tiles are new, and add them
                     * 
                     * most likely scenario for this is a run got only partially uploaded at a time.
                     * 
                     * Can't rely on users to remember to upload every picture from a run, or on connections to stay stable.
                     */




                    var alreadyInDBMapPoints = _db.MapPoints.Where(x => x.RunId == run.Id).Include(x => x.Tile).ToList();
                    int additionalUniqueTiles = 0;
                    int additionalTilesProcessed = 0;


                    foreach (var tile in data.Tiles)
                    {
                        var mapPoint = new MapPoint();


                        bool inMapPoints = alreadyInDBMapPoints.Any(x => x.Tile.Name == tile.Name && x.CoordsTaken == tile.Coords);
                        // by matching to the Name and the Coords Taken, we can tell if it is a screenshot already uploaded once.

                        if (!inMapPoints)
                        {
                            mapPoint.Run = run;
                            mapPoint.CoordsTaken = tile.Coords;
                            mapPoint.Objectives = tile.Objectives;
                            mapPoint.Ayatan = tile.Ayatan;
                            mapPoint.Medallion = tile.Medallion;
                            mapPoint.RareContainer = tile.RareContainer;
                            mapPoint.Cephalon = tile.Cephalon;
                            mapPoint.Somachord = tile.Somachord;
                            mapPoint.FrameFighter = tile.FrameFighter;
                            mapPoint.CaptureSpawn = tile.CaptureSpawn;
                            mapPoint.SimarisSpawn = tile.SimarisSpawn;
                            mapPoint.Cache = tile.Cache;
                            var t = CheckTileAlreadyExists(tile.Name);

                            if (t is null)
                            {
                                tile.Tileset = data.Tileset;
                                mapPoint.Tile = tile;
                                newTiles++;


                            }
                            else
                            {
                               
                               t.Tileset = _db.Tiles.Where(x => x.Name == t.Name).Include(x => x.Tileset).FirstOrDefault().Tileset;
                                

                                mapPoint.Tile = t;


                            }

                            bool newUniqueTile = alreadyInDBMapPoints.Any(x => x.Tile.Name == tile.Name);
                            if(newUniqueTile)
                            {
                                additionalUniqueTiles++;
                            }


                            additionalTilesProcessed++;
                            _db.MapPoints.Add(mapPoint);
                        }
                        //else skip that tile, because we found it already



                    }

                    run.UniqueTiles = _db.MapPoints.Where(x => x.RunId == run.Id).Select(x=>x.Tile.Name).Distinct().Count();
                    run.TotalTiles += additionalTilesProcessed;


                    data.AlreadyProcessed = true;


                }
                    newTilesPerRun.Add(newTiles);

                await _db.SaveChangesAsync(); // Saving the changes after each Map Identifier string. Maybe better to save all at once, ie move it out of the loop entirely?
            }


            return newTilesPerRun;

        }


        /* This functions takes a list of ImgMetaData, generated by the ProcessController when it processes all the images uploaded. 
         * 
         * it converts all that into a List of InsertReadyData which is subdivided into proper Models for EF in other functions. This single model of InsertReadyData is used for the
         * first 'Proof Read' view, including checkboxes to ignore them in the upload
         * 
         * To Do - need to add for it to ignore the tiles that don't have their checkbox checked.
         */


        public List<InsertReadyData> ConvertToDatabase(List<ImgMetaData> metaDataList)
        {

            GeneralFunctions _gf = new GeneralFunctions(_db);

            


            List<InsertReadyData> allMapsInsertReady = new List<InsertReadyData>();
            var singleMapInsertReady = new InsertReadyData();
            var mission = new Mission();
            var run = new Run();
            var tileset = new Tileset();
            var User = new User();
            List<Tile> uniqueTileList = new List<Tile>();
            List<Tile> allTilesUploadedList = new List<Tile>();
            string endLog = "";

            var lastItem = metaDataList.Last();
            var firstItem = metaDataList.First();

            for (int i = 0; i < metaDataList.Count(); i++)
            {
                
                if (metaDataList[i].First || metaDataList[i].Equals(firstItem)) // if this is a First Tile, or if it is the first item in the list
                {
                    
                    //add the unique data to the processing temp object
                    singleMapInsertReady.Mission = MissionInfoForEntireRun(metaDataList[i]);
                    singleMapInsertReady.Tileset = TilesetInfoForEntireRun(metaDataList[i]);
                    singleMapInsertReady.Run = RunInfoForEntireRun(metaDataList[i], singleMapInsertReady.Mission);

                    singleMapInsertReady.Run.UserID = _gf.GetUserId();
                    singleMapInsertReady.Run.Mission = singleMapInsertReady.Mission;
                    //processing.User = user;

                    
                }



                Tile tile = IndividualTileInformationForThisRun(metaDataList[i], singleMapInsertReady.Tileset);
                


                var checkAgainstDatabase = CheckTileAlreadyExists(metaDataList[i].TileName);
                if (checkAgainstDatabase is null)
                {
                    tile.NewTile = true;
                }
                else
                {
                    tile.NewTile = false;
                }

                var doesTileAlreadyExistInList = uniqueTileList.Where(x => x.Name == tile.Name).FirstOrDefault();


                //a list of only the unique tiles, so even if the tile has two copies in the same run, this list will only have one
                if (doesTileAlreadyExistInList is null)
                {

                    uniqueTileList.Add(tile);
                }

                

                //But also saving a list of all the tiles that were processed, for View purposes.
                allTilesUploadedList.Add(tile);

                // continually changing unitl the last run, where it will record the last logNum
                endLog = metaDataList[i].LogNum;


                if (metaDataList[i].Equals(lastItem) || metaDataList[i + 1].First) // if this is the last item in the list, or the next item is a FirstTile
                {
                    singleMapInsertReady.Run.LogRange += " - " + endLog;
                    singleMapInsertReady.Run.TotalTiles = allTilesUploadedList.Count();
                    singleMapInsertReady.Run.UniqueTiles = uniqueTileList.Count();
                    //add the list of tiles from this run - this is the list that will be used to generate MapPoints in the database
                    singleMapInsertReady.Tiles = uniqueTileList.ToArray();
                    singleMapInsertReady.CompleteTileList = allTilesUploadedList.ToArray();

                    // add to the list to be returned
                    allMapsInsertReady.Add(singleMapInsertReady);
                    //moving on to the next map identifier, set flag back to false so we can get the new mission data.


                    singleMapInsertReady = new InsertReadyData();
                    mission = new Mission();
                    run = new Run();
                    tileset = new Tileset();
                    uniqueTileList.Clear();
                    allTilesUploadedList.Clear();

                }

                if(!metaDataList[i].Equals(lastItem))
                {
                    if (metaDataList[i].MapIdentifier != metaDataList[i + 1].MapIdentifier && metaDataList[i + 1].First == false) //if the next tile is a different mapId but isn't marked as a First Tile (say the first tile got unchecked)
                    {
                        metaDataList[i + 1].First = true;
                    }
                }
                



            }


            return allMapsInsertReady;

        }

        private Tile IndividualTileInformationForThisRun(ImgMetaData singleTileInfo, Tileset tileset)
        {


            return  new Tile
            {
                Name = singleTileInfo.TileName,
                Tileset = tileset,
                Coords = singleTileInfo.Coords,
                Ayatan = singleTileInfo.AyatanStatue,
                Medallion = singleTileInfo.SyndicateMedallion,
                RareContainer = singleTileInfo.RareLootChest,
                Cephalon = singleTileInfo.Ordis,
                Somachord = singleTileInfo.Somachord,
                FrameFighter = singleTileInfo.Framefighter,
                CaptureSpawn = singleTileInfo.CaptureSpawn,
                SimarisSpawn = singleTileInfo.SimarisSpawn,
                Cache = singleTileInfo.Cache,
                Objectives = ParseObjectivesEnumForTile(singleTileInfo.Objective)
            };

            

        }

        private Objective ParseObjectivesEnumForTile(string singleTileObjective)
        {
            

            if (!String.IsNullOrEmpty(singleTileObjective))
            {
                Enum.TryParse(singleTileObjective, out Objective objective);
                return objective;

            }
            else
            {
                Enum.TryParse("Nothing", out Objective objective);
                return objective;
            }

        }

        private Run RunInfoForEntireRun(ImgMetaData singleRunData, Mission mission)
        {
            return new Run()
            {
                IdentityString = singleRunData.MapIdentifier,
                RunDate = DateTime.ParseExact(singleRunData.Date, "ddd MMM dd HH:mm:ss K yyyy", null),
                Mission = mission,
                LogRange = singleRunData.LogNum,
                FullRun = singleRunData.FullRun,
                MapPointsUsed = singleRunData.MapPointsRecorded
            };
            
            
        }

        private Tileset TilesetInfoForEntireRun(ImgMetaData singleRunData)
        {
            return new Tileset()
            {
                Name = singleRunData.Tileset,
                Faction = singleRunData.FactionName
            };

        }

        private Mission MissionInfoForEntireRun(ImgMetaData singleRunData)
        {
            return new Mission()
            {
                Type = singleRunData.MissionType
            };

           
        }



        //Refactoring Functions

        public bool CheckIfTilesetExistsInDBAlready(string tilesetName)
        {
            var tSet = _db.Tilesets.Where(x => x.Name == tilesetName).FirstOrDefault();
            if (tSet is null)
            { return false; }
            else
            { return true; }
        }

        public Tile CheckTileAlreadyExists(string tileName)
        {
            var tile = _db.Tiles.Where(x => x.Name == tileName).FirstOrDefault();
            if(tile is null)
            {
                return null;
            }
            else
            {
                return tile;
            }
        }
    }
}
