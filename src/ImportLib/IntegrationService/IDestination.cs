
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
    ///     Defines a contact for the Sql Server Integration Service Destination objects.
    /// </summary>
    /// <remarks>   
    ///     Different data destinations should satisfy this interface. such as Flat file 
    /// destinations, OleDB destination etc.
    /// </remarks>    
    public interface IDestination 
    {
        /// <summary>
        /// Get or set the <see cref="T:ImportLib.IDataDestination"/> instance 
        /// that represents the physical data destination.
        /// </summary>
        IDestinationStorage DataDestination
        {
            get;
            set;
        }

        /// <summary>
        /// Get or set the <see cref="T:ImportLib.ISsisSource"/> instance 
        /// </summary>
        ISource SsisSource
        {
            get;
            set;
        }
        
        /// <summary>
        /// Creates a connection manager instance for a given source file 
        /// into the context of the given <see cref="T:ImportLib.IntegrationService.SsisPackage"/> 
        /// object.
        /// </summary>
        /// <param name="package">An instance of <see cref="T:ImportLib.IntegrationService.SsisPackage"/>.</param>
        void CreateConnection(DTSPackage package);

        /// <summary>
        /// Creates the task that will create the destination storage (ex. database, table etc);
        /// </summary>
        /// <param name="package">The package where the task should be created</param>
        /// <returns>An instance of <see cref="T:ImportLib.IntegrationService.SsisExecutable"/></returns>
        DTSExecutable CreateStorageCreationTask(DTSPackage package);

        /// <summary>
        /// Creates the destination dataflow component.
        /// </summary>
        /// <param name="package">The package instance.</param>
        /// <param name="dataflowTask">The dataflow task under which the component will be created.</param>
        /// <returns>An instance of <see cref="T:SsisDataflowComponent"/>.</returns>
        DataflowComponent CreateDestinationDataFlowComponent(DTSPackage package, DTSExecutable dataflowTask);

        /// <summary>
        /// Initializse the destination dataflow component
        /// </summary>
        /// <param name="destinationDataFlowComponent">An instance of <see cref="T:SsisDataflowComponent"/></param>
        void InitializeDataflowComponent(DataflowComponent destinationDataFlowComponent);
    }
}
