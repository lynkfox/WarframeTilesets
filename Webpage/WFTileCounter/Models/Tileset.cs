using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WFTileCounter.Models
{
    public class Tileset
    {
        [Key]
        public int Id { get; set; }
        [Display(Name="Tileset: ")]
        public string Name { get; set; }
        [Display(Name = "Faction: ")]
        public string Faction { get; set; }

        public IEnumerable<Tile> Tiles { get; set; }
        
    }
}
