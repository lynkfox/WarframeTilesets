using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WFTileCounter.Models
{

    /* Contains all the necessary information to describe a given tile on the Tile Index of the website
     */
    [Table("TileDetails", Schema = "website")]
    public class TileDetail
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("TileName")]
        [MaxLength(100)]
        [Display(Name = "Tile Name: ")]
        [Required]
        public Tile Tile { get; set; }
        [MaxLength(100)]
        [Display(Name = "Popular Name: ")]
        public string PopularName { get; set; }
        [Display(Name = "Exits: ")]
        public int? Exits { get; set; }
        [Display(Name = "Alarm Consoles:")]
        public int? Consoles { get; set; }
        [Display(Name = "Locker Banks: ")]
        public int? LockerBanks { get; set; }
        [Display(Name = "Total Lockers: ")]
        public int? TotalLockers { get; set; }
        [Display(Name = "Secrets: ")]
        public int? Secrets { get; set; }
        [Display(Name = "Loot Rooms: ")]
        public int? LootRooms { get; set; }
        [Display(Name = "Hazards: ")]
        public int? Hazards { get; set; }
        [Display(Name = "Possible Objectives:")]
        [StringLength(50)]
        public string Objectives { get; set; }
        [Display(Name = "Description: ")]
        [Column(TypeName = "ntext")]
        public string Description { get; set; }
        [Display(Name = "Reason for Popular Name: ")]
        [Column(TypeName = "ntext")]
        public string BehindName { get; set; }

        public IEnumerable<VariantTile> VariantTiles { get; set; }
    }
}
