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
        Vehicle LastVehicle;
        bool isVehicleAttached;

        public TowTruck()
        {
            TowTruckVehicle = null;
            LastVehicle = null;
            isVehicleAttached = false;

            this.BindKey(Keys.Q, SpawnPacker);
            this.BindKey(Keys.Z, AttachVehicle);
        }



        public void SpawnPacker()
        {
            try
            {
                this.TowTruckVehicle = World.CreateVehicle(new Model("packer"), World.GetNextPositionOnStreet(Player.Character.Position));
                while (!this.TowTruckVehicle.Exists())
                    Wait(100);
                this.TowTruckVehicle.Extras(1).Enabled = false; // liquid tank
                this.TowTruckVehicle.Extras(3).Enabled = false; // double tower
                this.TowTruckVehicle.Extras(5).Enabled = false; // unloading ramp
            }
            catch (Exception)
            {
                Game.DisplayText("Didn't work", 1000);
            }
        }

        public bool isPlayerInValidPacker()
        {
            if (Player.Character.isInVehicle())
                if (Player.Character.CurrentVehicle.Model.Hash == 569305213 && !Player.Character.CurrentVehicle.Extras(1).Enabled)
                    return true;
            return false;
        }

        public bool canPackerTowTwoCars()
        {
             return Player.Character.CurrentVehicle.Extras(3).Enabled;
        }

        Vehicle towed;
        public void AttachVehicle()
        {
            if (isPlayerInValidPacker())
            {
                if (canPackerTowTwoCars())
                {
                    // two car tower
                }
                else
                {
                    try { bool b = Player.Character.CurrentVehicle.Metadata.isVehicleIn; }
                    catch (Exception crap)
                    {
                        Player.Character.CurrentVehicle.Metadata.isVehicleIn = false;
                        Player.Character.CurrentVehicle.Metadata.VehicleIn = null;
                    }

                    if (Player.Character.CurrentVehicle.Metadata.isVehicleIn && !Player.Character.CurrentVehicle.Extras(5).Enabled)
                    {
                        // a vehicle is attached, detach it
                        GTA.Native.Function.Call("DETACH_CAR", Player.Character.CurrentVehicle.Metadata.VehicleIn);
                        Player.Character.CurrentVehicle.Extras(5).Enabled = true;
                    }
                    else if (Player.Character.CurrentVehicle.Extras(5).Enabled)
                    {
                        Player.Character.CurrentVehicle.Extras(5).Enabled = false;
                        Player.Character.CurrentVehicle.Metadata.isVehicleIn = false;
                    }
                    else
                    {
                        float dist = 10.0f;
                        foreach (Vehicle v in World.GetAllVehicles())
                        {
                            if (v.Exists() && v != Player.Character.CurrentVehicle)
                                if (v.Position.DistanceTo2D(Player.Character.CurrentVehicle.Position) < dist)
                                {
                                    Player.Character.CurrentVehicle.Metadata.VehicleIn = v;
                                    dist = v.Position.DistanceTo2D(Player.Character.CurrentVehicle.Position);
                                }
                        }
                        if (Player.Character.CurrentVehicle.Metadata.VehicleIn.Position.DistanceTo2D(Player.Character.CurrentVehicle.Position) > 10.0f)
                        {
                            Player.Character.CurrentVehicle.Metadata.VehicleIn = null;
                            Player.Character.CurrentVehicle.Metadata.isVehicleIn = null;
                        }
                        else
                        {
                            GTA.Native.Function.Call("ATTACH_CAR_TO_CAR", Player.Character.CurrentVehicle.Metadata.VehicleIn, Player.Character.CurrentVehicle, false, 0f, Settings.GetValueFloat("Y", -.8f), Settings.GetValueFloat("Z", .84f), 0f, 0f, 0f);
                            Player.Character.CurrentVehicle.Metadata.isVehicleIn = true;
                        }

                    }
                }
            }
        }

        
    }
}
