using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadSimulator.Factories.EntityFactory
{
	public class Train : IEntity
	{
		public int amountOfWagons { get; set; } = 0;
		public string destination { get; set; }
		public string startLocation { get; set; }
		public Image wagonModel { get; set; }

		public Train(TempIdentity temp)
		{
			X = temp.X;
			Y = temp.Y;
			model = Image.FromFile(@"..\..\Assets\train.png");
			model.RotateFlip(RotateFlipType.Rotate180FlipX);//rotate image so its correctly displayed
			if (amountOfWagons > 0)
			{
				for (int i = 0; i != amountOfWagons; i++)
				{
					wagonModel = Image.FromFile(@"..\..\Assets\train.png");
					model.RotateFlip(RotateFlipType.Rotate180FlipX);//rotate image so its correctly displayed
				}

			}
		}
	}
}
