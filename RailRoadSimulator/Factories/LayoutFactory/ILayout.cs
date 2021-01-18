using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadSimulator.Factories.LayoutFactory
{
	public abstract class ILayout
	{
		public string areaType { get; set; }
		public Point position { get; set; }
		public int id { get; set; }
		public Image model { get; set; }
		//distance from an other room, its used for A* and pathfinding
		public int distance { get; set; } 
		public Dictionary<ILayout, int> neighbours { get; set; }
		//used for pathfinding
		public ILayout prev { get; set; }  

		public ILayout()
		{
			distance = Int32.MaxValue / 2;
			prev = null;
			neighbours = new Dictionary<ILayout, int>();
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

				g.DrawImage(model, position.X * size, position.Y * size);
			}
			return layout;
		}
	}
}
