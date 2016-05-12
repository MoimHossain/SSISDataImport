
#region Copyright notice
//===================================================================================
//
//===================================================================================
#endregion

#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;
using ImportLib.Platform;
using System.IO;
using System.Data;
using System.Data.OleDb;
using ImportLib.Supports;
using ImportLib.Sql.Helper;
using System.Diagnostics;
using ImportLib;
using ImportLib.Mappings;
#endregion

namespace ImportLib.Sql
{
    /// <summary>
    ///     Defines a data destination implementation of <see cref="T:ImportLib.IDataDestination"/>
    /// interface.
    /// </summary>
    public class SqlStorageDestination : DestinationStorage
    {
        // The name of the database server
        private string databaseServer = string.Empty;
        // database name 
        private string databaseName = string.Empty;
        // table name
        private string tableName = string.Empty;
        // integrated security
        private string integratedSecurity = string.Empty;
        // database user name
        private string databaseUserName = string.Empty;
        // database password
        private string databasePassword = string.Empty;

        /// <summary>
        /// Get the database user name
        /// </summary>
        public string DatabaseUserName
        {
            get { return databaseUserName; }
            set { databaseUserName = value; }
        }

        /// <summary>
        /// Get the database password
        /// </summary>
        public string DatabasePassword
        {
            get { return databasePassword; }
            set { databasePassword = value; }
        }

        /// <summary>
        /// Get the integrated security
        /// </summary>
        public string IntegratedSecurity
        {
            get { return integratedSecurity; }
            set { integratedSecurity = value; }
        }

        /// <summary>
        /// Get the connection string
        /// </summary>
        public string ConnectionString
        {
            get 
            { 
                string connString = string.Empty;
                connString = string.Format(
                        CoreConnectionString + ";Initial Catalog={0};",
                        databaseName);

                #region Logging
                Logger.WriteInformation("Connection string for the destination database-->" + connString);
                #endregion
                return connString;
            }
        }

        /// <summary>
        ///     Get a connection string that does not point any initial catalog
        /// </summary>
        public string CoreConnectionString
        {
            get
            {
                string connString = string.Empty;
                connString = string.Format(
                        "Data Source={0};Integrated Security={1};User Id={2};Password={3}",
                        databaseServer, integratedSecurity, databaseUserName, databasePassword);

                #region Logging
                Logger.WriteInformation("Connection string for the destination database-->" + connString);
                #endregion
                return connString;
            }
        }

        /// <summary>
        /// Get the database name
        /// </summary>
        public string DatabaseName
        {
            get { return databaseName; }
        }

        /// <summary>
        /// Get the table name
        /// </summary>
        public string TableName
        {
            get { return tableName; }
        }

        /// <summary>
        /// Get the Category of the underlying persistant storage
        /// </summary>
        public override StorageMedium StorageMedium
        {
            get { return StorageMedium.SqlServerDatabase; }
        }

        /// <summary>
        /// Perform the initialization task for this instance
        /// </summary>
        protected override void OnInitialization()
        {
            if (InitializationProperties.ContainsKey(Constants.IntegratedSecurity))
            {   // If user has provided Integrated security
                integratedSecurity = InitializationProperties[Constants.IntegratedSecurity];
            }
            // read the database credentials
            databaseUserName = InitializationProperties[Constants.DBUserName];
            databasePassword = InitializationProperties[Constants.DBUserPassword];

            databaseServer = InitializationProperties[Constants.SqlServer];
            databaseName = InitializationProperties[Constants.DatabaseName];
            tableName = InitializationProperties[Constants.TableName];

            // validate data
            Trace.Assert(!string.IsNullOrEmpty(databaseServer));
            // validate data
            Trace.Assert(!string.IsNullOrEmpty(databaseName));

            base.OnInitialization();
        }

        /// <summary>
        /// Create the datastore
        /// </summary>
        /// <param name="schemaTable">The schema table instance.</param>
        public void CreateDataStore(DataTable schemaTable)
        {
            string createDataStoreCommand = "create database "+ databaseName;

            try {
                // execute now
                SqlHelper.ExecuteNonQuery(CoreConnectionString, CommandType.Text, createDataStoreCommand);
            }
            catch (Exception databaseCreateFailedEx)
            {
                #region Logging
                Logger.WriteError("Database creation failed. Propably database already exists..");
                #endregion
            }
            
            #region Logging
            Logger.WriteInformation("Ensured datastore...completed.");
            #endregion


            // get the sql
            string commandText = GetTableCreationSql(schemaTable);

            #region Logging
            Logger.WriteInformation("Table creation statement for the designtime SSIS destination manger");
            #endregion


            // execute now
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, commandText);

            #region Logging
            Logger.WriteInformation("Datastore and table creation completed.");
            #endregion

        }

        /// <summary>
        /// Delete the datastore
        /// </summary>
        public void DeleteDataStore()
        {
            // get the sql
            string commandText = string.Format("DROP TABLE [{0}].[dbo].[{1}]",databaseName,tableName); ;
            // execute now
            SqlHelper.ExecuteNonQuery(ConnectionString, CommandType.Text, commandText);
        }

        /// <summary>
        /// Get the sql that used to create the data table
        /// </summary>
        /// <param name="schemaTable">The Schematable instance</param>
        /// <returns>The sql that can crate the table</returns>
        public string GetTableCreationSql(DataTable schemaTable)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(string.Format("CREATE TABLE [{0}].[dbo].[{1}] (",
                databaseName,
                tableName));
            bool commaRequired = false;

            foreach (DataRow row in schemaTable.Rows)
            {   // iterate
                string colName = row["ColumnName"] as string;
                if (commaRequired)
                    sql.Append(" , ");
                sql.Append("[" + colName + "]  " + GetDataType(row));
                commaRequired = true;
            }
            sql.Append(")");
            return sql.ToString();
        }

        /// <summary>
        /// Generates the SQL depending onto the mapping informations
        /// </summary>
        /// <param name="schemaTable"></param>
        /// <returns></returns>
        public string GetDestinationTableCreationSql(DataTable schemaTable)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append(string.Format("CREATE TABLE [{0}].[dbo].[{1}] (",
                databaseName,
                tableName));
            bool commaRequired = false;

            foreach (Column col in MapManager.DestinationColumns)
            {   // iterate
                string colName = col.ColumnName;

                if (commaRequired)
                    sql.Append(" , ");
                sql.Append("[" + colName + "]  " + "NVARCHAR(255)");
                commaRequired = true;
            }
            sql.Append(")");
            return sql.ToString();
        }



        /// <summary>
        /// Get the data type
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private string GetDataType(DataRow row)
        {
            return "NVARCHAR(255)"; // Implement later
        }
    }
}
