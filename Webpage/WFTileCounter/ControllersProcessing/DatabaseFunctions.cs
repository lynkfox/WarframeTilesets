using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using WFTileCounter.Models;
using WFTileCounter.ModelsLogic;
using WFTileCounter.ModelsView;

namespace WFTileCounter.ControllersProcessing
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

                var run = _db.Runs.Where(x => x.IdentityString == data.Run.IdentityString).FirstOrDefault();
                if (run is null)
                {

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
                    foreach (var tile in data.Tiles)
                    {
                        var mapPoint = new MapPoint();

                        var t = _db.Tiles.Where(x => x.Name == tile.Name).FirstOrDefault();

                        mapPoint.Run = data.Run;
                        mapPoint.CoordsTaken = tile.Coords;

                        if (t is null)
                        {
                            tile.Tileset = data.Tileset;
                            mapPoint.Tile = tile;
                            newTiles++;

                        }
                        else
                        {
                            t.Tileset = data.Tileset;
                            mapPoint.Tile = t;

                        }

                        _db.MapPoints.Add(mapPoint);


                    }

                }
                else
                {
                    /* If the Run was already found (Ie: The what I believe to be Unique Map Identifier string was already in the database)
                     * then figure out tiles are new, and add them
                     * 
                     * most likely scenario for this is a run got only partially uploaded at a time.
                     * 
                     * Can't rely on users to remember to upload every picture from a run, or on connections to stay stable.
                     */

                    var tSetName = _db.Tilesets.Where(x => x.Name == data.Tileset.Name).FirstOrDefault();


                    var tilesInDB = _db.Tiles.Where(x => x.Tileset == tSetName).ToList();
                    

                    var tileNames = tilesInDB.Select(x => x.Name).ToList();
                    var processNames = data.Tiles.Select(x => x.Name).ToList();

                    var mapPoint = new MapPoint();

                    foreach (var name in processNames)
                    {
                        if(!tileNames.Contains(name))
                        {

                            var newTile = data.Tiles.Where(x => x.Name == name).FirstOrDefault();

                            /* Learned something here. I was trying to set it as the Tileset from data, but to EF that is a different isntance of the tileset. Because
                             * both the Tile and the MapPoint are linking to objects ALREADY in the database, we need to fetch the DATABASE versions of them. Run
                             * was already fetched above at begining of if/else to check if it was null or not.
                             */
                            newTile.Tileset = tSetName;
                            mapPoint.Tile = newTile;
                            mapPoint.Run = run;

                            _db.MapPoints.Add(mapPoint);
                            newTiles++;
                        }
                    }

                    
                   
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

            var processedList = new List<InsertReadyData>();

            bool first = true;


            //in case multiple runs are uploaded at the same time, get a list of unique map identifiers (which are, as far as I can tell, unique per run)
            IEnumerable<string> distinctMapIdentifiers = metaDataList.Select(a => a.MapIdentifier).Distinct();


            foreach (var id in distinctMapIdentifiers)
            {
                var processing = new InsertReadyData();
                var mission = new Mission();
                var run = new Run();
                var tileset = new Tileset();
                var User = new User();
                List<Tile> uniqueTileList = new List<Tile>();
                List<Tile> allTilesUploadedList = new List<Tile>();
                string endLog = "";

                /*This foreach loop is designed to go through the entire list of uploaded tiles, dividing them out into seperate ready for database insert sets based on the map Identifier string
                 */

                var condensedList = metaDataList.Where(x => x.MapIdentifier == id);
                foreach (var item in condensedList)
                {

                    /* For the first tile in each Mission String, pull out the relevant details that don't change from tile to tile
                     * Mission Type, Tileset, Faction Name, IdentityString, and general DateTime.  Also grab the lowest LogNumber.
                     * Then add those items to the temporary processing InsertReadyData object, before heading on to add all the tiles in 
                     * further loops
                     */
                    if (first)
                    {
                        mission.Type = item.MissionType;
                        tileset.Name = item.Tileset;
                        tileset.Faction = item.FactionName;
                        run.IdentityString = id;
                        run.RunDate = DateTime.ParseExact(item.Date, "ddd MMM dd HH:mm:ss K yyyy", null);
                        run.Mission = mission;
                        run.LogRange = item.LogNum;

                        //test purposes, fix this to be dynamic later
                        run.UserID = 2;

                        //add the unique data to the processing temp object
                        processing.Mission = mission;
                        processing.Tileset = tileset;
                        processing.Run = run;
                        //processing.User = user;

                        first = false;
                    }

                    var tile = new Tile();
                    tile.Name = item.TileName;
                    tile.Tileset = tileset;
                    tile.Coords = item.Coords;

                    var doesTileAlreadyExistInList = uniqueTileList.Where(x => x.Name == tile.Name).FirstOrDefault();

                    //adding only if tile is unique
                    if (doesTileAlreadyExistInList is null)
                    {
                        uniqueTileList.Add(tile);
                    }
                    

                    //But also saving a list of all the tiles that were processed, for View purposes.
                    allTilesUploadedList.Add(tile);

                    // continually changing unitl the last run, where it will record the last logNum
                    endLog = item.LogNum;
                    
                    
                } 

                processing.Run.LogRange += " - " + endLog;
                processing.Run.TotalTiles = allTilesUploadedList.Count();
                processing.Run.UniqueTiles = uniqueTileList.Count();
                //add the list of tiles from this run - this is the list that will be used to generate MapPoints in the database
                processing.Tiles = uniqueTileList;
                processing.CompleteTileList = allTilesUploadedList;

                // add to the list to be returned
                processedList.Add(processing);
                //moving on to the next map identifier, set flag back to false so we can get the new mission data.
                first = true;

            }



            return processedList;

        }



        //Just a quicky to keep this out of other functions.

        public bool CheckTilesetExists(string tilesetName)
        {
            var tSet = _db.Tilesets.Where(x => x.Name == tilesetName).FirstOrDefault();
            if (tSet is null)
            { return false; }
            else
            { return true; }
        }
    }
}
