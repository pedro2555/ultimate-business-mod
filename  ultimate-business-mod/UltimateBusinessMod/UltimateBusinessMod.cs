﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GTA;
using System.Diagnostics;
using System.Reflection;

namespace UltimateBusinessMod
{
    public class UltimateBusinessMod : Script
    {
        #region internal use properties
        /// <summary>
        /// Gets if the Property ID returned by the PropertyProximity
        /// -1 if the player is not close to a property
        /// </summary>
        public static int ProximityPropertyID;
        
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

        public static Property[] Properties;

        /// <summary>
        /// The script entry point
        /// </summary>
        public UltimateBusinessMod()
        {
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
            // Log script start
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(Game.InstallFolder + "\\scripts\\UltimateBusinessMod.net.dll");
            string version = fvi.FileVersion;
            Log("UltimateBusinessModv" + version, "Started under GTA " + Game.Version.ToString());
            Log("UltimateBusinessModv" + version, "dsound.dll " + ((File.Exists(Game.InstallFolder + "\\dsound.dll")) ? "present" : "not present"));
            Log("UltimateBusinessModv" + version, "xlive.dll " + ((File.Exists(Game.InstallFolder + "\\xlive.dll")) ? "present" : "not present"));
            Log("UltimateBusinessModv" + version, "OS Version: " + getOSInfo());
            #endregion

            // wait for map to load
            Wait(5000);

            // Start loading database records
            Game.DisplayText("Loading Ultimate Business Mod Data...", 100000);
            Properties = Property.GetPropertiesList();
            // create blips for each property
            foreach (Property p in Properties)
            {
                try
                {
                    GTA.Object apple = World.CreateObject(new Model("amb_apple_1"), p.Location);
                    apple.Visible = false;
                    apple.Collision = false;
                    Blip b = apple.AttachBlip();
                    b.Icon = (p.Owned) ? (BlipIcon)80 : (BlipIcon)0;
                    b.Name = p.Name;
                    b.Scale = .7f;
                    b.ShowOnlyWhenNear = true;
                }
                catch (Exception crap) { Log("construct - foreach - blip drawing", crap.Message); }
            }

            //Game.Unpause();
            Game.DisplayText("Ultimate Business Mod Data loaded successfully.", 1000);

            this.Interval = 1000;
            this.Tick += new EventHandler(UltimateBusinessMod_Tick);

            action_key_timer = new System.Timers.Timer(100);
            action_key_timer.Elapsed += new System.Timers.ElapsedEventHandler(action_key_timer_Elapsed);
            action_key_timer.Enabled = true;
        }

        void action_key_timer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                if (Game.isGameKeyPressed(GameKey.Action))
                {
                    if (ProximityPropertyID != -1)
                    {
                        Properties[ProximityPropertyID].Owned = true;

                    }
                    else Game.DisplayText("Failed", 1000);

                    
                }
            }
            catch (Exception) { }
        }

        System.Timers.Timer action_key_timer;

        void UltimateBusinessMod_Tick(object sender, EventArgs e)
        {
            try
            {
                foreach (Property p in Properties)
                    if (Player.Character.Position.DistanceTo(p.Location) <= 3.0f)
                    {
                        if (!p.Owned)
                        {
                            GTA.Native.Function.Call(
                                "PRINT_STRING_WITH_LITERAL_STRING_NOW",
                                "STRING",
                                String.Format("{0} is selling for {1:C}. Press {2} to buy it.", p.Name, p.Cost, "~INPUT_FRONTEND_LB~"),
                                1100,
                                true);
                        }
                        else
                        {
                            GTA.Native.Function.Call(
                                "PRINT_STRING_WITH_LITERAL_STRING_NOW",
                                "STRING",
                                String.Format("Press {0} to check {1} datasheet", "~INPUT_FRONTEND_LB~", p.Name),
                                1100,
                                true);
                        }
                        ProximityPropertyID = p.ID;
                        return;
                    }
                ProximityPropertyID = -1;
            }
            catch (Exception crap)
            {
                //Log("UltimateBusinessMod_Tick", crap.Message);
            }
        }

        
    }
}