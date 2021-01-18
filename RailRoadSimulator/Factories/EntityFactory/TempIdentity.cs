using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadSimulator.Factories.EntityFactory
{
	public class TempIdentity : IEntity
	{
		public int endX { get; set; }
		public int endY { get; set; }

		public TempIdentity()
		{

		}
	}
}
