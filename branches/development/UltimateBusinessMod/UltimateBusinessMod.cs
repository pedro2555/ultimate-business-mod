/// Copyright (c) 2013, Pedro Rodrigues <prodrigues1990@gmail.com>
/// All rights reserved.
/// 
/// Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
/// 
/// 	-Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
/// 	-Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
/// 	-Neither the name of the author nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
/// 
/// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GTA;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace UltimateBusinessMod
{
    public class UltimateBusinessMod : Script
    {
        #region internal use functions
        internal void Msg(string message, int time)
        {
            GTA.Native.Function.Call(
                "PRINT_STRING_WITH_LITERAL_STRING_NOW",
                "STRING",
                message,
                time,
                true);
        }

        #endregion
        #region internal use properties

        public static Property ProximityProperty
        {
            get
            {
                return Properties[ProximityPropertyID];
            }
        }
        /// <summary>
        /// All 15 mission complete audio ids
        /// TRIGGER_MISSION_COMPLETE_AUDIO
        /// </summary>
        internal int[] buy_audio_list = { 6, 7, 10, 11, 15, 18, 24, 25, 27, 28, 33, 34, 35, 42, 43, 50, 51, 52, 53, 54, 55, 56, 57, 58, 59, 60, 61, 62, 63, 64, 65, 66, 67, 68, 69, 71 };
        /// <summary>
        /// Gets if the Property ID returned by the PropertyProximity
        /// -1 if the player is not close to a property
        /// </summary>
        public static int ProximityPropertyID;
        /// <summary>
        /// Defines if Property Manager is open
        /// </summary>
        public static bool isManagerOpen
        { get; set; }
        /// <summary>
        /// List of Property locations and specific data
        /// </summary>
        public static Property[] Properties;
        /// <summary>
        /// List of PropertyTypes
        /// </summary>
        public static PropertyType[] Types;
        #endregion

        #region database functions
        /// <summary>
        /// Checks scripts folder for UltimateBusinessMod.s3db
        /// </summary>
        /// <returns></returns>
        public bool CheckForDatabase()
        {
            if (!File.Exists(Game.InstallFolder + "\\scripts\\" + Settings.GetValueString("databasefile", "GENERAL", "UltimateBusinessMod.s3db")))
                return false;
            else
                Database.SetDatabase(Game.InstallFolder + "\\scripts\\" + Settings.GetValueString("databasefile", "GENERAL", "UltimateBusinessMod.s3db"));
            return true;
        }
        #endregion

        private PropertyManagerForm ManagerFrm;

        /// <summary>
        /// The script entry point
        /// </summary>
        public UltimateBusinessMod()
        {
            Log("Script load started", DateTime.Now.ToString("hh:mm:ss.fff tt"));

            #region property init
            ProximityPropertyID = -1;
            #endregion
            #region check for database file, abort script if not found
            LogFile.Path = Game.InstallFolder + "\\scripts\\UltimateBusinessMod.log";
            // check for database file
            if (!CheckForDatabase())
            {
                Game.DisplayText("Ultimate Business Mod Database file could not be found. Please re-install the mod.", 3000);
                Wait(3000);
                Abort();
            }
            #endregion
            #region logging script start
            // Get assembly version
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(Game.InstallFolder + "\\scripts\\UltimateBusinessMod.net.dll");
            string version = fvi.FileVersion;
            // log start
            Log("UltimateBusinessModv" + version, "Started under GTA " + Game.Version.ToString());
            Log("UltimateBusinessModv" + version, "dsound.dll " + ((File.Exists(Game.InstallFolder + "\\dsound.dll")) ? "present" : "not present"));
            Log("UltimateBusinessModv" + version, "xlive.dll " + ((File.Exists(Game.InstallFolder + "\\xlive.dll")) ? "present" : "not present"));
            Log("UltimateBusinessModv" + version, "OS Version: " + getOSInfo());
            #endregion


            // Start loading database records
            Properties = Property.GetPropertiesList();

            // wait for map to load
            Wait(6000);

            #region Load database data and project it on the map
            // create blips for each property
            foreach (Property p in Properties)
            {
                try
                {
                    // add a small invisible object on the property locatyion
                    GTA.Object apple = World.CreateObject(new Model("amb_apple_1"), p.Location);
                    apple.Visible = false;
                    apple.Collision = false;
                    // attach a blip for that object
                    Blip b = apple.AttachBlip();
                    b.Icon = (p.Owned) ? (BlipIcon)80 : (BlipIcon)0;
                    b.Name = p.Name;
                    b.Scale = .7f;
                    b.ShowOnlyWhenNear = true;
                    // keep the blip along with the property instance
                    p.blip = b;
                }
                catch (Exception crap) { Log("construct - foreach - blip drawing", crap.Message); }
            }
            #endregion
            Wait(500);
            // start our one second timer, let's see how it proves itself with more db records
            this.Interval = 1000;
            this.Tick += new EventHandler(UltimateBusinessMod_Tick);

            // update thread initialization
            Thread updateThread = new Thread(new ThreadStart(RealTimeUpdater));
            


// We are in DEBUG mode let's print usefull info
#if DEBUG
            this.PerFrameDrawing += new GraphicsEventHandler(UltimateBusinessMod_DebugPerFrameDrawing);
#endif
            // Initialize manager
            ManagerFrm = new PropertyManagerForm(Game.Resolution, Player);

            Log("Script load finished", DateTime.Now.ToString("hh:mm:ss.fff tt"));

            Wait(500);

            updateThread.Start();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void PropertyManagerFrm_Closed(object sender, EventArgs e)
        {
            isManagerOpen = false;
            //Game.Unpause();
        }


        #region methods
        /// <summary>
        /// Updates property income and costs over time
        /// 
        /// intended to run every 5 minutes of game time.
        /// </summary>
        public void RealTimeUpdater()
        {
            while (true)
            {
                try
                {
                    // Salary payment
                    if (!isManagerOpen && World.CurrentDayTime.Minutes % 5 == 0/*World.CurrentDate.DayOfWeek == DayOfWeek.Monday*/)
                    {
                        int index = 0;
                        foreach (Property p in UltimateBusinessMod.Properties)
                        {
                            if (p.StaffCount > 0)
                            {
                                int totalpayment = (p.StaffSal * p.StaffCount);
                                // check for available Player money
                                if (totalpayment > Player.Money)
                                {
                                    // Player can't pay staff
                                    UltimateBusinessMod.Properties[index].StaffPayed = false;
                                    UltimateBusinessMod.Properties[index].UpdateFlags();
                                    GTA.Native.Function.Call("DISPLAY_CASH", true);
                                    Msg(String.Format("Salaries from {0} haven't been payed.", p.Name), 1000);
                                }
                                else
                                {
                                    // Payment is successfull
                                    Player.Money -= totalpayment;
                                    GTA.Native.Function.Call("DISPLAY_CASH", true);
                                    if (!UltimateBusinessMod.Properties[index].StaffPayed)
                                    {
                                        UltimateBusinessMod.Properties[index].StaffPayed = true;
                                        UltimateBusinessMod.Properties[index].UpdateFlags();
                                    }
                                }
                            }
                            index++;
                        }
                    }
                }
                catch (Exception crap) { Log("Updater Thread", crap.Message); }
            }
        }

        #endregion

        #region main script events

        /// <summary>
        /// DEBUG on-screen info display
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UltimateBusinessMod_DebugPerFrameDrawing(object sender, GraphicsEventArgs e)
        {
            e.Graphics.Scaling = FontScaling.ScreenUnits;
            e.Graphics.DrawText(Game.FPS.ToString(), .9f, .87f);
            e.Graphics.DrawText(ProximityPropertyID.ToString(), .9f, .9f);
        }

        /// <summary>
        /// Main script tick
        /// 
        /// Handles PropertyProximity and Key handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void UltimateBusinessMod_Tick(object sender, EventArgs e)
        {
            // check enviroment state, player in combat, in water, dead, in vehicle, etc.., if player meets any of the bellow criteria PropertyProximity and KeyHandler should not work
            if (isManagerOpen || Player.Character.isInVehicle() || Player.Character.isDead || Player.Character.isGettingIntoAVehicle || Player.Character.isGettingUp || Player.Character.isInCombat || Player.Character.isInMeleeCombat || Player.Character.isInWater || Player.Character.isOnFire || Player.Character.isRagdoll || Player.Character.isShooting || Player.WantedLevel > 0)
                return;
            // Key handler
            // if Player is next to a property holding the Action key
            if (ProximityPropertyID != -1 && Game.isGameKeyPressed(GameKey.Action))
            {
                // if Player doesn't own the property and has money to buy it
                if (!ProximityProperty.Owned && Player.Money >= ProximityProperty.Cost)
                {
                    // buy the property
                    // avoid whistling at cabs
                    Player.Character.Task.ClearAllImmediately();
                    // set all need data in property instance
                    ProximityProperty.Owned = true;
                    ProximityProperty.blip.Icon = (BlipIcon)80;
                    // refresh the blip name, for some reason it gets change to the icon default name
                    ProximityProperty.blip.Name = ProximityProperty.Name;
                    // write the change to the database
                    ProximityProperty.UpdateFlags();
                    // trigger a random mission complete audio and notify the player
                    GTA.Native.Function.Call("TRIGGER_MISSION_COMPLETE_AUDIO", buy_audio_list[new Random().Next(0, buy_audio_list.Length)]);
                    Msg(String.Format("You have just bought {0} for {1:C}", ProximityProperty.Name, ProximityProperty.Cost), 1100);
                    GTA.Native.Function.Call("DISPLAY_CASH", true);
                    Player.Money -= ProximityProperty.Cost;
                }
                // Player doesn't have enought money to buy this property
                else if (!ProximityProperty.Owned && Player.Money < ProximityProperty.Cost)
                {
                    Msg(String.Format("You require {0:C} to buy {1}", ProximityProperty.Cost, ProximityProperty.Name), 1100);
                }
                // Player owns this property, let's open the manager
                else if (ProximityProperty.Owned)
                {
                    ///
                    /// Open Property Manager
                    ///

                    this.Tick -= new EventHandler(UltimateBusinessMod_Tick);
                    Player.Character.Task.ClearAllImmediately();
                    isManagerOpen = true;
                    //Game.Pause();
                    ManagerFrm.Show();
                    Player.Character.Task.ClearAllImmediately();
                    //Game.Unpause();
                    this.Tick += new EventHandler(UltimateBusinessMod_Tick);
                    GTA.Native.Function.Call("DISPLAY_CASH", true);
                }
            }
            // Proximity detection
            foreach (Property p in Properties)
            {
                // if Player is holding the action key, no need to waste our thread with property scans
                if (Game.isGameKeyPressed(GameKey.Action))
                    return;
                // if Property is ownable and the Player is next to it
                if (p.Ownable && Player.Character.Position.DistanceTo2D(p.Location) < 3)
                {
                    // updata ProximityPropertyID
                    ProximityPropertyID = p.ID - 1;
                    // display available actions
                    if (!p.Owned)
                        if (Player.Money <= p.Cost)
                            Msg(String.Format("You require {0:C} to buy {1}", p.Cost, p.Name), 1100);
                        else
                            Msg(String.Format("Hold {0} to buy {1} for {2:C}", "~INPUT_FRONTEND_LB~", p.Name, p.Cost), 1100);
                    else
                        Msg(String.Format("Hold {0} to open {1}'s manager", "~INPUT_FRONTEND_LB~", p.Name), 1100);
                    return;
                }
            }
            // foreach reached end so Player is not next to a property
            ProximityPropertyID = -1;
        }

        #endregion
        #region log functions
        /// <summary>
        /// Get os name and SP
        /// </summary>
        /// <returns></returns>
        string getOSInfo()
        {
            //Get Operating system information.
            OperatingSystem os = Environment.OSVersion;
            //Get version information about the os.
            Version vs = os.Version;

            //Variable to hold our return value
            string operatingSystem = "";

            if (os.Platform == PlatformID.Win32Windows)
            {
                //This is a pre-NT version of Windows
                switch (vs.Minor)
                {
                    case 0:
                        operatingSystem = "95";
                        break;
                    case 10:
                        if (vs.Revision.ToString() == "2222A")
                            operatingSystem = "98SE";
                        else
                            operatingSystem = "98";
                        break;
                    case 90:
                        operatingSystem = "Me";
                        break;
                    default:
                        break;
                }
            }
            else if (os.Platform == PlatformID.Win32NT)
            {
                switch (vs.Major)
                {
                    case 3:
                        operatingSystem = "NT 3.51";
                        break;
                    case 4:
                        operatingSystem = "NT 4.0";
                        break;
                    case 5:
                        if (vs.Minor == 0)
                            operatingSystem = "2000";
                        else
                            operatingSystem = "XP";
                        break;
                    case 6:
                        if (vs.Minor == 0)
                            operatingSystem = "Vista";
                        else
                            operatingSystem = "7";
                        break;
                    default:
                        break;
                }
            }
            //Make sure we actually got something in our OS check
            //We don't want to just return " Service Pack 2" or " 32-bit"
            //That information is useless without the OS version.
            if (operatingSystem != "")
            {
                //Got something.  Let's prepend "Windows" and get more info.
                operatingSystem = "Windows " + operatingSystem;
                //See if there's a service pack installed.
                if (os.ServicePack != "")
                {
                    //Append it to the OS name.  i.e. "Windows XP Service Pack 3"
                    operatingSystem += " " + os.ServicePack;
                }
                //Append the OS architecture.  i.e. "Windows XP Service Pack 3 32-bit"
                operatingSystem += " " + getOSArchitecture().ToString() + "-bit";
            }
            //Return the information we've gathered.
            return operatingSystem;
        }
        /// <summary>
        /// Get OS architecture in use.
        /// </summary>
        /// <returns></returns>
        int getOSArchitecture()
        {
            string pa = Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
            return ((String.IsNullOrEmpty(pa) || String.Compare(pa, 0, "x86", 0, 3, true) == 0) ? 32 : 64);
        }
        /// <summary>
        /// Saves an exception's message with the current date and time, and the method that originated it.
        /// </summary>
        /// <param name="methodName">The method that originated it</param>
        /// <param name="message">The exception's message</param>
        private void Log(string methodName, string message)
        {
            try
            {
                if (!string.IsNullOrEmpty(message))
                    using (StreamWriter streamWriter = File.AppendText(Game.InstallFolder + "\\scripts\\UltimateBusinessMod.log"))
                    {
                        string logLine = System.DateTime.Now + " @ " + methodName + " - " + message;
                        streamWriter.WriteLine(logLine);
                        streamWriter.Close();
                        Game.Console.Print(logLine);
                    }
            }
            catch { }
            finally
            {
#if DEBUG
                Game.DisplayText("Check UltimateBusinessMod.log - " + message, 500);
#endif
            }

        }
        #endregion
    }
}
