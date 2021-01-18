using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drawing.Railway
{
    class Station : IRailway
    {
        public Station()
        {
            model = Image.FromFile(@"..\..\Assets\Station.png");
        }
    }
}
