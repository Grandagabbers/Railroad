using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadSimulator.Factories.EntityFactory
{
	public class TempPerson : IEntity
	{
		public string startStation { get; set; }
		public string endStation { get; set; }

		public TempPerson()
		{

		}
	}
}
