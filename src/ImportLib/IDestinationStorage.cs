
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

namespace ImportLib
{
    /// <summary>
    ///     Defines a contact for the Data destination
    /// </summary>
    public interface IDestinationStorage
    {
        /// <summary>
        /// Initializes the data destination
        /// </summary>
        /// <param name="properties">A <see cref="T:IDictionary<string, string>"/> that contains
        /// the initialization properties</param>
        void InitializeDestination(IDictionary<string, string> properties);

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
