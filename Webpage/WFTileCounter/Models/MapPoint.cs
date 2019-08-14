using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFTileCounter.Models
{
    public class MapPoint
    {
        [Key]
        public int Id { get; set; }
        public int RunId { get; set; }
        [ForeignKey("TileName")]
        public Tile Tile { get; set; }
        public string CoordsTaken { get; set; }

        
        public Run Run { get; set; }
    }
}
