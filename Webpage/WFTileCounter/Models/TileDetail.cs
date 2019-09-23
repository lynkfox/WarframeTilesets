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
        [Display(Name = "DE Name")]
        [Required]
        public Tile Tile { get; set; }
        [MaxLength(100)]
        [Display(Name = "Popular Name")]
        public string PopularName { get; set; }
        [Display(Name = "#Exits")]
        public int? Exits { get; set; }
        [Display(Name = "#Consoles")]
        public int? Consoles { get; set; }
        [Display(Name = "#Lockers")]
        public int? Lockers { get; set; }
        [Display(Name = "#Secrets")]
        public int? Secrets { get; set; }
        [Display(Name = "#Closets")]
        public int? LootRooms { get; set; }
        [Display(Name = "#Hazards")]
        public int? Hazards { get; set; }
        
        [Display(Name = "General Room Description: ")]
        [Column(TypeName = "ntext")]
        public string Description { get; set; }
        [Display(Name = "Reason for Popular Name: ")]
        [Column(TypeName = "ntext")]
        public string BehindName { get; set; }
        [Display(Name = "Description of Secrets: ")]
        [Column(TypeName = "ntext")]
        public string SecretDescription { get; set; }
        [Display(Name = "Description of Hazards: ")]
        [Column(TypeName = "ntext")]
        public string HazardDescription { get; set; }

        //Objectives
        [Display(Name = "Start Point:")]
        public bool PlayerSpawn { get; set; }
        [Display(Name = "Extration Point:")]
        public bool PlayerExtract { get; set; }
        [Display(Name = "Mobile Defense:")]
        public bool MobileDefense { get; set; }
        [Display(Name = "Defectors Spawn:")]
        public bool DefectionSpawn { get; set; }
        [Display(Name = "Defection Rest:")]
        public bool DefectionRest { get; set; }
        [Display(Name = "Survival Pylon:")]
        public bool SurvivalPylon { get; set; }


        //Treasure Hunts
        
        public bool Ayatan { get; set; }
        public bool Medallion { get; set; }
        public bool Cephalon { get; set; }
        public bool Somachord { get; set; }
        public bool FrameFighter { get; set; }
        public bool Cache { get; set; }
        public bool CaptureSpawn { get; set; }
        public bool SimarisSpawn { get; set; }
        public bool RareContainer { get; set; }


        public IEnumerable<VariantTile> VariantTiles { get; set; }
    }
}
