using System;

namespace Woof.SystemEx {

    /// <summary>
    /// Event arguments for reporting item operations.
    /// </summary>
    /// <typeparam name="T">Item type.</typeparam>
    public class ItemEventArgs<T> : EventArgs {

        /// <summary>
        /// Gets the item processed.
        /// </summary>
        public T Item { get; }

        /// <summary>
        /// Creates new event arguments for item processing.
        /// </summary>
        /// <param name="item">Item being processed.</param>
        public ItemEventArgs(T item) => Item = item;

    }

}
