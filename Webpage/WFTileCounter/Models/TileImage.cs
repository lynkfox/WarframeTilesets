using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFTileCounter.Models
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

        [ForeignKey("TilesetName")]
        [Display(Name = "Tileset: ")]
        [MaxLength(100)]
        public Tileset Tileset { get; set; }

        [Display(Name = "View From ")]
        [MaxLength(50)]
        public string ViewName { get; set; }

        [MaxLength(50)]
        [Required]
        public string ImgName { get; set; }
    }
}
