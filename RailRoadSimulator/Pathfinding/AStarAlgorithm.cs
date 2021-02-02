using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadSimulator.Pathfinding
{

	public class PathFinding
	{
		//final list to go
		List<Tile> finalList;
		LayoutFactory fac = new LayoutFactory();
		public List<string> list { get; set; }
		public PathFinding()
		{
			fac.GenerateEntity();
		}
		/// <summary>
		/// This finds the path
		/// </summary>
		/// <param name="map">the layout of the map</param>
		/// <param name="train">startlocation</param>
		/// <param name="endLoc">endlocation</param>
		public List<Tile> findTiles(List<string> map, Train train)
		{
			finalList = new List<Tile>();
			list = new List<string>();

			
			foreach (var item in fac.layout)
			{
				if (!item.isOccupied && item.isDubbelTrack)
				{
					list.Add(item.whatIsIt.ToString());
				}
			}

			//MAKE A LIST with List<string> innnit and a bool isoccupied and bool isdubbeltrack
			//Use this to set all the isoccupied at each specific item

			//This gets the start coordinates of the train
			var start = new Tile();
			start.X = train.X;
			start.Y = train.Y;

			//start.Y = map.FindIndex(x => x.Contains(train.startLocation));
			//start.X = map[start.Y].IndexOf(train.startLocation);

			//This gets the end coordinates of the train
			var finish = new Tile();
			finish.X = train.endX;
			finish.Y = train.endY;

			//finish.Y = map.FindIndex(x => x.Contains(train.destination));
			//finish.X = map[finish.Y].IndexOf(train.destination);

			start.SetDistance(finish.X, finish.Y);

			var activeTiles = new List<Tile>();
			activeTiles.Add(start);
			var visitedTiles = new List<Tile>();

			while (activeTiles.Any())
			{
				//CHeck here if tile isoccupied and is not dubbeltrack
				var checkTile = activeTiles.OrderBy(x => x.CostDistance).First();

				if (checkTile.X == finish.X && checkTile.Y == finish.Y)
				{
					//We found the destination and we can be sure (Because the the OrderBy above)
					//That it's the most low cost option. 
					var tile = checkTile;
					//Console.WriteLine("Retracing steps backwards...");
					while (true)
					{
						//Console.WriteLine($"{tile.X} : {tile.Y}");
						if (map[tile.Y][tile.X] != start.whatIsIt || map[tile.Y][tile.X] != ' ' || map[tile.Y][tile.X] != finish.whatIsIt)
						{
							//convert the tiles with their coordinates to the right path
							var newMapRow = map[tile.Y].ToCharArray();
							newMapRow[tile.X] = '*';
							map[tile.Y] = new string(newMapRow);
						}
						//set isOccupied to true because this path will now be occupied
						tile.isOccupied = true;
						finalList.Add(tile);
						tile = tile.Parent;
						if (tile == null)
						{
							//reverse the final list to ensure it is from the start to the end 
							finalList.Reverse();
							Console.WriteLine("Train has path, will go to " + finish.X.ToString() + finish.Y.ToString());
							//map.ForEach(x => Console.WriteLine(x));

							//foreach (var item in finalList.ToList()) {
							//	foreach (var list in fac.layout.ToList()) {
							//		if (list.X == item.X && list.Y == item.Y) {
							//			fac.layout.Remove(list);
							//			fac.layout.Add(item);
							//		}
							//	}
							//}

							return finalList;
						}
					}
				}

				visitedTiles.Add(checkTile);
				activeTiles.Remove(checkTile);



				var walkableTiles = GetWalkableTiles(map, checkTile, finish);

				foreach (var walkableTile in walkableTiles.ToList())
				{
					//We have already visited this tile so we don't need to do so again!
					if (visitedTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
						continue;

					//It's already in the active list, but that's OK, maybe this new tile has a better value (e.g. We might zigzag earlier but this is now straighter). 
					if (activeTiles.Any(x => x.X == walkableTile.X && x.Y == walkableTile.Y))
					{
						var existingTile = activeTiles.First(x => x.X == walkableTile.X && x.Y == walkableTile.Y);
						if (existingTile.CostDistance > checkTile.CostDistance)
						{
							activeTiles.Remove(existingTile);
							activeTiles.Add(walkableTile);
						}
					}
					else
					{
						//We've never seen this tile before so add it to the list. 
						activeTiles.Add(walkableTile);
					}
				}
			}
			Console.WriteLine("No Path Found!");
			return null;

		}
		/// <summary>
		/// Gets possible tiles to go to
		/// </summary>
		/// <param name="map">the layout</param>
		/// <param name="currentTile">tile where currenly is</param>
		/// <param name="targetTile">target tile to go to</param>
		/// <param name="endLoc">end location</param>
		/// <returns>next possible tiles in a list</returns>
		private static List<Tile> GetWalkableTiles(List<string> map, Tile currentTile, Tile targetTile)
		{
			
			var possibleTiles = new List<Tile>()
			{
				new Tile { X = currentTile.X, Y = currentTile.Y - 1, Parent = currentTile, Cost = currentTile.Cost + 1 },
				new Tile { X = currentTile.X, Y = currentTile.Y + 1, Parent = currentTile, Cost = currentTile.Cost + 1 },
				new Tile { X = currentTile.X - 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1 },
				new Tile { X = currentTile.X + 1, Y = currentTile.Y, Parent = currentTile, Cost = currentTile.Cost + 1 },
			};

			possibleTiles.ForEach(tile => tile.SetDistance(targetTile.X, targetTile.Y));

			var maxX = map.First().Length - 1;
			var maxY = map.Count - 1;
			//This are the ways a train can go
			return possibleTiles
					.Where(tile => tile.X >= 0 && tile.X <= maxX)
					.Where(tile => tile.Y >= 0 && tile.Y <= maxY)
					.Where(tile => map[tile.Y][tile.X] != ' ')
					.ToList();
		}
	}
	}


