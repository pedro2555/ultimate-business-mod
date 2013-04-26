using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace UltimateBusinessMod
{
    /// <summary>
    /// Static use of logging
    /// </summary>
    public static class LogFile
    {
        /// <summary>
        /// Absolute path to the log file
        /// </summary>
        public static string Path
        { get; set; }

        /// <summary>
        /// Saves an exception's message with the current date and time, and the method that originated it.
        /// </summary>
        /// <param name="methodName">The method that originated it</param>
        /// <param name="message">The exception's message</param>
        public static void Log(string methodName, string message)
        {
            try
            {
                if (!string.IsNullOrEmpty(message))
                    using (StreamWriter streamWriter = File.AppendText(Path))
                    {
                        string logLine = System.DateTime.Now + " @ " + methodName + " - " + message;
                        streamWriter.WriteLine(logLine);
                        streamWriter.Close();
                    }
            }
            catch { }

        }
    }
}
