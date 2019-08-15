using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WFTileCounter.Models
{
    public class User
    {
        

        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string email { get; set; }
        public int RunsUploaded { get; set; }


        public IEnumerable<Run> Runs { get; set; }

    }
}
