
#region Copyright notice
//===================================================================================
//
//===================================================================================
#endregion

#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;
using ImportLib.Mappings;
#endregion

namespace ImportLib.Platform
{
    /// <summary>
    ///     Provides an abstract implementation of <see cref="T:IDataDestination"/> interface.
    /// </summary>
    public abstract class DestinationStorage : IDestinationStorage
    {
        private IDictionary<string, string> initializationProperties;

        /// <summary>
        /// Get or set the initialization properties
        /// </summary>
        protected IDictionary<string, string> InitializationProperties
        {
            get { return initializationProperties; }
            set { initializationProperties = value; }
        }

        // map manager
        private ColumnMappingController mapManager;

        #region IDataDestination Members

        /// <summary>
        /// Initializes the data destination
        /// </summary>
        /// <param name="properties">A <see cref="T:IDictionary<string, string>"/> that contains
        /// the initialization properties</param>
        public void InitializeDestination(IDictionary<string, string> properties)
        {
            initializationProperties = properties;  // initialization
            OnInitialization();           // Perform Initialization tasks
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
        /// Perform the initialization task for this instance
        /// </summary>
        protected virtual void OnInitialization()
        {
            // No tasks for here
        }
    }
}
