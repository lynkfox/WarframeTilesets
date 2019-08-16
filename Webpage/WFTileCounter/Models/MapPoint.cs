using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFTileCounter.Models
{
    /*This table contains the meat of this project. Each Map Point has the corresponding Tile that it is, and the run it belongs to.
     * 
     * This table is the connection between Runs and Tiles, and what will be used to figure out how common various tiles are in the procedual generation
     * 
     */
    
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
