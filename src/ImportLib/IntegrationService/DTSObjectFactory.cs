
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
using ImportLib.IntegrationService.Specialized;
#endregion

namespace ImportLib.IntegrationService
{
    /// <summary>
    ///     Defines some factory methods that help to load specialized 
    /// instances for some defined interfaces.
    /// </summary>
    public sealed class DTSObjectFactory
    {
        /// <summary>
        /// Preventing instance creation
        /// </summary>
        private DTSObjectFactory()
        { }

        /// <summary>
        ///     Get a <see cref="T:ImportLib.IntegrationService.ISsisSource"/>
        /// instance for a given <see cref="T:ImportLib.IDataSource"/> object.
        /// </summary>
        /// <param name="source">Given <see cref="T:ImportLib.IDataSource"/> object</param>
        /// <returns>An instance of <see cref="T:ImportLib.IntegrationService.ISsisSource"/>.</returns>
        public static ISource GetSsisSource(ISourceStorage source)
        {
            ISource ssisSource = null;
            switch (source.StorageMedium)
            {
                case StorageMedium.DelimitedFile:
                    ssisSource = new FlatFileSource();
                    break;

            }
            return ssisSource;
        }

        /// <summary>
        ///     Get a <see cref="T:ImportLib.IntegrationService.ISsisDestination"/>
        /// instance for a given <see cref="T:ImportLib.IDataDestination"/> object.
        /// </summary>
        /// <param name="destination">Given <see cref="T:ImportLib.IDataDestination"/> object</param>
        /// <returns>An instance of <see cref="T:ImportLib.IntegrationService.ISsisDestination"/>.</returns>
        public static IDestination GetSsisDestination(IDestinationStorage destination)
        {
            IDestination ssisDestination = null;
            switch (destination.StorageMedium)
            {
                case StorageMedium.SqlServerDatabase:
                    ssisDestination = new SqlServerDataDestination();
                    break;
            }
            return ssisDestination;
        }

        /// <summary>
        ///     Get a <see cref="T:ImportLib.IntegrationService.ISsisDataflowManager"/>
        /// instance.
        /// </summary>
        /// <returns>An instance of <see cref="T:ImportLib.IntegrationService.ISsisDataflowManager"/>.</returns>
        public static IDataflowManager GetSsisDataflowManager()
        {
            IDataflowManager ssisTransformer = null;
            ssisTransformer = new DataflowManager();
            return ssisTransformer;
        }

    }
}
