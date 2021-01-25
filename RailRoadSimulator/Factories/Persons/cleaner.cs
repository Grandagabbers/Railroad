using Drawing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadSimulator.Factories.Persons
{
    class Cleaner : IPerson
    {
        public Cleaner()
        {
            modelPerson = Image.FromFile(@"..\..\Assets\Cleaner.png");
        }
    }
}
