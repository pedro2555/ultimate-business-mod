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
