using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadSimulator
{
	public class Train : IEntity
	{
		public string destination { get; set; }
		public string startLocation { get; set; }
		public Image wagonModel { get; set; }
		public int capacity { get; set; } = 1;
		//List of people in the train and where they want to go
		public List<Person> personsInTrain { get; set; }

		public Train(TempIdentity temp)
		{
			personsInTrain = new List<Person>();
			amountOfWagons = temp.amountOfWagons;
			areaType = temp.areaType;
			X = temp.X;
			Y = temp.Y;
			model = Image.FromFile(@"..\..\Assets\train.png");
			model.RotateFlip(RotateFlipType.Rotate180FlipX);//rotate image so its correctly displayed
			if (amountOfWagons > 1)
			{
				for (int i = 0; i != amountOfWagons; i++)
				{
					capacity = capacity + 1;
					wagonModel = Image.FromFile(@"..\..\Assets\train.png");
					model.RotateFlip(RotateFlipType.Rotate180FlipX);//rotate image so its correctly displayed
				}

			}
		}

	}
}
