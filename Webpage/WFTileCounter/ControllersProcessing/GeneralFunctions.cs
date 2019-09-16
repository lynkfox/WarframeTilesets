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

namespace WFTileCounter.ControllersProcessing
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

        /* Temp function for use between multiple computers, but will need to be made dynamic for a server.
         * 
         * If you are Running this on your own before I've reached a release stage, then you need to change path below to where your Warframe Pictures directory is.
         * 
         * To Do: Change to GetPath(string tempName) to generate the path of the temp folder that is created when a user uploads pictures
         */
        public string GetPath()
        {
            //return Path.Combine(System.IO.Directory.GetCurrentDirectory(), "wwwroot", "Uploads");


            return @"C:\Users\lynkf\Pictures\Warframe";
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

            var picList = System.IO.Directory.GetFiles(path);

            

            foreach (var pic in picList)
            {
                var metaData = GetMetaData(pic);

                if(metaData is null)
                {
                    continue;

                }else if(metaData.MissionType.Contains("???") || metaData.Tileset.Contains("???"))
                {
                    //if the MissionType or the Tileset hits the final return, it will add ??? to the name. - this is usually a bad image, or something new that hasn't been added to special cases yet
                    // so we check for that so we can auto exclude files that don't have the proper checks for their MissionType/Tileset
                    metaData.KeepThis = false;
                    metaData.UnknownValue = true;
                }
                else
                {
                    metaData.UnknownValue = false;
                }

                metaData.FileName = Path.GetFileName(pic);
                

                /* To Do -- Get LogedIn User from TempData? Cookies? Session?
                 * 
                 */
                
                string duplicateCheck = metaList.Where(x => x.TileName == metaData.TileName && x.MapIdentifier == metaData.MapIdentifier).Select(x => x.TileName).FirstOrDefault();
                if(metaList.Count !=0 && !string.IsNullOrEmpty(duplicateCheck) )
                {

                    if (metaData.TileName.Contains("Capture"))
                    {
                        metaData.KeepThis = false;
                        metaData.PossibleDupe = true;
                    }
                    else if (metaData.TileName.Contains("DeadEnd") || metaData.TileName.Contains("Cap") || metaData.TileName.Contains("Closet") ||metaData.TileName.Contains("Loot"))
                    {
                        metaData.PossibleDupe = false;
                        metaData.KeepThis = true;
                    }
                    else
                    {
                        metaData.KeepThis = false;
                        metaData.PossibleDupe = true;
                    }
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
            bool atLeastOneValidImageMoved = false;
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
                            mapId = mapInfo.Last().Substring(0,mapInfo.Last().Length - 3);
                            // needs to use UserID eventually...
                            newMapIdDirectoryPath = Path.Combine(Directory.GetCurrentDirectory(),"wwwRoot", "Uploads", userId, mapId);
                            Directory.CreateDirectory(newMapIdDirectoryPath);
                            validWFImage = true;
                        }
                        else
                        { validWFImage = false;  }
                    }
                }

                if(validWFImage)
                {
                    string newFileNameAndPath = Path.Combine(newMapIdDirectoryPath, Path.GetFileName(file).ToString());
                    File.Move(file, newFileNameAndPath);
                    if(newDirectoryPaths.Count == 0 || !newDirectoryPaths.Contains(newMapIdDirectoryPath))
                    {
                        newDirectoryPaths.Add(newMapIdDirectoryPath);
                    }
                    atLeastOneValidImageMoved = true;
                }
            }

            if (atLeastOneValidImageMoved)
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
                        metaData.MapIdentifier = mapInfo[2];
                        metaData.TileName = GetTileName(metaData.FactionName, tileNameHolder, metaData.Tileset);
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



                        string[] tilesetNames = CheckDuplicateTileDiffTileset(metaData.TileName, metaData.Tileset);

                        metaData.Tileset = tilesetNames[0];
                        metaData.AlternateTileset = tilesetNames[1];

                        // The Spy Vaults are identical with the same names across many different tilesets for corpus and grineer.
                        // Lua Spy Vaults are unique to lua, no need to change them
                        // Kuva Spyvaults are unique to Grineer Fortress, and we're using Kuva for their faction name.
                        // we're overriding whatever the CheckDuplicateTileDifTileset might have set here, on purpose.
                        if (metaData.TileName.Contains("SpyVault") && ( metaData.FactionName =="Grineer" || metaData.FactionName=="Corpus" )) 
                        {
                            metaData.AlternateTileset = metaData.FactionName;
                            metaData.Tileset = metaData.FactionName + "SpyVault";
                        }


                        

                        

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

        private string GetTileName(string faction, string tilename, string tileset)
        {

            string threeLtrQualifier = tilename.Substring(0, 3);

            if(tileset == "CorpusGasCity" && threeLtrQualifier == "Gas")
            {
                return "Corpus" + tilename;
            }
            else if (tileset == "CorpusGasCity" && threeLtrQualifier != "Gas")
            {
                return "CorpusGas" + tilename;
            } else if (tileset == "CorpusArchwing" && threeLtrQualifier.Substring(0,2) == "TR")
            {
                string adjustedTileName = tilename.Substring(2);
                return "CorpusArchwing" + tilename;
            }
            else if (tileset == "CorpusArchwing" && threeLtrQualifier.Substring(0, 2) != "TR")
            {
                return "CorpusArchwing" + tilename;
            }
            else if (tileset == "CorpusIcePlanet" && threeLtrQualifier == "Ice")
            {
                return "Corpus" + tilename;
            }
            else if (tileset == "CorpusIcePlanet" && threeLtrQualifier != "Ice")
            {
                return "CorpusIce" + tilename;
            }
            else if (tileset == "CorpusOutpost" && tilename.Contains("Outpost"))
            {
                return "Corpus" + tilename;
            }
            else if (tileset == "CorpusOutpost" && !tilename.Contains("Outpost"))
            {
                if(threeLtrQualifier == "Crp")
                {
                    string adjustedTileName = tilename.Substring(3);
                    return "CorpusOutpost" + adjustedTileName;
                }
                return "CorpusOutpost" + tilename;
            }
            else if (tileset == "CorpusShip" && threeLtrQualifier == "Crp")
            {
                string adjustedTileName = tilename.Substring(3);
                return "CorpusShip" + adjustedTileName;
            }
            else if (tileset == "CorpusShip" && threeLtrQualifier != "Crp")
            {
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
            else if (tileset == "GrineerArchwing" && tilename.Contains("Spline"))
            {
                int tileSetLength = "Spline".Length;
                string adjustedTileName = tilename.Substring(tileSetLength);
                return "GrineerArchwing" + adjustedTileName;
            }
            else if (tileset == "GrineerArchwing" && !tilename.Contains("Spline"))
            {
                return "GrineerArchwing" + tilename;
            }
            else if (tileset == "GrineerAsteroid" && threeLtrQualifier == "Grn")
            {
                string adjustedTileName = tilename.Substring(3);
                return "GrineerAsteroid" + adjustedTileName;
            }
            else if (tileset == "GrineerAsteroid" && threeLtrQualifier != "Grn")
            {
                return "GrineerAsteroid" + tilename;
            }
            else if (tileset == "GrineerForest" && threeLtrQualifier == "Gft")
            {
                string adjustedTileName = tilename.Substring(3);
                return "GrineerForest" + adjustedTileName;
            }
            else if (tileset == "GrineerForest" && threeLtrQualifier == "Gft")
            {
                return "GrineerForest" + tilename;
            }
            else if (tileset == "GrineerFortress" && tilename.Contains("Fort"))
            {
                string adjustedTileName = tilename.Substring(4);
                return "GrineerFortress" + adjustedTileName;
            }
            else if (tileset == "GrineerFortress" && threeLtrQualifier != "For")
            {
                return "GrineerFortress" + tilename;
            }
            else if (tileset == "GrineerGalleon" && tilename.Contains("Galleon"))
            {
                return "Grineer" + tilename;
            }
            else if (tileset == "GrineerGalleon" && !tilename.Contains("Galleon"))
            {
                if(threeLtrQualifier == "Grn")
                {
                    string adjustedTileName = tilename.Substring(3);
                    return "GrineerGalleon" + adjustedTileName;
                }
                return "GrineerGalleon" + tilename;
            }
            else if (tileset == "GrineerOcean" && tilename.Contains("GrineerOcean"))
            {
                return tilename;
            }
            else if (tileset == "GrineerOcean" && !tilename.Contains("GrineerOcean"))
            {
                return "GrineerOcean" + tilename;
            }
            else if (tileset == "GrineerSettlement" && threeLtrQualifier == "Cmp")
            {
                string adjustedTileName = tilename.Substring(3);
                return "GrineerSettlement" + tilename;
            }
            else if (tileset == "GrineerSettlement" && threeLtrQualifier != "Cmp")
            {
                if(tilename.Contains("GrineerSettlement"))
                {
                    return tilename;
                }
                else
                {
                    return "GrineerSettlement" + tilename;
                }
            }
            else if (tileset == "GrineerShipyard" && tilename.Contains("Shipyards"))
            {
                return "Grineer" + tilename;
            }
            else if (tileset == "GrineerShipyard" && !tilename.Contains("Shipyards"))
            {
                return "GrineerShipyards" + tilename;
            }
            else if (tileset == "GrineerToCorpus" && tilename.Contains("InvasionG2C"))
            {
                return tilename;
            }
            else if (tileset == "GrineerToCorpus" && !tilename.Contains("InvasionG2C"))
            {
                return "InvasionG2C" + tilename;
            }
            else if (tileset == "InfestedCorpusShip" && tilename.Contains("Infested"))
            {
                return tilename;
            }
            else if (tileset == "InfestedCorpusShip" && !tilename.Contains("Infested"))
            {
                return "Infested" + tilename;
            }
            else if (tileset == "OrokinMoon" && tilename.Contains("Moon"))
            {
                return "Orokin"+tilename;
            }
            else if (tileset == "OrokinMoon" && !tilename.Contains("Moon"))
            {
                return "OrokinMoon" + tilename;
            }
            else if (tileset == "OrokinTower" && tilename.Contains("OrokinTower"))
            {
                return tilename;
            }
            else if (tileset == "OrokinTower" && !tilename.Contains("OrokinTower"))
            {
                return "OrokinTower" + tilename;
            }
            else if (tileset == "OrokinTowerDerelict" && tilename.Contains("Derelict"))
            {
                int tileNameLength = tilename.Length;
                int lengthOfDerelictWord = "Derelict".Length;
                if(tilename.Substring(tileNameLength-lengthOfDerelictWord) == "Derelict") // if the end of the tilename has Derelict on it already, we're going to move it to the front.
                {
                    string adjustedName = tilename.Substring(0, tileNameLength - lengthOfDerelictWord);
                    return "OrokinDerelict" + adjustedName;
                }
                else
                {
                    return "Orokin" + tilename;
                }
                
            }
            else if (tileset == "OrokinTowerDerelict" && !tilename.Contains("Derelict"))
            {
                return "OrokinDerelict" + tilename;
            }

            /*


            //switch Gri to Grn to maintain how DE did it on tilesets that have it already

            if (threeLtrQualifier == "Gri")
            {
                threeLtrQualifier = "Grn";
            }
            else if (threeLtrQualifier == "Cor") // To keep a sort of standard op, do the same for Corpus 
            {
                threeLtrQualifier = "Crp";
            } else if (threeLtrQualifier == "Oro")
            {
                threeLtrQualifier = "Okn";
            }


            // first get the first 3 letters of the Faction: Cor, Gri, Oro, Inf

            string alreadyThreeLtrQualifierCheck = tilename.Substring(0, 3);


            //Special casses:


            // some of the Derelict tiles don't have Derelict in them. Add it to the end, like the others
            if (tileset == "OrokinTowerDerelict") 
            {
                if(tilename.Contains("Derelict"))
                {
                    return tilename;
                }
                else
                {
                    return tilename + "Derelict";
                }
            }
            //These are Invasion names for the Tilesets. They use the same names as the regular tiles, but they have battle damage, usually. Adding a qualifier to seperate
            else if (tileset == "GrineerToCorpus") 
            {
                return "Inv" + tilename;
            }
            else if (tileset == "CorpusToGrineer")
            {
                return "Inv" + tilename;

            }
            else if (tileset == "GrineerForest") //Grineer Forest Special case, uses Gft already so no need to add anything
            {
                return tilename;
            }
            else if (tileset == "CorpusGasCity") //Everyone of these tiles begins with Gas, making it unique enough.
            {
                return tilename;
            }
            else if (alreadyThreeLtrQualifierCheck != threeLtrQualifier || tilename.Contains("Corner")) //edge case, corner triggers Cor
            {//if not, add it to make all tile names Unique (so DeadEnd2 on CorpusShip and DeadEnd2 on Orokin Tower are different.


                

                //finally check if it already starts with Crp or Grn, or contains another qualifier that will make it unique.
                if (alreadyThreeLtrQualifierCheck == "Crp" || alreadyThreeLtrQualifierCheck == "Grn" || tilename.Contains("Grineer") || tilename.Contains("Corpus")
                    || tilename.Contains("Infested") || tilename.Contains("Moon") || tilename.Contains("Derelict"))
                {
                    return tilename;
                }
                else
                {
                    return threeLtrQualifier + tilename;
                }


            }
            else
            {
                return tilename;
            }

            */

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
            string[] notValidTilesetPossibles;
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


        private string[] CheckDuplicateTileDiffTileset(string tileName, string tileset)
        {
            var tileInDbPlusTileset = _db.Tiles.Where(x => x.Name == tileName).Include(x => x.Tileset).FirstOrDefault();
            string[] tilesetNames = new string[2];

            if (tileInDbPlusTileset is null) // Can't find the tile in the db, must be a new one. Safe to set Tileset
            {
                tilesetNames[0] = tileset;
                tilesetNames[1] = null;
                return tilesetNames;
            }
            else if (tileInDbPlusTileset.Tileset.Name != tileset && !string.IsNullOrEmpty(tileInDbPlusTileset.AlternateTileset))
            {
                //find the tile, and it already has an alternate. 


                /* note - this does not work if we find out a tile might stretch across 3 tilesets. I don't ... think they do? but we'll have to watch out
                 */
                tilesetNames[0] = tileInDbPlusTileset.Tileset.Name;
                tilesetNames[1] = tileInDbPlusTileset.AlternateTileset;
                return tilesetNames;
            }
            else if(tileInDbPlusTileset.Tileset.Name != tileset && string.IsNullOrEmpty(tileInDbPlusTileset.AlternateTileset))
            {
                //find the tile, and it has a null for AlternateTileset, and the tileset in the db is different than the one we're passing in.
                tilesetNames[0] = tileInDbPlusTileset.Tileset.Name;
                tilesetNames[1] = tileset;
                return tilesetNames;
            } else if(tileInDbPlusTileset.Tileset.Name == tileset)
            {
                //if the tile in the database has the same tilesetname as whats been passed in
                
                tilesetNames[0] = tileset;
                tilesetNames[1] = tileInDbPlusTileset.AlternateTileset;
                return tilesetNames;
                
            } else // should have caught all of possiblities above, but just in case: 
            {
                tilesetNames[0] = tileset;
                tilesetNames[1] = tileInDbPlusTileset.AlternateTileset;
                return tilesetNames;
            }
        }

        private TileImage GetMapImagePath(string tileName)
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
