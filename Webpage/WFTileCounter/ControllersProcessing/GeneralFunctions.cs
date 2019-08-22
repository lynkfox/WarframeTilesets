using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WFTileCounter.Models;
using WFTileCounter.ModelsLogic;
using WFTileCounter.ModelsView;

namespace WFTileCounter.ControllersProcessing
{
    public class GeneralFunctions
    {

        /* This class holds all my general use logic functions, that are needed between controllers.
         */





        /* I feel like I'm doing this wrong. I only have this hear because I use a function in DatabaseFunctions to check if something exists before I go about with the next step.
         * 
         * So i have to have a Dbcontext to pass to the new DatabaseFunctions constructor.
         * 
         * but ... i feel like there is something i'm missing here.
         * 
         */
        private readonly DatabaseContext _db;


        public GeneralFunctions(DatabaseContext context)
        {
            _db = context;
        }

        //Temp function for use between multiple computers, but will need to be made dynamic for a server.

        //If you are Running this on your own before I've reached a release stage, then you need to change path below to where your Warframe Pictures directory is.
        public string GetPath()
        {
            //return Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", "Uploads");


            return @"C:\Users\lynkf\Pictures\Warframe";
        }

        public List<MissionType> GenMissionList()
        {
            List<MissionType> list = new List<MissionType>();


            /*
             * before csv file
             * 
             * 
            string[] missionTypeDiff = { "GrineerAsteroidBossVor", "CorpusShipJackalBoss", "GrineerForestBoss", "GrineerSettlementBoss", "GrineerShipyardsAssassinate", "CorpusGasBoss",
                                        "CorpusIcePlanetAssassinate", "GrineerGalleonBoss", "GrineerOceanAssassinate", "CorpusShipHyenaAssassinate", "CorpusOutpostAmbulasBoss",
                                        "GrineerAsteroidBossKela", "InfestedCorpusShipJ3GolemAssassinate", "InfestedCorpusShipAssassinate", "OrokinTowerDerelictBoss", "BossInfested",
                                        "CorpusGasCityRopalolystBoss", "Boss", "SentientArtifact", "ShipPurify", "ColonistRescue", "Rescue", "MobileDefense", "Assassinate", "KelaArena",
                                         "SabotageForest", "Intel", "CorpusArena", "TRRace", "TRSabotage", "Pursuit" };
            string[] commonName = {"Vor Assassination", "Jackal Assassination", "Vay Hek Assassination", "Lech Kril Assassination", "Vor + Kril Assassination", "Alad V Assassination",
                                    "Raptors Assassination", "Sargas Ruk Assassination", "Tyl Regor Assassination", "Hyena Assassination", "Ambulas Assassination",
                                    "Kela De Thaym Assassination", "Jordas Golem Assassination", "Mutalist Alad V Assassination", "Lephantis Assassination", "Phorid Assassination",
                                    "Ropalolyst Assassination","Assassination", "Disruption", "Infested Salvage", "Defection", "Rescue", "Mobile Defense", "Assassiation", "Rathuum", "Sabatoge", "Spy", "Index", "Rush (Archwing)", "Sabotage (Archwing)", "Pursuit", "Defense", "Excavation", "Sabotage", "Survival", "Exterminate", "Interception", "Hijack", "Spy", "Capture" };


            
            for (int i = 0; i < commonName.Length; i++)
            {
               

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
            */

            var path = Path.Combine(
                             System.IO.Directory.GetCurrentDirectory(), "wwwroot", "lib", "lists");

            path = Path.Combine(path, "wftMissionNames.csv");

            if(!File.Exists(path))
            {
                return null;

            }
            else
            {
               
                string[] namePairs = File.ReadAllLines(path);
                foreach(var line in namePairs)
                {
                    var mission = new MissionType();
                    string[] splitPairs = line.Split(',');
                    mission.CommonName = splitPairs[1];
                    mission.InGameName = splitPairs[0];
                    list.Add(mission);
                }
            }


            
            return list;
        }


        public string GetMissionType(string value)
        {

            string name;

            var types = GenMissionList();
            if(types is null)
            {
                // this... is very wrong. gotta fix this.
                throw new Exception();
            }

            //These two have a bad habit of NOT having the mission type on the TilesetName like the rest do. SO.. special case.
            if (value == "CorpusShip")
            {
                return "Assassination (The Sergeant)";
            }
            if (value == "GrineerAsteroid")
            {
                return "Sabotage (Drill)";
            }

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


            
            

            return value+"??? - Check Me";

        }

        public string GetTileSet(string value)
        {
            string[] missionType;
            string[] notValid;
            var _df = new DatabaseFunctions(_db);

            var path = Path.Combine(
                             System.IO.Directory.GetCurrentDirectory(), "wwwroot", "lib", "lists");

            path = Path.Combine(path, "wftTilesetCuts.csv");

            if (!File.Exists(path))
            {
                return "Error";

            }
            else
            {

                missionType = File.ReadAllLines(path);
                
            }

            path = Path.Combine(
                             System.IO.Directory.GetCurrentDirectory(), "wwwroot", "lib", "lists");

            path = Path.Combine(path, "nonProcedualSets.csv");

            if (!File.Exists(path))
            {
                return "Error";

            }
            else
            {

                notValid = File.ReadAllLines(path);

            }

            
            for(int i=0; i<notValid.Length; i++)
            {
                string badPic = "*" + notValid[i] + "*";

                if(Regex.IsMatch(value, WildCardToRegular(badPic)))
                {
                    return "Error";
                }
            }
            

            /*
            string[] missionType = { "BossVor", "JackalBoss", "HyenaAssassinate", "AmbulasBoss","BossKela", "J3GolemAssassinate", "BossInfested",
                                        "RopalolystBoss", "Boss", "Assassinate", "SentientArtifact", "ShipPurify", "ColonistRescue", "Rescue", "MobileDefense", 
                                         "SabotageForest", "Intel", "Pursuit" };
            */
            int length = value.Length;

            if(_df.CheckTilesetExists(value)) // checks the database to see if the Tileset already exists.
            {
                return value;
            }

            if (value.Contains("TR")) // special case, just get it over with.
            {
                return "Trench Run (Archwing)";
            }
                
            if (value.Contains("Space"))
            {
                return "Free Space (Archwing)";
            }
            if (value == "CorpusGasBoss")
            {
                return "CorpusGasCity";
            }
            if (value.Contains("Arena")) // this shouldn't be needed anymore
            {

                return value;
            }
            if (value.Contains("ProcLevel")) // Some Archwing Missions have ProcLevel at the end of their missiontype, remove it first.
            {
                value = value.Remove(length - 9);
                length = value.Length; // update the new length

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
            return value; 
        }


        

        public List<ImgMetaData> GetMetaList(string path)
        {
            List<ImgMetaData> metaList = new List<ImgMetaData>();

            var picList = System.IO.Directory.GetFiles(path);

            //There should only ever be one spawn or exit tile,so we'll set flags when we find the first one.
            bool spawn = false;
            bool exit = false;

            foreach (var pic in picList)
            {
                var metaData = new ImgMetaData();

                var metaValues = GetMetaData(pic);

                if(metaValues[2]=="Error" || metaValues[0]=="ArenaFight")
                { continue; }

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
                metaData.KeepThis = true;


                if(metaData.MissionType.Contains("???") || metaData.Tileset.Contains("???"))
                {
                    //if the MissionType or the Tileset hits the final return, it will add ??? to the name.
                    // so we check for that so we can auto exclude files that don't have the proper checks for their MissionType/Tileset
                    metaData.KeepThis = false;
                    metaData.UnknownValue = true;
                }
                else
                {
                    metaData.UnknownValue = false;
                }
                

                /*Note for next bit - add a check if the Filename is NOT Warframe####
                 */

                if(metaList.Count !=0 && metaData.TileName ==metaList.Last().TileName)
                {
                    /*Checking to see if the name of this file being processed is the same as the last.
                     * This is far from perfect. However, f6 screenshot automatically names the images as Warframe#### in sequential order
                     * so if the tile names are the same there is a higher chance that the image was taken within the same tile than any other
                     * method I can think of to track this. We can't say for certain there aren't two tiles in a row though, so it only will produce
                     * a warning, and auto uncheck the 'Keep This' box
                     */
                    metaData.KeepThis = false;
                    metaData.PossibleDupe = true;
                }else
                {
                    metaData.PossibleDupe = false;
                }


                /* There should only ever be one Spawn/Start or Exit/Extraction tile in a map. If there are more than one in the uploads, then its probably a duplicate image.
                 * so we'll check for them, if we have already found one, we'll set KeepThis = false so it gets auto unchecked and duplicate flag on the view gets added
                 * 
                 * 
                 * this obviously has an issue if more than one run is tossed into the processing at once. So we need to add some checks for that.
                 */
                if(metaData.TileName.Contains("Spawn") || metaData.TileName.Contains("Start") )
                {
                    if(spawn)
                    {
                        metaData.KeepThis = false;
                        metaData.PossibleDupe = true;
                    }
                    else
                    {
                        spawn = true;
                    }
                }else if (metaData.TileName.Contains("Exit") || metaData.TileName.Contains("Extraction"))
                {
                    if (exit)
                    {
                        metaData.KeepThis = false;
                        metaData.PossibleDupe = true;
                    }
                    else
                    {
                        exit = true;
                    }
                }

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



                        string tileset = GetTileSet(mapInfo.Last());

                        values.Add(tileset);
                        
                        

                        mapInfo.RemoveAt(mapInfo.Count - 1);



                        //add the Faction info, with a few special cases.
                        if (mapInfo.Last() == "SpaceBattles") // Corpus Archwing
                            values.Add("Corpus");
                        else if (mapInfo.Last() == "Space") //Grineer Archwing
                            values.Add("Grineer");
                        else if (mapInfo.Last() == "SpecialMissions") //Maroo Treasure Hunt
                        {// special missions takes up a slot in the / / / list, so we remove it, and check the next one to add
                            mapInfo.RemoveAt(mapInfo.Count - 1);
                            values.Add(mapInfo.Last());
                        }
                        else
                            values.Add(mapInfo.Last());

                     

                        
                        mapInfo.RemoveAt(mapInfo.Count - 1);



                        //Fix the Tile name (too many duplicates!!!)
                        string newTileName = AddQualifierToTileName(values.Last(), tileInfo.Last(), tileset);

                     
                        values.Add(newTileName);






                        
                        tileInfo.RemoveAt(tileInfo.Count - 1);
                        //add the coords.
                        values.Add(coords);
                        //debug purposes, add the log #
                        values.Add(log);
                    }
                    else  //this SHOULD cover all the weird maps, like Hubs and what not. There is error checking just in case elsewqhere, but... 
                    {
                        values.Add("Error"); // not the most elegant way to do this.
                        /*
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
                        */
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

        private string AddQualifierToTileName(string faction, string tilename, string tileset)
        {

            string threeLtrQualifier = faction.Substring(0, 3); ; // save the Faction name so we can deal with stupid duplicate tile names

            // first get the first 3 letters of the Faction: Cor, Gri, Oro, Inf
             
            string tileFactionCheck = tilename.Substring(0, 3);
            string newTileName;

            //check to see if the tile name already has it as the first three... 

            if (tileset == "GrineerToCorpus") //Invasion Missions sometimes use the same tilenames, so add a qualifier
            {
                newTileName = "G2C" + tilename;
            }
            else if (tileset == "CorpusToGrineer")
            {
                newTileName = "C2G" + tilename;

            }
            else if (tileset == "GrineerForest") //Grineer Forest Special case, uses Gft 
            {
                newTileName = tilename;
            }
            else if (tileFactionCheck != threeLtrQualifier || tilename.Contains("Corner")) //edge case, corner triggers Cor
            {//if not, add it to make all tile names Unique (so DeadEnd2 on CorpusShip and DeadEnd2 on Orokin Tower are different.


                //switch Gri to Grn to maintain how DE is already doing it on OTHER tilesets... damn lack of consistancy

                if (threeLtrQualifier == "Gri")
                {
                    threeLtrQualifier = "Grn";
                }
                else if (threeLtrQualifier == "Cor") // To keep a sort of standard op, do the same for Corpus 
                {
                    threeLtrQualifier = "Crp";
                }


                //finally check if it already starts with Crp or Grn, or contains another qualifier that will make it unique.
                if (tileFactionCheck == "Crp" || tileFactionCheck == "Grn" || tilename.Contains("Grineer") || tilename.Contains("Corpus")
                    || tilename.Contains("Infested") || tilename.Contains("Moon") || tilename.Contains("Derelict"))
                {
                    newTileName = tilename;
                }
                else
                {
                    newTileName = threeLtrQualifier + tilename;
                }


            }
            else
            {
                newTileName = tilename;
            }

            return newTileName;
        }



        //quick function for * wildcard search.
        private static String WildCardToRegular(String value)
        {
            return "^" + Regex.Escape(value).Replace("\\*", ".*") + "$";
        }
    }

}
