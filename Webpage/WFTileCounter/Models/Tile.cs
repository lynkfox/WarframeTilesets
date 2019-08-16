﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFTileCounter.Models
{
    public class Tile
    {
        /* Each individual Tile has its own entry.
         * 
         * At the very least a more friendly name will be given to it (PopularName).
         * 
         * Additional Columns will probably be added: Objectives, #of Luckers, #of Consoles, the directory where all the images are stored for this room, ect.
         * 
         */
        [Key]
        public string Name { get; set; }
        public string PopularName { get; set; }
        
        [ForeignKey("TilesetName")]
        public Tileset Tileset { get; set; }

        public IEnumerable<MapPoint> MapPoints { get; set; }

        [NotMapped]
        public string Coords { get; set; }
        //to be added to the MapPoints table during insert.





    }
}
