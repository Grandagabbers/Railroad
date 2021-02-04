using Newtonsoft.Json;
using RailRoadSimulator.Pathfinding;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadSimulator
{
	public class LayoutFactory : AbstractFactory
	{
        //list of layout
		public List<ILayout> layout = new List<ILayout>();
        //list of finalstring layout
        public List<string> finalLay = new List<string>();
        //list of alltiles
        public List<Tile> allTiles = new List<Tile>();
        //2d array with all coordinates
        public ILayout[,] coordinates { get; set; }
        public void GenerateEntity()
		{
			DeserializeLayout();

            //add to layout every tile
            foreach (Tile tile in allTiles) {
                layout.Add(GenerateRoom<ILayout>(tile.areaType, tile));
            }

           CreateOverview();
        }

        /// <summary>
        /// read out the trc layout file and save it in a temporary list with temporary layout
        /// </summary>
        private void DeserializeLayout()
        {
            finalLay = File.ReadAllLines(@"..\..\final-assignment.trc").ToList();

            int y = 0;
            //foreach loops to check what the layout has
            foreach (var singleLine in finalLay)
            {
                int x = 0;
                foreach (var singleChar in singleLine)
                {
                    Tile tile = new Tile();
                    switch (singleChar)
                    {
                        //this checks if it is a letter from the alphabet, if so make station
                        case 'A':
                        case 'B':
                        case 'C':
                        case 'D':
                        case 'E':
                            tile.areaType = "Station";
                            tile.whatIsIt = singleChar;
                            tile.X = x;
                            tile.Y = y;
                            allTiles.Add(tile);
                            break;
                        //This is the remise trains come and go to
                        case 'R':
                            tile.areaType = "Remise";
                            tile.whatIsIt = singleChar;
                            tile.X = x;
                            tile.Y = y;
                            allTiles.Add(tile);
                            break;
                        //use default because we want to check every item in the list
                        default:
                            //if singlechar is not a space then its a track so make that class
                            if (singleChar != ' ')
                            {
                                tile.whatIsIt = singleChar;
                                tile.areaType = "Track";
                                if (tile.dubbelTrack.Contains(singleChar))
                                {
                                    tile.isDubbelTrack = true;
                                }
                                tile.X = x;
                                tile.Y = y;
                                allTiles.Add(tile);
                                break;
                            }
                            break;
                    }
                    x++;
                }
                y++;
            }
		}

        /// <summary>
        /// Generate a room
        /// </summary>
        /// <typeparam name="T">type of room</typeparam>
        /// <param name="lay">areaType of the item</param>
        /// <param name="temp">temperary room for constructor</param>
        /// <returns>instance of the given type room</returns>
        public ILayout GenerateRoom<T>(string lay, Tile temp) where T : ILayout
        {
            //get the type of the room
            Type type = Type.GetType(lay);

            //if type is found return a instance of that type
            if (type != null)
            {
                return (ILayout)Activator.CreateInstance(type, temp);
            }
            //else if type is not found.
            //search the namespace/current assembly for all types that are available
            //make a list out of all the types and look if one of those types contain the name of the areatype
            //if so set that type and create an instance of it.
            //if not, return the temp layout.
            else
            {
                string nspace = "RailRoadSimulator";

                //query for all the types
                var q = from x in Assembly.GetExecutingAssembly().GetTypes()
                        where x.IsClass && x.Namespace == nspace
                        select x;

                List<string> types = new List<string>();
                //put the query in the list
                foreach (Type t in q)
                {
                    types.Add(t.ToString());
                }
                //search the list and if found return instance. 
                foreach (string t in types)
                {
                    if (t.Contains(lay))
                    {
                        type = Type.GetType(t);
                        return (ILayout)Activator.CreateInstance(type, temp);
                    }
                }

            }
            return temp;
        }

        /// <summary>
        /// Overview of the layout
        /// </summary>
        /// <returns>2D array with layout</returns>
        private ILayout[,] CreateOverview()
        {
            Point maxArray = new Point();
            maxArray.X = 0;
            maxArray.Y = 0;


            //set the dimensions of the 2dArray
            foreach (ILayout lay in layout)
            {
                if (lay.X > maxArray.X)
                {
                    maxArray.X = lay.X;
                }
                if (lay.Y > maxArray.Y)
                {
                    maxArray.Y = lay.Y;
                }
            }

            //create 2dArray
            coordinates = new ILayout[maxArray.X + 2, maxArray.Y + 1];

            //add the items  to the array
            foreach (ILayout lay in layout)
            {
                coordinates[lay.X, lay.Y] = lay;         
                
            }
            return coordinates;
        }
    }
}
