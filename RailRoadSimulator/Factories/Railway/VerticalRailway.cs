using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Drawing.Railway
{
    class VerticalRailway : IRailway
    {
        public VerticalRailway()
        {
            model = Image.FromFile(@"..\..\Assets\location.png");
        }
    }
}
