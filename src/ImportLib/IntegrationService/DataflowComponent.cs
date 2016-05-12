
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

using PipeLineWrapper = Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
#endregion

namespace ImportLib.IntegrationService
{
    /// <summary>
    /// A simple wrapper for the SSIS's <see cref="T:Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSComponentMetaData90"/>.
    /// </summary>
    public class DataflowComponent
    {
        // inner instance
        private PipeLineWrapper.IDTSComponentMetaData90 dataflowComponent;
        // component instance
        private CManagedComponentWrapper componentInstance;

        /// <summary>
        /// Get or set the inner object
        /// </summary>
        public PipeLineWrapper.IDTSComponentMetaData90 InnerObject
        {
            get { return dataflowComponent; }
            set { dataflowComponent = value; }
        }

        /// <summary>
        /// Get the managed component wrapper instance
        /// </summary>
        public CManagedComponentWrapper ComponentInstance
        {
            get
            {
                if (componentInstance == null)
                {
                    // create a new instance
                    componentInstance = dataflowComponent.Instantiate();
                }
                return componentInstance;
            }
        }

        /// <summary>
        /// Creates a new instance.
        /// </summary>
        /// <param name="dataflowTask">An executable that wraps the dataflow task inside it.</param>
        /// <param name="componentClassID">The component class ID that should be created.</param>
        /// <param name="name">The name of the component.</param>
        public DataflowComponent(DTSExecutable dataflowTask,string componentClassID,string name)
        {
            Executable dataflowExecutable = dataflowTask.InnerObject;       // get the inner object
            TaskHost taskHost = dataflowExecutable as TaskHost;             // get the task host
            MainPipe mainPipeInstance = taskHost.InnerObject as MainPipe;   // get the pipe instance

            // validate
            Debug.Assert(mainPipeInstance != null);
            // now create the component
            dataflowComponent = mainPipeInstance.ComponentMetaDataCollection.New();
            // set the name
            dataflowComponent.Name = name;
            // set the component class ID
            dataflowComponent.ComponentClassID = componentClassID;
        }
    }
}
