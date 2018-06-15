using System;

namespace Woof.SystemEx {

    /// <summary>
    /// Generic exception event arguments missing in <see cref="System"/> namespace.
    /// </summary>
    public class ExceptionEventArgs : EventArgs {

        /// <summary>
        /// Gets the exception.
        /// </summary>
        public Exception Exception { get; }

        /// <summary>
        /// Gets the optional tag object if available.
        /// </summary>
        public object Tag { get; }

        /// <summary>
        /// Creates new exception event arguments.
        /// </summary>
        /// <param name="exception">The exception to pass.</param>
        /// <param name="tag">Optional tag object.</param>
        public ExceptionEventArgs(Exception exception, object tag = null) {
            Exception = exception;
            Tag = tag;
        }

    }

}