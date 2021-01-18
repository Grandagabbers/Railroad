using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RailroadEvents;
using RailRoadSimulator.Factories.EntityFactory;
using RailRoadSimulator.Factories.LayoutFactory;
using RailRoadSimulator.Pathfinding;

namespace RailRoadSimulator
{
	public class Manager : RailroadEventListener
	{
		MainForm main { get; set; }
		string itemKey { get; set; }
		string itemValue { get; set; }

		public ILayout[,] coordinates { get; set; }
		PathFinding path = new PathFinding();
		public Manager(ILayout[,] coordinates, MainForm main)
		{
			this.main = main;
			this.coordinates = coordinates;

			//CreateGraph(coordinates);

			RailroadEventManager.Register(this);
			//To speed up for testing
			RailroadEventManager.RRTE_Factor = RailroadEventManager.RRTE_Factor * 4f;
		}
		//Adapter pattern
		public void Notify(RailroadEvent evt)
		{
			//if message is 'Someone puked'
			//eventtype = cleaning emergency is fired

			//if message is 'Slippery! Reduce speed by half'
			//eventtype = leaves on track is called


			//Console.WriteLine("Event fired, message is:  " + evt.Message);
			//Console.WriteLine("EventType is:  " + evt.EventType);

			//check if data is not null, if so fill variables with given data of that event
			if (evt.Data != null)
			{
				foreach (var item in evt.Data) {
					itemKey = item.Key;
					itemValue = item.Value;
				}
			}


			//Make if else statements based on every fired event
			if (evt.EventType == RailroadEventType.SPAWN_TRAIN)
			{
				TempIdentity temp = new TempIdentity();

				foreach (var item in coordinates) {
					if (item != null) {
						if (item.areaType == "Station" && item.whatIsIt == Char.Parse(itemValue)) {
							temp.X = item.X;
							temp.Y = item.Y;
							break;
						}
					}
				}
				//Spawn a train
				Train train = new Train(temp);
				int number = 0;
				string extract = "";
				//this is a string now, maybe convert the string to int
				//then we have to search for the number in the string
				for (int i = 0; i < itemKey.Length; i++)
				{
					if (Char.IsDigit(itemKey[i]))
						extract += itemKey[i];
				}

				if (extract.Length > 0)
				{
					number = int.Parse(extract);
				}
				train.amountOfWagons = number;
				train.startLocation = itemValue;
				//Key is amount of wagons
				//Value is startlocation of train
				Console.WriteLine("Amount of wagons Key is: " + itemKey);
				Console.WriteLine("Amoubt of wagons Value is: " + itemValue);
				


			}
			else if (evt.EventType == RailroadEventType.SPAWN_PASSENGER)
			{
				//create new person/passanger
				TempIdentity temp = new TempIdentity();

				foreach (var item in coordinates)
				{
					if (item != null)
					{
						if (item.areaType == "Station" && item.whatIsIt == itemKey.Last())
						{
							temp.X = item.X;
							temp.Y = item.Y;
							break;
						}
						if (item.areaType == "Station" && item.whatIsIt == itemValue.Last())
						{
							temp.endX = item.X;
							temp.endY = item.Y;
							break;
						}
					}
				}
				Person person = new Person(temp);
				//debug purposes
				person.startLoc = itemKey;
				person.endLoc = itemValue;

				//set startcoordinates
				//check if train is at station, if so then step in train
				//start timer, if person waits too long for train -> delete person: he dies


				//Key is start station
				//Value is end station
				Console.WriteLine("Person spawned, Station Key is: " + itemKey);
				Console.WriteLine("Person spawned, Station Value is: " + itemValue);
				

			}
			else if (evt.EventType == RailroadEventType.CLEANING_EMERGENCY)
			{
				//clean everything, so create maids
					Console.WriteLine("Cleaning emergency Key is: " + itemKey);
					Console.WriteLine("Cleaning emergency Value is: " + itemValue);
				

			}
			else if (evt.EventType == RailroadEventType.LEAVES_ON_TRACK)
			{
				Stopwatch watch = new Stopwatch();
				watch.Start();
				
				//leaves on track so slow game by half
				RailroadEventManager.RRTE_Factor = RailroadEventManager.RRTE_Factor / 2f;
				//timer.Interval = timer.Interval * 2;

				//Slow down RRTE factor by half
				//Key is duration
				//Value is how many seconds and then normal again
				Console.WriteLine("Leaves on track Key is: " + itemKey);
				Console.WriteLine("Leaves on track Value is: " + itemValue);
				while (watch.ElapsedMilliseconds != 10000) {

				}
				Console.WriteLine("10 Seconds have passed, speed up again");
				RailroadEventManager.RRTE_Factor = RailroadEventManager.RRTE_Factor * 2f;
				watch.Stop();

			}
			else if (evt.EventType == RailroadEventType.RETIRE_TRAIN)
			{
				char startLocation = itemKey.First<char>();
				//start the train
				//start pathfinding at station itemKey
				var finalLay = System.IO.File.ReadAllLines(@"..\..\simple-8.trc").ToList<String>();
				path.findTiles(finalLay, startLocation, 'C');

				//Key is where to go from?
				//value is null
				Console.WriteLine("Retire Key is: " + itemKey);
				Console.WriteLine("Retire Value is: " + itemValue);
				
			}
		}

		/// <summary>
		/// Look for every room what the neighbours are 
		/// and store them
		/// </summary>
		/// <param name="coordinates">All the room coordinates.</param>
		//public void CreateGraph(ILayout[,] coordinates)
		//{
		//	this.coordinates = coordinates;

		//	//This only gives neighbours on left and right, still have to make function for top and bottom
		//	foreach (ILayout lay in coordinates)
		//	{

		//		if (lay != null)
		//		{
		//			lay.neighbours.Clear();
		//			int left = lay.position.X - 1;
		//			int right = lay.position.X + 1;
		//			int top = lay.position.Y + 1;
		//			int bottom = lay.position.Y - 1;
		//			while (left >= 0 && coordinates[left, lay.position.Y] == null)
		//			{
		//				left--;
		//			}
		//			while (right < coordinates.GetLength(0) && coordinates[right, lay.position.Y] == null)
		//			{
		//				right++;
		//			}
		//			while (top < coordinates.GetLength(1) && coordinates[lay.position.X, top] == null)
		//			{
		//				top++;
		//			}
		//			while (bottom >= 0 && coordinates[lay.position.X, bottom] == null)
		//			{
		//				bottom++;
		//			}
		//			//if (top < coordinates.GetLength(1) && coordinates[lay.position.X, top] != null)
		//			//{
		//			//	ILayout Top = coordinates[lay.position.X, top];
		//			//	lay.neighbours.Add(Top, 1);
		//			//}
		//			//if (bottom >= 0 && coordinates[lay.position.X, bottom] != null)
		//			//{
		//			//	ILayout Bottom = coordinates[lay.position.X, bottom];
		//			//	lay.neighbours.Add(Bottom, 1);
		//			//}
		//			if (left >= 0 && coordinates[left, lay.position.Y] != null)
		//			{
		//				ILayout Left = coordinates[left, lay.position.Y];
		//				lay.neighbours.Add(Left, 1);
		//			}
		//			if (right < coordinates.GetLength(0) && coordinates[right, lay.position.Y] != null)
		//			{
		//				ILayout Right = coordinates[right, lay.position.Y];
		//				lay.neighbours.Add(Right, 1);
		//			}
		//			foreach (var a in lay.neighbours)
		//			{
		//				Console.WriteLine("LayoutItem: " + lay.id + " Has neighbours: " + a.Key.id + " amount of neighbours= " + lay.neighbours.Count);
		//			}
		//		}
		//	}

		//}
	}
}
