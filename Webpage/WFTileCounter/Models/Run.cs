using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WFTileCounter.Models
{
    /* This table is the major information table. Each Run is a seperate mission instance, with its own map.
     * 
     * The goal is to eventually figure out how often certain tiles appear in a run, and that means this table is important for defining how often that tileset
     * was used during a mission.
     * 
     * LogRange is probably not going to be important, because it appears to be the line value of the continually running log while an active Warframe session is going,
     * continued between missions. Storing it just in case but...
     * 
     * TotalTiles is only derived from how many images were uploaded and processed with the unique IdentityString. As such, if someone takes multiple pictures from inside
     * inside the same isntance of a tile, then this number will be incorrect. Unfortuantelly, there does not seem to be a way to tell that this is happening, not easily. 
     * 
     * At the moment, relying on Users to remove duplicates, which is var from ideal.
     * 
     * Unique Tiles is the the more important number, this determines how many different tiles there were in the map, ignoring all repeats.
     * 
     * IdentitySTring is what appears to be, in the MetaData, a unique string that I believe is associated with the procedually generated map - most likely so DE can regenerate
     * that map if they feel it is needed to check for a bug. 
     * 
     * Maybe someday, with enough information, we'll be able to parse that string back.
     */
    public class Run
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Date of Run: ")]
        public DateTime RunDate { get; set; }
        [ForeignKey("MissionType")]
        [Display(Name = "Mission Type: ")]
        [MaxLength(100)]
        public Mission Mission { get; set; }
        [Display(Name = "User ID #: ")]
        public int UserID { get; set; } = 1;
        [Display(Name ="Map Identifier: ")]
        [MaxLength(250)]
        [Required]
        public string IdentityString { get; set; }
        [Display(Name = "Log Values: ")]
        [MaxLength(250)]
        public string LogRange { get; set; }
        [Display(Name = "Images Checked: ")]
        public int TotalTiles { get; set; }
        [Display(Name = "Unique Tiles: ")]
        public int UniqueTiles { get; set; }
        [Display(Name = "Partial Run?")]
        [StringLength(1)]
        public bool FullRun { get; set; }
        [Display(Name ="Map Points Recorded:")]
        public bool MapPointsUsed { get; set; }



        public User User { get; set; }
        
        public IEnumerable<MapPoint> MapPoints { get; set; }

        [NotMapped]
        [Display(Name = "# Unique Tiles: ")]
        public int TilesAdded { get; set; }
    }
}
