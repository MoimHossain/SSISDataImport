
#region Copyright notice
//===================================================================================
//
//===================================================================================
#endregion

#region Using Directives
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Text;
using System.IO;
#endregion


namespace ImportLib.Supports
{
    /// <summary>
    ///     Defines a logger for this application
    /// </summary>
    public sealed class Logger
    {
        private static ILogProvider logProvider;

        /// <summary>
        /// 
        /// </summary>
        public static ILogProvider LogProvider
        {
            get { return logProvider; }
            set { logProvider = value; }
        }
	
        /// <summary>
        /// Write message
        /// </summary>
        /// <param name="message">message</param>
        public static void WriteInformation(string message)
        {
            WriteText(message, "Information");
        }

        /// <summary>
        /// Writes warning 
        /// </summary>
        /// <param name="message">message</param>
        public static void WriteWarning(string message)
        {
            WriteText(message, "Warning");
        }

        /// <summary>
        /// Writes message
        /// </summary>
        /// <param name="message">message</param>
        public static void WriteError(string message)
        {
            WriteText(message, "Error");
        }

        /// <summary>
        /// Writes the message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        private static void WriteText(string message, string type)
        {
            if (logProvider != null)
                logProvider.WriteMessage(message);
            try
            {
                using (StreamWriter writer = new StreamWriter(Constants.LogfileName, true))
                {
                    writer.WriteLine(string.Format( DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + " {0} --> {1}",type,message));
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.Write(ex.Message);
            }
        }
    }
}
