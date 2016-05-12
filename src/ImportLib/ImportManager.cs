
#region Copyright notice
//===================================================================================
//
//===================================================================================
#endregion

#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;
using ImportLib.Delimited;
using ImportLib.Supports;
using System.Data;
using ImportLib.Sql;
using ImportLib.IntegrationService;
using ImportLib.Mappings;
#endregion

namespace ImportLib
{
    /// <summary>
    /// Controls the data transformation jobs
    /// </summary>
    public sealed class ImportManager
    {
        /// <summary>
        /// Creates a new instance
        /// </summary>
        public ImportManager()
        {
            
        }
        
        /// <summary>
        /// Transfers data from a source persistant storage to a destination storage
        /// </summary>
        /// <param name="dataSource">
        ///     The <see cref="ImportLib.StorageMedium"/> that describes the source storage type.
        /// </param>
        /// <param name="dataDestination">
        ///     The <see cref="ImportLib.StorageMedium"/> that describes the destination storage type.
        /// </param>
        /// <param name="sourceProperties">
        ///     A <see cref="T:System.Collections.Generic.IDictionary<string,string>"/> that
        /// contains the properties need by the source persistant storage.
        /// </param>
        /// <param name="destinationProperties">
        ///     A <see cref="T:System.Collections.Generic.IDictionary<string,string>"/> that
        /// contains the properties need by the destination persistant storage.
        /// </param>
        /// <param name="mapMmanager">
        ///     A <see cref="T:ImportLib.Mappings.MapManager"/> instance that contains the 
        /// mapping informations between source columns and destination columns.
        /// </param>
        /// <returns>An instance of <see cref="T:ImportLib.ExecutionResult"/> contains the result.</returns>
        public ExecutionResult Transfer(
            StorageMedium dataSource, StorageMedium dataDestination, 
            IDictionary<string, string> sourceProperties, IDictionary<string, string> destinationProperties, 
            ColumnMappingController mapMmanager)
        {
            return Transfer(
                new ImportJob(dataSource, dataDestination, sourceProperties, destinationProperties, mapMmanager)
            );
        }

        /// <summary>
        /// Performs the Job.
        /// </summary>
        /// <param name="job">An instance of <see cref="T:ImportLib.DataTransferJob"/></param>
        /// <returns>An instance of <see cref="T:ImportLib.ExecutionResult"/> contains the result.</returns>
        public ExecutionResult Transfer(ImportJob job)
        {
            ISourceStorage dataSource = ObjectFactory.GetDataSource(job.DataSource);
            dataSource.InitializeSource(job.SourceProperties);            
            dataSource.MapManager = job.ColumnMappingController;                        

            IDestinationStorage dataDestination = ObjectFactory.GetDataDestination(job.DataDestination);
            dataDestination.InitializeDestination(job.DestinationProperties);
            dataDestination.MapManager = job.ColumnMappingController;

            DTSManager mgr = new DTSManager();
            mgr.LogCreated += new ImportLib.IntegrationService.Logging.LogCreatedDelegate(Manager_LogCreated);

            #region Logging
            Logger.WriteInformation("Initiating operation");
            #endregion

            return mgr.Transfer(dataSource, dataDestination);
        }

        private ILogProvider logProvider;

        /// <summary>
        ///     Get or set the LogProvider
        /// </summary>
        public ILogProvider LogProvider
        {
            get { return logProvider; }
            set { logProvider = value; }
        }
	

        /// <summary>
        ///     Log created
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Manager_LogCreated(object sender, ImportLib.IntegrationService.Logging.LogEventArgs e)
        {
            if (logProvider != null)
            {
                logProvider.WriteMessage(e.MessageText);
            }
        }
    }
}
