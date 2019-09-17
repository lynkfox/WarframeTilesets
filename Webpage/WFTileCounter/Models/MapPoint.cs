﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WFTileCounter.Models
{
    /*This table contains the meat of this project. Each Map Point has the corresponding Tile that it is, and the run it belongs to.
     * 
     * This table is the connection between Runs and Tiles, and what will be used to figure out how common various tiles are in the procedual generation
     * 
     */
    
    public class MapPoint
    {

        public enum Objective
        {
            Nothing,
            MobileDefense,
            CaptureSpawn,
            DefectionRestPoint,
            SimarisSpawn,
            KuvaSiphon
        }

        public enum Scanable
        {
            Nothing,
            Ordis,
            Somachord,
            FrameFighter,
            OrdisSoma,
            OrdisFrame,
            SomaFrame,
            AllThree
        }

        public enum Collectible
        {
            Nothing,
            AyatanStatue,
            Medallion,
            AyatanMedallion
        }
        [Key]
        public int Id { get; set; }
        [Display(Name = "Run ID: ")]
        [Required]
        public int RunId { get; set; }
        [ForeignKey("TileName")]
        [MaxLength(100)]
        [Display(Name ="Tile Name: ")]
        [Required]
        public Tile Tile { get; set; }
        [StringLength(50, MinimumLength = 10)]
        [Display(Name = "Screenshot Coordinates: ")]
        public string CoordsTaken { get; set; }


        public Objective? Objectives { get; set; }
        public Scanable? Scanables { get; set; }
        public Collectible? Collectibles { get; set; }

        public Run Run { get; set; }
    }
}
