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


        // getting error that says 'multiple uses of Tile being used in context' --- i am pretty sure that is because I am passing the context to this set of functions.

            // this seems bad. so... figure this part out.

        public async Task<List<int>> InsertIntoDatabase(List<InsertReadyData> processed)
        {
            
            List<int> newList = new List<int>();

            /* Take in a list of data that has been converted from the viewModel (ImgMetaData) into the various models for the database, but contained in
             * the single model of InsertReadyData
            */

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




                    /*  -- currently unused, and also CODED WRONG but I'll fix that when I get to that stage
                     *  
                    var user = _db.Users.Where(x => x.Id == data.Id).FirstOrDefault();
                    if(user is null)
                    {
                        if(data.User.Id == 0)
                        {
                            var user1 = new User { Username = "Anonymous", email = "None" };
                            _db.Users.Add(user1);
                        }else
                        {
                            _db.Users.Add(data.User);
                        }

                    }
                    else
                    {
                        data.User = user;
                    }
                    */


                    _db.Runs.Add(data.Run);

                    //Debugging lines. Left them to remind myself what I had to look for.
                    //Debug.WriteLine("\n\n" + data.Run.IdentityString + " | "+ data.Tileset.Name + " | " + data.Mission.Type + "\n\n");
                    //await _db.SaveChangesAsync();

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

                        //Debug.WriteLine("\n\n" + mapPoint.Tile.Name + " # " +mapPoint.Tile.Name.Length +"\n" + mapPoint.Tile.Coords + " # " + mapPoint.Tile.Coords.Length + "\n\n");
                        //await _db.SaveChangesAsync();

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
                             * was fetched above to check if it was null or not.
                             */
                            newTile.Tileset = tSetName;
                            mapPoint.Tile = newTile;
                            mapPoint.Run = run;

                            //Debug.WriteLine("\n\n" + mapPoint.Tile.Name + " # " + mapPoint.Tile.Name.Length + "\n" + mapPoint.Tile.Coords + " # " + mapPoint.Tile.Coords.Length + "\n\n");

                            _db.MapPoints.Add(mapPoint);
                            newTiles++;
                        }
                    }

                    
                   
                }

                newList.Add(newTiles);

                await _db.SaveChangesAsync();
            }


            return newList;

        }


        /* This functions takes a list of ImgMetaData, generated by the ProcessController when it processes all the images uploaded. 
         * 
         * it converts all that into a List of InsertReadyData which is subdivided into proper Models for EF
         */


        public List<InsertReadyData> ConvertToDatabase(List<ImgMetaData> metaDataList)
        {

            //variable declare


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
                List<Tile> tiles = new List<Tile>();
                List<Tile> allTiles = new List<Tile>();
                string endLog = "";

                /*This foreach loop is designed to go through the entire list of uploaded tiles, dividing them out into seperate ready for database insert sets based on the map Identifier string
                 */

                var condensedList = metaDataList.Where(x => x.MapIdentifier == id);
                foreach (var item in condensedList)
                {

                    
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
                        run.UserID = 1;




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

                    var test = tiles.Where(x => x.Name == tile.Name).FirstOrDefault();

                    //adding only if tile is unique
                    if (test is null)
                    {
                        tiles.Add(tile);
                    }
                    


                    allTiles.Add(tile);

                    // continually changing unitl the last run, where it will record the last logNum
                    endLog = item.LogNum;
                    
                    
                } 

                processing.Run.LogRange += " - " + endLog;
                processing.Run.TotalTiles = allTiles.Count();
                processing.Run.UniqueTiles = tiles.Count();
                //add the list of tiles from this run
                processing.Tiles = tiles;
                processing.CompleteTileList = allTiles;

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
