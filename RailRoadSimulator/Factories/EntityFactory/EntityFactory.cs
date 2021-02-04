using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadSimulator
{
	public class EntityFactory : AbstractFactory
	{
		public void GenerateEntity()
		{
		}

        /// <summary>
        /// Makes a person and based on the variables it will be either a maid or a person.
        /// </summary>
        /// <param name="product">What person it's gonna be.</param>
        /// <param name="tempPerson">The temperory variables for the person that is about to be created.</param>
        /// <returns>Either a maid or a person.</returns>
        public IEntity GetPerson(string product, TempIdentity tempPerson)
        {
            Type type = Type.GetType(product);
            if (type != null)
            {
                return (IEntity)Activator.CreateInstance(type);
            }
            else
            {
                string nspace = "RailRoadSimulator";

                //qeury for all the types
                var q = from x in Assembly.GetExecutingAssembly().GetTypes()
                        where x.IsClass && x.Namespace == nspace
                        select x;

                List<string> types = new List<string>();
                //put the query in the list
                foreach (Type t in q)
                {
                    types.Add(t.ToString());
                }
                //search the list and if found return instance. 
                foreach (string t in types)
                {
                    if (t.Contains(product))
                    {
                        type = Type.GetType(t);
                        return (IEntity)Activator.CreateInstance(type, tempPerson);
                    }
                }

            }
            return null;
        }

    }
}

