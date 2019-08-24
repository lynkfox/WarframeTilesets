using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WFTileCounter.ModelsView
{
    public class MultipleMapIdentifiers
    {
        [Display(Name = "Map: ")]
        public string MapIdentifier { get; set; }
        public List<ImgMetaData> ImgMetaDatas { get; set; }

        [Display(Name = "Full Run? ")]
        public bool FullRun { get; set; }
    }
}
