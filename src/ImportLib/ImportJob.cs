
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
    ///     Defines a Job that can be performed for import or export data betwee two
    /// persistant storage
    /// </summary>
    [Serializable()]
    public class ImportJob
    {
        /// Apparently defineing the JOb queue name here in this clas.. later we will
        /// move this definition somewhere more appropriate
        public const String JobQueueName = @".\Private$\DataTransferJobQueue";

        /// <summary>
        /// Creates a new Data transfer Job
        /// </summary>
        /// <param name="dataSource">The source storage type</param>
        /// <param name="dataDestination">The destination storage type</param>
        /// <param name="sourceProperties">Source storage initialization properties</param>
        /// <param name="destinationProperties">Destination storage initialization properties</param>
        /// <param name="mapMmanager">A manager that defines the mappings</param>
        public ImportJob(StorageMedium dataSource, StorageMedium dataDestination,
            IDictionary<string, string> sourceProperties, IDictionary<string, string> destinationProperties,
            ColumnMappingController mapManager)
        {
            this.dataSource = dataSource;
            this.dataDestination = dataDestination;
            this.sourceProperties = sourceProperties;
            this.destinationProperties = destinationProperties;
            this.columnMappingController = mapManager;
        }

        private StorageMedium dataSource;

        /// <summary>
        /// Get or set the source persistant storage type
        /// </summary>
        public StorageMedium DataSource
        {
            get { return dataSource; }
            set { dataSource = value; }
        }

        private StorageMedium dataDestination;

        /// <summary>
        /// Get or set the destination persistant storage type
        /// </summary>
        public StorageMedium DataDestination
        {
            get { return dataDestination; }
            set { dataDestination = value; }
        }

        private IDictionary<string, string> sourceProperties;

        /// <summary>
        /// Get or set the source initialization properties
        /// </summary>
        public IDictionary<string, string> SourceProperties
        {
            get { return sourceProperties; }
            set { sourceProperties = value; }
        }

        private IDictionary<string, string> destinationProperties;

        /// <summary>
        /// Get or set the destination initialization properties
        /// </summary>
        public IDictionary<string, string> DestinationProperties
        {
            get { return destinationProperties; }
            set { destinationProperties = value; }
        }

        private ColumnMappingController columnMappingController;

        /// <summary>
        /// Get or set the map manager
        /// </summary>
        public ColumnMappingController ColumnMappingController
        {
            get { return columnMappingController; }
            set { columnMappingController = value; }
        }
	
    }
}
