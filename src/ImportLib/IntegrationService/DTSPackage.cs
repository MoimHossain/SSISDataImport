
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
using ImportLib.Delimited;
using System.Diagnostics;
using Microsoft.SqlServer.Dts.Runtime;
using RuntimeWrapper = Microsoft.SqlServer.Dts.Runtime.Wrapper;

using ImportLib.IntegrationService.Logging;
#endregion

namespace ImportLib.IntegrationService
{
    /// <summary>
    ///     A simple wrapper for the SSIS's <see cref="T:Microsoft.SqlServer.Dts.Runtime.Package"/>
    /// class.
    /// </summary>
    public class DTSPackage
    {
        private Package package;

        /// <summary>
        ///     Get or set the <see cref="T:Microsoft.SqlServer.Dts.Runtime.Package"/> instance
        /// </summary>
        public Package InnerObject
        {
            get { return package; }
            set { package = value; }
        }

        private EventLogProvider eventLogProvider;

        /// <summary>
        ///     Get or set the EventLogProvider
        /// </summary>
        public EventLogProvider LogProvider
        {
            get { return eventLogProvider; }
            set { eventLogProvider = value; }
        }
	

        /// <summary>
        ///     Creates a new instance
        /// </summary>
        public DTSPackage() : this( DTSPackage.DefaultName , string.Empty)
        {
            
        }

        /// <summary>
        ///     Creates a new instance.
        /// </summary>
        /// <param name="name">The name of the package</param>
        public DTSPackage(string name) : this( name, string.Empty )
        {

        }

        /// <summary>
        ///     Creates a new instance of SsisPackage
        /// </summary>
        /// <param name="name">The name of the package</param>
        /// <param name="description">The description of the package</param>
        public DTSPackage(string name, string description) 
        {
            InitializePackage(name, description);   // Initialize the package
        }

        /// <summary>
        ///     Initialize a package instance
        /// </summary>
        /// <param name="name">The name of the package</param>
        /// <param name="description">The description of the package</param>
        private void InitializePackage(string name, string description)
        {
            // initialize the package
            package = new Package();
            package.CreationDate = DateTime.Now;
            package.ProtectionLevel = DTSProtectionLevel.DontSaveSensitive;
            package.Name = name;
            package.Description = description;
            package.DelayValidation = true;
            package.PackageType = Microsoft.SqlServer.Dts.Runtime.DTSPackageType.DTSDesigner90;
            // Initialize a log provider that will keep the logs
            InitializeLogProvider();
        }

        /// <summary>
        ///     Initialize the log provider
        /// </summary>
        private void InitializeLogProvider()
        {
            // Enable the log
            package.LoggingMode = DTSLoggingMode.Enabled;
            // Select the log provider using a moniker - we are apparenlty using the event log - rather writing our own.
            //LogProvider log = package.LogProviders.Add("DTS.LogProviderEventLog.1");
            LogProvider log = package.LogProviders.Add(typeof(EventLogProvider).AssemblyQualifiedName);
            eventLogProvider = log.InnerObject as EventLogProvider;
            log.Name = string.Concat("Log provider for " ,package.Name);
            log.Description = "Logs Event info to Windows Event Log";
            package.LoggingOptions.SelectedLogProviders.Add(log);
            LoggingOptions loggingOptions = package.LoggingOptions;
            loggingOptions.EventFilterKind = DTSEventFilterKind.Inclusion;
            string[] events = new string[] { "OnError", "OnTaskFailed", "OnWarning" };
            loggingOptions.EventFilter = events;
            DTSEventColumnFilter eventColumnFilter = new DTSEventColumnFilter();
            eventColumnFilter.Computer = true;
            eventColumnFilter.Operator = true;
            eventColumnFilter.SourceName = true;
            eventColumnFilter.SourceID = true;
            eventColumnFilter.ExecutionID = true;
            eventColumnFilter.MessageText = true;
            eventColumnFilter.DataBytes = false;
            loggingOptions.SetColumnFilter("OnWarning", eventColumnFilter);
            loggingOptions.SetColumnFilter("OnError", eventColumnFilter);
            loggingOptions.SetColumnFilter("OnTaskFailed", eventColumnFilter);            
        }

        /// <summary>
        ///     Execute the underlying SSIS package
        /// </summary>
        /// <returns>An instance of <see cref="T:ExecutionResult"/> class.</returns>
        public ExecutionResult ExecutePackage()
        {
            // execute this package
            DTSExecResult res = package.Execute();
            // The result that will be given to the user
            ExecutionResult result;

            if (res == DTSExecResult.Success)
            {   // If it was success
                result = new ExecutionResult();
            }
            else
            {
                // The execution error instance
                ExecutionError[] errors = new ExecutionError[package.Errors.Count];
                for (int i = 0; i < package.Errors.Count; ++ i )
                {   // Iterate
                    DtsError dtsError = package.Errors[i];  // get the DTS Error
                    errors[i] = new ExecutionError(dtsError.Description, dtsError);
                }
                // create the result
                result = new ExecutionResult(ExecResult.Failed, errors);
            }

            return result;
        }

        /// <summary>
        ///     Save the pacakge to a file.
        /// </summary>
        /// <param name="fileName">File name</param>
        public void SaveToDisc(string fileName)
        {
            Application app = new Application();
            app.SaveToXml(fileName, package, null);
        }

        /// <summary>
        ///     Get the default name for a package
        /// </summary>
        private static string DefaultName
        {
            get 
            {
                string dateTime = string.Format("{0}_{1}", DateTime.Today.ToString("MMMddyyyy"),DateTime.Now.ToString("hhmmtt"));
                return string.Format("SSISPackage_{0}",dateTime);
            }
        }
    }
}
