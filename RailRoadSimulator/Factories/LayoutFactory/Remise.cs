using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadSimulator
{
	public class Remise : ILayout
	{
		public List<Train> trainsInStorage { get; set; }
		public Remise(Tile temp)
		{
			trainsInStorage = new List<Train>();
			areaType = temp.areaType;
			X = temp.X;
			Y = temp.Y;
			model = Image.FromFile(@"..\..\Assets\station.png");
			model.RotateFlip(RotateFlipType.Rotate180FlipX);//rotate image so its correctly displayed

		}
	}
}
