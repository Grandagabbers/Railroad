using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadSimulator.Factories.LayoutFactory
{
	public class Station : ILayout
	{
		public string classification { get; set; } //amount of stars a room has
		public Station(TempLayout temp)
		{
			areaType = temp.areaType;
			position = temp.position;
			classification = temp.classification;
			model = Image.FromFile(@"..\..\Assets\station.png");
			model.RotateFlip(RotateFlipType.Rotate180FlipX);//rotate image so its correctly displayed
			id = temp.id;
		}
	}
}
