using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WFTileCounter.ModelsLogic;


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

    }

}
