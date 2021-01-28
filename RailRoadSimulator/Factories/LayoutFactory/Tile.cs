using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadSimulator
{
	public class Tile : ILayout
	{
		public bool isDubbelTrack { get; set; } = false;

		//The distance is essentially the estimated distance, ignoring walls to our target. 
		//So how many tiles left and right, up and down, ignoring walls, to get there. 
		public void SetDistance(int targetX, int targetY)
		{
			this.Distance = Math.Abs(targetX - X) + Math.Abs(targetY - Y);
		}
		//list to check all rideable tiles for testing
	//	public List<char> rideableTracks = new List<char>{
	//	 '╔', '╦', '╫',
	//	 '╩', '╝', '╚',
	//	 '╟', '╢', '╚',
	//	 '╗', '└', '┌', '┐',
	//	 '║', '═', '─', '┘',
	//	 //Stations 'A', 'B', 'C', 'D', 'E', 'Remise', 
	//};
		
		//List with all dubbelTracks used to set isdubbeltrack
		public List<char> dubbelTrack = new List<char>
		{
			'╔', '╦', '╫',
			'╩', '╝', '╚',
			'╟', '╢', '╚',
			'╗','║', '═',
		};
	}
}
