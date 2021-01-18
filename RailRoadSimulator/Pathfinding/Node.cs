using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadSimulator.Pathfinding
{
	public class Node
	{
		private int x;
		private int y;

		public int gCost;
		public int hCost;
		public int fCost;

		public Node cameFromNode;

		public Node()
		{

		}
	}
}
