using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadSimulator
{
	public class Person : IEntity
	{
		public string startLoc { get; set; }
		public string endLoc { get; set; }
		public Person(TempIdentity temp)
		{
			areaType = temp.areaType;
			X = temp.X;
			Y = temp.Y;
			model = Image.FromFile(@"..\..\Assets\train.png");
			model.RotateFlip(RotateFlipType.Rotate180FlipX);//rotate image so its correctly displayed
		}
	}
}
