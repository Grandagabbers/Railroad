using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadSimulator
{
	public class Draw
	{
        public int drawSizeItem { get; } = 50;
		public Draw()
		{

		}

        /// <summary>
        /// Draw the layout of the hotel
        /// </summary>
        /// <param name="coordinates">layout of the game</param>
        /// <returns>bitmap with the layout</returns>
        public Bitmap DrawLayout(ILayout[,] coordinates)
        {
            //set width and height for bitmap
            int width = coordinates.GetLength(0) * drawSizeItem;
            int height = coordinates.GetLength(1) * drawSizeItem;

            Bitmap layout = new Bitmap(width, height);
            //foreach item in the layout, draw the item.
            foreach (ILayout lay in coordinates)
            {
                if (lay != null)
                {
                    lay.Draw(layout, drawSizeItem);
                }

            }

            //flip the bitmap so the background is displayed correctly
           layout.RotateFlip(RotateFlipType.Rotate180FlipX);

            return layout;
        }

        /// <summary>
        /// Draw the personLayout 
        /// </summary>
        /// <param name="personLayout">bitmap to draw on</param>
        /// <param name="people">persons to draw</param>
        /// <returns>personLaoyout with the drawn persons</returns>
        public Bitmap DrawPersonLayout(Bitmap personLayout, Dictionary<IEntity, Tile> people)
        {
            //Foreach person in people
            //Draw it on the bitmap
            foreach (IEntity everyPerson in people.Keys)
            {
                personLayout = everyPerson.DrawPerson((Bitmap)personLayout, drawSizeItem);
            }

            //flip bitmap so it displays correctly
            personLayout.RotateFlip(RotateFlipType.Rotate180FlipX);

            return (Bitmap)personLayout;
        }
    }
}
