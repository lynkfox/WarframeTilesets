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

namespace WFTileCounter.ControllersProcessing
{

    public class ProcessController : Controller
    {

        private readonly DatabaseContext _db; //database context shortcut
        

        public ProcessController(DatabaseContext context)
        {
            _db = context;
        }


        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ProcessFiles()
        {
            ICollection<MetaProcessed> metaList = new List<MetaProcessed>();

            var path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", "Uploads");

            var picList = System.IO.Directory.GetFiles(path);

            foreach (var pic in picList)
            {
                var proc = new MetaProcessed();

                var metaValues = GetMetaData(pic);
                
                string[] pathCut = pic.Split('\\');
                proc.FileName = pathCut[pathCut.Length - 1];
                proc.Date = metaValues.Last();
                proc.Values = metaValues;
                proc.MapIdentifier = metaValues[0];
                proc.MissionType = metaValues[1];
                proc.Tileset = metaValues[2];
                proc.FactionName = metaValues[3];
                proc.TileName = metaValues[4];

                

                metaList.Add(proc);
            }

            



            return View("ProcessFiles", metaList);
        }

        




        /* Returns a list of the Metadata from Warframe In Game Screenshot button (f6)
         * 
         * Index   -    Value
         * 0       -    Map Identifier String
         * 1       -    MissionType
         * 2       -    Tileset
         * 3       -    Faction Name
         * 4       -    TileName (internal)
         * 5       -    Date of file last modified
         */


        public List<string> GetMetaData(string path)
        {

            var values = new List<string>();
            var directories = ImageMetadataReader.ReadMetadata(path);
            string coords = "P: ";
            List<string> tileInfo = new List<string>();
            List<string> mapInfo = new List<string>();
            var _gf = new GeneralFunctions();

            foreach (var directory in directories)
            {
                if(directory.Name == "JpegComment")
                {
                    values = directory.Tags[0].Description.Split(new char[] { ' ' }).ToList();

                    //remove all the extra info we don't need
                    values.RemoveAll(x => x == "" || x == "Zone:" || x == "Log:" || x== "P:");


                    if(values.Count == 7) // This should be the standard case.
                    {
                        //get the mapInfo out - which is of a varrying size dependingon the map. We want the last 3  parts.
                        mapInfo = values[0].Split('/').ToList();
                        //remove it from the values list
                        values.RemoveAt(0);

                        //get the tile info out. Which should all be the same size, but just in case, we only want the last part anyways
                        tileInfo = values[0].Split('/').ToList();
                        values.RemoveAt(0);

                        //pull out all the coords and put them together in one string.

                        for (int i = 0; i < 4; i++)
                        {
                            if (i == 3)
                            {
                                coords += " | " + values[0];
                            }
                            else
                            {
                                coords += values[0];
                            }

                            values.RemoveAt(0);
                        }

                        //all that should be left is the Log Number which we don't need for anything. (internal logs i think) so toss it.
                        // and just in case, we'll make sure we clear it out.
                        values.Clear();
                        

                        //Add the MapIdentifier string, and toss it out.
                        values.Add(mapInfo.Last());
                        mapInfo.RemoveAt(mapInfo.Count - 1);
                        //add the Mission Type + Tileset, and toss it out
                        values.Add(_gf.GetMissionType(mapInfo.Last()));
                        values.Add(_gf.GetTileSet(mapInfo.Last()));
                        
                        mapInfo.RemoveAt(mapInfo.Count - 1);
                        //add the Faction info, with a few special cases.
                        if (mapInfo.Last() == "SpaceBattles")
                            values.Add("Corpus");
                        else if (mapInfo.Last() == "Space")
                            values.Add("Grineer");
                        else
                            values.Add(mapInfo.Last());

                        mapInfo.RemoveAt(mapInfo.Count - 1);
                        //add the tile name. - toss it in case I need the rest of the string...
                        values.Add(tileInfo.Last());
                        tileInfo.RemoveAt(tileInfo.Count - 1);
                        //add the coords.
                        values.Add(coords);
                    }
                    else if(values.Count == 6) // Arena's Special Case 
                    {
                        //get the tile information.
                        mapInfo  = values[0].Split('/').ToList();
                        values.RemoveAt(0);

                        for (int i = 0; i < 4; i++)
                        {
                            if (i == 3)
                            {
                                coords += " | " + values[0];
                            }
                            else
                            {
                                coords += values[0];
                            }

                            values.RemoveAt(0);
                        }
                        //nothing left in these cases
                        values.Clear();

                        

                        //Add a custom MapIdentifier
                        values.Add("ArenaFight");
                        //Save the Tile Name, then toss it out to get to the next bit that we need.
                        var tileName = mapInfo.Last();
                        mapInfo.RemoveAt(mapInfo.Count - 1);
                        //add the Mission Type + Tileset - not toss, we still need it.

                        var arenaType = _gf.GetMissionType(mapInfo.Last());
                        values.Add(arenaType);
                        values.Add(arenaType); // Tileset is Rathuum or Index and th is the same as the Mission Type.
                        if (arenaType == "Index")
                            values.Add("Nef Anyo");
                        else if (arenaType == "Rathuum")
                            values.Add("Kela de Thaym");
                        mapInfo.Clear(); //clear this list
                        //Add the 'Tile Name' ... which is just the MapIdentifier again.
                        values.Add(tileName);
                        values.Add(coords);
                    } else
                    {// not a valid file then, doesn't have the meta dat we need, toss it. ... i hope.
                        values = null;
                    }
                    



                }

                //find the date of the file and add it.
                if(directory.Name == "File")
                {
                    string date = directory.Tags[2].Description;
                    values.Add(date);
                }

               

                if (directory.HasError)
                {
                    foreach (var error in directory.Errors)
                        Console.WriteLine($"ERROR: {error}");
                }
            }

            

            return values;
        }
    }
}

