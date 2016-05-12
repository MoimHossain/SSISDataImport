
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

using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
#endregion


namespace ImportLib.IntegrationService.Specialized
{
    /// <summary>
    ///     Encapsulates some SSIS specific utility methods inside this class
    /// </summary>
    public class DTSUtils
    {
        /// <summary>
        ///     Connects two executables by creating a constraint precedence.
        /// </summary>
        /// <param name="package">The package instance.</param>
        /// <param name="firstExecutable">The first executable instance.</param>
        /// <param name="secondExecutable">The second executable instance.</param>
        public static void ConnectExecutables(DTSPackage package, DTSExecutable firstExecutable, DTSExecutable secondExecutable)
        {
            PrecedenceConstraint precedenceConstraint = 
                package.InnerObject.PrecedenceConstraints.Add(firstExecutable.InnerObject, secondExecutable.InnerObject);
            precedenceConstraint.Value = DTSExecResult.Success;             // Apparently. later make this as parameter
            precedenceConstraint.EvalOp = DTSPrecedenceEvalOp.Constraint;
        }

        /// <summary>
        /// Create a path between two dataflow components.
        /// </summary>
        /// <param name="dataflowTask">The dataflow task instance.</param>
        /// <param name="sourceDataflowComponent">The source dataflow component</param>
        /// <param name="destinationDataflowComponent">The destination dataflow component</param>
        public static void CreatePathBetweenDataflowComponents(DTSExecutable dataflowTask,DataflowComponent sourceDataflowComponent, DataflowComponent destinationDataflowComponent)
        {
            Executable dataflowExecutable = dataflowTask.InnerObject;       // get the inner object
            TaskHost taskHost = dataflowExecutable as TaskHost;             // get the task host
            MainPipe mainPipeInstance = taskHost.InnerObject as MainPipe;   // get the pipe instance
            // now create the path
            IDTSPath90 path = mainPipeInstance.PathCollection.New();
            path.AttachPathAndPropagateNotifications(
                sourceDataflowComponent.InnerObject.OutputCollection[0],
                destinationDataflowComponent.InnerObject.InputCollection[0]);
        }
    }
}
