using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadSimulator
{
	public class Track : ILayout
	{
		public bool isDubbelTrack { get; set; } = false;
		public Track(Tile temp)
		{
			areaType = temp.areaType;
			X = temp.X;
			Y = temp.Y;
			model = Image.FromFile(@"..\..\Assets\track.png");
			whatIsIt = temp.whatIsIt;
			if (whatIsIt == '═' || whatIsIt == '─') { 
				model.RotateFlip(RotateFlipType.Rotate90FlipX);//rotate image so its correctly displayed
			}
			model.RotateFlip(RotateFlipType.Rotate180FlipX);//rotate image so its correctly displayed
		}
	}
}
