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


        /* Reads a CSV file and returns a list of what DE uses to name their Missions added on to the Tileset Names, and what we're going to call them so its easier to read
         * 
         * it also handles many odd casses, and 'duplicates' by the longer version being first in the list, and the shorter version being later
         */
        


        

        public List<ImgMetaData> GetMetaList(string path)
        {
            List<ImgMetaData> metaList = new List<ImgMetaData>();

            var picList = System.IO.Directory.GetFiles(path);

            //There should only ever be one spawn or exit tile,so we'll set flags when we find the first one.
            bool spawn = false;
            bool exit = false;

            foreach (var pic in picList)
            {
                var metaData = GetMetaData(pic);

                if(metaData is null)
                {
                    continue;

                }else if(metaData.MissionType.Contains("???") || metaData.Tileset.Contains("???"))
                {
                    //if the MissionType or the Tileset hits the final return, it will add ??? to the name.
                    // so we check for that so we can auto exclude files that don't have the proper checks for their MissionType/Tileset
                    metaData.KeepThis = false;
                    metaData.UnknownValue = true;
                }
                else
                {
                    metaData.UnknownValue = false;
                }


                /*To Do - a check if the Filename is NOT Warframe####
                 */

                string duplicateCheck = metaList.Where(x => x.TileName == metaData.TileName && x.MapIdentifier == metaData.MapIdentifier).Select(x => x.TileName).FirstOrDefault();
                if(metaList.Count !=0 && !string.IsNullOrEmpty(duplicateCheck) &&( !metaData.TileName.Contains("DeadEnd") || !metaData.TileName.Contains("Cap") || !metaData.TileName.Contains("Closet") || metaData.TileName.Contains("Capture")) )
                { 
                    /* Go through the list of tiles already added, and see if any of them have the same name.
                     * 
                     * if a duplicate name is found, mark as possible dupe unless it contains DeadEnd, Closet or Cap (but not Capture) - as these are very common tiles where there is always more than one.
                     */
                    metaData.KeepThis = false;
                    metaData.PossibleDupe = true;
                }else
                {
                    metaData.PossibleDupe = false;
                }


                /* There should only ever be one Spawn/Start or Exit/Extraction tile in a map. If there are more than one in the uploads, then its probably a duplicate image.
                 * so we'll check for them, if we have already found one, we'll set KeepThis = false so it gets auto unchecked and duplicate flag on the view gets added
                 * 
                 * 
                 * this obviously has an issue if more than one run is tossed into the processing at once. So we need to add some checks for that.
                 */
                if(metaData.TileName.Contains("Spawn") || metaData.TileName.Contains("Start") )
                {
                    if(spawn)
                    {
                        metaData.KeepThis = false;
                        metaData.PossibleDupe = true;
                    }
                    else
                    {
                        spawn = true;
                    }
                }else if (metaData.TileName.Contains("Exit") || metaData.TileName.Contains("Extraction"))
                {
                    if (exit)
                    {
                        metaData.KeepThis = false;
                        metaData.PossibleDupe = true;
                    }
                    else
                    {
                        exit = true;
                    }
                }

                metaList.Add(metaData);
            }

            return metaList;
        }

        



        /* Returns a list of the Metadata from Warframe In Game Screenshot button (f6)
         * 
         */


        public ImgMetaData GetMetaData(string path)
        {
            var metaData = new ImgMetaData();
            var values = new List<string>();
            var directories = MetadataExtractor.ImageMetadataReader.ReadMetadata(path);
            List<string> mapInfo = new List<string>();

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
                        metaData.MapIdentifier = mapInfo[2];
                        metaData.TileName = GetTileName(metaData.FactionName, tileNameHolder, metaData.Tileset);
                        mapInfo.Clear();
                        metaData.TileImageInfo = GetMapImagePath(metaData.TileName);


                        if(metaData.Tileset=="GrineerFortress")
                        {
                            metaData.FactionName = "GrineerQueens";
                        }
                        if (metaData.Tileset == "OrokinDerelict")
                        {
                            metaData.FactionName = "Infested";
                        }
                        
                        // The Spy Vaults are identical with the same names across many different tilesets for corpus and grineer.
                        // Lua Spy Vaults are unique to lua, no need to change them
                        // Kuva Spyvaults are unique to Grineer Fortress, and we're using Kuva for their faction name.
                        if (metaData.TileName.Contains("SpyVault") &&(metaData.FactionName =="Grineer" || metaData.FactionName=="Corpus")) 
                        {
                            metaData.Tileset = metaData.FactionName + "SpyVault";
                        }


                        metaData.AlternateTileset = CheckDuplicateTileDiffTileset(metaData.TileName, metaData.Tileset);


                    }
                    else  //this SHOULD cover all the weird maps, like Hubs and what not. There is error checking just in case elsewhere, but... 
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

            return metaData;
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

            string threeLtrQualifier = faction.Substring(0, 3);


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


            return value + "??? - Check Me";

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
                    return stringTilesetMission + " ??? Check Me!";
                }
            }


            int length = stringTilesetMission.Length;

            if (_df.CheckTilesetExists(stringTilesetMission)) // checks the database to see if the Tileset already exists. Mostly for the two edge cases from Missions that don't have a Mission Type
            {
                return stringTilesetMission;
            }


            if (stringTilesetMission.Contains("TR")) // special case, just get it over with.
            {
                return "CorpusTrenchRunArchwing";
            }
            if (stringTilesetMission.Contains("Space"))
            {
                return "GrineerFreeSpaceArchwing";
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


        private bool CheckDuplicateTileDiffTileset(string tileName, string tileset)
        {
            var tileInDbPlusTileset = _db.Tiles.Where(x => x.Name == tileName).Include(x => x.Tileset).FirstOrDefault();

            if (tileInDbPlusTileset is null)
            {
                return false;
            }
            else if (tileInDbPlusTileset.Tileset.Name != tileset)
            {
                return true;
            }
            else //if they have the same tileset name
            {
                return false;
            }
        }

        private TileImage GetMapImagePath(string tileName)
        {
            var isTileInDB = _db.Tiles.Where(x => x.Name == tileName).Include(x => x.Tileset).FirstOrDefault();


            var defaultTileImageData = new TileImage { ImagePath = "LotusFlower.png", AltText = "No Map Image Uploaded Yet" };


            if (isTileInDB is null)
                return defaultTileImageData;
            else
            {
                var doesTileHaveImg = _db.TileImages.Where(x => x.Tile == isTileInDB);

                if (doesTileHaveImg.Count() > 0)
                {
                    var tileMapImg = _db.TileImages.Where(x => x.ViewName == "Map").FirstOrDefault();
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





        //quick function for * wildcard search.
        private static String WildCardToRegular(String value)
        {
            return "^" + Regex.Escape(value).Replace("\\*", ".*") + "$";
        }
    }

}
