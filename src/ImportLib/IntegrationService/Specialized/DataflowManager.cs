
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


namespace ImportLib.IntegrationService.Specialized
{
    /// <summary>
    /// A concrete implementation of the <see cref="T:ImportLib.IntegrationService.IDataflowManager"/>
    /// interface.
    /// </summary>
    public class DataflowManager : IDataflowManager
    {
        // The moniker for the data flow task
        private const string dataFlowTaskMoniker = "DTS.Pipeline.1";
        // source
        private ISource source;
        // destination
        private IDestination destination;

        #region ISsisTransformer Members

        /// <summary>
        /// Get or set the <see cref="T:ImportLib.ISource"/> instance 
        /// </summary>
        public ISource DtsSource
        {
            get
            {
                return source;
            }
            set
            {
                source = value;
            }
        }

        /// <summary>
        /// Get or set the <see cref="T:ImportLib.IDestination"/> instance 
        /// </summary>
        public IDestination DTSDestination
        {
            get
            {
                return destination;
            }
            set
            {
                destination = value;
            }
        }

        /// <summary>
        /// Creates the data flow task.
        /// </summary>
        /// <param name="package">The package where the the data flow task should be created.</param>
        /// <returns>An instance of <see cref="T:ImportLib.IntegrationService.SsisExecutable"/>.</returns>
        public DTSExecutable CreateDataFlowTask(DTSPackage package)
        {
            // create a new executable
            DTSExecutable executable = new DTSExecutable(package, dataFlowTaskMoniker);
            // create the data flow component to build the executable
            CreateDataflowComponents(package,executable);
            return executable;
        }

        
        #endregion

        /// <summary>
        /// Create the dataflow components for the dataflow task instance
        /// </summary>
        /// <param name="package">The package instance.</param>
        /// <param name="dataflowTask">The data flow task instance.</param>
        private void CreateDataflowComponents(DTSPackage package, DTSExecutable dataflowTask)
        {
            #region Logging
            Logger.WriteInformation("SsisDataflowManager::CreateDataFlowComponents()--> going to create the dataflow tasks");
            #endregion
            // create the source compoenent
            DataflowComponent sourceDataflowComponent 
                = source.CreateSourceDataFlowComponent(package, dataflowTask);
            // initialize the newly created dataflow component
            source.InitializeDataflowComponent(sourceDataflowComponent);
            // create the destination component
            DataflowComponent destinationDataflowComponent
                = destination.CreateDestinationDataFlowComponent(package, dataflowTask);            
            // create a path that will connect the source and destination components
            DTSUtils.CreatePathBetweenDataflowComponents(dataflowTask, sourceDataflowComponent, destinationDataflowComponent);
            // initialize the destination dataflow component
            destination.InitializeDataflowComponent(destinationDataflowComponent);

            #region Logging
            Logger.WriteInformation("SsisDataflowManager::CreateDataFlowComponents()--> going to create the dataflow tasks...Completed");
            #endregion            
        }
    }
}
