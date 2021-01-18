using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadSimulator.Factories.LayoutFactory
{
	public class Track : ILayout
	{
		public Track(TempLayout temp)
		{
			areaType = temp.areaType;
			position = temp.position;
			model = Image.FromFile(@"..\..\Assets\track.png");
			model.RotateFlip(RotateFlipType.Rotate180FlipX);//rotate image so its correctly displayed
			id = temp.id;
		}
	}
}
