using System;
using System.Collections;
using System.Collections.Generic;

namespace Woof.SystemEx {

    /// <summary>
    /// Date range generator
    /// </summary>
    public class DateRange : IEnumerable<DateTime> {

        /// <summary>
        /// Creates a date range to iterate between 2 dates.
        /// </summary>
        /// <param name="since">Start date, the first returned value.</param>
        /// <param name="until">End date, NOT INCLUDED, iteration ends before that date.</param>
        public DateRange(DateTime since, DateTime until) {
            Since = since.Date;
            Until = until.Date;
        }

        /// <summary>
        /// Enumerates all dates between start date (inclusive) and end date (exclusive).
        /// </summary>
        /// <returns>Generic enumerator.</returns>
        public IEnumerator<DateTime> GetEnumerator() {
            var d = Since;
            while (d < Until) {
                yield return d.Date;
                d = d.AddDays(1);
            }
        }

        /// <summary>
        /// Enumerates all dates between start date (inclusive) and end date (exclusive).
        /// </summary>
        /// <returns>Non-generic enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Start date.
        /// </summary>
        private readonly DateTime Since;

        /// <summary>
        /// End date.
        /// </summary>
        private readonly DateTime Until;

    }

}