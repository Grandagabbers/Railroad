using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using RailroadEvents;
using RailRoadSimulator.Pathfinding;

namespace RailRoadSimulator
{
	public class Manager : RailroadEventListener
	{
		MainForm main { get; set; }
		string itemKey { get; set; }
		string itemValue { get; set; }
		public bool evacuation { get; set; } = false;

		public List<IEntity> people = new List<IEntity>();
		public List<IEntity> trains = new List<IEntity>();

		List<string> layout = new List<string>();

		EntityFactory fac = new EntityFactory();
		LayoutFactory layFac { get; set; }
		public ILayout[,] coordinates { get; set; }
		PathFinding path = new PathFinding();
		public KeyValuePair<int, bool> Leaves { get; set; }
		public Dictionary<char, bool> ReturnToRemisePair { get; set; }
		public Manager(ILayout[,] coordinates, MainForm main)
		{
			this.main = main;
			this.coordinates = coordinates;

			layFac = new LayoutFactory();
			layFac.GenerateEntity();
			layout = System.IO.File.ReadAllLines(@"..\..\final-assignment.trc").ToList<string>();

			RailroadEventManager.Register(this);
			//To speed up for testing
			//RailroadEventManager.RRTE_Factor = RailroadEventManager.RRTE_Factor * 4f;
		}

		public void FindPath(Train current)
		{
			//THESE LINES DETERMINE THE PATH BASED ON THE PEOPLE IN THE TRAIN
			//THIS IS OLD AND NOT HOW TRAINS WORK 
			//Dictionary<Point, int> amount = new Dictionary<Point, int>();
			////Console.WriteLine("current train is full");
			//foreach (var item in current.personsInTrain)
			//{
			//	Point test = new Point(item.endX, item.endY);
			//	if (amount.Keys.Contains(test))
			//	{
			//		amount[test] = amount.Values.First() + 1;
			//	}
			//	else
			//	{
			//		amount.Add(test, 1);
			//	}
			//}
			//if (amount.Count > 0)
			//{
			//	var keyOfMaxValue = amount.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
			//	current.endX = keyOfMaxValue.X;
			//	current.endY = keyOfMaxValue.Y;
			//}

			//make the endTile of the train
			//Tile endTile = new Tile();
			//endTile.X = current.endX;
			//endTile.Y = current.endY;

			List<Tile> trainPath = path.findTiles(layout, current);
			current.hasPath = true;

			current.NextStation(current);
			foreach (var item in coordinates) {
				if (item != null)
				{
					if (current.endX != item.X || current.endY != item.Y)
					{
						if (item.areaType == "Station" && item.whatIsIt == current.destination)
						{
							current.endX = item.X;
							current.endY = item.Y;
						}
					}
				}
			}
			current.eventQueue.AddLast(trainPath);
			current.route = trainPath;
			current.eventQueue.RemoveLast();
		}

		public void CheckIfPeopleAtStation(Train train)
		{
			train.waitCount++;
			//make function that people get in even if it is not their endstation

			//check if train is not full
			if (train.personsInTrain.Count < train.capacity) {
				foreach (var person in people.ToList())
				{
					if (person.areaType.Contains("Person") && train.X == person.X && train.Y == person.Y)
					{
						Person pers = (Person)person;
						bool checkedIn = pers.CheckIn(train);
						//if person endloc is same as train endloc check in, else not
						//if (checkedIn)
						//{
							Console.WriteLine("Person checked in");
							train.personsInTrain.Add((Person)person);
							//remove people because they are now in train
							people.Remove(person);
						//}
					}
				}
			}
			//if there are people in the train find the route the most people need to go to
			if (train.personsInTrain.Count > 0 && train.hasPath == false)
			{
				train.routeCounter = 0;
				FindPath(train);
				train.waitCount = 0;
			}
			//this does not work currently
			//else if (train.routeCounter == 0 || train.route == null && train.endX != train.X && train.endY != train.Y)
			//{
			//	foreach (var item in coordinates)
			//	{
			//		if (item != null && item.areaType == "Station" && item.whatIsIt == train.destination)
			//		{
			//			train.endX = item.X;
			//			train.endY = item.Y;
			//		}
			//	}
			//	//find path to end destination if there is no route
			//	FindPath(train);
			//}

			//if train has waited 5 ticks
			if (train.waitCount == 5)
			{
				FindPath(train);
				//set waitcount back to 0 
				train.waitCount = 0;
			}
			else
			{
				Console.WriteLine("Train is waiting for: " + train.waitCount);
			}
		}
			public void CheckOutPeople(Train train)
		{
			foreach (var person in train.personsInTrain.ToList())
			{
				//if position is correct remove people from train because they are at their location
				if (person.endX == train.X && person.endY == train.Y)
				{
					train.personsInTrain.Remove(person);
					Console.WriteLine("Person checked out");
				}
			}
		}

		public void AddMaid(Train train)
		{
			foreach (var person in train.personsInTrain.ToList())
			{
				//if position is correct remove people from train because they are at their location
				if (person.endX == train.X && person.endY == train.Y)
				{
					if (train.capacity < 1)
					{
						//people.Add((Maid)fac.GetPerson("Person", temp));
					}
				}
			}
		}


		//Adapter pattern
		public void Notify(RailroadEvent evt)
		{

			//check if data is not null, if so fill variables with given data of that event
			if (evt.Data != null)
			{
				foreach (var item in evt.Data)
				{
					itemKey = item.Key;
					itemValue = item.Value;
				}
			}


			//Make if else statements based on every fired event
			if (evt.EventType == RailroadEventType.SPAWN_TRAIN)
			{
				TempIdentity temp = new TempIdentity();
				temp.areaType = "Train";
				foreach (var item in coordinates)
				{
					if (item != null)
					{
						if (item.areaType == "Remise")
						{
							temp.X = item.X;
							temp.Y = item.Y;
						}
						if (temp.endX != item.X || temp.endY != item.Y)
						{
							if (item.areaType == "Station" && item.whatIsIt == Char.Parse(itemValue))
							{
								temp.endX = item.X;
								temp.endY = item.Y;
								temp.endStationName = itemValue.Last();
							}
						}
					}
				}
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
				//this is the amount of wagons the train has
				temp.amountOfWagons = number;
				//Spawn a train
				trains.Add((Train)fac.GetPerson("Train", temp));
				var cur = (Train)trains.Last();
				cur.startLocation = "R";

				FindPath((Train)trains.Last());


				//Key is amount of wagons
				//Value is startlocation of train
				//Console.WriteLine("Amount of wagons Key is: " + itemKey);
				//Console.WriteLine("Amoubt of wagons Value is: " + itemValue);
			}
			else if (evt.EventType == RailroadEventType.SPAWN_PASSENGER)
			{
				//create new person/passanger
				TempIdentity temp = new TempIdentity();

				temp.areaType = "Person";

				if (itemKey.Last() != itemValue.Last()) {

				//	//look for start loc
				//	temp.Y = layout.FindIndex(x => x.Contains(itemKey.Last()));
				//	temp.X = layout[temp.Y].IndexOf(itemKey.Last());

				//	//look for end loc
				//temp.endY = layout.FindIndex(x => x.Contains(itemValue.Last()));
				//temp.endX = layout[temp.endY].IndexOf(itemValue.Last());
				//temp.endStationName = itemValue.Last();
				}
				//check if begin and end is not the same
				if (itemKey.Last() != itemValue.Last())
				{
					foreach (var item in coordinates)
					{
						if (item != null)
						{
							if (temp.X != item.X || temp.Y != item.Y)
							{
								if (item.areaType == "Station" && item.whatIsIt == itemKey.Last())
								{
									temp.X = item.X;
									temp.Y = item.Y;
								}
							}
							if (temp.endX != item.X || temp.endY != item.Y)
							{
								if (item.areaType == "Station" && item.whatIsIt == itemValue.Last())
								{
									temp.endX = item.X;
									temp.endY = item.Y;
									temp.endStationName = itemValue.Last();
								}
							}

						}
					}
					people.Add((Person)fac.GetPerson("Person", temp));
				}
				else
				{
					Console.WriteLine("Person spawned at their end location");
				}
			}
			else if (evt.EventType == RailroadEventType.CLEANING_EMERGENCY)
			{
				//clean everything, so create maids
				Console.WriteLine("Cleaning emergency Key is: " + itemKey);
				Console.WriteLine("Cleaning emergency Value is: " + itemValue);

				TempIdentity temp = new TempIdentity();
				temp.areaType = "Maid";

				foreach (var item in coordinates)
				{
					if (item != null)
					{
						if (temp.X != item.X || temp.Y != item.Y)
						{
							if (item.areaType == itemKey && item.whatIsIt == itemValue.Last())
							{
								temp.X = item.X;
								temp.Y = item.Y;
								break;
							}
						}
						//if (temp.endX != item.X || temp.endY != item.Y)
						//{
						//	if (item.areaType == "Station" && item.whatIsIt == itemValue.Last())
						//	{
						//		temp.endX = item.X;
						//		temp.endY = item.Y;
						//		temp.endStationName = itemValue.Last();
						//		if (trains.Capacity <= 1)
						//		{
						//			//Spawn a Maid
						//			people.Add((Maid)fac.GetPerson("Person", temp));
						//		}
						//	}
						//}
					}					
				}

				//add the maid to the people list
				people.Add((Maid)fac.GetPerson("Maid", temp));
				Console.WriteLine("Maid spawned at loc: " + itemValue);

			}
			else if (evt.EventType == RailroadEventType.LEAVES_ON_TRACK)
			{
			
				//Slow down RRTE factor by half
				//Key is duration
				//Value is how many seconds and then normal again
				Console.WriteLine("Leaves on track Key is: " + itemKey);
				Console.WriteLine("Leaves on track Value is: " + itemValue);

				int number = 0;
				string extract = "";
				//this is a string now, maybe convert the string to int
				//then we have to search for the number in the string
				for (int i = 0; i < itemValue.Length; i++)
				{
					if (Char.IsDigit(itemValue[i]))
						extract += itemValue[i];
				}

				if (extract.Length > 0)
				{
					number = int.Parse(extract);
				}
				Leaves = new KeyValuePair<int, bool>(number, true);

			}
			else if (evt.EventType == RailroadEventType.RETIRE_TRAIN)
			{
				//start pathfinding at station itemKey

				//Key is where to go from?
				//value is null
				Console.WriteLine("Retire Key is: " + itemKey);
				Console.WriteLine("Retire Value is: " + itemValue);
				TempIdentity temp = new TempIdentity();

				foreach (var item in coordinates)
				{
					if (item != null)
					{
						if (item.areaType == "Remise")
						{
							temp.endX = item.X;
							temp.endY = item.Y;
							break;
						}
					}
				}
				//set keyvaluepair use this to determine which train has to come back to remise
				//ReturnToRemisePair = new KeyValuePair<char, bool>(Char.Parse(itemKey), true);


			}
		}
		public void ReturnToRemise(Train train)
		{
			foreach (var item in coordinates)
			{
				if (item != null)
				{
					if (item.areaType == "Remise")
					{
						train.endX = item.X;
						train.endY = item.Y;
						break;
					}
				}
			}
			if (train.hasPath == false) {
				FindPath(train);
			}
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

