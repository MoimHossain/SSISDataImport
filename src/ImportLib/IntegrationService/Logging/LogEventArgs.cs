using System;
using System.Collections.Generic;
using System.Text;

namespace ImportLib.IntegrationService.Logging
{
    /// <summary>
    /// Log event arguments
    /// </summary>
    /// <author>
    ///     Moim Hossain
    /// </author>
    public class LogEventArgs : EventArgs
    {
        private string logEntryName;

        /// <summary>
        /// get or set the logEntryName
        /// </summary>
        public string LogEntryName
        {
            get { return logEntryName; }
            set { logEntryName = value; }
        }

        private string computerName;

        /// <summary>
        /// 
        /// </summary>
        public string ComputerName
        {
            get { return computerName; }
            set { computerName = value; }
        }

        private string messageText;

        /// <summary>
        /// 
        /// </summary>
        public string MessageText
        {
            get { return messageText; }
            set { messageText = value; }
        }

        /// <summary>
        /// Creaets a new instance
        /// </summary>
        /// <param name="logEntryName"></param>
        /// <param name="computerName"></param>
        /// <param name="messageText"></param>
        public LogEventArgs(string logEntryName, string computerName, string messageText)
        {
            this.logEntryName = logEntryName;
            this.computerName = computerName;
            this.messageText = messageText;
        }
            //, computerName, operatorName, sourceName, sourceID, executionID, messageText, startTime, endTime, dataCode, dataBytes
    }
}
