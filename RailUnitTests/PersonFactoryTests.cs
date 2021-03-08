using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RailRoadSimulator;

namespace RailUnitTests
{
	[TestClass]
	public class PersonFactoryTests
	{
        [TestMethod]
        public void DoesFactoryReturnMaidsAndEntity()
        {
            //arrange
            EntityFactory personFactory = new EntityFactory();
            TempIdentity tempMaid = new TempIdentity();
            TempIdentity tempEntity = new TempIdentity();
            string resultMaid;
            string resultCustomer;

            //act
            IEntity maid = personFactory.GetPerson("Maid", tempMaid);
            IEntity person = personFactory.GetPerson("Person", tempEntity);
            Type typeMaid = maid.GetType();
            Type typeCostumer = person.GetType();
            resultMaid = typeMaid.Name.ToString();
            resultCustomer = typeCostumer.Name.ToString();

            //assert
            Assert.AreEqual("Maid", resultMaid);
            Assert.AreEqual("Person", resultCustomer);
        }
    }
}
