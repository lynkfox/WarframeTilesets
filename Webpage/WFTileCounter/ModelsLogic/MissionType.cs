using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WFTileCounter.ModelsLogic
{
    public class MissionType
    {
        //Simple model for generating a list of Mission Types from a csv file, that contains what the name of the mission looks like in the MetaData, and what the rest of the world calls it.
        public string InGameName { get; set; }
        public string CommonName { get; set; }

    }
    
}
