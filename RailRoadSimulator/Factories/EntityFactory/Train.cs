﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadSimulator
{
	public class Train : IEntity
	{
		public char destination { get; set; }
		public string startLocation { get; set; }
		public Image wagonModel { get; set; }
		public int capacity { get; set; } = 8;
		public bool hasPath { get; set; } = false;
		public bool isCleaning { get; set; } = false;
		public bool hasBeenCleaned { get; set; } = false;
		public bool returning { get; set; } = false;
		public int cleanWait { get; set; } = 0;
		public int firstX { get; set; }
		public int firstY { get; set; }
		public int Xend { get; set; } = 0;
		public int Yend { get; set; } = 0;
		public bool waitCount { get; set; } = false;
		public int waitAmount { get; set; } = 0;
		//list of maids in train

		public List<Maid> maidsInTrain = new List<Maid>();
		//list of persons from the train used at cleaning
		public List<Person> peopleFromTrain { get; set; }
		//list of all stations a train can go to

		public List<string> allStations = new List<string>()
		{
			"A",
			"B",
			"C",
			"D",
			"E",
			"R",
		};
		//List of people in the train and where they want to go
		public List<Person> personsInTrain { get; set; }

		public Train(TempIdentity temp)
		{
			peopleFromTrain = new List<Person>();
			personsInTrain = new List<Person>();
			amountOfWagons = temp.amountOfWagons;
			areaType = temp.areaType;
			X = temp.X;
			Y = temp.Y;
			firstX = temp.endX;
			firstY = temp.endY;
			endX = temp.endX;
			endY = temp.endY;
			model = Image.FromFile(@"..\..\Assets\train.png");
			model.RotateFlip(RotateFlipType.Rotate180FlipX);//rotate image so its correctly displayed
			//if train has more wagons
			if (amountOfWagons > 1)
			{
				//check how many wagons
				for (int i = 0; i != amountOfWagons; i++)
				{
					//increase capacity per wagon
					capacity = capacity + 8;
					wagonModel = Image.FromFile(@"..\..\Assets\train.png");
					model.RotateFlip(RotateFlipType.Rotate180FlipX);//rotate image so its correctly displayed
				}

			}
			if (allStations.Contains(temp.endStationName.ToString())) {
				destination = temp.endStationName;
			}
		}

		/// <summary>
		/// Ensures the next station is set correctly
		/// </summary>
		public void NextStation(Train train)
		{
				switch (train.destination)
				{
					case 'A':
						startLocation = "A";
						destination = 'B';
						break;
					case 'B':
						startLocation = "B";
						destination = 'C';
						break;
					case 'C':
						startLocation = "C";
						destination = 'D';
						break;
					case 'D':
						startLocation = "D";
						destination = 'E';
						break;
					case 'E':
						startLocation = "E";
						destination = 'A';
						break;
					default:
						startLocation = "R";
						destination = 'R';
						break;
				}
			
		}

	}
}
