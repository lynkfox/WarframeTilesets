using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WFTileCounter.ModelsView
{
    public class ImgMetaData
    {
        public string FileName { get; set; }
        public string TileName { get; set; }
        public string MissionType { get; set; }
        public string Tileset { get; set; }
        public string FactionName { get; set; }
        public string Date { get; set; }
        public string MapIdentifier { get; set; }
        public string Coords { get; set; }
        public string LogNum { get; set; }

        public string ImgPath { get; set; }

    }
}
