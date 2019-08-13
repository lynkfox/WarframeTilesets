using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WFTileCounter.Models;
using WFTileCounter.ModelsLogic;
using WFTileCounter.ModelsView;

namespace WFTileCounter.ControllersProcessing
{
    public class GeneralFunctions
    {

        private readonly DatabaseContext _db; //database context shortcut


        public GeneralFunctions(DatabaseContext context)
        {
            _db = context;
        }


        //Temp function for use between multiple computers, but will need to be made dynamic for a server.
        public string GetPath()
        {
            //return Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", "Uploads");


            return @"C:\Users\lynkf\Pictures\Warframe";
        }

        public List<MissionType> GenMissionList()
        {
            List<MissionType> list = new List<MissionType>();
            
            

            string[] missionTypeDiff = { "SentientArtifact", "ShipPurify", "ColonistRescue", "Rescue", "MobileDefense", "Assassinate", "KelaArena", "SabotageForest", "Intel", "CorpusArena", "TRRace", "TRSabotage", "Pursuit" };
            string[] commonName = { "Disruption", "Infested Salvage", "Defection", "Rescue", "Mobile Defense", "Assassiation", "Rathuum", "Sabatoge", "Spy", "Index", "Rush (Archwing)", "Sabotage (Archwing)", "Pursuit", "Defense", "Excavation", "Sabotage", "Survival", "Exterminate", "Interception", "Hijack", "Spy", "Capture" };

           

            for(int i = 0; i < commonName.Length; i++)
            {
                var mission = new MissionType();

                if (i < missionTypeDiff.Length)
                {
                    mission.CommonName = commonName[i];
                    mission.InGameName = missionTypeDiff[i];
                }
                else
                {
                    mission.CommonName = commonName[i];
                    mission.InGameName = commonName[i];
                }

                list.Add(mission);
            }

            
            return list;
        }


        public string GetMissionType(string value)
        {

            string name;

            var types = GenMissionList();

            foreach (var type in types)
            {
                if (value.Contains(type.InGameName))
                {
                    name = type.CommonName;

                    if (value.Contains("ProcLevel"))
                    {
                        name += " (Archwing)";
                    }

                    return name;
                }

            }

            return "Assassination ??? - Check Me";

        }

        public string GetTileSet(string value)
        {
            string[] missionType = { "SentientArtifact", "ShipPurify", "ColonistRescue", "Rescue", "MobileDefense", "Assassinate", "SabotageForest", "Intel",  "Capture", "Excavation", "Defense", "Sabotage", "Survival", "Exterminate", "Interception", "Hijack", "Spy" };

            int length = value.Length;

            if (value.Contains("ProcLevel")) // Archwing Missions have ProcLevel at the end of their missiontype, remove it first.
            {
                value = value.Remove(length - 9);
                length = value.Length; // update the new length

            }

            if (value.Contains("TR")) // special case, just get it over with.
                return "CorpusShip (Archwing)";

            if (value.Contains("Space"))
                return "GrineerSpace (Archwing)";

            if (value.Contains("Arena"))
            {

                return value;
            }


            foreach (string mission in missionType)
            {

                int mLength = mission.Length;

                if (value.Contains(mission))
                {
                    if ((length - mLength) > 0)
                    {
                        return value.Remove((length - mLength));
                    }

                }

            }
            return value + " ??? WHAT IS THIS??? "; 
        }


        public async Task InsertIntoDatabase(List<InsertReadyData> processed)
        {
            
            foreach (var data in processed)
            {
                var run = _db.Runs.Where(x => x.IndentityString == data.Run.IndentityString).FirstOrDefault();
                if (run is null)
                {
                    


                    var miss = _db.Missions.Where(x => x.Type == data.Mission.Type).FirstOrDefault();
                    if (miss is null)
                    {
                        _db.Missions.Add(data.Mission);
                        data.Run.Mission = data.Mission;
                    }
                    else
                    {
                        data.Mission = miss;
                        data.Run.Mission = miss;
                    }

                    var tSet = _db.Tilesets.Where(x => x.Name == data.Tileset.Name).FirstOrDefault();
                    if (miss is null)
                    {
                        _db.Tilesets.Add(data.Tileset);

                    }
                    else
                    {
                        data.Tileset = tSet;
                    }



                    /*
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


                    List<MapPoint> map = new List<MapPoint>();
                    foreach (var tile in data.Tiles)
                    {
                        var mapPoint = new MapPoint();

                        var t = _db.Tiles.Where(x => x.Name == tile.Name).FirstOrDefault();

                        mapPoint.Run = data.Run;

                        if (t is null)
                        {
                            tile.Tileset = data.Tileset;
                            _db.Tiles.Add(tile);
                            mapPoint.Tile = tile;
                        }
                        else
                        {
                            t.Tileset = data.Tileset;
                            mapPoint.Tile = t;

                        }

                        _db.MapPoints.Add(mapPoint);

                        await _db.SaveChangesAsync();

                    }

                }
                else
                {
                    data.Run = run;
                    data.Run.Mission = data.Mission;
                }


                

                await _db.SaveChangesAsync();
            }

            


        }
    
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
                foreach (var item in metaDataList)
                {

                    string identifier = item.MapIdentifier;
                    if (identifier == id)
                    {
                        if (first)
                        {
                            mission.Type = item.MissionType;
                            tileset.Name = item.Tileset;
                            tileset.Faction = item.FactionName;
                            run.IndentityString = id;
                            run.RunDate = DateTime.ParseExact(item.Date, "ddd MMM dd HH:mm:ss K yyyy", null);
                            run.Mission = mission;

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

                        //adding only if tile is unique
                        if (!tiles.Contains(tile))
                        {
                            tiles.Add(tile);
                        }


                    }
                }

                //add the list of tiles from this run
                processing.Tiles = tiles;

                // add to the list to be returned
                processedList.Add(processing);
                //moving on to the next map identifier, set flag back to false so we can get the new mission data.
                first = true;

            }



            return processedList;

        }
        

        public List<ImgMetaData> GetMetaList(string path)
        {
            List<ImgMetaData> metaList = new List<ImgMetaData>();

            var picList = System.IO.Directory.GetFiles(path);

            foreach (var pic in picList)
            {
                var metaData = new ImgMetaData();

                var metaValues = GetMetaData(pic);

                string[] pathCut = pic.Split('\\');
                metaData.ImgPath = pic;
                metaData.FileName = pathCut[pathCut.Length - 1];
                metaData.Date = metaValues.Last();
                metaData.MapIdentifier = metaValues[0];
                metaData.MissionType = metaValues[1];
                metaData.Tileset = metaValues[2];
                metaData.FactionName = metaValues[3];
                metaData.TileName = metaValues[4];
                metaData.Coords = metaValues[5];
                metaData.LogNum = metaValues[6];



                metaList.Add(metaData);
            }

            return metaList;
        }


        /* Returns a list of the Metadata from Warframe In Game Screenshot button (f6)
         * 
         * Index   -    Value
         * 0       -    Map Identifier String
         * 1       -    MissionType
         * 2       -    Tileset
         * 3       -    Faction Name
         * 4       -    TileName (internal)
         * 5       -    Coords
         * 6       -    Log#
         * 7       -    Date of File
         */


        public List<string> GetMetaData(string path)
        {

            var values = new List<string>();
            var directories = MetadataExtractor.ImageMetadataReader.ReadMetadata(path);
            string coords = "P: ";
            string log = "";
            List<string> tileInfo = new List<string>();
            List<string> mapInfo = new List<string>();

            foreach (var directory in directories)
            {
                if (directory.Name == "JpegComment")
                {
                    values = directory.Tags[0].Description.Split(new char[] { ' ' }).ToList();

                    //remove all the extra info we don't need
                    values.RemoveAll(x => x == "" || x == "Zone:" || x == "Log:" || x == "P:");




                    if (values.Count == 7) // This should be the standard case.
                    {
                        //get the mapInfo out - which is of a varrying size dependingon the map. We want the last 3  parts.
                        mapInfo = values[0].Split('/').ToList();
                        //remove it from the values list
                        values.RemoveAt(0);

                        //get the tile info out. Which should all be the same size, but just in case, we only want the last part anyways
                        tileInfo = values[0].Split('/').ToList();
                        values.RemoveAt(0);

                        //pull out all the coords and put them together in one string.

                        for (int i = 0; i < 4; i++)
                        {
                            if (i == 3)
                            {
                                coords += " | " + values[0];
                            }
                            else
                            {
                                coords += values[0];
                            }

                            values.RemoveAt(0);
                        }


                        //all that should be left is the Log Number which we don't need for anything. (internal logs i think) so toss it.

                        //saving it here for debug purposes
                        log = values[0];


                        // and just in case, we'll make sure we clear it out.
                        values.Clear();


                        //Add the MapIdentifier string, and toss it out.
                        values.Add(mapInfo.Last());
                        mapInfo.RemoveAt(mapInfo.Count - 1);
                        //add the Mission Type + Tileset, and toss it out
                        values.Add(GetMissionType(mapInfo.Last()));
                        values.Add(GetTileSet(mapInfo.Last()));

                        mapInfo.RemoveAt(mapInfo.Count - 1);
                        //add the Faction info, with a few special cases.
                        if (mapInfo.Last() == "SpaceBattles")
                            values.Add("Corpus");
                        else if (mapInfo.Last() == "Space")
                            values.Add("Grineer");
                        else
                            values.Add(mapInfo.Last());

                        mapInfo.RemoveAt(mapInfo.Count - 1);
                        //add the tile name. - toss it in case I need the rest of the string...
                        values.Add(tileInfo.Last());
                        tileInfo.RemoveAt(tileInfo.Count - 1);
                        //add the coords.
                        values.Add(coords);
                        //debug purposes, add the log #
                        values.Add(log);
                    }
                    else if (values.Count == 6) // Arena's Special Case 
                    {
                        //get the tile information.
                        mapInfo = values[0].Split('/').ToList();
                        values.RemoveAt(0);

                        for (int i = 0; i < 4; i++)
                        {
                            if (i == 3)
                            {
                                coords += " | " + values[0];
                            }
                            else
                            {
                                coords += values[0];
                            }

                            values.RemoveAt(0);
                        }
                        //That leaves the log number, saving for debug purposes then clearing the list to reuse

                        log = values[0];
                        values.Clear();



                        //Add a custom MapIdentifier
                        values.Add("ArenaFight");
                        //Save the Tile Name, then toss it out to get to the next bit that we need.
                        var tileName = mapInfo.Last();
                        mapInfo.RemoveAt(mapInfo.Count - 1);
                        //add the Mission Type + Tileset - not toss, we still need it.

                        var arenaType =GetMissionType(mapInfo.Last());
                        values.Add(arenaType);
                        values.Add(arenaType); // Tileset is Rathuum or Index and th is the same as the Mission Type.
                        if (arenaType == "Index")
                            values.Add("Nef Anyo");
                        else if (arenaType == "Rathuum")
                            values.Add("Kela de Thaym");
                        mapInfo.Clear(); //clear this list
                        //Add the 'Tile Name' ... which is just the MapIdentifier again.
                        values.Add(tileName);
                        values.Add(coords);
                        values.Add(log);
                    }
                    else
                    {// not a valid file then, doesn't have the meta dat we need, toss it. ... i hope.
                        values = null;
                    }




                }

                //find the date of the file and add it.
                if (directory.Name == "File")
                {
                    string date = directory.Tags[2].Description;
                    values.Add(date);
                }



                if (directory.HasError)
                {
                    foreach (var error in directory.Errors)
                        Console.WriteLine($"ERROR: {error}");
                }
            }



            return values;
        }
    }

}
