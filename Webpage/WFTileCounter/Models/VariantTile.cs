using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace WFTileCounter.Models
{

    /* This table holds the various variants for a tile. It can probably be refined to be concurant unique ness, but at the moment, searches will search either side
     * 
     * Variants are the same basic room with just small differences.  ( ClosetCommon1 and ClosetCommon2) 
     * 
     * They may not be in the same tileset (ie: OptionalLootChallenge1 in Orokin Tower and OptionalLootChallenge1Derelict in Orokin Tower Derelict)
     */
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

        [NotMapped]
        public string TilesetPath { get; set; }
        [NotMapped]
        public string TilePath { get; set; }
    }
}
