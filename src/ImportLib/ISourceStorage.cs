
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

namespace ImportLib
{
    /// <summary>
    /// Defines a contact for the data source
    /// </summary>
    public interface ISourceStorage
    {
        /// <summary>
        /// Initializes the data source
        /// </summary>
        /// <param name="properties">A <see cref="T:IDictionary<string, string>"/> that contains
        /// the initialization properties</param>
        void InitializeSource(IDictionary<string, string> properties);

        /// <summary>
        /// Get the schema table for the data source 
        /// </summary>
        /// <returns>An instance of <see cref="T:DataTable"/> that contains the schema
        /// informations of the data source</returns>
        DataTable GetSchemaTable();

        /// <summary>
        /// Get the Category of the underlying persistant storage
        /// </summary>
        StorageMedium StorageMedium { get; }

        /// <summary>
        /// Get or set the mapping manager
        /// </summary>
        ColumnMappingController MapManager { get; set; }
    }
}
