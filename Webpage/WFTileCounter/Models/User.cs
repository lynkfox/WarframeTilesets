using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WFTileCounter.Models
{
    public class User
    {
        /* Eventual table for user profiles.
         * 
         * Will probably add salted hash pw fields into here
         */

        [Key]
        public int Id { get; set; }
        public string Username { get; set; }
        public string email { get; set; }
        public int RunsUploaded { get; set; }


        public IEnumerable<Run> Runs { get; set; }

    }
}
