using System;
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
        [MaxLength(100)]
        [Display(Name= "DE Tile Name: ")]
        public string Name { get; set; }
        [ForeignKey("TilesetName")]
        [Display(Name="Tileset: ")]
        [MaxLength(100)]
        public Tileset Tileset { get; set; }



        public TileDetail TileDetail { get; set; }
        public IEnumerable<TileImage> TileImages { get; set; }
        public IEnumerable<MapPoint> MapPoints { get; set; }



        //These are for keeping the same information with the same tile, but are to be added to the MapPoints table instead. 
        [NotMapped]
        public string Coords { get; set; }
        [NotMapped]
        public bool NewTile { get; set; }
        [NotMapped]
        public string Collectibles { get; set; }
        [NotMapped]
        public string Objectives { get; set; }
        [NotMapped]
        public string Scanables { get; set; }
        [NotMapped]
        public string Spawnable { get; set; }


    }
}
