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


        public IEnumerable<ProcessedData> ConvertToDatabase(List<MetaProcessed> meta)
        {

            //variable declare

            var processing = new ProcessedData();
            var processedList = new List<ProcessedData>();
            var mission = new Mission();
            var run = new Run();
            var tileset = new Tileset();
            List<Tile> tiles = new List<Tile>();
            bool first = true;


            //in case multiple runs are uploaded at the same time, get a list of unique map identifiers (which are, as far as I can tell, unique per run)
            IEnumerable<string> distinctMapIdentifiers = meta.Select(a => a.MapIdentifier).Distinct();
            

            foreach(var id in distinctMapIdentifiers)
            {
                foreach (var item in meta)
                {
                    
                    string identifier = item.MapIdentifier;
                    if(identifier == id)
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
                        if(!tiles.Contains(tile))
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
                first = false; 
                
            }

            

            return processedList;

        }
    }

}
