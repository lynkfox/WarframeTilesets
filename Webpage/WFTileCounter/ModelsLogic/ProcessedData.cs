using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WFTileCounter.Models;

namespace WFTileCounter.ModelsLogic
{
    public class ProcessedData
    {
        public Mission Mission { get; set; }
        public Run Run { get; set; }
        public Tileset Tileset { get; set; }
        public IEnumerable<Tile> Tiles { get; set; }
    }
}
