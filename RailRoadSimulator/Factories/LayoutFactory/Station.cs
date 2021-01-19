using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadSimulator
{
	public class Station : ILayout
	{
		public Station(Tile temp)
		{
			areaType = temp.areaType;
			X = temp.X;
			Y = temp.Y;
			model = Image.FromFile(@"..\..\Assets\station.png");
			whatIsIt = temp.whatIsIt;
			model.RotateFlip(RotateFlipType.Rotate180FlipX);//rotate image so its correctly displayed
		}
	}
}
