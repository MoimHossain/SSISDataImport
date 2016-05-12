
#region Copyright notice
//===================================================================================
//
//
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
using System.Diagnostics;
#endregion

namespace ImportLib.Delimited
{
    /// <summary>
    /// Encapsulates the implementation of <see cref="T:ImportLib.Platform.DataSource"/>    
    /// </summary>
    /// <remarks>
    ///     This implementation is specific to the delimited source files. (including
    /// csv, text and other flat file sources).
    /// </remarks>
    /// <author>
    ///     Moim Hossain
    /// </author>
    public class DelimitedDataSource : SourceStorage
    {
        // connection string format
        private const string connectionStringFormat = "Provider=Microsoft.Jet.OLEDB.4.0;Extended Properties=\'text;HDR={0};FMT=Delimited\';Data Source=";
        // connection string
        private string connectionString;
        // delimiter
        private string delimiter = ",";         // default is CSV
        private string headerRowDelimiter = "\r\n";
        private string fileName = string.Empty; // file name
        private bool firstRowIsHeader;          // flag for header
        private string textQualifier;

        /// <summary>
        /// Get the text qualifier
        /// </summary>
        public string TextQualifier
        {
            get { return textQualifier; }
        }
	

        /// <summary>
        /// Get the delimiter character
        /// </summary>
        public string Delimiter
        {
            get { return delimiter; }
        }

        /// <summary>
        /// Get the header row delimiter
        /// </summary>
        public string HeaderRowDelimiter
        {
            get { return headerRowDelimiter; }
        }

        /// <summary>
        /// Get the file name
        /// </summary>
        public string FileName
        {
            get { return fileName; }
        }

        /// <summary>
        /// Is First Row contains the Header informations
        /// </summary>
        public bool FirstRowIsHeader
        {
            get { return firstRowIsHeader; }
        }	

        /// <summary>
        /// Get the Category of the underlying persistant storage
        /// </summary>
        public override StorageMedium StorageMedium
        {
            get { return StorageMedium.DelimitedFile; }
        }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        public DelimitedDataSource()
        {
            firstRowIsHeader = false;
        }

        /// <summary>
        /// Performs the initialization tasks for the Delimited source instance
        /// </summary>
        protected override void OnInitialization()
        {   
            if (InitializationProperties.ContainsKey(Constants.Delimiter))
            {
                delimiter = InitializationProperties[Constants.Delimiter] as string;
            }
            if (InitializationProperties.ContainsKey(Constants.HeaderRowDelimiter))
            {
                headerRowDelimiter = InitializationProperties[Constants.HeaderRowDelimiter];
            }
            if (InitializationProperties.ContainsKey(Constants.TextQualifier))
            {
                textQualifier = InitializationProperties[Constants.TextQualifier];
                // If it is supplied from the user then it must be not-null
                Debug.Assert(textQualifier != null);
            }

            ReadHeaderRowPresence();    // If the header row is presnt
            string header = FirstRowIsHeader ? "Yes" : "No";
            connectionString = String.Format(connectionStringFormat, header);

            fileName = string.Empty + InitializationProperties[Constants.FileName] as string;
            if (string.IsNullOrEmpty(fileName.Trim()))
            {
                throw new ApplicationException("File name is invalid.");
            }

            
            base.OnInitialization();
        }

        /// <summary>
        /// Read if the header row is present into the file
        /// </summary>
        private void ReadHeaderRowPresence()
        {
            if (InitializationProperties.ContainsKey(Constants.FirstRowIsHeader) && InitializationProperties[Constants.FirstRowIsHeader] != null)
            {   // If the header row is present
                string value = InitializationProperties[Constants.FirstRowIsHeader].ToUpper();
                firstRowIsHeader = value.StartsWith("Y") || value.StartsWith("T");
                                    // If we provide the value YES or TRUE both will work
            }
        }

        /// <summary>
        /// We need to create a schema file because we will read the source 
        /// file (delimited file) schema thru a Ole reader and the Ole reader 
        /// need a schema file to read those informations.
        /// </summary>
        private void CreateSchemaFile()
        {
            #region Logging
            Logger.WriteInformation("Creating the OleDB schema file for the flat file source");
            #endregion
            FileInfo fInfo = new FileInfo(fileName);
            if (!fInfo.Exists)
            {
                throw new ApplicationException("Source file does not exits.");
            }

            string schemaFileName = Path.Combine(fInfo.DirectoryName, "schema.ini");
            // If the file already exist
            if (File.Exists(schemaFileName))
                File.Delete(schemaFileName);    // Delete it
            using (StreamWriter writer = new StreamWriter(schemaFileName))
            {
                // Write apropriate content in the schema.ini file
                writer.WriteLine(string.Format("[{0}]", fInfo.Name));
                writer.WriteLine(string.Format("Format=Delimited({0})", delimiter));
                writer.WriteLine("TextDelimiter=\"");

                if (InitializationProperties.ContainsKey("ColNameHeader"))
                    writer.WriteLine(string.Format("ColNameHeader={0}", InitializationProperties["ColNameHeader"]));

                // Close and save the schema.ini file
                writer.Close();
            }
            #region Logging
            Logger.WriteInformation("Creating the OleDB schema file for the flat file source........completed");
            #endregion
            
        }

        /// <summary>
        /// Delete the schema file that was created before schema retrieval task.
        /// </summary>
        private void DeleteSchemaFile()
        {
            FileInfo fInfo = new FileInfo(fileName);
            if (!fInfo.Exists)
            {
                throw new ApplicationException("Source file does not exits.");
            }

            string schemaFileName = Path.Combine(fInfo.DirectoryName, "schema.ini");
            // If the file already exist
            if (File.Exists(schemaFileName))
                File.Delete(schemaFileName);    // Delete it
        }
               

        /// <summary>
        /// Get the schema table for the data source 
        /// </summary>
        /// <returns>An instance of <see cref="T:DataTable"/> that contains the schema
        /// informations of the data source</returns>
        protected override System.Data.DataTable GetSchemaTableCore()
        {
            #region Logging
            Logger.WriteInformation("Reading the source column schema");
            #endregion
            DataTable table = null;                     // DataTable instance

            FileInfo fInfo = new FileInfo(fileName);    // File informations
            // Make the connection string
            string conStr = connectionString + fInfo.DirectoryName;

            // Create the schema file in order to read thru Ole
            CreateSchemaFile();

            // Create a connection objct and open it
            using (OleDbConnection connection = new OleDbConnection(conStr))
            {
                connection.Open();
                // Create a query to read the OleDb source
                string commandText = string.Format("SELECT * FROM [{0}]", fInfo.Name);
                using (OleDbCommand command = new OleDbCommand(commandText, connection))
                {
                    // Execute the query and get the DataReader
                    using (IDataReader reader = command.ExecuteReader())
                    {
                        // get the schema TABLE
                        table = reader.GetSchemaTable();
                    }
                }
                connection.Close();     // close the connection
            }
            // delete the schema file
            DeleteSchemaFile();
            #region Logging
            Logger.WriteInformation("Reading the source column schema........completed");
            #endregion         
   
            // Update the table column names
            UpdateTextQualifier(table);
            return table;
        }

        /// <summary>
        /// Updates each column with text qualifier
        /// </summary>
        /// <param name="table"></param>
        /// <remarks>
        ///     For some reason we failed to get teh text qualifier while reading a file through 
        /// Ole reader, so we are apprently manually creating the qualifiers
        /// </remarks>
        private void UpdateTextQualifier(DataTable table)
        {            
            if (textQualifier != null)
            {   // If user has supplied a text qualifier
                foreach (DataColumn clm in table.Columns)
                {
                    clm.ReadOnly = false;
                }
                foreach (DataRow row in table.Rows)
                {
                    row["ColumnName"] = 
                        string.Format("{0}{1}{2}", textQualifier, row["ColumnName"] as string, textQualifier);
                }
                foreach (DataColumn clm in table.Columns)
                {
                    clm.ReadOnly = true;
                }
            }
        }
    }
}
