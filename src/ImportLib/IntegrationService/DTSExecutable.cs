
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
using ImportLib.Delimited;
using System.Diagnostics;
using Microsoft.SqlServer.Dts.Runtime;
using RuntimeWrapper = Microsoft.SqlServer.Dts.Runtime.Wrapper;

#endregion

namespace ImportLib.IntegrationService
{
    /// <summary>
    /// A simple wrapper for the SSIS's <see cref="T:Microsoft.SqlServer.Dts.Runtime.Executable"/>
    /// </summary>
    public class DTSExecutable
    {
        // inner object
        private Executable executable;

        /// <summary>
        /// Get or set the underlying <see cref="T:Microsoft.SqlServer.Dts.Runtime.Executable"/> instance.
        /// </summary>
        public Executable InnerObject
        {
            get { return executable;  }
            set { executable = value; }
        }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="package">The package where the task should be created</param>
        /// <param name="taskType">The type of the task</param>
        public DTSExecutable(DTSPackage package, Type taskType) : this( package, taskType.AssemblyQualifiedName)
        {
        }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="package">The package where the task should be created</param>
        /// <param name="moniker">The moniker of the task</param>
        public DTSExecutable(DTSPackage package,string moniker)
        {
            // validate
            Debug.Assert(package != null && package.InnerObject != null);
            // create the task here
            executable = package.InnerObject.Executables.Add(moniker);
        }
    }
}
