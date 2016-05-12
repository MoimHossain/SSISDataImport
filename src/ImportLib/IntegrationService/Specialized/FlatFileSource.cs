
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
using ImportLib.Delimited;
using System.Diagnostics;
using Microsoft.SqlServer.Dts.Runtime;
using RuntimeWrapper = Microsoft.SqlServer.Dts.Runtime.Wrapper;
using System.Xml;
using ImportLib.Supports;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using ImportLib;
#endregion

namespace ImportLib.IntegrationService.Specialized
{
    /// <summary>
    /// Contains the implementations of the <see cref="T:ImportLib.IntegrationService.ISsisSource"/>
    /// interface.
    /// </summary>
    public class FlatFileSource : ISource
    {
        // The delimited data source intance
        private DelimitedDataSource delimitedDataSource;
        // The Moniker text for the flatfile source
        public const string FlatFileMoniker = @"FLATFILE";
        // The source data flow component GUID
        public const string SourceDataFlowComponentID = "{90C7770B-DE7C-435E-880E-E718C92C0573}";
        // schema table
        private DataTable schemaTable = null;
        // The connection manager instance
        private ConnectionManager connectionManager;


        /// <summary>
        /// Creates a new instance
        /// </summary>
        public FlatFileSource()
        {
            
        }

        #region ISsisSource Members

        /// <summary>
        /// get or set the <see cref="T:ImportLib.IDataSource"/> instance 
        /// that represents the physical data source.
        /// </summary>
        public ISourceStorage DataSource
        {
            get { return delimitedDataSource; }
            set
            {
                // initializing
                delimitedDataSource = value as DelimitedDataSource;
                // Get the schema table
                schemaTable = value.GetSchemaTable();
                // Assert
                Debug.Assert(delimitedDataSource != null);
            }
        }

        /// <summary>
        /// Creates a connection manager instance for a given source file 
        /// into the context of the given <see cref="T:ImportLib.IntegrationService.SsisPackage"/> 
        /// object.
        /// </summary>
        /// <param name="package">An instance of <see cref="T:ImportLib.IntegrationService.SsisPackage"/>.</param>        
        public void CreateConnection(DTSPackage package)
        {
            #region Logging
            Logger.WriteInformation("Creating connection to the source file.");
            #endregion
            // creating a connection manager instance using the FLATFILE moniker
            connectionManager = package.InnerObject.Connections.Add(FlatFileMoniker);            
            connectionManager.ConnectionString = delimitedDataSource.FileName;
            connectionManager.Name = "SSIS Connection Manager for Files";
            connectionManager.Description = string.Concat("SSIS Connection Manager");
            // Setting some common properties of the connection manager object
            connectionManager.Properties["ColumnNamesInFirstDataRow"].SetValue(connectionManager, delimitedDataSource.FirstRowIsHeader);
            connectionManager.Properties["Format"].SetValue(connectionManager, "Delimited");
            connectionManager.Properties["HeaderRowDelimiter"].SetValue(connectionManager, delimitedDataSource.HeaderRowDelimiter);
            if (delimitedDataSource.TextQualifier != null)
            {   // If user has been specified a text qualifier then put it into the connection string property
                connectionManager.Properties["TextQualifier"].SetValue(connectionManager, delimitedDataSource.TextQualifier);
            }
            // create the source columns into the connection manager
            CreateSourceColumns();

            #region Logging
            Logger.WriteInformation("Creating connection to the source file.....Completed");
            #endregion            
        }

        /// <summary>
        /// Creates the source dataflow component.
        /// </summary>
        /// <param name="package">The package instance.</param>
        /// <param name="dataflowTask">The dataflow task under which the component will be created.</param>
        /// <returns>An instance of <see cref="T:SsisDataflowComponent"/>.</returns>
        public DataflowComponent CreateSourceDataFlowComponent(DTSPackage package, DTSExecutable dataflowTask)
        {
            // create the component
            DataflowComponent sourceDataFlowComponent = new DataflowComponent(dataflowTask, SourceDataFlowComponentID, "Source Data Flow component");            

            return sourceDataFlowComponent;
        }

        /// <summary>
        /// Initializse the source dataflow component
        /// </summary>
        /// <param name="sourceDataFlowComponent">An instance of <see cref="T:SsisDataflowComponent"/></param>
        public void InitializeDataflowComponent(DataflowComponent sourceDataFlowComponent)
        {
            #region Logging
            Logger.WriteInformation("Initializing the managed instance for the source file.");
            #endregion
            // load the COM for the given GUID
            CManagedComponentWrapper managedFlatFileInstance = sourceDataFlowComponent.ComponentInstance;
            // get the populate the properties
            managedFlatFileInstance.ProvideComponentProperties();
            // putting the connection
            if (sourceDataFlowComponent.InnerObject.RuntimeConnectionCollection.Count > 0)
            {   // If connection is necessary
                sourceDataFlowComponent.InnerObject.RuntimeConnectionCollection[0].ConnectionManagerID =
                                connectionManager.ID;
                sourceDataFlowComponent.InnerObject.RuntimeConnectionCollection[0].ConnectionManager =
                                DtsConvert.ToConnectionManager90(connectionManager);

            }
            // establish a connection
            managedFlatFileInstance.AcquireConnections(null);
            // Initialize the metadata
            managedFlatFileInstance.ReinitializeMetaData();
            // create the mapping now
            IDTSExternalMetadataColumn90 exOutColumn;
            foreach (IDTSOutputColumn90 outColumn in
                sourceDataFlowComponent.InnerObject.OutputCollection[0].OutputColumnCollection)
            {   // create the MAP
                exOutColumn =
                    sourceDataFlowComponent.InnerObject.OutputCollection[0].ExternalMetadataColumnCollection[outColumn.Name];
                // map it
                managedFlatFileInstance.MapOutputColumn(
                    sourceDataFlowComponent.InnerObject.OutputCollection[0].ID, outColumn.ID, exOutColumn.ID, true);
            }
            // Release the connection now
            managedFlatFileInstance.ReleaseConnections();

            #region Logging
            Logger.WriteInformation("Initializing the managed instance for the source file......completed");
            #endregion            
        }

        #endregion

        /// <summary>
        /// Creates the source columns for the falt file connection manager instance
        /// </summary>
        private void CreateSourceColumns()
        {
            // get the actuall connection manger instance
            RuntimeWrapper.IDTSConnectionManagerFlatFile90 flatFileConnection =
                connectionManager.InnerObject as RuntimeWrapper.IDTSConnectionManagerFlatFile90;

            RuntimeWrapper.IDTSConnectionManagerFlatFileColumn90 column;
            RuntimeWrapper.IDTSName90 name;
            
            // trace the current count
            Debug.WriteLine(flatFileConnection.Columns.Count);

            DataTable schemaTable = DataSource.GetSchemaTable(); // get the schema table
            
            foreach (DataRow row in schemaTable.Rows)
            {   // iterate
                string colName = row["ColumnName"] as string;    // get the col name
                // now create a new column for the connection manager
                column = flatFileConnection.Columns.Add();       // if this is the last row

                if (schemaTable.Rows.IndexOf(row) == (schemaTable.Rows.Count - 1))
                    column.ColumnDelimiter = delimitedDataSource.HeaderRowDelimiter;
                // add the row delimiter
                else
                    column.ColumnDelimiter = delimitedDataSource.Delimiter;

                column.TextQualified = delimitedDataSource.TextQualifier != null;
                column.ColumnType = "Delimited";
                column.DataType = RuntimeWrapper.DataType.DT_WSTR;  // Apparently I am giving this..I need to do Rnd no this for clear idea
                column.DataPrecision = 0;
                column.DataScale = 0;
                name = (RuntimeWrapper.IDTSName90)column;
                name.Name = colName;
            }
        }
    }
}
