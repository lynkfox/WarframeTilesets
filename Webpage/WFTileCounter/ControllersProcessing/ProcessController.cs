using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MetadataExtractor;
using System.IO;
using WFTileCounter.ModelsLogic;
using WFTileCounter.ModelsView;
using WFTileCounter.Models;
using Newtonsoft.Json;
using System.Diagnostics;

namespace WFTileCounter.ControllersProcessing
{

    /*This Controller is mostly used at the moment for testing purposes. - It has everything needed to parse, display, and insert the data into the database.
     * 
     * However, future release iterations I want a lot of that more seperate, or more streamlined. When I proceed to actually taking Uploaded images, storing them,
     * and processsing their meta data, this controller will be depreciated for a more accurately named, and more streamlined version
     * 
     */


    public class ProcessController : Controller
    {

        private readonly DatabaseContext _db; //database context shortcut
        

        public ProcessController(DatabaseContext context)
        {
            _db = context;
        }


        public async Task<IActionResult> DeveloperSkip()
        {

            /* Developer Environment Only Link that skips the filter check and will upload everything without checking the checkboxes*/

            var _gf = new GeneralFunctions(_db); // class that holds various methods for clean use.
            var _df = new DatabaseFunctions(_db); // class that holds various database methods for clean use

            List<ImgMetaData> metaList = new List<ImgMetaData>();
            var path = _gf.GetPath();
            metaList = _gf.GetMetaList(path);

            List<InsertReadyData> insert = _df.ConvertToDatabase(metaList);

            ViewBag.newTiles  = await _df.InsertIntoDatabase(insert);

            return View("Success", insert);
        }

        

        public IActionResult ProcessFiles()
        {
            List<ImgMetaData> metaList = new List<ImgMetaData>();

            var _gf = new GeneralFunctions(_db); // class that holds various methods for clean use.

            var path = _gf.GetPath(); // get the path to the temp folder where all the images are stored waiting to be processed
            metaList = _gf.GetMetaList(path); //proces the images for their MetaData, storing it in the ImgMetaData model




            //Move this next block of code into a Method  --- and into the method that returns the list of ImgMetaData - get rid of the MultipleMapIdentifiers and add a Bool 'First' to each ImgData list, as well as the 'Partial Run' then check both

            List<MultipleMapIdentifiers> distinctMapWithTiles = new List<MultipleMapIdentifiers>();

            //Find out if there is more than one run in this upload
            var listOfMissionIdentifiers = metaList.Select(x => new { x.MapIdentifier }).Distinct().ToList();

            if(listOfMissionIdentifiers.Count() == 0)
            { // nothing uploaded, return error view
                return View("NoData");
            }else if(listOfMissionIdentifiers.Count() > 1)
            {
                //if there is more than one run, seperate all the images out into individual runs
                foreach(var mapID in listOfMissionIdentifiers)
                {
                    var oneMapSet = new MultipleMapIdentifiers();
                    List<ImgMetaData> singleMapImgsList = new List<ImgMetaData>();

                    int mapIDlength = mapID.ToString().Length; // the distinct LINQ procedure adds the column name in there, so we're taking it out.
                    oneMapSet.MapIdentifier = mapID.ToString().Substring(18, (mapIDlength - 20)); 
                    foreach (var tileImg in metaList)
                    {
                        if(tileImg.MapIdentifier == oneMapSet.MapIdentifier)
                        {
                            singleMapImgsList.Add(tileImg);
                        }
                    }

                    oneMapSet.ImgMetaDatas = singleMapImgsList;

                    distinctMapWithTiles.Add(oneMapSet);

                }
            }else
            {
                var singleMap = new MultipleMapIdentifiers();
                singleMap.MapIdentifier = listOfMissionIdentifiers.First().ToString();
                singleMap.ImgMetaDatas = metaList;
                distinctMapWithTiles.Add(singleMap);
            }







            return View("Review", distinctMapWithTiles);

        }

        
        [HttpPost]
        public async Task<IActionResult> Keep(List<MultipleMapIdentifiers> datas)
        {
            var _gf = new GeneralFunctions(_db); // class that holds various methods for clean use.
            var _df = new DatabaseFunctions(_db); // class that holds various database methods for clean use
            List<ImgMetaData> keepTheseTiles = new List<ImgMetaData>();
            List<InsertReadyData> allMapsAllTiles = new List<InsertReadyData>();

            foreach(var map in datas)
            {
                foreach (var img in map.ImgMetaDatas)
                {
                    if (img.UnknownValue) //If we get the UnknownValue flag set to true, then there is bad data in the process. Make sure this tile is NOT processed
                        img.KeepThis = false;

                    if (img.KeepThis)
                    {
                        keepTheseTiles.Add(img);
                    }
                    //Debug.WriteLine("Tile Name: " + piece.FileName + " Keep? : " + piece.KeepThis);
                }

                if (keepTheseTiles.Count() == 0)
                {
                    continue;
                }
                else
                {
                    List<InsertReadyData> singleMapList = _df.ConvertToDatabase(keepTheseTiles);

                    allMapsAllTiles.AddRange(singleMapList);
                }
            }

            if(allMapsAllTiles.Count==0)
            {
                return View("NoData");
            }

            ViewBag.newTiles = await _df.InsertIntoDatabase(allMapsAllTiles);

            return View("Success", allMapsAllTiles);
        }
    }
}

