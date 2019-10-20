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

        public enum Objective
        {
            Nothing,
            MobileDefense,
            DefectionSpawn,
            DefectionRestPoint,
            KuvaSiphon,
            SurvivalPylon
        }

        
        [Key]
        public int Id { get; set; }
        [Display(Name = "Run ID: ")]
        [Required]
        public int RunId { get; set; }
        [ForeignKey("TileName")]
        [MaxLength(100)]
        [Display(Name ="Tile Name: ")]
        [Required]
        public Tile Tile { get; set; }
        [StringLength(50, MinimumLength = 10)]
        [Display(Name = "Screenshot Coordinates: ")]
        public string CoordsTaken { get; set; }

        
        public Objective Objectives { get; set; }

        public bool Ayatan { get; set; }
        public bool Medallion { get; set; }
        public bool RareContainer { get; set; }


        public bool Cephalon { get; set; }
        public bool Somachord { get; set; }
        public bool FrameFighter { get; set; }

        public bool CaptureSpawn { get; set; }
        public bool SimarisSpawn { get; set; }
        public bool Cache { get; set; }


        public Run Run { get; set; }
    }
}
