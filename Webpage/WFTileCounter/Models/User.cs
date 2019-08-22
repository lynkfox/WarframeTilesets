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
        [Display(Name ="Username: ")]
        [Required]
        [StringLength(150,MinimumLength =5)]
        public string Username { get; set; }
        [Display(Name ="Email Address: ")]
        [Required]
        [StringLength(150,MinimumLength =7)]
        public string email { get; set; }
        public int RunsUploaded { get; set; }
        // Research how to make this a Code First Calculated Column, so that it automatically traes the Count() of Runs with this User's Id.


        public IEnumerable<Run> Runs { get; set; }

    }
}
