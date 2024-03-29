﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public string ShortTileName { get; set; }
        public string TilesetName { get; set; }
        public TileDetail Details { get; set; }
        public IEnumerable<TileImage> Images { get; set; }
        
        public List<VariantTile> Variants { get; set; }
        public List<TileImage> MapImages { get; set; } //because we want a default image if there is no map image stored, and dont want it to show up in the 'All Images' section

        public List<SelectListItem> Numbers { get; set; }


        public IEnumerable<IFormFile> ImageFiles { get; set; }
    }
}
