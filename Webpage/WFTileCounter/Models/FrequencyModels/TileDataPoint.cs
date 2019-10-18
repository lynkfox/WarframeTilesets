using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WFTileCounter.Models.FrequencyModels
{
    public class TileDataPoint
    {
        public string TileName { get; set; }
        public List<MissionAppearance> MissionTileNumbers { get; set; }


    }

    public class MissionAppearance
    {
        public int TotalAppearances { get; set; }

        public string MissionName { get; set; }

        public double PercentLikelyhoodOfAppearance { get; set; }
    }

    public class TilesPerMission
    {
        public string MissionName { get; set; }
        public int TotalTilesForMission { get; set; }
    }

    public class RunsPerMission
    {
        public string MissionName { get; set; }
        public int TotalRunsForMiss { get; set; }
    }

    public class CleanedUpDatabaseData
    {
        public string TileName { get; set; }
        public int RunId { get; set; }
        public string MissionName { get; set; }
    }

}

