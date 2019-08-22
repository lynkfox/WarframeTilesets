using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WFTileCounter.Models
{
    [Table("VariantTiles", Schema = "website")]
    public class VariantTile
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("TileName")]
        [MaxLength(100)]
        [Display(Name = "Tile Name: ")]
        [Required]
        public TileDetail Details { get; set; }
        [MaxLength(100)]
        [Display(Name = "Variant: ")]
        [Required]
        public string VariantTileName { get; set; }
    }
}
