
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
#endregion

namespace ImportLib.IntegrationService
{
    /// <summary>
    ///     Defines a contact that will remain into the middle of a source and destination.
    /// </summary>
    /// <remarks>
    ///     Concrete implementations will act as a middle layer of the source and destination
    /// and will perform the mappings/transformations related tasks.
    ///     Infact we can create the mapping related logic inside the concrete implementations 
    /// of this interface. Or we can write simply another layer for that purpose which will remain
    /// between the transformations and destination layer.
    /// </remarks>
    public interface IDataflowManager
    {
        /// <summary>
        /// Get or set the <see cref="T:ImportLib.ISsisSource"/> instance 
        /// </summary>
        ISource DtsSource
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set the <see cref="T:ImportLib.ISsisDestination"/> instance 
        /// </summary>
        IDestination DTSDestination
        {
            get;
            set;
        }

        /// <summary>
        /// Creates the data flow task.
        /// </summary>
        /// <param name="package">The package where the the data flow task should be created.</param>
        /// <returns>An instance of <see cref="T:ImportLib.IntegrationService.SsisExecutable"/></returns>
        DTSExecutable CreateDataFlowTask(DTSPackage package);
    }
}
