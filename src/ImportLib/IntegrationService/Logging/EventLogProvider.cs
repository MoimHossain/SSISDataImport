
#region Copyright notice
//===================================================================================
//
//===================================================================================
#endregion

#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Diagnostics;
using ImportLib.Sql;
using Microsoft.SqlServer.Dts.Runtime;
using ImportLib.Supports;
using System.Xml;
using Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask;
#endregion

namespace ImportLib.IntegrationService.Logging
{
    /// <summary>
    /// A custom log provider class
    /// </summary>
    /// <author>
    ///     Moim Hossain
    /// </author>
    [DtsLogProvider(DisplayName = "LogProvider", Description = "Log provider for DTS packages.", LogProviderType = "Custom")]
    public class EventLogProvider : LogProviderBase
    {
        /// <summary>
        /// Configuration string
        /// </summary>
        public override string ConfigString
        {
            get
            {
                return string.Empty;
            }
            set
            {
                
            }
        }
        
        /// <summary>
        /// Open log
        /// </summary>
        public override void OpenLog()
        {
            base.OpenLog();
        }

        /// <summary>
        /// Closing log
        /// </summary>
        public override void CloseLog()
        {
            base.CloseLog();
        }

        /// <summary>
        /// Initializations
        /// </summary>
        /// <param name="connections"></param>
        /// <param name="events"></param>
        /// <param name="refTracker"></param>
        public override void InitializeLogProvider(Connections connections, IDTSInfoEvents events, ObjectReferenceTracker refTracker)
        {
            base.InitializeLogProvider(connections, events, refTracker);
        }

        /// <summary>
        /// Write a log
        /// </summary>
        /// <param name="logEntryName"></param>
        /// <param name="computerName"></param>
        /// <param name="operatorName"></param>
        /// <param name="sourceName"></param>
        /// <param name="sourceID"></param>
        /// <param name="executionID"></param>
        /// <param name="messageText"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="dataCode"></param>
        /// <param name="dataBytes"></param>
        public override void Log(string logEntryName, string computerName, string operatorName, string sourceName, string sourceID, string executionID, string messageText, DateTime startTime, DateTime endTime, int dataCode, byte[] dataBytes)
        {
            //base.Log(logEntryName, computerName, operatorName, sourceName, sourceID, executionID, messageText, startTime, endTime, dataCode, dataBytes);

            LogEventArgs e = new LogEventArgs(logEntryName,computerName,messageText);
            //
            OnLogCreated(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnLogCreated(LogEventArgs e)
        {
            LogCreatedDelegate mLogCreated = this.LogCreated;

            if (mLogCreated != null)
            {
                mLogCreated(this, e);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public event LogCreatedDelegate LogCreated;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void LogCreatedDelegate ( object sender , LogEventArgs e );
}
