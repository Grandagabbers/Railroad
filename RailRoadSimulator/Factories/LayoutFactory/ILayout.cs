using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadSimulator
{
	public abstract class ILayout
	{
		public string areaType { get; set; }
		public int X { get; set; }
		public int Y { get; set; }

		public Image model { get; set; }
		//distance from an other room, its used for A* and pathfinding
		public Tile Parent { get; set; }
		public int Cost { get; set; }
		public int Distance { get; set; }
		public int CostDistance => Cost + Distance;
		public char whatIsIt { get; set; }
		public bool isOccupied { get; set; } = false;
		public ILayout()
		{

		}

		/// <summary>
		/// Draw the room on the bitmap
		/// </summary>
		/// <param name="layout">bitmap to draw on</param>
		/// <returns>bitmap with the room drawn on it</returns>
		public Bitmap Draw(Bitmap layout, int size)
		{

			using (Graphics g = Graphics.FromImage(layout))
			{

				g.DrawImage(model, X * size, Y * size);
			}
			return layout;
		}
	}
}
