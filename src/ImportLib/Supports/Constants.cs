
#region Copyright notice
//===================================================================================
//
//===================================================================================
#endregion

#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;
#endregion

namespace ImportLib.Supports
{
    /// <summary>
    /// Contains the constants fields
    /// </summary>
    public class Constants
    {
        // delimiter
        public const string Delimiter = "delimiter";
        // has header row
        public const string FirstRowIsHeader = "headerRow";
        // HeaderRowDelimiter
        public const string HeaderRowDelimiter = "HeaderRowDelimiter";
        // The text qualifier
        public const string TextQualifier = "TextQualifier";
        // filename
        public const string FileName = "fileName";

        // The sql server name
        public const string SqlServer = "sqlserver";
        // database name
        public const string DatabaseName = "databaseName";
        // Table name
        public const string TableName = "tableName";
        // User name
        public const string DBUserName = "dbUserName";
        // password
        public const string DBUserPassword = "dbUserPassword";
        // Integrated security
        public const string IntegratedSecurity = "integratedSecurity";
        // logging file
        public const string LogfileName = "C:\\SSISTransport.log";
    }
}
