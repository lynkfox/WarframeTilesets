using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WFTileCounter.Models;

namespace WFTileCounter.ModelsLogic
{
    public class InsertReadyData
    {
        //this model is used to hold all the various EF Models for easy transfer between Controllers.

        [Key]
        public int Id { get; set; }
        public Mission Mission { get; set; }
        public Run Run { get; set; }
        public Tileset Tileset { get; set; }
        public User User { get; set; }
        public IEnumerable<Tile> Tiles { get; set; }
        public IEnumerable<Tile> CompleteTileList { get; set; }

        //if the Run.MapIdentifier was already found in the database, this flag get set to true.
        public bool AlreadyProcessed { get; set; }
    }
}
