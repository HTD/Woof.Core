using System;

namespace Woof.Automation {

    /// <summary>
    /// Event arguments for reporting progress in integer percents.
    /// </summary>
    public class PercentEventArgs : EventArgs {

        /// <summary>
        /// Gets the integer percent value of the operation progress.
        /// </summary>
        public int Value { get; }

        /// <summary>
        /// Creates new event arguments for reporting percent progress.
        /// </summary>
        /// <param name="value">Integer percent value of the operation progress.</param>
        public PercentEventArgs(int value) => Value = value;

    }

}