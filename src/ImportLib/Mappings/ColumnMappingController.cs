
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
    ///     Encapsulates the mapping informations for a source data source 
    /// and a destination data source
    /// </summary>
    [Serializable()]
    public class ColumnMappingController
    {
        /// <summary>
        /// Creates a new instance
        /// </summary>
        public ColumnMappingController()
        {
            mappings = new List<Map>();
        }

        private List<Map> mappings;

        /// <summary>
        /// Get or set the mappings
        /// </summary>
        public List<Map> Mappings
        {
            get { return mappings; }
            set { mappings = value; }
        }

        //private Column[] sourceColumns;

        /// <summary>
        /// Get or set the source columns
        /// </summary>
        //public Column[]SourceColumns
        //{
        //    get { return sourceColumns; }
        //    set { sourceColumns = value; }
        //}

        private Column[] destinationColumns;

        /// <summary>
        /// Get or set the destination columns
        /// </summary>
        public Column[] DestinationColumns
        {
            get { return destinationColumns; }
            set { destinationColumns = value; }
        }

        /// <summary>
        /// Get the destination columns array where the columns are not bind to any source columns
        /// </summary>
        public Column[] UnmappedDestinationColumns
        {
            get
            {
                List<Column> unmappedColumns = new List<Column>();
                foreach (Column destinationColumn in destinationColumns)
                {   // iterate 
                    if (!ContainsInDestinationMap(destinationColumn))
                    {   // if no mapping found
                        unmappedColumns.Add(destinationColumn);
                    }
                }
                return unmappedColumns.ToArray();
            }
        }

        /// <summary>
        /// Get the source columns where the columns are not bind with any destination columns
        /// </summary>
        public Column[] SuppressedSourceColumns(DataTable srcSchemaTable)
        {
            List<Column> suppressedColumns = new List<Column>();
            foreach (DataRow row in srcSchemaTable.Rows)
            {   // iterate 
                string columnName = row["columnName"] as string;
                if (!ContainsInSourceMap(columnName))
                {   // if no mapping found
                    suppressedColumns.Add(new Column(columnName));
                }
            }
            return suppressedColumns.ToArray();
        }

        /// <summary>
        /// Determine if the specified source column is in suppressed list or not
        /// </summary>
        public bool IsSuppressedSourceColumn(string sourceColumnName, DataTable srcSchemaTable)
        {
            return Array.IndexOf<Column>(SuppressedSourceColumns(srcSchemaTable), new Column(sourceColumnName)) > -1;
        }

        /// <summary>
        /// Get the destination column for a given source column 
        /// </summary>
        /// <param name="sourceColumnName">The specified source column</param>
        /// <returns>An instance of <see cref="T:ImportLib.Mappings.Column"/>
        /// which is the destination for the given source column name.</returns>
        public Column GetDestinationColumn(string sourceColumnName)
        {
            foreach (Map map in mappings)
            {   // iterate
                if (map.SourceColumn.ColumnName.Equals(sourceColumnName))
                    return map.DestinationColumn;
            }
            throw new ApplicationException("No mapping defined for the source column " + sourceColumnName);
        }

        /// <summary>
        /// Determines if the specified column is contains into the mapping as a source
        /// </summary>
        /// <param name="destinationColumn">The source column</param>
        /// <returns><c>true</c> if the source column found into the mapping, <c>false</c> otherwise.</returns>
        private bool ContainsInSourceMap(string sourceColumnName)
        {
            foreach (Map map in mappings)
            {   // iterate
                if (map.SourceColumn.ColumnName.Equals(sourceColumnName))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Determines if the specified column is contains into the mapping as a destination
        /// </summary>
        /// <param name="destinationColumn">The destination column</param>
        /// <returns><c>true</c> if the destination column found into the mapping, <c>false</c> otherwise.</returns>
        private bool ContainsInDestinationMap(Column destinationColumn)
        {
            foreach (Map map in mappings)
            {   // iterate
                if (map.DestinationColumn.Equals(destinationColumn))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Validate the content
        /// </summary>
        private void Validate()
        {
            //if (sourceColumns == null) throw new NullReferenceException("SourceColumns is not set to an instance of an object");
            if (destinationColumns == null) throw new NullReferenceException("DestinationColumns is not set to an instance of an object");
        }
    }
}
