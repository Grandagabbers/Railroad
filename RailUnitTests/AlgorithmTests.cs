using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RailRoadSimulator;
using RailRoadSimulator.Pathfinding;

namespace RailUnitTests
{
	[TestClass]
	public class AlgorithmTests
	{
        /// <summary>
        /// checks if algorithm returns a path from start to end
        /// </summary>
        [TestMethod]
        public void DoesDijkstraSendToPrefRoom()
        {
            //arrange
            TempIdentity temp = new TempIdentity();
            temp.areaType = "Train";

            temp.X = 5;
            temp.Y = 9;

            temp.endX = 9;
            temp.endY = 16;
            temp.endStationName = 'B';
            temp.amountOfWagons = 1;
            

            PathFinding aStar = new PathFinding();
            LayoutFactory fac = new LayoutFactory();
            fac.GenerateEntity();
            
            Train train = new Train(temp);

            Point actual = new Point(9, 16);
            //act
            List<Tile> path = aStar.findTiles(fac.finalLay, train);
            Tile last = path.Last();
            Point lastPoint = new Point(last.X, last.Y);
            //assert
            Assert.AreEqual(actual, lastPoint);
        }
    }
}
