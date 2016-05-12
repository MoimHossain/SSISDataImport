
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
    ///     Defines a column.
    /// </summary>
    /// <remarks>
    ///     Instance of this class will rigidly used for mapping purpose only.
    /// We could use <see cref="T:System.Data.DataColumn"/> for this purpose, but
    /// we need to marshall instances of this class across applications (ex. ASP.NET
    /// environment to Windows Services) and <see cref="T:System.Data.DataColumn"/> is
    /// not suitable for this purpose. We need a serializable instance for this purpose.
    /// </remarks>
    [Serializable()]
    public class Column
    {
        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="columnName">The name of the column</param>
        public Column(string columnName) : this(columnName,DbType.String)
        {
        }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="columnName">The name of the column</param>
        /// <param name="dataType">The data type of the column</param>
        public Column(string columnName,DbType dataType)
        {
            this.columnName = columnName;
            this.dataType = dataType;
        }

        private string columnName;

        /// <summary>
        /// Get or set the column name
        /// </summary>
        public string ColumnName
        {
            get { return columnName; }
            set { columnName = value; }
        }

        private DbType dataType;

        /// <summary>
        /// Get or set the data type
        /// </summary>
        public DbType DataType
        {
            get { return dataType; }
            set { dataType = value; }
        }

        /// <summary>
        /// Determins whether the specified System.Object is equal to the current System.Object
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            return this.columnName.Equals( (obj as Column).columnName );
        }

        /// <summary>
        /// Serves a hash function for this type
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return this.columnName.GetHashCode();
        }

        /// <summary>
        /// Represents as a string
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return columnName;
        }
    }
}
