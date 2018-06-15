using System;
using System.Threading;
using System.Threading.Tasks;

namespace Woof.DebugEx {

    /// <summary>
    /// A tool for testing methods for freezing / lags.
    /// </summary>
    public class FreezeTest {

        /// <summary>
        /// A subject to test.
        /// </summary>
        private readonly Action Subject;

        /// <summary>
        /// Timeout value in milliseconds.
        /// </summary>
        private int Timeout;

        /// <summary>
        /// True if subject completed.
        /// </summary>
        private bool IsDone;

        /// <summary>
        /// Creates a freeze test for the test subject.
        /// </summary>
        /// <param name="subject">An action to run and test for freezes or lags.</param>
        public FreezeTest(Action subject) => Subject = subject;

        /// <summary>
        /// Starts a race between test subject and the watch dog. If watch dog completes first, a <see cref="TimeoutException"/> is thrown.
        /// </summary>
        /// <param name="timeout">Timeout value in milliseconds.</param>
        public void Test(int timeout) {
            Timeout = timeout;
            var tested = new Task(Tested, null, TaskCreationOptions.LongRunning);
            var watchdog = new Task(WatchDog, null, TaskCreationOptions.LongRunning);
            tested.Start();
            watchdog.Start();
            Task.WaitAny(tested, watchdog);
            if (!IsDone) throw new TimeoutException("FREEZE DETECTED.");
        }

        /// <summary>
        /// Testing task.
        /// </summary>
        /// <param name="state">Required for task creation.</param>
        private void Tested(object state) {
            Subject();
            IsDone = true;
        }

        /// <summary>
        /// Watch dog task.
        /// </summary>
        /// <param name="state">Required for task creation.</param>
        private void WatchDog(object state) => Thread.Sleep(Timeout);

    }

}