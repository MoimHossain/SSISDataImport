
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
    ///     Encapsulates the result of a package execution
    /// </summary>
    public class ExecutionResult
    {
        private ExecResult result;

        /// <summary>
        /// Get the Execution Result
        /// </summary>
        public ExecResult ExecResult
        {
            get { return result; }
        }

        private ExecutionError[] errors;

        /// <summary>
        /// Get the Errors
        /// </summary>
        public ExecutionError[] Errors
        {
            get { return errors; }
        }
	
        /// <summary>
        /// Creates a new instance
        /// </summary>
        internal ExecutionResult() : this( ExecResult.Success,new ExecutionError[]{})
        {
            // Nothing TODO
        }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="result">
        ///     The <see cref="T:ImportLib.ExecResult"/>.
        /// </param>
        /// <param name="errors">
        ///     An array of <see cref="T:ImportLib.ExecutionError"/> objects.
        /// </param>
        internal ExecutionResult(ExecResult result, ExecutionError[] errors)
        {
            this.result = result;
            this.errors = errors;
        }
    }

    /// <summary>
    /// The Execution result values
    /// </summary>
    public enum ExecResult
    {
        /// <summary>
        /// The execution was successful
        /// </summary>
        Success,

        /// <summary>
        /// The execution was unsuccessful
        /// </summary>
        Failed,

        /// <summary>
        /// Completion
        /// </summary>
        Completion,

        /// <summary>
        /// Cancelled
        /// </summary>
        Cancelled
    }
}
