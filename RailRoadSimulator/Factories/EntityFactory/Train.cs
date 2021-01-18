using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadSimulator.Factories.EntityFactory
{
	public class Train : IEntity
	{
		public string amountOfWagons { get; set; }
		public string destination { get; set; }
		public string startLocation { get; set; }

		public Train()
		{
			
		}
	}
}
