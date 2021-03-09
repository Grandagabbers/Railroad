using System;
using System.Collections.Generic;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RailRoadSimulator;

namespace RailUnitTests
{
	[TestClass]
	public class LayoutFactoryUnitTests
	{
		public LayoutFactory fac = new LayoutFactory();

		//checks if layout is correct
		[TestMethod]
		public void LayoutReadOutOfTrcFile()
		{
			//arrange
			LayoutFactory fac = new LayoutFactory();
			fac.GenerateEntity();
			//act
			List<ILayout> output = fac.layout;
			//assert
			Assert.AreEqual(output, fac.layout);
		}
		/// <summary>
		/// checks if amount of items in list actually are in the list
		/// </summary>
		[TestMethod]
		public void AreThereAsMuchItemsInTheListAsThereReallyAre()
		{
			//arrange
			fac.GenerateEntity();
			//act
			List<ILayout> output = fac.layout;
			//assert
			Assert.AreEqual(output.Count, fac.layout.Count);
		}

		//checks if items are places correctS
		[TestMethod]
		public void CheckIfCreateOverviewTheItemInTheCorrectSpotSets()
		{
			//arrange
			LayoutFactory fac = new LayoutFactory();
			fac.GenerateEntity();


			//act
			ILayout item = fac.coordinates[9, 16];
			Point position = new Point(item.X, item.Y);
			Point actualPosition = new Point(9, 16);

			//assert
			Assert.AreEqual(actualPosition, position);
		}
	}
}
