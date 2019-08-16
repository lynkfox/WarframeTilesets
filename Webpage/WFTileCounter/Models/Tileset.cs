using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WFTileCounter.Models
{
    public class Tileset
    {
        /* table to differentiate the different tilesets. Probably no more information will be added here.
         * 
         */

        [Key]
        [Display(Name="Tileset: ")]
        public string Name { get; set; }
        [Display(Name = "Faction: ")]
        public string Faction { get; set; }

        public IEnumerable<Tile> Tiles { get; set; }
        
    }
}
