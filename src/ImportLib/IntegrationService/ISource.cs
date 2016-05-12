
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
    ///     Defines a contact for the Sql Server Integration Service source objects.
    /// </summary>
    /// <remarks>   
    ///     Different SSIS source objects should satisfy this interface. Like 
    /// Flat file data source, OleDB data source etc.
    /// </remarks>
    public interface ISource
    {
        /// <summary>
        /// get or set the <see cref="T:ImportLib.IDataSource"/> instance 
        /// that represents the physical data source.
        /// </summary>
        ISourceStorage DataSource
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
        /// Creates the source dataflow component.
        /// </summary>
        /// <param name="package">The package instance.</param>
        /// <param name="dataflowTask">The dataflow task under which the component will be created.</param>
        /// <returns>An instance of <see cref="T:SsisDataflowComponent"/>.</returns>
        DataflowComponent CreateSourceDataFlowComponent(DTSPackage package, DTSExecutable dataflowTask);

        /// <summary>
        /// Initializse the source dataflow component
        /// </summary>
        /// <param name="sourceDataFlowComponent">An instance of <see cref="T:SsisDataflowComponent"/></param>
        void InitializeDataflowComponent(DataflowComponent sourceDataFlowComponent);
    }
}
