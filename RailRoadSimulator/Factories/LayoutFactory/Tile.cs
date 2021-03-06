﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadSimulator
{
	public class Tile : ILayout
	{
		//The distance is essentially the estimated distance, ignoring walls to our target. 
		//So how many tiles left and right, up and down, ignoring walls, to get there. 
		public void SetDistance(int targetX, int targetY)
		{
			this.Distance = Math.Abs(targetX - X) + Math.Abs(targetY - Y);
		}
		
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
