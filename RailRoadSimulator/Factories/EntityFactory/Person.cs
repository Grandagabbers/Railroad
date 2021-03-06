﻿using System;
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
		//timer to check if people die because they have to wait for too long
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
		/// <summary>
		/// checks how long a person was waiting
		/// </summary>
		/// <returns>true or false based on if the person has died or not</returns>
		public bool isWaiting()
		{
			if (timer.ElapsedMilliseconds >= 45000) {
				Console.WriteLine("45 secs passed so die");
				timer.Stop();
				return true;
			}
			return false;
		}
		/// <summary>
		/// checks if the endstation from the train is the same as where he wants to go
		/// this can be used if people know which trains are coming and going
		/// currently not being used because of change in pathfinding
		/// </summary>
		/// <param name="train">the current train at that station</param>
		/// <returns>true if endloc is the same, else false</returns>
		public bool CheckIn(Train train)
		{
			if (train.destination == endStationName)
			{
				return true;
			}
			return false;
		}
		/// <summary>
		/// checks if train is at the end loc the person wants to be
		/// this can be used if people know which trains are coming and going
		/// currently not being used because of change in pathfinding
		/// </summary>
		/// <param name="train">the current train at that station</param>
		/// <returns>true if endloc is the same, else false</returns>
		public bool CheckOut(Train train)
		{
			if (train.X == endX && train.Y == endY) {
				return true;
			}
			return false;
		}
	}
}
