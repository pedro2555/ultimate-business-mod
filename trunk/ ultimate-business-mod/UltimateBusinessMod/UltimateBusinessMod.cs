using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GTA;

namespace UltimateBusinessMod
{
    public class UltimateBusinessMod : Script
    {
        

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
                Game.DisplayText("Check UltimateBusinessMod.log - " + message, 3000);
#endif
            }

        }
        #endregion

        PropertyType[] PropertyTypes;
        Property[] Properties;

        /// <summary>
        /// The script entry point
        /// </summary>
        public UltimateBusinessMod()
        {
            LogFile.Path = Game.InstallFolder + "\\scripts\\UltimateBusinessMod.log";
            // check for database file
            if (!CheckForDatabase())
            {
                Game.DisplayText("Ultimate Business Mod Database file could not be found. Please re-install the mod.", 3000);
                Wait(3000);
                Abort();
            }

            // Log script start
            Log("UltimateBusinessMod", "Started under GTA " + Game.Version.ToString());
            Log("UltimateBusinessMod", "dsound.dll " + ((File.Exists(Game.InstallFolder + "\\dsound.dll")) ? "present" : "not present"));
            Log("UltimateBusinessMod", "xlive.dll " + ((File.Exists(Game.InstallFolder + "\\xlive.dll")) ? "present" : "not present"));
            Log("UltimateBusinessMod", "OS Version: " + getOSInfo());

            Game.DisplayText("Loading Ultimate Business Mod Data...", 100000);
            //Game.Pause();

            //PropertyTypes = PropertyType.GetTypesList();
            Properties = Property.GetPropertiesList();

            foreach (Property p in Properties)
            {
                Blip b = GTA.Blip.AddBlip(p.Location);
                b.Icon = (BlipIcon)0; // 0 and 80
                b.Name = p.Name;
                b.Display = BlipDisplay.ArrowAndMap;
                b.Friendly = true;
                b.RouteActive = false;
                b.ShowOnlyWhenNear = true;
                b.Scale = .7f;
            }

            //Game.Unpause();
            Game.DisplayText("Ultimate Business Mod Data loaded successfully.", 1000);

            this.PerFrameDrawing += new GraphicsEventHandler(UltimateBusinessMod_PerFrameDrawing);
        }

        void UltimateBusinessMod_PerFrameDrawing(object sender, GraphicsEventArgs e)
        {
            GTA.Native.Function.Call("PRINT_STRING_WITH_LITERAL_STRING_NOW",
                "STRING",
                String.Format("x:{0} y:{1} z:{2}", Player.Character.Position.X, Player.Character.Position.Y, Player.Character.Position.Z),
                10,
                true);
        }


    }
}
