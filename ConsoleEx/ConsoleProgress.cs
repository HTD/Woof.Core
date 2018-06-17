using System;

namespace Woof.ConsoleEx {

    /// <summary>
    /// A special object allowing to display dots by one thread, while the other already displayed something else on the console.
    /// </summary>
    public class ConsoleProgress {

        /// <summary>
        /// Backup of console state.
        /// </summary>
        ConsoleState B;

        /// <summary>
        /// Last console state.
        /// </summary>
        private ConsoleState L;

        /// <summary>
        /// Current console state.
        /// </summary>
        ConsoleState C {
            get => ConsoleEx.State; set => ConsoleEx.State = value;
        }

        /// <summary>
        /// Creates progress dots placeholder at current cursor position.
        /// </summary>
        public ConsoleProgress() {
            L = C;
            Console.WriteLine();
        }

        /// <summary>
        /// Displays subsequent progress dot.
        /// </summary>
        public void Dot() {
            lock (Console.Out) {
                B = C;
                C = L;
                Console.Write('.');
                L = C;
                C = B;
            }
        }

        /// <summary>
        /// Ends current dot bar with a message.
        /// </summary>
        /// <param name="msg"></param>
        public void Done(string msg) {
            lock (Console.Out) {
                B = C;
                C = L;
                Console.WriteLine(msg);
                L = C;
                C = B;
                if (ConsoleEx.IsHexColorEnabled) Console.ResetColor();
            }
        }

    }

}