using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTA;
using System.Windows.Forms;

namespace TowTruck
{
    public class TowTruck : Script
    {
        Vehicle TowTruckVehicle;

        TowTruck()
        {
            this.BindKey(Keys.Q, SpawnPacker);
        }


        public void SpawnPacker()
        {
            this.TowTruckVehicle = World.CreateVehicle(new Model("packer"), World.GetNextPositionOnStreet(Player.Character.Position));
        }
    }
}
