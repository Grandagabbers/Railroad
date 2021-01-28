using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Diagnostics;

namespace RailRoadSimulator
{
	public class Person : IEntity
	{
		public Stopwatch timer { get; } = new Stopwatch();
		public Person(TempIdentity temp)
		{
			//if 20 secs have passed person will die
			timer.Start();
			areaType = temp.areaType;
			X = temp.X;
			Y = temp.Y;
			endX = temp.endX;
			endY = temp.endY;
			endStationName = temp.endStationName;
			model = Image.FromFile(@"..\..\Assets\Guest.png");
			model.RotateFlip(RotateFlipType.Rotate180FlipX);//rotate image so its correctly displayed
		}
		public bool isWaiting()
		{
			if (timer.ElapsedMilliseconds >= 20000) {
				Console.WriteLine("20 secs passed so die");
				timer.Stop();
				return true;
			}
			return false;
		}
	}
}
