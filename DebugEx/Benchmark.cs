using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Woof.DebugEx {

    /// <summary>
    /// Fast code benchmarking tool.
    /// </summary>
    public static class Benchmark {

        /// <summary>
        /// Compares performance of 2 actions and returns a to b performance gain in percents.
        /// </summary>
        /// <param name="test1">Action 1</param>
        /// <param name="test2">Action 2</param>
        /// <param name="time">Test time.</param>
        /// <returns>Percent gain of action 1 over action 2.</returns>
        public static int Compare(Action test1, Action test2, double time = 1.0) {
            long t1 = 0, t2 = 0;
            using (var cts = new CancellationTokenSource()) {
                var token = cts.Token;
                var l1 = new Action(() => { while (!token.IsCancellationRequested) { test1(); t1++; } });
                var l2 = new Action(() => { while (!token.IsCancellationRequested) { test2(); t2++; } });
                var stop = new Action(() => { Thread.Sleep((int)(time * 1000.0)); cts.Cancel(); });
                Parallel.Invoke(l1, l2, stop);
            }
            return (int)Math.Round(100 * (t1 / (double)t2 - 1));
        }

        /// <summary>
        /// Test maximum single thread performance of specified action in operations per second.
        /// </summary>
        /// <param name="action">Action to test.</param>
        /// <param name="time">Test time.</param>
        /// <returns>Operations per second.</returns>
        public static long Perf(Action action, double time) {
            long counter = 0;
            using (var cts = new CancellationTokenSource()) {
                var token = cts.Token;
                var loops = new Action[2];
                loops[0] = new Action(() => { while (!token.IsCancellationRequested) { action(); counter++; } });
                loops[1] = new Action(() => { Thread.Sleep((int)(time * 1000.0)); cts.Cancel(); });
                Parallel.Invoke(loops);
            }
            return (long)Math.Round(counter / time);
        }

        /// <summary>
        /// Executes all actions parallely in loops, returns ops per second per each action.
        /// </summary>
        /// <param name="time">Test time.</param>
        /// <param name="actions">Actions to test.</param>
        /// <returns></returns>
        public static long[] Race(double time = 1.0, params Action[] actions) {
            var times = new Ref[actions.Length];
            using (var cts = new CancellationTokenSource()) {
                var token = cts.Token;
                var loops = new Action[actions.Length + 1];
                for (int i = 0, n = actions.Length; i < n; i++) {
                    var a = actions[i];
                    var t = times[i] = new Ref();
                    loops[i] = new Action(() => { while (!token.IsCancellationRequested) { a(); t.Value++; } });
                }
                loops[actions.Length] = new Action(() => { Thread.Sleep((int)(time * 1000.0)); cts.Cancel(); });
                Parallel.Invoke(loops);
            }
            return times.Select(i => (long)Math.Round(i.Value / time)).ToArray();
        }

        /// <summary>
        /// Helper class for passing Int64 by reference to lambdas.
        /// </summary>
        private sealed class Ref { public long Value; }

    }

}