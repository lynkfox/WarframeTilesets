    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
using WFTileCounter.Models;

namespace WFTileCounter.ModelsView
    {
    public class TileDetailsViewModel
    {
        public Tile Tile{ get; set; }
        public TileDetail Details { get; set; }
        public IEnumerable<TileImage> Images { get; set; }
        public IEnumerable<VariantTile> Variants { get; set; }
        public TileImage Map { get; set; } //because we want a default image if there is no map image stored, and dont want it to show up in the 'All Images' section

    }
}
