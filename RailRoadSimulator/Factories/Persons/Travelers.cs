using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drawing
{
    class Travelers : IPerson
    {
        public Travelers()
        {
            modelPerson = Image.FromFile(@"..\..\Assets\location.png");
        }
    }
}
