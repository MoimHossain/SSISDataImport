
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
    ///     Encapsultes an error that occured during the execution
    /// </summary>
    public class ExecutionError
    {
        private object innerObject;

        /// <summary>
        ///     Get the underlying error object
        /// </summary>
        public object InnerObject
        {
            get { return innerObject; }
        }

        private string description;

        /// <summary>
        ///     Get the error description
        /// </summary>
        public string ErrorDescription
        {
            get { return description; }
        }	

        /// <summary>
        ///     Creates a new instance
        /// </summary>
        /// <param name="description">
        ///     The error description.
        /// </param>
        /// <param name="innerObject">
        ///     The underlying error object.
        /// </param>
        public ExecutionError(string description, object innerObject)
        {
            this.description = description;
            this.innerObject = innerObject;
        }
    }
}
