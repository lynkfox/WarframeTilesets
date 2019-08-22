using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WFTileCounter.Models
{

    /* Currently this table just holds the Mission name. Further details might be added eventually
     * 
     */
    public class Mission
    {
        [Key]
        [Display(Name = "Mission Type: ")]
        [StringLength(100)]
        public string Type { get; set; }

        
        public IEnumerable<Run> Runs { get; set; }

    }
}
