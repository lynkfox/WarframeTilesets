﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WFTileCounter.Models
{
    public class Run
    {
        [Key]
        public int Id { get; set; }
        public DateTime RunDate { get; set; }
        public int MissionID { get; set; }
        public int UserID { get; set; }


        public User User { get; set; }
        public Mission Mission { get; set; }
        public IEnumerable<MapPoint> MapPoints { get; set; }
    }
}
