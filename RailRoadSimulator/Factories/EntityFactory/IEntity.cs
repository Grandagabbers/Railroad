using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadSimulator.Factories.EntityFactory
{
	public abstract class IEntity
	{

		public Image model { get; set; }

		public IEntity()
		{

		}
	}
}
