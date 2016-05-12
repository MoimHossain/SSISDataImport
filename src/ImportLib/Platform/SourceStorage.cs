
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
using ImportLib.Mappings;
#endregion

namespace ImportLib.Platform
{
    /// <summary>
    ///     Provides an abstract implementation of <see cref="T:IDataSource"/> interface.
    /// </summary>
    public abstract class SourceStorage : ISourceStorage
    {
        // The schema table instance - we are using this so that we will retrive the schema only once
        // not at each call of GetSchemaTable()
        private DataTable schemaTable;
        // initialization properties
        private IDictionary<string,string> initializationProperties;

        /// <summary>
        /// Get or set the initialization properties
        /// </summary>
        protected IDictionary<string,string> InitializationProperties
        {
            get { return initializationProperties; }
            set { initializationProperties = value; }
        }

        // map manager
        private ColumnMappingController mapManager;

        #region IDataSource Members

        /// <summary>
        /// Initializes the data source
        /// </summary>
        /// <param name="properties">A <see cref="T:IDictionary<string, string>"/> that contains
        /// the initialization properties</param>
        public void InitializeSource(IDictionary<string, string> properties)
        {
            initializationProperties = properties;  // initialization
            OnInitialization();           // Perform Initialization tasks
        }
        
        /// <summary>
        /// Get the schema table for the data source 
        /// </summary>
        /// <returns>An instance of <see cref="T:DataTable"/> that contains the schema
        /// informations of the data source</returns>
        public System.Data.DataTable GetSchemaTable()
        {
            if (schemaTable == null)
                schemaTable = GetSchemaTableCore();
            return schemaTable;
        }

        /// <summary>
        /// Get the Category of the underlying persistant storage
        /// </summary>
        public abstract StorageMedium StorageMedium
        {
            get;
        }

        /// <summary>
        /// Get or set the mapping manager
        /// </summary>
        public ColumnMappingController MapManager
        {
            get
            {
                return mapManager;
            }
            set
            {
                mapManager = value;
            }
        }
        #endregion

        /// <summary>
        /// Get the schema table for the data source 
        /// </summary>
        /// <returns>An instance of <see cref="T:DataTable"/> that contains the schema
        /// informations of the data source</returns>
        protected abstract System.Data.DataTable GetSchemaTableCore();

        /// <summary>
        /// Perform the initialization task for this instance
        /// </summary>
        protected virtual void OnInitialization()
        {
            // No tasks for here
        }
    }
}
