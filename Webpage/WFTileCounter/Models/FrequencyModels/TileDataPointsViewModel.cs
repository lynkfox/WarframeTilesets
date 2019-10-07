using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WFTileCounter.Models.FrequencyModels
{
    public class TileDataPointsViewModel
    {
        public List<TilesPerMission> TotalTilesPerMission { get; set; }

        public List<RunsPerMission> TotalRunsPerMissionList { get; set; }

        public List<TileDataPoint> DataPoints { get; set; }

        public string TilesetName { get; set; }

        public int TotalRuns { get; set; }
        
    }
}
