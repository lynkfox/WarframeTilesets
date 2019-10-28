using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WFTileCounter.Models.FrequencyModels
{
    public class CollectiblesViewModel
    {
        public List<CollectibleFrequencePerTile> TileCollectiblesList { get; set; }
        public string Tileset { get; set; }
        public double AyatanTotal { get; set; }
        public double AyatanPercentage { get; set; }
        public double MedallionTotal { get; set; }
        public double MedalionPercentage { get; set; }
        public double CephalonTotal { get; set; }
        public double CephalonPercentage { get; set; }
        public double SomachordTotal { get; set; }
        public double SomachordPercentage { get; set; }
        public double FrameFighterTotal { get; set; }
        public double FrameFighterPercentage { get; set; }
        public double RareContainerTotal { get; set; }
        public double RareContainerPercentage { get; set; }
        public double SabotageCacheTotal { get; set; }
        public double SabotageCachePercentage { get; set; }
    }
    public class CollectibleFrequencePerTile
    {
        public string Tilename { get; set; }
        public double AyatanCount { get; set; }
        public double AyatanPercentagePerRun { get; set; }
        public double MedallionCount { get; set; }
        public double MedalionPercentagePerRun { get; set; }
        public double CephalonCount { get; set; }
        public double CephalonPercentagePerRun { get; set; }
        public double SomachordCount { get; set; }
        public double SomachordPercentagePerRun { get; set; }
        public double FrameFighterCount { get; set; }
        public double FrameFighterPercentagePerRun { get; set; }
        public double RareContainerCount { get; set; }
        public double RareContainerPercentagePerRun { get; set; }
        public double  SabotageCacheCount { get; set; }
        public double SabotageCachePercentagePerRun { get; set; }

        internal bool AreThereAnyCollectiblesInThisTile()
        {
           return  this.AyatanCount + this.MedallionCount + this.CephalonCount + this.SomachordCount + this.FrameFighterCount + this.RareContainerCount + this.SabotageCacheCount > 0;
        }
    }

    public class CleanedUpCollectibleDatabaseInfo
    {
        public string Tilename { get; set; }
        public bool Ayatan { get; set; }
        public bool Medallion { get; set; }
        public bool Cephalon { get; set; }
        public bool Somachord { get; set; }
        public bool FrameFighter { get; set; }
        public bool RareContainer { get; set; }
        public bool SabotageCache { get; set; }
    }
}
