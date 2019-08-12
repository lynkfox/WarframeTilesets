using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace WFTileCounter.Models
{
    public class MapPoint
    {
        [Key]
        public int Id { get; set; }
        public int RunId { get; set; }
        public int TileId { get; set; }

        public Tile Tile { get; set; }
        public Run Run { get; set; }
    }
}
