using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WFTileCounter.Models;
using WFTileCounter.ModelsLogic;
using WFTileCounter.ModelsView;

namespace WFTileCounter.BuisnessLogic
{
    public class GeneralFunctions
    {

        /* This class holds all my general use logic functions, that are needed between controllers.
         */





        /* I feel like I'm doing this wrong. I only have this hear because I use a function in DatabaseFunctions to check if something exists before I go about with the next step.
         * 
         * So i have to have a Dbcontext to pass to the new DatabaseFunctions constructor.
         * 
         * but ... i feel like there is something i'm missing here.
         * 
         */
        private readonly DatabaseContext _db;


        public GeneralFunctions(DatabaseContext context)
        {
            _db = context;
        }

        /* 
         * This is a Developer function that just grabs straight from my picture directory on the development environment. Does not work outside of that.
         */
        public string DeveloperAutoGrabGetPath()
        {
            //return Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", "Uploads");


            if(Directory.Exists(@"C:\Users\lynkf\Pictures\Warframe"))
            {
                return @"C:\Users\lynkf\Pictures\Warframe";
            }
            else
            {
                return @"C:\Users\lynkf\Desktop\WarframeTilesets\Uploads";
            }
        }


        public string GetPath(ImgMetaData data)
        {
            string userId = GetUserId().ToString();
            string mapID = data.MapIdentifier.Substring(0, data.MapIdentifier.Length - 3);
            string fileName = data.FileName;

            return Path.Combine(Directory.GetCurrentDirectory(), "wwwRoot", "Uploads", userId, mapID, fileName);
        }


        /* Reads a CSV file and returns a list of what DE uses to name their Missions added on to the Tileset Names, and what we're going to call them so its easier to read
         * 
         * it also handles many odd casses, and 'duplicates' by the longer version being first in the list, and the shorter version being later
         */
        


        

        public List<ImgMetaData> GetMetaList(string path)
        {
            List<ImgMetaData> metaList = new List<ImgMetaData>();

            if(string.IsNullOrEmpty(path) || !Directory.Exists(path))
            {
                return null;
            }

            var picList = System.IO.Directory.GetFiles(path);

            

            foreach (var pic in picList)
            {
                var metaData = GetMetaData(pic);

                if(metaData is null) // this will happen if it is not a proper warframe screnshot.
                {
                    continue;

                }else
                {
                    metaData.SetUnknownFlag();
                }

                metaData.FileName = Path.GetFileName(pic);
                
                /* To Do -- Get LogedIn User from TempData? Cookies? Session?
                 * 
                 */
                 
                bool duplicateCheck = metaList.Where(x => x.TileName == metaData.TileName && x.MapIdentifier == metaData.MapIdentifier).Any();
                if(metaList.Count !=0 && duplicateCheck )
                {
                    metaData.SetDuplicateFlags(metaList.Last());
                }
                else
                {
                    metaData.PossibleDupe = false;
                    metaData.KeepThis = true;
                }

                metaList.Add(metaData);
            }



            //now organize the list of images by run - ordering by date also solves the issue if the files are not named Warframe####

            //get the individual mapIdentifiers
            var listOfMissionIdentifiers = metaList.Select(x => x.MapIdentifier).Distinct().ToList();

            if(listOfMissionIdentifiers.Count() == 0)
            {
                return null;

            } else 
            {
                metaList = metaList.OrderBy(x => x.MapIdentifier).ThenBy(x => x.Date).ToList();

                foreach(var identifier in listOfMissionIdentifiers)
                {
                    metaList.Find(x => x.MapIdentifier == identifier).First = true;
                    metaList.Find(x => x.MapIdentifier == identifier).FullRun = true;
                    metaList.Find(x => x.MapIdentifier == identifier).MapPointsRecorded = true;
                }
            }



            return metaList;
        }

        internal List<string> MoveFilesToMapIdDirectory(string uploadPath)
        {
            
            string[] filePaths = Directory.GetFiles(uploadPath);
            bool validWFImage = true;
            string newMapIdDirectoryPath ="";
            List<string> newDirectoryPaths = new List<string>();
            bool atLeastOneValidImageInDirectory = false;
            string userId = GetUserId().ToString();

            foreach(var file in filePaths)
            {


                var directories = MetadataExtractor.ImageMetadataReader.ReadMetadata(file);
                var values = new List<string>();
                var mapInfo = new List<string>();



                foreach (var directory in directories)
                {
                    string mapId = "";
                    if (directory.Name == "JpegComment")
                    {
                        values = directory.Tags[0].Description.Split(new char[] { ' ' }).ToList();

                        //remove all the extra info we don't need
                        values.RemoveAll(x => x == "" || x == "Zone:" || x == "Log:" || x == "P:");
                        if (values.Count == 7)
                        {
                            mapInfo = values[0].Split('/').ToList();

                            if(CheckBadTilesInList(values))
                            {
                                validWFImage = false;
                            }
                            else
                            {
                                mapId = mapInfo.Last().Substring(0, mapInfo.Last().Length - 3);
                                newMapIdDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwRoot", "Uploads", userId, mapId);
                                Directory.CreateDirectory(newMapIdDirectoryPath);
                                validWFImage = true;
                            }
                            
                        }
                        else
                        {
                            validWFImage = false;
                        }
                    }
                }

                if(validWFImage)
                {
                    string newFileNameAndPath = Path.Combine(newMapIdDirectoryPath, Path.GetFileName(file).ToString());
                    if(!File.Exists(newFileNameAndPath))
                    {
                        File.Move(file, newFileNameAndPath);
                        
                        
                    } else //if file exists already
                    {
                        File.Delete(file); //delete it out of the temp uploads directory to clean up.
                    }
                    if (newDirectoryPaths.Count == 0 || !newDirectoryPaths.Contains(newMapIdDirectoryPath))
                    {
                        newDirectoryPaths.Add(newMapIdDirectoryPath);
                    }
                    atLeastOneValidImageInDirectory = true;

                }
            }

            if (atLeastOneValidImageInDirectory)
            {
                return newDirectoryPaths;
            }
            else
            {
                return null;
            }
            

        }

        



        /* Returns a single instance of ImgMetaData of the Metadata from Warframe In Game Screenshot button (f6)
         * 
         */


        public ImgMetaData GetMetaData(string path)
        {
            var metaData = new ImgMetaData();
            var values = new List<string>();
            var directories = MetadataExtractor.ImageMetadataReader.ReadMetadata(path);
            List<string> mapInfo = new List<string>();
            bool validFile = false;

            foreach (var directory in directories)
            {
                if (directory.Name == "JpegComment")
                {
                    values = directory.Tags[0].Description.Split(new char[] { ' ' }).ToList();

                    //remove all the extra info we don't need
                    values.RemoveAll(x => x == "" || x == "Zone:" || x == "Log:" || x == "P:");




                    if (values.Count == 7) // This should be the standard case.
                    {
                        //get the mapInfo out - which is of a varrying size depending on the map. We want the last 3  parts.
                        mapInfo = values[0].Split('/').ToList(); 
                        int validIndex = mapInfo.Count() - 3;
                        if(mapInfo[validIndex]=="SpecialMissions")
                        { // Maroo Treasure Hunt has an extra spot in the string, making the map info 8 long instead of 7. Find it and remove it.
                            mapInfo.Remove("SpecialMissions");
                            validIndex -= 1;
                        }
                        mapInfo.RemoveRange(0, validIndex);

                        //get the tile info out. 
                        string tileNameHolder = values[1].Split('/').ToList().Last();
                        
                        values.RemoveRange(0, 2); // get rid of what we just used



                        //pull out all the coords and put them together in one string.

                        for (int i = 0; i < 4; i++)
                        {
                            if (i == 3)
                            {
                                metaData.Coords += " | " + values[0];
                            }
                            else
                            {
                                metaData.Coords += values[0];
                            }

                            values.RemoveAt(0);
                        }


                        //saving the log number, for possible debuging?
                        metaData.LogNum = values[0];
                        // nothing else we need, so clear the list just to be safe.
                        values.Clear();

                        metaData.FactionName = GetFactionName(mapInfo[0]);
                        metaData.MissionType = GetMissionType(mapInfo[1]);
                        metaData.Tileset = GetTileSet(mapInfo[1]);

                        if(string.IsNullOrEmpty(metaData.Tileset))
                        { // the only way that tileset will be null is if the tileset is one listed in the Non ProcedualSets list. if its null, then this is not a good tile, and we're tossing it.
                            return null;
                        }
                        int mapIdLength = mapInfo[2].Length;
                        metaData.MapIdentifier = mapInfo[2].Substring(0, mapIdLength - 3); // last 3 are .lp which we don't need for any purpose that I can see yet.
                        metaData.TileName = GetTileName(tileNameHolder, metaData.Tileset);
                        mapInfo.Clear();
                        metaData.TileImageInfo = GetMapImagePath(metaData.TileName);


                        //Special Cases
                        if(metaData.Tileset=="GrineerFortress")
                        {
                            metaData.FactionName = "GrineerQueens";
                        }
                        if (metaData.Tileset == "OrokinDerelict")
                        {
                            metaData.FactionName = "Infested";
                        }


                        //get the new imagepath for where it will be stored on the server
                        metaData.FileName = Path.GetFileName(path);
                        string userId = GetUserId().ToString();
                        string screenShotImagePath = Path.Combine("Uploads", userId, metaData.MapIdentifier, metaData.FileName);
                        metaData.UploadedScreenshotImagePath = screenShotImagePath;


                        validFile = true;

                    }
                    else  //Covers arena maps and other non Proceduals. Also covers if the file has a JpegComment but not formated like warframes. Not a warframe image then!
                    {
                        return null;
                        
                    }
                    
                }

                //find the date of the file and add it.
                if (directory.Name == "File")
                {
                    metaData.Date = directory.Tags[2].Description;
                }


            }
            if(validFile)
            {
                return metaData;
            } else // This should catch all instances of not Warframe files
            {
                return null;
            }
            
        }
        

        private string GetFactionName(string possFaction)
        {
            if(possFaction == "SpaceBattles")
            {
                return "Corpus";

            }else if (possFaction == "Space")
            {
                return "Grineer";

            } 
            else if (possFaction =="Transitional")
            {
                return "Invasion";
            }
            else
            {
                return possFaction;
            }

        }

        private string GetTileName(string tilename, string tileset)
        {

            

            if(tileset == "CorpusGasCity")
            {
                tilename = tilename.Replace("CorpusGasCity", "");
                tilename = tilename.Replace("CrpGasCity", "");
                tilename = tilename.Replace("GasCity", "");
                tilename = tilename.Replace("CorpusGas", "");
                tilename = tilename.Replace("City", "");
                return "CorpusGas" + tilename;
            }else if (tileset == "CorpusArchwing")
            {
                tilename = tilename.Replace("TR", "");
                tilename = tilename.Replace("CorpusArchwing", "");
                return "CorpusArchwing" + tilename;
            }
            else if (tileset == "CorpusIcePlanet")
            {
                tilename = tilename.Replace("CorpusIcePLanet", "");
                tilename = tilename.Replace("IcePlanet", "");
                tilename = tilename.Replace("CrpIce", "");
                tilename = tilename.Replace("Ice", "");
                return "CorpusIce" + tilename;
            }
            else if (tileset == "CorpusOutpost")
            {
                tilename = tilename.Replace("CorpusOutpost", "");
                tilename = tilename.Replace("CrpOutpost", "");
                tilename = tilename.Replace("Outpost", "");
                return "CorpusOutpost" + tilename;
            }
            else if (tileset == "CorpusShip")
            {
                tilename = tilename.Replace("GuildShip", "");
                tilename = tilename.Replace("CrpShip", "");
                tilename = tilename.Replace("Crp", "");
                tilename = tilename.Replace("Ship", "");
                tilename = tilename.Replace("Guid", "");
                return "CorpusShip" + tilename;
            }
            else if (tileset == "CorpusToGrineer" && tilename.Contains("InvasionC2G"))
            {
                return tilename;
            }
            else if (tileset == "CorpusToGrineer" && !tilename.Contains("InvasionC2G"))
            {
                return "InvasionC2G" + tilename;
            }
            else if (tileset == "GrineerArchwing")
            {
                tilename = tilename.Replace("Spline", "");
                return "GrineerArchwing" + tilename;
            }
            else if (tileset == "GrineerAsteroid")
            {
                tilename = tilename.Replace("GrineerAsteroid", "");
                tilename = tilename.Replace("GrnAsteroid", "");
                tilename = tilename.Replace("Asteroid", "");
                tilename = tilename.Replace("Grn", "");
                return "GrineerAsteroid" + tilename;
            }
            else if (tileset == "GrineerForest")
            {
                tilename = tilename.Replace("GftRemastered", "");
                tilename = tilename.Replace("GrineerForest", "");
                tilename = tilename.Replace("GrnForest", "");
                tilename = tilename.Replace("Forest", "");
                tilename = tilename.Replace("Grn", "");
                tilename = tilename.Replace("Gft", "");
                return "GrineerForest" + tilename;
            }
            else if (tileset == "GrineerFortress")
            {
                tilename = tilename.Replace("GrineerFortress", "");
                tilename = tilename.Replace("GrineerFort", "");
                tilename = tilename.Replace("GrnFortress", "");
                tilename = tilename.Replace("GrnFort", "");
                tilename = tilename.Replace("Fortress", "");
                tilename = tilename.Replace("Fort", "");
                tilename = tilename.Replace("Grn", "");
                return "GrineerFortress" + tilename;
            }
            else if (tileset == "GrineerGalleon")
            {
                tilename = tilename.Replace("GrineerGalleon", "");
                tilename = tilename.Replace("GrnGalleon", "");
                tilename = tilename.Replace("Grineer", "");
                tilename = tilename.Replace("Galleon", "");
                tilename = tilename.Replace("Grn", "");
                return "GrineerGalleon" + tilename;
            }
            else if (tileset == "GrineerOcean")
            {
                tilename = tilename.Replace("GrineerOcean", "");
                tilename = tilename.Replace("GrnOcean", "");
                tilename = tilename.Replace("Ocean", "");
                tilename = tilename.Replace("Grineer", "");
                tilename = tilename.Replace("Grn", "");
                return "GrineerOcean" + tilename;
            }
            else if (tileset == "GrineerSettlement")
            {
                tilename = tilename.Replace("GrineerSettlement", "");
                tilename = tilename.Replace("GrineerCamp", "");
                tilename = tilename.Replace("GrinnerCmp", "");
                tilename = tilename.Replace("GrnSettlement", "");
                tilename = tilename.Replace("GrnCamp", "");
                tilename = tilename.Replace("GrnCmp", "");
                tilename = tilename.Replace("Settlment", "");
                tilename = tilename.Replace("Camp", "");
                tilename = tilename.Replace("Grineer", "");
                tilename = tilename.Replace("Grn", "");
                return "GrineerSettlement" + tilename;
                
            }
            else if (tileset == "GrineerShipyards")
            {
                tilename = tilename.Replace("GrineerShipyards", "");
                tilename = tilename.Replace("GrnShipyards", "");
                tilename = tilename.Replace("Shipyards", "");
                tilename = tilename.Replace("Grineer", "");
                tilename = tilename.Replace("Grn", "");
                return "Grineer" + tilename;
            }
            else if (tileset == "GrineerToCorpus" && tilename.Contains("InvasionG2C"))
            {
                return tilename;
            }
            else if (tileset == "GrineerToCorpus" && !tilename.Contains("InvasionG2C"))
            {
                return "InvasionG2C" + tilename;
            }
            else if (tileset == "InfestedCorpusShip")
            {
                tilename = tilename.Replace("InfestedCorpusShip", "");
                tilename = tilename.Replace("CorpusShip", "");
                tilename = tilename.Replace("Infested", "");
                return "Infested"+tilename;
            }
            
            else if (tileset == "OrokinMoon")
            {
                tilename = tilename.Replace("OrokinMoon", "");
                tilename = tilename.Replace("OrokinLua", "");
                tilename = tilename.Replace("Orokin", "");
                tilename = tilename.Replace("Lua", "");
                tilename = tilename.Replace("Moon", "");
                return "OrokinMoon"+tilename;
            }
            else if (tileset == "OrokinTower")
            {
                tilename = tilename.Replace("OrokinTower", "");
                tilename = tilename.Replace("Orokin", "");
                tilename = tilename.Replace("Tower", "");
                return "OrokinTower" + tilename;
            }
            else if (tileset == "OrokinTowerDerelict")
            {
                tilename = tilename.Replace("OrokinTowerDerelict", "");
                tilename = tilename.Replace("OrokinTower", "");
                tilename = tilename.Replace("OrokinDerelict", "");
                tilename = tilename.Replace("TowerDerelict", "");
                tilename = tilename.Replace("Orokin", "");
                tilename = tilename.Replace("Tower", "");
                tilename = tilename.Replace("Derelict", "");
                return "OrokinDerelict" + tilename;
                
                
            }
            


            return "??? tset: " +tileset + "tile: " + tilename;
            
        }

        public List<MissionType> GenMissionList()
        {
            List<MissionType> list = new List<MissionType>();


            var path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", "lib", "lists", "wftMissionNames.csv");


            if (!File.Exists(path))
            {
                //To Do - Throw proper Exception
                return null;

            }
            else
            {

                string[] namePairs = File.ReadAllLines(path);
                foreach (var line in namePairs)
                {
                    var mission = new MissionType();
                    string[] splitPairs = line.Split(',');
                    mission.CommonName = splitPairs[1];
                    mission.InGameName = splitPairs[0];
                    list.Add(mission);
                }
            }



            return list;
        }


        public string GetMissionType(string value)
        {

            string name;

            var types = GenMissionList();
            if (types is null)
            {
                // this... is very wrong. gotta fix this.
                throw new Exception();
            }

            //These two have a bad habit of NOT having the mission type on the TilesetName like the rest do. SO.. special case.
            if (value == "CorpusShip")
            {
                return "Assassination (The Sergeant)";
            }
            if (value == "GrineerAsteroid")
            {
                return "Sabotage (Drill)";
            }

            foreach (var type in types)
            {
                if (value.Contains(type.InGameName))
                {
                    name = type.CommonName;


                    return name;
                }

            }


            return value + "??? - Check Me: " +value;

        }



        /* This function checks against a list of values that might appear (from nonProcedualSets) that correspond with Hubs or Open Worlds. if it finds the value passed too it
         * equals that it returns an exception.
         * 
         * It then checks the value against a list of words that will be cut off to leave it with just the Tileset (because this value will be some string of TilesetMissionType,
         * like GrineerForestRescue
         * 
         * it cuts that amount off and returns the truncated string
         */
        public string GetTileSet(string stringTilesetMission)
        {
            string[] validTilesetNames;
            var _df = new DatabaseFunctions(_db);

            var path = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", "lib", "lists", "wftTilesetNames.csv");



            if (!File.Exists(path))
            {
                return null; //needs proper exception handling

            }
            else
            {

                validTilesetNames = File.ReadAllLines(path);

            }


            if(CheckBadTilesSingle(stringTilesetMission))
            {
                return null;
            }
            /*
            path = Path.Combine(
                             System.IO.Directory.GetCurrentDirectory(), "wwwroot", "lib", "lists", "nonProcedualSets.csv");


            if (!File.Exists(path))
            {
                return null;

            }
            else
            {

                notValidTilesetPossibles = File.ReadAllLines(path);

            }

            // all the 'Bad Sets' --should-- be caught by the return null in GetMetaData function, as they should have a different count parsed out from the JpegComment.
            // but just in case, we put this here to catch any we missed.
            for (int i = 0; i < notValidTilesetPossibles.Length; i++)
            {
                string badPic = "*" + notValidTilesetPossibles[i] + "*";

                if (Regex.IsMatch(stringTilesetMission, WildCardToRegular(badPic)))
                {
                    return null;
                }
            }
            */


            int length = stringTilesetMission.Length;

            if (_df.CheckTilesetExists(stringTilesetMission)) // checks the database to see if the Tileset already exists. Mostly for the two edge cases from Missions that don't have a Mission Type
            {
                return stringTilesetMission;
            }


            if (stringTilesetMission.Contains("TR")) // special case, just get it over with.
            {
                return "CorpusArchwing";
            }
            if (stringTilesetMission.Contains("Space"))
            {
                return "GrineerArchwing";
            }
            if (stringTilesetMission == "CorpusGasBoss")
            {
                return "CorpusGasCity";
            }
            

            foreach(var tileset in validTilesetNames)
            {
                if(stringTilesetMission.Contains(tileset))
                {
                    return tileset;
                }
            }

            //else return the whole thing so we can see what it is for future iterations
            return stringTilesetMission + " ??? Check Me!";
        }

        private bool CheckBadTilesInList(List<string> values)
        {
            foreach(var metaData in values)
            {
                if(CheckBadTilesSingle(metaData))
                {
                    return true;
                }
            }
            return false;
        }

        private bool CheckBadTilesSingle(string stringTilesetMission)
        {
            string[] notValidTilesetPossibles;
            var path = Path.Combine(
                             System.IO.Directory.GetCurrentDirectory(), "wwwroot", "lib", "lists", "nonProcedualSets.csv");


            if (!File.Exists(path))//if the file dissapears we're just going to return true and not let a single file go through... need an exception here.
            {
                return true;

            }
            else
            {

                notValidTilesetPossibles = File.ReadAllLines(path);

            }

            // all the 'Bad Sets' --should-- be caught by the return null in GetMetaData function, as they should have a different count parsed out from the JpegComment.
            // but just in case, we put this here to catch any we missed.
            for (int i = 0; i < notValidTilesetPossibles.Length; i++)
            {
                string badPic = "*" + notValidTilesetPossibles[i] + "*";

                if (Regex.IsMatch(stringTilesetMission, WildCardToRegular(badPic)))
                {
                    return true;
                }
                
            }
            return false;
        }


        public TileImage GetMapImagePath(string tileName)
        {
            var isTileInDB = _db.Tiles.Where(x => x.Name == tileName).Include(x => x.Tileset).Include(x=>x.TileImages).FirstOrDefault();


            var defaultTileImageData = new TileImage { ImagePath = "LotusFlower.png", AltText = "No Map Image Uploaded Yet" };


            if (isTileInDB is null)
                return defaultTileImageData;
            else
            {
                var doesTileHaveImg = _db.TileImages.Where(x => x.Tile == isTileInDB);

                if (doesTileHaveImg.Count() > 0)
                {
                    var tileMapImg = isTileInDB.TileImages.Where(x => x.ViewName == "Map").FirstOrDefault();
                    if (tileMapImg is null)
                    {

                        return defaultTileImageData;
                    }
                    else
                    {

                        tileMapImg.ImagePath = isTileInDB.Tileset.Name + "/" + isTileInDB.Name + "/" + tileMapImg.ImageName;
                        return tileMapImg;
                    }
                }
                else
                {
                    return defaultTileImageData;
                }

            }
        }



        public int GetUserId()
        {
            //return = ViewData["UserID"].ToString();  -- temp data got get this to work?
            return 1;
        }



        //quick function for * wildcard search.
        private static String WildCardToRegular(String value)
        {
            return "^" + Regex.Escape(value).Replace("\\*", ".*") + "$";
        }
    }

}
