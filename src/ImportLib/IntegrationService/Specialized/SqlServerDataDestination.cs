
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
using System.Diagnostics;
using ImportLib.Sql;
using Microsoft.SqlServer.Dts.Runtime;
using ImportLib.Supports;
using System.Xml;
using Microsoft.SqlServer.Dts.Tasks.ExecuteSQLTask;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using ImportLib;
#endregion

namespace ImportLib.IntegrationService.Specialized
{
    /// <summary>
    /// Contains the implementation of the IDataDestination interface.
    /// </summary>
    public class SqlServerDataDestination : IDestination
    {
        // The sql server data destination
        private SqlStorageDestination sqlDataDestination;
        // The Moniker text for the Ole DB
        public const string OleDBMoniker = "OLEDB";
        // the compoenent ID
        public const string OleDBDestinationDataFlowComponentID = "{E2568105-9550-4F71-A638-B7FE42E66922}";
        // source
        private ISource ssisSource;
        // connection manager
        private ConnectionManager connectionManager;

        #region ISsisDestination Members

        /// <summary>
        /// Get or set the <see cref="T:ImportLib.IDataDestination"/> instance 
        /// that represents the physical data destination.
        /// </summary>
        public IDestinationStorage DataDestination
        {
            get { return sqlDataDestination; }
            set
            {
                // initializing 
                sqlDataDestination = value as SqlStorageDestination;
                // Assert
                Debug.Assert(sqlDataDestination != null);
            }
        }

        /// <summary>
        /// Get or set the <see cref="T:ImportLib.ISsisSource"/> instance 
        /// </summary>
        public ISource SsisSource
        {
            get { return ssisSource; }
            set { ssisSource = value; }
        }

        /// <summary>
        /// Creates a connection manager instance for a given source file 
        /// into the context of the given <see cref="T:ImportLib.IntegrationService.SsisPackage"/> 
        /// object.
        /// </summary>
        /// <param name="package">An instance of <see cref="T:ImportLib.IntegrationService.SsisPackage"/>.</param>
        public void CreateConnection(DTSPackage package)
        {
            // Creating a connection using the oledb moniker
            connectionManager = package.InnerObject.Connections.Add(OleDBMoniker);
            connectionManager.ConnectionString = GetSsisConnectionString();
            connectionManager.Name = "SSIS Connection Manager for Oledb";
            connectionManager.Description = string.Concat("SSIS Connection Manager for ", sqlDataDestination.DatabaseName);            
        }

        /// <summary>
        /// Creates the task that will create the destination storage (ex. database, table etc);
        /// </summary>
        /// <param name="package">The package where the task should be created</param>
        /// <returns>An instance of <see cref="T:ImportLib.IntegrationService.SsisExecutable"/></returns>
        public DTSExecutable CreateStorageCreationTask(DTSPackage package)
        {
            // get the sql task type
            Type taskType = typeof(ExecuteSQLTask);
            // cteate a task of type ExecuteSQLTask
            DTSExecutable executable = new DTSExecutable(package, taskType);

            // now configuring the new task
            TaskHost taskHost = executable.InnerObject as TaskHost;         // get the Task host instance
            ExecuteSQLTask sqlTask = taskHost.InnerObject as ExecuteSQLTask;// get the sql task from the host
            sqlTask.Connection = connectionManager.Name;                    // set the connection manager
            sqlTask.SqlStatementSource =
                sqlDataDestination.GetDestinationTableCreationSql(ssisSource.DataSource.GetSchemaTable());             
                                                                            // set the sql that generates the table

            return executable;
        }

        /// <summary>
        /// Creates the destination dataflow component.
        /// </summary>
        /// <param name="package">The package instance.</param>
        /// <param name="dataflowTask">The dataflow task under which the component will be created.</param>
        /// <returns>An instance of <see cref="T:SsisDataflowComponent"/>.</returns>
        public DataflowComponent CreateDestinationDataFlowComponent(DTSPackage package, DTSExecutable dataflowTask)
        {
            #region Logging
            Logger.WriteInformation("Creating managed instances for the destination database");
            #endregion
            // create the component now
            DataflowComponent destinationDataFlowComponent = new DataflowComponent(dataflowTask, OleDBDestinationDataFlowComponentID, "Destination Oledb Component");

            // Before going thru the initialization we need to craete the destination table
            // because the SSIS object model will try to access that table fore reading the metadata
            sqlDataDestination.CreateDataStore(ssisSource.DataSource.GetSchemaTable());

            // get the COM instance
            CManagedComponentWrapper managedOleInstance = destinationDataFlowComponent.ComponentInstance;
            // populate the properties
            managedOleInstance.ProvideComponentProperties();
            // setting the connection
            if (destinationDataFlowComponent.InnerObject.RuntimeConnectionCollection.Count > 0)
            {   // If connection is necessary
                destinationDataFlowComponent.InnerObject.RuntimeConnectionCollection[0].ConnectionManagerID =
                    connectionManager.ID;
                destinationDataFlowComponent.InnerObject.RuntimeConnectionCollection[0].ConnectionManager =
                                DtsConvert.ToConnectionManager90(connectionManager);

            }
            // Set the custom properties.
            managedOleInstance.SetComponentProperty("AccessMode", 0);                   // Table of View mode
            managedOleInstance.SetComponentProperty("AlwaysUseDefaultCodePage", false); // Default Codepage
            managedOleInstance.SetComponentProperty("DefaultCodePage", 1252);           // Set it
            managedOleInstance.SetComponentProperty("FastLoadKeepIdentity", false);     // Fast load
            managedOleInstance.SetComponentProperty("FastLoadKeepNulls", false);
            managedOleInstance.SetComponentProperty("FastLoadMaxInsertCommitSize", 0);
            managedOleInstance.SetComponentProperty("FastLoadOptions", "TABLOCK,CHECK_CONSTRAINTS");
            managedOleInstance.SetComponentProperty("OpenRowset",
                string.Format("[{0}].[dbo].[{1}]", sqlDataDestination.DatabaseName, sqlDataDestination.TableName));


            
            #region Logging
            Logger.WriteInformation("Creating managed instances for the destination database....completed");
            #endregion
            return destinationDataFlowComponent;            
        }

        /// <summary>
        /// Initializse the destination dataflow component
        /// </summary>
        /// <param name="destinationDataFlowComponent">An instance of <see cref="T:SsisDataflowComponent"/></param>
        public void InitializeDataflowComponent(DataflowComponent destinationDataFlowComponent)
        {
            #region Logging
            Logger.WriteInformation("Creating the destination columns and their mappings");
            #endregion
            // Get the COM instance
            CManagedComponentWrapper managedOleInstance = destinationDataFlowComponent.ComponentInstance;
            
            // Now activate a connection and create the mappings
            // =========================================================================
            // Establish a connection
            managedOleInstance.AcquireConnections(null);
            // initialize the metadata
            managedOleInstance.ReinitializeMetaData();
            // Get the destination's default input and virtual input.
            IDTSInput90 input = destinationDataFlowComponent.InnerObject.InputCollection[0];
            IDTSVirtualInput90 vInput = input.GetVirtualInput();

            // Iterate through the virtual input column collection.
            foreach (IDTSVirtualInputColumn90 vColumn in vInput.VirtualInputColumnCollection)
            {
                bool res = sqlDataDestination.MapManager.IsSuppressedSourceColumn(vColumn.Name,ssisSource.DataSource.GetSchemaTable());
                if (!res)
                {
                    // Call the SetUsageType method of the destination
                    //  to add each available virtual input column as an input column.
                    managedOleInstance.SetUsageType(
                       input.ID, vInput, vColumn.LineageID, DTSUsageType.UT_READONLY);
                }
            }

            IDTSExternalMetadataColumn90 exColumn;
            foreach (IDTSInputColumn90 inColumn in 
                destinationDataFlowComponent.InnerObject.InputCollection[0].InputColumnCollection)
            {   // create the map
                exColumn = destinationDataFlowComponent.InnerObject.InputCollection[0].ExternalMetadataColumnCollection[inColumn.Name];
                string destName = sqlDataDestination.MapManager.GetDestinationColumn(exColumn.Name).ColumnName;
                exColumn.Name = destName;             

                managedOleInstance.MapInputColumn(destinationDataFlowComponent.InnerObject.InputCollection[0].ID, inColumn.ID, exColumn.ID);
            }
            // Now release the connection
            managedOleInstance.ReleaseConnections();

            // Now remove the table that we did create for the SSIS object model
            sqlDataDestination.DeleteDataStore();

            #region Logging
            Logger.WriteInformation("Creating the destination columns and their mappings.....completed");
            #endregion            
        }

        #endregion

        /// <summary>
        /// Get the SSIS compatible connection string.
        /// </summary>
        /// <returns>A Connection string that is compatible with SSIS</returns>
        /// <remarks>
        ///     The SSIS Oledb connections uses a provider different than usual Sql client provider.
        /// It is "SQLNCLI". Without this provier the SSIS cant create a connection. (!)
        /// On the other hand this provider (SQLNCLI) cant be used as Sql Client (.NET class) connections.
        /// So this procedure converts a Sql client connection string to SSIS compatibale connection
        /// string by modifying the provider.
        /// </remarks>
        private string GetSsisConnectionString()
        {
            //connectionManager.ConnectionString = "Data Source=VSTS;Initial Catalog=TEST;Provider=SQLNCLI;Integrated Security=SSPI;Auto Translate=false;";
            //ConMgr.ConnectionString = "Data Source=VSTS;Initial Catalog=TEST;Integrated Security=True";
            string connectionString = sqlDataDestination.ConnectionString;      // get the sql connection string
            Dictionary<string, string> connectionProperties = new Dictionary<string, string>();

            foreach( string part in connectionString.Split(";".ToCharArray()))
            {   // Itereate thru the properties of the connection string
                string[] keyValue = part.Split("=".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                if (keyValue != null && keyValue.Length == 2)
                {
                    string propertyName = keyValue[0].Trim();  // the name of the property
                    string valueName = keyValue[1].Trim();     // the value of the property
                    // create the entry
                    connectionProperties.Add(propertyName, valueName);
                }
            }
            // Now update these followings
            connectionProperties["Provider"] = "SQLNCLI";
            connectionProperties["Integrated Security"] = "SSPI";
            connectionProperties["Auto Translate"] = "false";
            
            // Now we are going to create the SSIS compatible connectionstring
            StringBuilder ssisCompatibaleConnectionString = new StringBuilder();
            for (Dictionary<string, string>.Enumerator iterator = connectionProperties.GetEnumerator(); iterator.MoveNext(); )
            {   // Iterate
                if (ssisCompatibaleConnectionString.Length > 0)
                {   // If already there is some properties added
                    ssisCompatibaleConnectionString.Append(";");
                }
                ssisCompatibaleConnectionString.Append(string.Format("{0}={1}", iterator.Current.Key, iterator.Current.Value));
            }

            return ssisCompatibaleConnectionString.ToString();            
        }
    }
}
