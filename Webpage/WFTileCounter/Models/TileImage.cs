using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFTileCounter.Models

    /* contains the Image Path, the Name of the Image for a given tile
     */
{
    [Table("TileImages", Schema = "website")]
    public class TileImage
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("TileName")]
        [MaxLength(100)]
        [Display(Name = "Tile Name: ")]
        [Required]
        public Tile Tile { get; set; }

        [Display(Name = "View From: ")]
        [MaxLength(50)]
        public string ViewName { get; set; }

        [MaxLength(250)]
        [Required]
        public string ImagePath { get; set; } // path should start  (tilesetName) / (tilename) / (imagename).png or jpg  - from ~/wwwroot/img/tilesets/ is assumed for each image
        [MaxLength(250)]
        [Required]
        public string AltText { get; set; }
    }
}
