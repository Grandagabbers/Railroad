using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadSimulator
{
	public abstract class IEntity
	{
        public string areaType { get; set; }
        public bool eventStarted { get; set; } = false;
        public int X { get; set; }
		public int Y { get; set; }
        public int endX { get; set; }
        public int endY { get; set; }
        public char endStationName { get; set; }
        public int timeBusyEvent { get; set; } //keeps track of much time  a person spent so far in a event
        public Image model { get; set; }
        public LinkedList<List<Tile>> eventQueue = new LinkedList<List<Tile>>();// a queue containing routes that the person must walk in the future. 
        public int amountOfWagons { get; set; } = 0;
        public ILayout currentRoom { get; set; }//room where the person is atm

        private int imageWidth = 60; //width of the person in pixels
        private int imageHeight = 75;//height of the person in pixels
        public List<Tile> route; //current route person is walking
        private int routeCounter = 0; //counter which keeps track where person is on its current route

        public IEntity()
		{
            route = new List<Tile>();
		}

        /// <summary>
        /// Makes the animation possible by moving the person to next coordinate.
        /// </summary>
        public void WalkTo()
        {
            if (routeCounter <= route.Count - 1)
            {

                this.X = this.route[routeCounter].X; //set the current postition to the current position of the route the person is walking
                this.Y = this.route[routeCounter].Y;
                routeCounter++;

            }

        }

        /// <summary>
        /// Draws the person inside of the room he is in.
        /// </summary>
        /// <param name="person">The sprite of the person that is about to be drawn.</param>
        /// <param name="sizeRoom">Need this int to make the character the size of the room so if you change it they change with the it.</param>
        /// <returns>The sprite of the right person in the right room.</returns>
        public Bitmap DrawPerson(Bitmap person, int sizeRoom)
        {

            if (this.areaType.Contains("Train"))//draw train
            {

                using (Graphics g = Graphics.FromImage(person))
                {

                    g.DrawImage(model, X * sizeRoom, Y * sizeRoom, imageWidth, imageHeight);
                }
            }
            else if (this.areaType.Contains("Person"))//draw Customer
            {
                using (Graphics g = Graphics.FromImage(person))
                {
                    g.DrawImage(model, X * sizeRoom, Y * sizeRoom, imageWidth, imageHeight);
                }
            }


            return person;
        }
    }
}
