using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadSimulator.Pathfinding
{
	public class Tile
	{
		public string areaType { get; set; }
		public bool isDubbelTrack { get; set; } = false;
		public int X { get; set; }
		public int Y { get; set; }
		public int Cost { get; set; }
		public int Distance { get; set; }
		public int CostDistance => Cost + Distance;
		public Tile Parent { get; set; }

		//The distance is essentially the estimated distance, ignoring walls to our target. 
		//So how many tiles left and right, up and down, ignoring walls, to get there. 
		public void SetDistance(int targetX, int targetY)
		{
			this.Distance = Math.Abs(targetX - X) + Math.Abs(targetY - Y);
		}
		//list to check all rideable tiles
		public List<char> rideableTracks = new List<char>{
		 '╔', '╦', '╫',
		 '╩', '╝', '╚',
		 '╟', '╢', '╚',
		 '╗', '└', '┌', '┐',
		 '║', '═', '─', '┘',
		 'A', 'B', 'C', 'D',
	};
	}
}
