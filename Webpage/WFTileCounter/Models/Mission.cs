using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WFTileCounter.Models
{
    public class Mission
    {
        [Key]
        public int Id { get; set; }
        [Display(Name ="Mission Type: ")]
        public string Type { get; set; }

        public IEnumerable<Run> Runs { get; set; }

    }
}
