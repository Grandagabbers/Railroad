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

namespace RailRoadSimulator.Factories.LayoutFactory
{
	public class LayoutFactory : AbstractFactory
	{
		//Temporary list of layout
		public List<TempLayout> tempRooms = new List<TempLayout>();
		public List<ILayout> layout = new List<ILayout>();
        public List<String> finalLay = new List<String>();
        public List<Tile> allTiles = new List<Tile>();
        PathFinding path = new PathFinding();
        //public Elevator elevator;
        public ILayout[,] coordinates { get; set; }
        public void GenerateEntity()
		{
			DeserializeLayout();

			//foreach temproom create a definite room
			//add definite room to the room list
			foreach (TempLayout temp in tempRooms)
			{

				if (temp.areaType != null)
				{
					layout.Add(GenerateRoom<ILayout>(temp.areaType, temp));
				}
			}

            CreateOverview();
        }

		/// <summary>
		/// read out the json layout file and save it in a temporary list with temporary rooms
		/// </summary>
		private void DeserializeLayout()
		{
            //var json = File.ReadAllLines(@"..\..\final-assignment.trc");
            finalLay = File.ReadAllLines(@"..\..\simple-8.trc").ToList();
            foreach (var singleLine in finalLay) {
                foreach (var singleChar in singleLine) {
                    Tile tile = new Tile();
                    var test = Char.IsLetter(singleChar);
                    switch (test)
                    {
                        //this checks if it is a letter from the alphabet, if so make station
                        case true:
                            tile.areaType = "";
                            allTiles.Add(tile);
                            break;
                    }
                    switch (singleChar) {
                        case ' ':
                            tile.areaType = "";
                            allTiles.Add(tile);
                            break;
                    }

                }
            }
            

			//convert json to templayout
			//tempRooms = JsonConvert.DeserializeObject<List<TempLayout>>(json);
		}

        /// <summary>
        /// Generate a room
        /// </summary>
        /// <typeparam name="T">type of room</typeparam>
        /// <param name="lay">areaType of the item</param>
        /// <param name="temp">temperary room for constructor</param>
        /// <returns>instance of the given type room</returns>
        public ILayout GenerateRoom<T>(string lay, TempLayout temp) where T : ILayout
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
            //if not, return the temp room.
            else
            {
                string nspace = "RailRoadSimulator.Factories.LayoutFactory";

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
                if (lay.position.X > maxArray.X)
                {
                    maxArray.X = lay.position.X;
                }
                if (lay.position.Y > maxArray.Y)
                {
                    maxArray.Y = lay.position.Y;
                }
            }

            //create 2dArray
            coordinates = new ILayout[maxArray.X + 1, maxArray.Y + 1];

            //add the items  to the array
            foreach (ILayout lay in layout)
            {
                coordinates[lay.position.X, lay.position.Y] = lay;
            }
            return coordinates;
        }
    }
}
