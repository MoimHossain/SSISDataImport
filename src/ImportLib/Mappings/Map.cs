
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

using System.Xml;
#endregion


namespace ImportLib.Mappings
{
    /// <summary>
    ///     Defines a map between a source column and a destination column.
    /// Also defines the type of source column and destination column.
    /// </summary>
    [Serializable()]
    public class Map
    {
        /// <summary>
        /// Create a new instance
        /// </summary>
        public Map()
            : this(string.Empty, string.Empty)
        {
        }

        /// <summary>
        /// Create a new instance 
        /// </summary>
        /// <param name="sourceColumnName">The name of the source column</param>
        /// <param name="destinationColumnName">The name of the destination column</param>
        public Map(string sourceColumnName, string destinationColumnName)
            : this(sourceColumnName, destinationColumnName, DbType.String, DbType.String)
        {

        }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="sourceColumnName">The name of the source column</param>
        /// <param name="destinationColumnName">The name of the destination column</param>
        /// <param name="sourceColumnType">The source column type</param>
        /// <param name="destinationColumnType">The destination column type</param>
        public Map(
            string sourceColumnName, string destinationColumnName, 
            DbType sourceColumnType, DbType destinationColumnType)
            : this( 
            new Column(sourceColumnName,sourceColumnType) , 
            new Column(destinationColumnName,destinationColumnType)
            )
        {
            
        }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="sourceColumn">Source Column</param>
        /// <param name="destinationColumn">Destination column</param>
        public Map(Column sourceColumn, Column destinationColumn)
        {
            this.sourceColumn = sourceColumn;
            this.destinationColumn = destinationColumn;
        }

        private Column sourceColumn;

        /// <summary>
        /// Get or set the source column
        /// </summary>
        public Column SourceColumn
        {
            get { return sourceColumn; }
            set { sourceColumn = value; }
        }

        private Column destinationColumn;

        /// <summary>
        /// Get or set the destination column
        /// </summary>
        public Column DestinationColumn
        {
            get { return destinationColumn; }
            set { destinationColumn = value; }
        }	
    }
}
