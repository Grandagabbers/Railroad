using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RailRoadSimulator
{
	public class Maid : IEntity
	{
        public bool busy { get; set; } //store if the maid is busy with something
        /// <summary>
        /// copy the tempPerson and create a maid
        /// </summary>
        /// <param name="tempPerson">the tempperson which becomes a maid</param>
        public Maid(TempIdentity temp)
        {
            areaType = temp.areaType;
            //this.id = temp.id;
            //this.busy = temp.busy;
            X = temp.X;
            Y = temp.Y;
            model = Image.FromFile(@"..\..\Assets\maid.png");//the model
            model.RotateFlip(RotateFlipType.Rotate180FlipX);//rotate so its displayed correctly
        }
    }
}
