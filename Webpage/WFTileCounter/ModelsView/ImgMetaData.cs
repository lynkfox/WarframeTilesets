using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WFTileCounter.Models;

namespace WFTileCounter.ModelsView
{
    public class ImgMetaData
    {
        /* This Class holds all the needed information in regards to what will be put into EF Models and the database.
         * 
         * I kept it seperate out like this for the reason of being able to process it server side, show it in a view, and toss it around as needed without
         * having to worry about the dbContext and EF becoming tangled and confused. 
         * 
         * The data is kept in this format and this Model is used for all Views. It is only converted int InsertReadyData right before it is sent to the
         * methods that insert the data into the database.
         *
         */ 

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

        public TileImage TileImageInfo { get; set; }


        [Display(Name = "Possible Duplicate!!!")]
        public bool PossibleDupe { get; set; }
        // for the view, to help alert if perhaps adding a duplicate tile
        [Display(Name = "Something is Unknown!!!")]
        public bool UnknownValue { get; set; }
        // for the view, to help alert if some data isn't properly set up to be parsed in MissionType or Tileset


        public bool AlternateTileset { get; set; } // For recording if this tile name appears (with a pretty much identical room) in multiple tilesets


        [Display(Name ="Process Image?")]
        public bool KeepThis { get; set; }



    }
}
