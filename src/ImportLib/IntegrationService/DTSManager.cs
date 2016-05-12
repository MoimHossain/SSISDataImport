
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

using System.Xml;

using Microsoft.SqlServer.Dts.Runtime;
using Microsoft.SqlServer.Dts.Pipeline;

using RuntimeWrapper = Microsoft.SqlServer.Dts.Runtime.Wrapper;
using PipelineWrapper = Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;

using ImportLib.Supports;
using ImportLib.IntegrationService.Specialized;
using ImportLib.IntegrationService.Logging;

#endregion

namespace ImportLib.IntegrationService
{
    /// <summary>
    /// Encapsulates the Transfer logic through SSIS APIs
    /// </summary>
    public sealed class DTSManager
    {
        /// <summary>
        /// Creates a new instance
        /// </summary>
        public DTSManager()
        {
            
        }

        /// <summary>
        ///     Transfers data from source to destination
        /// </summary>
        /// <param name="source">An implementation of <see cref="T:ImportLib.IDataSource"/> interface.</param>
        /// <param name="destination">An implementation of <see cref="T:ImportLib.IDataDestination"/> interface.</param>
        /// <returns>An instance of <see cref="T:ExecutionResult"/> class.</returns>
        public ExecutionResult Transfer(ISourceStorage source, IDestinationStorage destination)
        {
            #region Logging
            Logger.WriteInformation("Initiating SSIS Source");
            #endregion
            // Get the SSIS Source instance
            ISource ssisSource = DTSObjectFactory.GetSsisSource(source);

            #region Logging
            Logger.WriteInformation("Initiating SSIS Destination");
            #endregion

            // get the destination instance
            IDestination ssisDestination = DTSObjectFactory.GetSsisDestination(destination);

            #region Logging
            Logger.WriteInformation("Initiating DataFlow Tasks");
            #endregion

            // Get the data flow manager instance
            IDataflowManager ssisDataflowManager = DTSObjectFactory.GetSsisDataflowManager();

            // setting some dependencies
            ssisSource.DataSource = source;                         // SSIS source needs the physical data source
            ssisDestination.SsisSource = ssisSource;                // SSIS destination needs SSIS Source 
                                                                    // (Later we will break this with a transformer object
            ssisDestination.DataDestination = destination;          // SSIS destination needs physical data destination
            ssisDataflowManager.DtsSource = ssisSource;            // SSIS dataflow manager needs SSIS Source
            ssisDataflowManager.DTSDestination = ssisDestination;  // SSIS dataflow manager needs SSIS destination

            #region Logging
            Logger.WriteInformation("Creating DTS package and connections");
            #endregion

            DTSPackage ssisPackage = new DTSPackage();            // Build the SSIS package
            ssisPackage.LogProvider.LogCreated += new ImportLib.IntegrationService.Logging.LogCreatedDelegate(LogProvider_LogCreated);
            // Creating the connection for the Source               
            ssisSource.CreateConnection(ssisPackage);               // Populate the connections
            // Creating the connection for the Destination
            ssisDestination.CreateConnection(ssisPackage);          // Populate the connections

            // creates the storage creation task..its a preparation task for the SSIS package
            DTSExecutable preparationTask = ssisDestination.CreateStorageCreationTask(ssisPackage);
            // Now create the dataflow task that will perform the real data moving operation
            DTSExecutable dataflowTask = ssisDataflowManager.CreateDataFlowTask(ssisPackage);

            // Now connecting these two task 
            DTSUtils.ConnectExecutables(ssisPackage, preparationTask, dataflowTask);

            #region Logging
            Logger.WriteInformation("Package creation completed now saving.");
            #endregion

            #region DEBUG
            // save it to file
            ssisPackage.SaveToDisc( AppDomain.CurrentDomain.BaseDirectory +  "\\Final.dtsx");
            #endregion

            #region Logging
            Logger.WriteInformation("Executing package now...........");
            #endregion

            // execute the package
            return ssisPackage.ExecutePackage();
        }

        /// <summary>
        /// Keeping the log
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void LogProvider_LogCreated(object sender, ImportLib.IntegrationService.Logging.LogEventArgs e)
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
}



#region OBSOLETE
//string destinationConnectionGuid = ssisDestination.CreateConnection(ssisPackage, @"DestinationConnectionOLEDB");
//guidTranslations.Add(@"{DAC2F320-E1C0-420E-B50B-76CC4A4B8B1F}", destinationConnectionGuid);

//string sourceConnectionGuid = ssisSource.CreateConnection(ssisPackage, @"SourceConnectionFlatFile");
//guidTranslations.Add(@"{73BEE501-6614-41D9-810C-81639A5C692D}", sourceConnectionGuid);



//string dataflowTaskName = @"Data Flow Task";
//string sqlTaskName = @"Preparation SQL Task";

//ssisDestination.CreateTableCreationTask(ssisPackage, sqlTaskName);

//object dfComponentData = CreateDataFlowComponent(ssisSource, ssisDestination, ssisTransformer);
//ssisPackage.CreateDataFlowComponent(dataflowTaskName,dfComponentData);

//CreatePrecedenceConstraints(ssisPackage, sqlTaskName, dataflowTaskName);

////DTSExecResult res = ssisPackage.InnerObject.Execute();

//Application app = new Application();

//app.SaveToXml( "d:\\moim\\progDts.dtsx", ssisPackage.InnerObject,null);
//Package pack = app.LoadPackage("d:\\moim\\progDts.dtsx", null);
//pack.DelayValidation = true;
//DTSExecResult res = pack.Execute();

//using (System.IO.StreamWriter wr = new System.IO.StreamWriter("d:\\moim\\err.log", true))
//{
//    foreach (DtsError error in pack.Errors)
//    {

//        wr.WriteLine(error.Description);
//        object value = error;
//    }
//}
#endregion