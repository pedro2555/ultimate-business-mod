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
        private void Msg(string message, int time)
        {
            GTA.Native.Function.Call(
                "PRINT_STRING_WITH_LITERAL_STRING_NOW",
                "STRING",
                message,
                time,
                true);
        }

        private Vehicle Controller;
        private Blip ControllerBlip;

        public TowTruck()
        {
            Controller = null;
            ControllerBlip = null;

            this.BindKey(Keys.Q, SpawnPacker);

            this.Interval = 1000;
            this.Tick += new EventHandler(TowTruck_Tick);

            this.PerFrameDrawing += new GraphicsEventHandler(TowTruck_PerFrameDrawing);
        }

        void TowTruck_PerFrameDrawing(object sender, GraphicsEventArgs e)
        {
            if (Player.Character.isInVehicle())
            {
                if (Player.Character.CurrentVehicle.Speed < 1)
                {
                    if (Player.Character.CurrentVehicle.Model.Hash == 569305213) // packer
                    {
                        if (Controller == null)
                            Msg("Hold ~INPUT_FRONTEND_LB~ to grab the Tow Truck Controller.", 500);
                    }
                }
                else if (Controller != null && Player.Character.CurrentVehicle == Controller)
                {
                    Controller.Extras(5).Enabled = false;
                    Controller = null;
                    ControllerBlip.Delete();
                    ControllerBlip = null;
                }
            }
        }

        void TowTruck_Tick(object sender, EventArgs e)
        {
            if (Player.Character.isInVehicle())
            {
                if (Player.Character.CurrentVehicle.Speed < 1 && Player.Character.CurrentVehicle.Model.Hash == 569305213 && Game.isGameKeyPressed(GameKey.Action) && Controller == null)
                {
                    Controller = Player.Character.CurrentVehicle;
                    ControllerBlip = Player.Character.CurrentVehicle.AttachBlip();
                }
            }
            else
            {
                if (Controller != null && Game.isGameKeyPressed(GameKey.Action))
                {
                    Vehicle nearestVehicle = World.GetClosestVehicle(Player.Character.Position, 10);
                    if (nearestVehicle.Exists())
                    {
                        if (nearestVehicle != Controller)
                        {
                            GTA.Native.Function.Call("ATTACH_CAR_TO_CAR", nearestVehicle, Controller, false, 0f, Settings.GetValueFloat("Y", -.8f), Settings.GetValueFloat("Z", .84f), 0f, 0f, 0f);
                            Controller.Metadata.VehicleA = nearestVehicle;
                        }
                        else
                        {
                            Vehicle VehicleA = null;
                            try { VehicleA = Controller.Metadata.VehicleA ;}
                            catch (Exception) {}
                            if (Controller.Metadata.VehicleA != null && Controller.Metadata.VehicleA.Exists())
                            {

                                Controller.Extras(5).Enabled = true;
                                GTA.Native.Function.Call("DETACH_CAR", VehicleA);
                                
                                Wait(2);
                                VehicleA.FreezePosition = true;
                                Wait(2);
                                VehicleA.FreezePosition = false;
                                Controller.Metadata.VehicleA = null;
                            }
                        }
                    }
                    else
                        Msg("Get next to a vehicle.", 3000);
                }
            }
        }



        public void SpawnPacker()
        {
            try
            {
                Vehicle TowTruckVehicle = null;
                TowTruckVehicle = World.CreateVehicle(new Model("packer"), World.GetNextPositionOnStreet(Player.Character.Position));
                while (!TowTruckVehicle.Exists())
                    Wait(100);
                TowTruckVehicle.Extras(1).Enabled = false; // liquid tank
                TowTruckVehicle.Extras(3).Enabled = false; // double tower
                TowTruckVehicle.Extras(5).Enabled = false; // unloading ramp
            }
            catch (Exception)
            {
                Game.DisplayText("Didn't work", 1000);
            }
        }
    }
}
