
#region Copyright notice
//===================================================================================
//
//===================================================================================
#endregion

#region Using Directives
using System;
using System.Collections.Generic;
using System.Text;
using ImportLib.Delimited;
using ImportLib.Supports;
using System.Data;
using ImportLib.Sql;
using ImportLib.IntegrationService;
#endregion

namespace ImportLib
{
    /// <summary>
    ///     Defines some factory methods that help to create instances for 
    /// some defined interfaces.
    /// </summary>
    public sealed class ObjectFactory
    {
        /// <summary>
        /// Preventing instance creation.
        /// </summary>
        private ObjectFactory()
        {

        }

        /// <summary>
        ///     Get a <see cref="T:ImportLib.IDataSource"/>
        /// instance for a given <see cref="T:ImportLib.StorageMedium"/>.
        /// </summary>
        /// <param name="storageMedium">The <see cref="ImportLib.StorageMedium"/>.</param>
        /// <returns>An instance of <see cref="T:ImportLib.IDataSource"/>.</returns>
        public static ISourceStorage GetDataSource(StorageMedium storageMedium)
        {
            ISourceStorage sourceInstance = null;      // The IDatasource instance

            switch (storageMedium)
            {
                case StorageMedium.DelimitedFile:
                    sourceInstance = new DelimitedDataSource();
                    break;
            }

            return sourceInstance;
        }

        /// <summary>
        ///     Get a <see cref="T:ImportLib.IDataDestination"/>
        /// instance for a given <see cref="T:ImportLib.StorageMedium"/>.
        /// </summary>
        /// <param name="storageMedium">The <see cref="ImportLib.StorageMedium"/>.</param>
        /// <returns>An instance of <see cref="T:ImportLib.IDataDestination"/>.</returns>
        public static IDestinationStorage GetDataDestination(StorageMedium storageMedium)
        {
            IDestinationStorage destinationInstance = null;      // The IDataDestination instance

            switch (storageMedium)
            {
                case StorageMedium.SqlServerDatabase:
                    destinationInstance = new SqlStorageDestination();
                    break;
            }

            return destinationInstance;
        }
    }
}
