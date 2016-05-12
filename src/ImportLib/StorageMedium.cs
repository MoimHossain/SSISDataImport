
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

namespace ImportLib
{
    /// <summary>
    /// Categorized the different persistant storages
    /// </summary>
    [Serializable()]
    public enum StorageMedium
    {
        /// <summary>
        /// Delimited files. Such as csv, txt etc
        /// </summary>
        DelimitedFile,

        /// <summary>
        /// Sql server databases
        /// </summary>
        SqlServerDatabase,

        /// <summary>
        /// Excel sheets
        /// </summary>
        Excel,

        /// <summary>
        /// Portable document format files (pdf)
        /// </summary>
        PortableDocumentFormat
    }
}
