﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WFTileCounter.Models;
using WFTileCounter.ModelsView;

namespace WFTileCounter.BuisnessLogic
{
    public class TileFunctions
    {

        private readonly DatabaseContext _db; //database context shortcut



        public TileFunctions(DatabaseContext context)
        {
            _db = context;
        }


        public TileDetailsViewModel GetFullTileInformation(string tileName)
        {
            var _gf = new GeneralFunctions(_db);
            var fullDetailsOfTile = new TileDetailsViewModel();

            Tile tile = GetDetailsFromDatabase(tileName);

            

            if(tile is null)
            {
                return new TileDetailsViewModel { ShortTileName = "Does Not Exist" };
            }

            if( tile.TileDetail is null)
            {
                tile.TileDetail = new TileDetail();
            }
            if( tile.TileDetail.VariantTiles is null)
            {
                tile.TileDetail.VariantTiles = new List<VariantTile>();
            }
            if (tile.TileImages is null)
            {
                tile.TileImages = new List<TileImage>();
            }

            fullDetailsOfTile.Tile = tile;
            fullDetailsOfTile.Details = tile.TileDetail;
            fullDetailsOfTile.Variants = tile.TileDetail.VariantTiles.ToList();
            fullDetailsOfTile.Images = tile.TileImages.Where(x=>x.ViewName !="Map");

            //As long as there are Variants listed (count>0) then add the needed information to the variant info: TilesetPath Name and TilePath name for link purposes
            
            if(fullDetailsOfTile.Variants.Count()>0)
            {
                foreach(var variant in fullDetailsOfTile.Variants)
                {
                    var varTile = _db.Tiles.Where(x => x.Name == variant.VariantTileName).Include(x=>x.Tileset).FirstOrDefault();

                    variant.TilesetPath = varTile.Tileset.Name;

                    variant.TilePath = varTile.Name.Replace(variant.TilesetPath, "");

                }
            }

            //For editing screen, add blank variantTiles to the list up to 5 or 1+total count

            int totalVariantFields = 5;
            if (fullDetailsOfTile.Variants.Count()>=5)
            {
                totalVariantFields = fullDetailsOfTile.Variants.Count() + 1;
            }
            while(fullDetailsOfTile.Variants.Count() < totalVariantFields)
            {
                fullDetailsOfTile.Variants.Add(new VariantTile { Details = fullDetailsOfTile.Tile.TileDetail, VariantTileName = "" });
            }
            




            var mapPointCollectibles = _db.MapPoints.Where(x => x.Tile.Name == tileName);

            if(mapPointCollectibles.Where(x => x.Ayatan).Any() || tile.TileDetail.Ayatan)
            {
                fullDetailsOfTile.Details.Ayatan = true;
            }
            if (mapPointCollectibles.Where(x => x.CaptureSpawn).Any() || tile.TileDetail.CaptureSpawn)
            {
                fullDetailsOfTile.Details.CaptureSpawn = true;
            }
            if (mapPointCollectibles.Where(x => x.FrameFighter).Any() || tile.TileDetail.FrameFighter)
            {
                fullDetailsOfTile.Details.FrameFighter = true;
            }
            if (mapPointCollectibles.Where(x => x.Medallion).Any() || tile.TileDetail.Medallion)
            {
                fullDetailsOfTile.Details.Medallion= true;
            }
            if (mapPointCollectibles.Where(x => x.RareContainer).Any() || tile.TileDetail.RareContainer)
            {
                fullDetailsOfTile.Details.RareContainer = true;
            }
            if (mapPointCollectibles.Where(x => x.SimarisSpawn).Any() || tile.TileDetail.SimarisSpawn)
            {
                fullDetailsOfTile.Details.SimarisSpawn = true;
            }
            if (mapPointCollectibles.Where(x => x.Somachord).Any() || tile.TileDetail.Somachord)
            {
                fullDetailsOfTile.Details.Somachord = true;
            }
            if (mapPointCollectibles.Where(x => x.Cache).Any() || tile.TileDetail.Cache)
            {
                fullDetailsOfTile.Details.Cache = true;
            }

            bool kuvaInMapPoints = mapPointCollectibles.Where(x => x.Objectives.ToString() == "KuvaSiphon").Any();
            bool mobileDefInMapPoints = mapPointCollectibles.Where(x => x.Objectives.ToString() == "MobileDefense").Any();
            bool defectionSpawnInMapPoints = mapPointCollectibles.Where(x => x.Objectives.ToString() == "DefectionSpawn").Any();
            bool defectionRestInMapPoints = mapPointCollectibles.Where(x => x.Objectives.ToString() == "DefectionRestPoint").Any();
            bool survivalPylonInMapPoints = mapPointCollectibles.Where(x => x.Objectives.ToString() == "SurvivalPylon").Any();
            bool extractorInMapPoints = mapPointCollectibles.Where(x => x.Objectives.ToString() == "Extractor").Any();
            bool sabotageObjectiveInMapPoints = mapPointCollectibles.Where(x => x.Objectives.ToString() == "NonReactorSabotage").Any();

            if (kuvaInMapPoints || tile.TileDetail.KuvaSiphon)
            {
                fullDetailsOfTile.Details.KuvaSiphon = true;
            }
            if (mobileDefInMapPoints || tile.TileDetail.MobileDefense)
            {
                fullDetailsOfTile.Details.MobileDefense = true;
            }
            if (defectionSpawnInMapPoints || tile.TileDetail.DefectionSpawn)
            {
                fullDetailsOfTile.Details.DefectionSpawn = true;
            }
            if (defectionRestInMapPoints || tile.TileDetail.DefectionRest)
            {
                fullDetailsOfTile.Details.DefectionRest = true;
            }
            if (survivalPylonInMapPoints || tile.TileDetail.SurvivalPylon)
            {
                fullDetailsOfTile.Details.SurvivalPylon = true;
            }
            if (extractorInMapPoints || tile.TileDetail.Extractor)
            {
                fullDetailsOfTile.Details.Extractor = true;
            }
            if (sabotageObjectiveInMapPoints || tile.TileDetail.Sabotage)
            {
                fullDetailsOfTile.Details.Sabotage = true;
            }



            foreach (var img in fullDetailsOfTile.Images)
            {
                img.ImagePath = GetImagePath(img.ImageName, tile.Name);
            }

            fullDetailsOfTile.Images = OrderImages(fullDetailsOfTile.Images);

            fullDetailsOfTile.MapImages = tile.TileImages.Where(x => x.ViewName.Contains("Map")).OrderBy(x=>x.ViewName).ToList();

            if (fullDetailsOfTile.MapImages is null || fullDetailsOfTile.MapImages.Count()==0)
            {
                var defaultTileImageData = new TileImage { ImagePath = "LotusFlower.png", AltText = "No Map Image Uploaded Yet", ViewName="No Map Image Uploaded Yet"};

                fullDetailsOfTile.MapImages.Add(defaultTileImageData);
            }
            else
            {
                foreach (var map in fullDetailsOfTile.MapImages)
                {
                    map.ImagePath = GetImagePath(map.ImageName, tile.Name);
                }
            }
            


            return fullDetailsOfTile;
        }

        private Tile GetDetailsFromDatabase(string tileName)
        {
            return _db.Tiles.Where(x => x.Name == tileName)
                .Include(x=>x.Tileset)
                .Include(x => x.TileDetail).ThenInclude(x => x.VariantTiles)
                .Include(x => x.TileImages)
                .FirstOrDefault();

           
        }

        private IEnumerable<TileImage> OrderImages(IEnumerable<TileImage> imageList)
        {
           

            if(imageList.Any() == false)
            {
                return imageList;
            }

            List<TileImage> overviewImagesSorted = SortOverviewImages(imageList);

            List<TileImage> exitImagesSorted = SortExitImages(imageList);

            List<TileImage> secretImagesSorted = SortSecretImages(imageList);

            List<TileImage> lootRoomImagesSorted = SortLootRoomImages(imageList);

            List<TileImage> anyOtherImagesSorted = SortRemainingImages(imageList);

            List<TileImage> allImagesSorted = new List<List<TileImage>>() { overviewImagesSorted, exitImagesSorted, secretImagesSorted, lootRoomImagesSorted, anyOtherImagesSorted, }
                                                                    .SelectMany(x=>x).ToList();
            /*Personal Note - The above is making a List that each object is a list of tile images. So a List of a Lists. Then SelectMany is flattening it into one long list.
             */

            return allImagesSorted;

        }

        private List<TileImage> SortRemainingImages(IEnumerable<TileImage> imageList)
        {
            return imageList.Where(x => !x.ImageName.Contains("Closet") 
                                    && !x.ImageName.Contains("Secret") 
                                    && !x.ImageName.Contains("Exit") 
                                    && !x.ImageName.Contains("Overview") 
                                    && !x.ImageName.Contains("Map")
                                    && !x.ImageName.Contains("Path"))
                            .OrderBy(x => x.ImageName)
                            .ToList();
        }

        private List<TileImage> SortLootRoomImages(IEnumerable<TileImage> imageList)
        {
            return imageList.Where(x => x.ImageName.Contains("Closet")).OrderBy(x => x.ImageName).ToList();
        }

        private List<TileImage> SortSecretImages(IEnumerable<TileImage> imageList)
        {
            return imageList.Where(x => x.ImageName.Contains("Secret") || x.ImageName.Contains("Path")).OrderBy(x => x.ImageName).ToList();
        }

        private List<TileImage> SortExitImages(IEnumerable<TileImage> imageList)
        {
            return imageList.Where(x => x.ImageName.Contains("Exit")).OrderBy(x => x.ImageName).ToList();
        }

        private List<TileImage> SortOverviewImages(IEnumerable<TileImage> imageList)
        {
            return imageList.Where(x => x.ImageName.Contains("Overview")).OrderBy(x => x.ImageName).ToList();

        }



        private string GetImagePath(string imageName, string tileName)
        {
            var tileInformation =_db.Tiles.Where(x => x.Name == tileName).Include(x => x.Tileset).FirstOrDefault();


            return Path.Combine(tileInformation.Tileset.Name, tileInformation.Name, imageName);

        }
    }
}
