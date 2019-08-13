using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WFTileCounter.Models
{
    public class Tile
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string PopularName { get; set; }
        public int TilesetId { get; set; }

        public Tileset Tileset { get; set; }

        public IEnumerable<MapPoint> MapPoints { get; set; }





    }
}
