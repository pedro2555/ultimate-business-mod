using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GTA;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Diagnostics;

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

            // Get assembly version
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(Game.InstallFolder + "\\scripts\\TowTruck.net.dll");
            string version = fvi.FileVersion;
            Game.Console.Print("Started TowTruckScript v" + version);
        }

        void TowTruck_PerFrameDrawing(object sender, GraphicsEventArgs e)
        {
            // Display help message when player is a valid vehicle in valid situation
            if (Player.Character.isInVehicle())
            {
                // Make sure the vehicle is not moving
                if (Player.Character.CurrentVehicle.Speed < 0.025 && Player.Character.CurrentVehicle.CurrentRPM < 0.21)
                {
                    // Make sure the vehicle fits all the required models
                    if (Player.Character.CurrentVehicle.Model.Hash == 569305213) // packer
                    {
                        // check if the player has already grabed the controller.
                        if (Controller == null)
                            Msg("Hold ~INPUT_FRONTEND_LB~ to grab the ~y~Tow Truck Controller~w~.", 100);
                    }
                }
            }
        }

        void TowTruck_Tick(object sender, EventArgs e)
        {
            // Imediatly making sure the player is not to far away with a controller on hand
            if (Controller != null && Player.Character.Position.DistanceTo(Controller.Position) > 30)
            {
                Game.DisplayText("Killed 1", 1000);
                Controller.Extras(5).Enabled = false;
                Controller = null;
                ControllerBlip.Delete();
                ControllerBlip = null;
            }
            if (Player.Character.isInVehicle())
            {
                // Check if player got back to the Tow Truck and remove the controller from hand
                if (Controller != null && Player.Character.CurrentVehicle == Controller && Player.Character.CurrentVehicle.Speed > 1)
                {
                    Game.DisplayText("Killed 1", 1000);
                    Controller.Extras(5).Enabled = false;
                    Controller = null;
                    ControllerBlip.Delete();
                    ControllerBlip = null;
                }
                // Check for the key press and all required situations, must meet the help message
                if (Player.Character.CurrentVehicle.Speed < 0.025 && Player.Character.CurrentVehicle.CurrentRPM < 0.21 && Player.Character.CurrentVehicle.Model.Hash == 569305213 && Game.isGameKeyPressed(GameKey.Action) && Controller == null)
                {
                    Controller = Player.Character.CurrentVehicle;
                    ControllerBlip = Controller.AttachBlip();
                    ControllerBlip.Color = BlipColor.Yellow;
                    ControllerBlip.Display = BlipDisplay.ArrowOnly;
                    ControllerBlip.Scale = 2.0f;
                    Player.Character.Task.ClearAll();
                    Wait(5);
                    Player.Character.Task.LeaveVehicle(Controller, false);
                }
            }
            else
            {
                // Test if the player has the controller in hand and pressing the action key to tow the car
                if (Controller != null && Game.isGameKeyPressed(GameKey.Action))
                {
                    // Grab the nearest vehicle
                    Vehicle nearestVehicle = World.GetClosestVehicle(Player.Character.Position, 10);
                    if (nearestVehicle.Exists())
                    {
                        if (nearestVehicle != Controller)
                        {
                            AttachCar(Controller, nearestVehicle);
                        }
                        else
                        {
                            DetachCar(Controller);
                        }
                    }
                    else
                        Msg("Get next to a vehicle.", 3000);
                }
            }
        }

        public bool AttachCar(Vehicle TowTruck, Vehicle Car)
        {
            if (TowTruck.Exists() && Car.Exists())
            {
                if (TowTruck.Extras(3).Enabled)
                {
                    // Two car truck
                    Vehicle VehicleA = null;
                    VehicleA = Controller.Metadata.VehicleA;
                    if (VehicleA == null)
                    {
                        GTA.Native.Function.Call("ATTACH_CAR_TO_CAR", Car, Controller, false, 
                            Settings.GetValueFloat("XOffset", "002", 0.0f), 
                            Settings.GetValueFloat("YOffset", "002", 0.0f), 
                            Settings.GetValueFloat("ZOffset", "002", 0.0f), 
                            Settings.GetValueFloat("XRotation", "002", 0.0f), 
                            Settings.GetValueFloat("YRotation", "002", 0.0f), 
                            Settings.GetValueFloat("ZRotation", "002", 0.0f));
                        TowTruck.Metadata.VehicleA = Car;
                        return true;
                    } 
                    Vehicle VehicleB = null;
                    VehicleB = Controller.Metadata.VehicleB;
                    if (VehicleB == null)
                    {
                        GTA.Native.Function.Call("ATTACH_CAR_TO_CAR", Car, Controller, false,
                            Settings.GetValueFloat("XOffset", "003", 0.0f),
                            Settings.GetValueFloat("YOffset", "003", 0.0f),
                            Settings.GetValueFloat("ZOffset", "003", 0.0f),
                            Settings.GetValueFloat("XRotation", "003", 0.0f),
                            Settings.GetValueFloat("YRotation", "003", 0.0f),
                            Settings.GetValueFloat("ZRotation", "003", 0.0f));
                        TowTruck.Metadata.VehicleB = Car;
                        return true;
                    }
                }
                else
                {
                    // One car truck
                    // Attach this vehicle to the available slot in the Tow Truck or do nothing
                    try { Vehicle VehicleA = Controller.Metadata.VehicleA; }
                    catch (Exception)
                    {
                        GTA.Native.Function.Call("ATTACH_CAR_TO_CAR", Car, Controller, false,
                            Settings.GetValueFloat("XOffset", "001", 0.0f),
                            Settings.GetValueFloat("YOffset", "001", 0.0f),
                            Settings.GetValueFloat("ZOffset", "001", 0.0f),
                            Settings.GetValueFloat("XRotation", "001", 0.0f),
                            Settings.GetValueFloat("YRotation", "001", 0.0f),
                            Settings.GetValueFloat("ZRotation", "001", 0.0f));
                        TowTruck.Metadata.VehicleA = Car;
                        return true;
                    }
                }
            }
            return false;
        }

        public bool DetachCar(Vehicle TowTruck)
        {
            if (TowTruck.Exists())
            {
                if (TowTruck.Extras(3).Enabled)
                {
                    // Two car truck
                    Vehicle VehicleA = null;
                    VehicleA = Controller.Metadata.VehicleA;
                    if (VehicleA != null)
                    {
                        Controller.Extras(5).Enabled = true;
                        GTA.Native.Function.Call("DETACH_CAR", VehicleA);
                        Controller.Metadata.VehicleA = null;
                        VehicleA.PlaceOnGroundProperly();
                        return true;
                    }
                    Vehicle VehicleB = null;
                    VehicleB = Controller.Metadata.VehicleB;
                    if (VehicleB != null)
                    {
                        Controller.Extras(5).Enabled = true;
                        GTA.Native.Function.Call("DETACH_CAR", VehicleB);
                        Controller.Metadata.VehicleB = null;
                        VehicleB.PlaceOnGroundProperly();
                        return true;
                    }
                }
                else
                {
                    Vehicle VehicleA = null;
                    try { VehicleA = Controller.Metadata.VehicleA; }
                    catch (Exception) { return false; }
                    Controller.Extras(5).Enabled = true;
                    GTA.Native.Function.Call("DETACH_CAR", VehicleA);
                    Controller.Metadata.VehicleA = null;
                    VehicleA.PlaceOnGroundProperly();
                    return true;
                }
            }
            return false;
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
                TowTruckVehicle.Extras(3).Enabled = true; // double tower
                TowTruckVehicle.Extras(5).Enabled = false; // unloading ramp
            }
            catch (Exception)
            {
                Game.DisplayText("Didn't work", 1000);
            }
        }
    }
}
