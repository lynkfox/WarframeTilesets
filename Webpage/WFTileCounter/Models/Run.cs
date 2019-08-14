using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WFTileCounter.Models
{
    public class Run
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Date of Run: ")]
        public DateTime RunDate { get; set; }
        [ForeignKey("MissionType")]
        public Mission Mission { get; set; }
        public int UserID { get; set; }
        [Display(Name ="Map Identifier: ")]
        public string IdentityString { get; set; }
        [Display(Name = "Log Values: ")]
        public string LogRange { get; set; }
        [Display(Name = "# Tiles Counted: ")]
        public int TotalTiles { get; set; }
        [Display(Name = "# Unique Tiles: ")]
        public int UniqueTiles { get; set; }



        public User User { get; set; }
        
        public IEnumerable<MapPoint> MapPoints { get; set; }

        [NotMapped]
        [Display(Name = "# Unique Tiles: ")]
        public int TilesAdded { get; set; }
    }
}
