using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WFTileCounter.ModelsView
{
    public class ImgMetaData
    {
        [Display(Name ="Image Name: ")]
        public string FileName { get; set; }
        [Display(Name = "Room: ")]
        public string TileName { get; set; }
        [Display(Name = "Mission: ")]
        public string MissionType { get; set; }
        [Display(Name = "Tileset: ")]
        public string Tileset { get; set; }
        [Display(Name = "Faction: ")]
        public string FactionName { get; set; }
        [Display(Name = "Date Taken: ")]
        public string Date { get; set; }
        [Display(Name = "Map ID: ")]
        public string MapIdentifier { get; set; }
        [Display(Name = "Coords Taken: ")]
        public string Coords { get; set; }
        [Display(Name = "Log value: ")]
        public string LogNum { get; set; }

        public string ImgPath { get; set; }

        [Display(Name ="Process Image?")]
        public bool KeepThis { get; set; }

    }
}
