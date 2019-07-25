using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Woof.SystemEx {

    /// <summary>
    /// Simple time trigger for scheduled events.
    /// </summary>
    public class TimeTrigger : IDisposable {

        /// <summary>
        /// Occurs when time set or repeat interval elapsed.
        /// </summary>
        public event EventHandler Elapsed;

        /// <summary>
        /// <para>Gets or sets time schedule as string.</para>
        /// Accepted formats:
        /// <list type="bullet">
        /// <item><term>12:34</term><description> - daily at 12:34,</description></item>
        /// <item><term>5s</term><description>- every 5 seconds,</description></item>
        /// <item><term>0@12:34</term><description>- at 12:34 every Sunday,</description></item>
        /// <item><term>05@12:34</term><description>- at 12:34 every 5th day of the month,</description></item>
        /// <item><term>LD@12:34</term><description>- at 12:34 every 5th day of the month,</description></item>
        /// </list>
        /// </summary>
        public string Schedule {
            get => _Schedule; set {
                T.Stop();
                TimeOfDay = null;
                Match match;
                int hours, minutes, x;
                string unit;
                match = RxDaily.Match(value);
                if (match.Success) {
                    Interval = TimeSpan.FromDays(1);
                    hours = Int32.Parse(match.Groups[1].Value);
                    minutes = Int32.Parse(match.Groups[2].Value);
                    TimeOfDay = TimeSpan.FromMinutes(minutes + 60 * hours);
                    _Schedule = value;
                    return;
                }
                match = RxWeekly.Match(value);
                if (match.Success) {
                    Interval = TimeSpan.FromDays(1);
                    DayOfWeek = Int32.Parse(match.Groups[1].Value);
                    hours = Int32.Parse(match.Groups[2].Value);
                    minutes = Int32.Parse(match.Groups[3].Value);
                    TimeOfDay = TimeSpan.FromMinutes(minutes + 60 * hours);
                    _Schedule = value;
                    return;
                }
                match = RxMonthly.Match(value);
                if (match.Success) {
                    Interval = TimeSpan.FromDays(1);
                    if (match.Groups[1].Value == "LD") LastDayOfTheMonth = true;
                    else DayOfMonth = Int32.Parse(match.Groups[1].Value);
                    hours = Int32.Parse(match.Groups[2].Value);
                    minutes = Int32.Parse(match.Groups[3].Value);
                    TimeOfDay = TimeSpan.FromMinutes(minutes + 60 * hours);
                    _Schedule = value;
                    return;
                }
                match = RxInterval.Match(value);
                if (match.Success) {
                    x = Int32.Parse(match.Groups[1].Value);
                    unit = match.Groups[2].Value;
                    Interval = TimeSpan.FromTicks(x * Ticks[unit]);
                    _Schedule = value;
                    return;
                }
                throw new ArgumentException();
            }
        }

        /// <summary>
        /// Creates and binds timer for the time trigger.
        /// </summary>
        private TimeTrigger() {
            T = new System.Timers.Timer { Enabled = false };
            T.Elapsed += delegate { OnElapsed(EventArgs.Empty); };
        }

        /// <summary>
        /// Creates time trigger based on interval alone.
        /// </summary>
        /// <param name="interval">Time inverval in which the <see cref="Elapsed"/> event will be triggered.</param>
        public TimeTrigger(TimeSpan interval) : this() => Interval = interval;

        /// <summary>
        /// Creates daily time trigger.
        /// </summary>
        /// <param name="dailyAt">Time of day the <see cref="Elapsed"/> event will be triggered.</param>
        public TimeTrigger(DateTime dailyAt) : this() {
            TimeOfDay = dailyAt.TimeOfDay;
            Interval = TimeSpan.FromDays(1);
        }

        /// <summary>
        /// Creates time trigger with schedule defined as string.
        /// </summary>
        /// <param name="schedule">
        /// <para>Accepted formats:</para>
        /// <list type="bullet">
        /// <item><term>12:34</term><description> - daily at 12:34, </description></item>
        /// <item><term>5s</term><description> - every 5 seconds, </description></item>
        /// <item><term>0@12:34</term><description> - at 12:34 every Sunday, </description></item>
        /// <item><term>05@12:34</term><description> - at 12:34 every 5th day of the month, </description></item>
        /// <item><term>LD@12:34</term><decription> - at 12:34 every last day of the month.</decription></item>
        /// </list>
        /// </param>
        /// <exception cref="ArgumentException">Thrown when schedule string is not recognized.</exception>
        public TimeTrigger(string schedule) : this() => Schedule = schedule;

        /// <summary>
        /// Sets the time trigger.
        /// </summary>
        /// <param name="trigger">If true first <see cref="Elapsed"/> event will be triggered immediately.</param>
        public void Set(bool trigger = false) {
            if (TimeOfDay.HasValue) {
                T.Interval = TimeSpan.FromTicks(
                    Interval.Ticks >= D
                        ? (
                            TimeOfDay.Value >= DateTime.Now.TimeOfDay
                                ? (TimeOfDay.Value.Ticks - DateTime.Now.TimeOfDay.Ticks)
                                : (Interval.Ticks + TimeOfDay.Value.Ticks - DateTime.Now.TimeOfDay.Ticks)
                        )
                        : Interval.Ticks
                ).TotalMilliseconds;
            } else T.Interval = Interval.TotalMilliseconds;
            if (trigger && !ShouldSkip()) Elapsed?.Invoke(this, EventArgs.Empty);
            System.Diagnostics.Debug.Print($"It's {DateTime.Now.TimeOfDay}, time trigger is set to fire in {TimeSpan.FromMilliseconds(T.Interval)} from now...");
            T.Start();
        }

        /// <summary>
        /// Returns true if daily schedule is not due and trigger should be skipped.
        /// </summary>
        /// <returns>True if daily schedule is not due.</returns>
        private bool ShouldSkip() {
            if (!TimeOfDay.HasValue) return false;
            var now = DateTime.Now;
            if (DayOfWeek.HasValue && (int)now.DayOfWeek != DayOfWeek.Value) return true;
            if (DayOfMonth.HasValue && now.Month != DayOfMonth.Value) return true;
            if (LastDayOfTheMonth && now.Month != DateTime.DaysInMonth(now.Year, now.Month)) return true;
            return false;
        }

        /// <summary>
        /// Triggers <see cref="Elapsed"/> event.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnElapsed(EventArgs e) {
            T.Stop();
            if (!ShouldSkip()) Elapsed?.Invoke(this, e);
            Set();
        }

        /// <summary>
        /// Disposes the timer.
        /// </summary>
        public void Dispose() => T.Dispose();

        #region Private data

        /// <summary>
        /// Time of day to start the time trigger.
        /// </summary>
        private TimeSpan? TimeOfDay;

        /// <summary>
        /// Repeat interval.
        /// </summary>
        private TimeSpan Interval;

        /// <summary>
        /// Day of the week number: 1 - Monday, 7 - Sunday.
        /// </summary>
        private int? DayOfWeek;

        /// <summary>
        /// Day of the month number.
        /// </summary>
        private int? DayOfMonth;

        /// <summary>
        /// True if trigger in the last day of the month.
        /// </summary>
        private bool LastDayOfTheMonth;

        /// <summary>
        /// Daily syntax.
        /// </summary>
        private static readonly Regex RxDaily = new Regex(@"^(\d\d?):(\d\d)$", RegexOptions.Compiled);

        /// <summary>
        /// Weekly syntax.
        /// </summary>
        private static readonly Regex RxWeekly = new Regex(@"^(\d)@(\d\d?):(\d\d)$", RegexOptions.Compiled);

        /// <summary>
        /// Monthly syntax.
        /// </summary>
        private static readonly Regex RxMonthly = new Regex(@"^(\d\d|LD)@(\d\d?):(\d\d)$", RegexOptions.Compiled);

        /// <summary>
        /// Interval syntax.
        /// </summary>
        private static readonly Regex RxInterval = new Regex(@"^(\d+)(ms|s|m|h|d)$", RegexOptions.Compiled);

        /// <summary>
        /// System timer.
        /// </summary>
        private readonly System.Timers.Timer T;

        /// <summary>
        /// Ticks per day.
        /// </summary>
        private static readonly long D = TimeSpan.FromDays(1).Ticks;

        /// <summary>
        /// Tick amounts for time units.
        /// </summary>
        private static readonly Dictionary<string, long> Ticks = new Dictionary<string, long> {
            ["ms"] = TimeSpan.FromMilliseconds(1).Ticks,
            ["s"] = TimeSpan.FromSeconds(1).Ticks,
            ["m"] = TimeSpan.FromMinutes(1).Ticks,
            ["h"] = TimeSpan.FromHours(1).Ticks,
            ["d"] = TimeSpan.FromDays(1).Ticks
        };

        /// <summary>
        /// Schedule cache.
        /// </summary>
        private string _Schedule;

        #endregion

    }

}