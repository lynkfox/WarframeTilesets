﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WFTileCounter.Models;
using WFTileCounter.ModelsLogic;
using WFTileCounter.ModelsView;

namespace WFTileCounter.ControllersProcessing
{
    public class GeneralFunctions
    {

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

            if (value == "CorpusShip")
            {
                return "The Sergeant Assassination";
            }
            if (value == "GrineerAsteroid")
            {
                return "Drilling Machine Sabotage";
            }

            return value+"??? - Check Me";

        }

        public string GetTileSet(string value)
        {
            string[] missionType;
            var _df = new DatabaseFunctions(_db);

            var path = Path.Combine(
                             System.IO.Directory.GetCurrentDirectory(), "wwwroot", "lib", "lists");

            path = Path.Combine(path, "wftTilesetCuts.csv");

            if (!File.Exists(path))
            {
                return null;

            }
            else
            {

                missionType = File.ReadAllLines(path);
                
            }

            /*
            string[] missionType = { "BossVor", "JackalBoss", "HyenaAssassinate", "AmbulasBoss","BossKela", "J3GolemAssassinate", "BossInfested",
                                        "RopalolystBoss", "Boss", "Assassinate", "SentientArtifact", "ShipPurify", "ColonistRescue", "Rescue", "MobileDefense", 
                                         "SabotageForest", "Intel", "Pursuit" };
            */
            int length = value.Length;

            if(_df.CheckTilesetExists(value)) // checks the database to see if the Tileset already exists. Useful for Assasination Missions that may not have the type after it.
            {
                return value;
            }

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
                metaData.KeepThis = true;



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
                        else if (mapInfo.Last() == "SpecialMissions")
                            values.Add("Maroo");
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
