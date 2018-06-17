using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Woof.AssemblyEx;
using Woof.ConsoleEx.ConsoleFilters;

namespace Woof.ConsoleEx {

    /// <summary>
    /// Window console extensions.
    /// </summary>
    public static class ConsoleEx {

        /// <summary>
        /// Gets or sets a value indicating that HexColor filter is enabled.
        /// </summary>
        public static bool IsHexColorEnabled {
            get => _IsHexColorEnabled; set { if (_IsHexColorEnabled = value) Console.SetOut(new HexColor()); else Console.SetOut(Console.Out); }
        }

        /// <summary>
        /// Sets console window size to a preset dimensions.
        /// </summary>
        public static ConsoleSize Size {
            set {
                int w = 0, h = 0, wl = Console.LargestWindowWidth, hl = Console.LargestWindowHeight;
                switch (value) {
                    case ConsoleSize.Normal:
                        Console.SetWindowSize(80, 25);
                        return;
                    case ConsoleSize.Double:
                        w = 160; h = 50;
                        if (w > wl) w = wl;
                        if (h > hl) h = hl;
                        Console.SetWindowSize(w, h);
                        break;
                    case ConsoleSize.Max:
                        Console.SetWindowSize(wl, hl);
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets current console state.
        /// </summary>
        public static ConsoleState State {
            get => new ConsoleState {
                Background = Console.BackgroundColor,
                Foreground = Console.ForegroundColor,
                X = Console.CursorLeft,
                Y = Console.CursorTop,
                WinX = Console.WindowLeft,
                WinY = Console.WindowTop
            };
            set {
                Console.BackgroundColor = value.Background;
                Console.ForegroundColor = value.Foreground;
                Console.SetCursorPosition(value.X, value.Y);
                Console.SetWindowPosition(value.WinX, value.WinY);
            }
        }

        private static bool _IsHexColorEnabled;

        #region Internal debug log

        // TODO: Better logger.

        private static ConcurrentQueue<string> DebugLog;
        private static Timer DebugLogTimer;

        private static void DebugLogTimerCallback(object state) {
            if (DebugLog == null) return;
            while (DebugLog.TryDequeue(out var message)) lock (Console.Out) Console.WriteLine(message);
            DebugLogTimer.Dispose();
            DebugLogTimer = null;
        }

        private static void DebugLogInitialize() {
            if (DebugLog == null) DebugLog = new ConcurrentQueue<string>();
        }

        private static void DebugLogUpdate() {
            if (DebugLogTimer == null) DebugLogTimer = new Timer(DebugLogTimerCallback, null, 16, 0);
        }

        #endregion

        /// <summary>
        /// Displays application header from entry assembly info.
        /// </summary>
        /// <param name="useColor">If true, color console output <see cref="HexColor"/> will be used.</param>
        public static void AssemblyHeader(bool useColor = false) {
            var ai = new AssemblyInfo();
            Console.Clear();
            Console.OutputEncoding = Encoding.UTF8;
            IsHexColorEnabled = useColor;
            Console.Title = ai.Product;
            Separator();
            Console.WriteLine(useColor ? $"`fff`{ai.Title} version {ai.Version}`" : $"{ai.Title} version {ai.Version}");
            Console.WriteLine(useColor ? $"`077`{ai.Description}`" : ai.Description);
            Console.WriteLine(useColor ? $"`444`{ai.Copyright}, All rights reserved.`" : ai.Copyright);
            Separator();
        }

        /// <summary>
        /// Displays messages to console, doesn't block calling thread, locks the console out to prevent mangled text from multiple threads.
        /// </summary>
        /// <param name="messages">One or more messages to display.</param>
        public static void Log(params string[] messages) {
            var timestamp = DateTime.Now;
            DebugLogInitialize();
            foreach (var message in messages) DebugLog.Enqueue(IsHexColorEnabled ? $"`077`{DateTime.Now:HH:mm:ss.fff}` {message}" : $"{DateTime.Now.TimeOfDay} {message}");
            DebugLogUpdate();
        }

        /// <summary>
        /// Displays one or more debug messages on console after initial "DEBUG: ", doesn't block the calling thread.
        /// </summary>
        /// <param name="serverity">0: debug, 1: warning, 2: error.</param>
        /// <param name="messages">Messages to log.</param>
        public static void LogDebug(int serverity, params string[] messages) {
            var timestamp = DateTime.Now;
            string headerText = null;
            string headerColor = null;
            switch (serverity) {
                case 0: headerText = "DEBUG"; headerColor = "`0ff`"; break;
                case 1: headerText = "WARNING"; headerColor = "`ff0`"; break;
                case 2: headerText = "ERROR"; headerColor = "`f00`"; break;
            }
            var header = IsHexColorEnabled ? $"`077`{timestamp:HH:mm:ss.fff}` {headerColor}{headerText}:` " : $"{timestamp:HH:mm:ss.fff} {headerText}: ";
            DebugLogInitialize();
            for (int i = 0, n = messages.Length; i < n; i++) DebugLog.Enqueue(i < 1 ? header + messages[i] : messages[i]);
            DebugLogUpdate();
        }

        /// <summary>
        /// Displays one or more debug messages on console after initial "DEBUG: ", doesn't block the calling thread.
        /// </summary>
        /// <param name="severity">Severity code: 'I' for debug, 'W' for warning, 'E' for error.</param>
        /// <param name="messages">Messages to log.</param>
        public static void LogDebug(char severity, params string[] messages) { 
            switch (severity) {
                case 'I': LogDebug(0, messages); return;
                case 'W': LogDebug(1, messages); return;
                case 'E': LogDebug(2, messages); return;
                default: throw new ArgumentException("severity");
            }
        }

        /// <summary>
        /// Displays header message.
        /// </summary>
        /// <param name="message">Header message.</param>
        public static void HeaderMessage(string message) => Console.WriteLine(IsHexColorEnabled ? $"`ccc`{message}`" : message);

        /// <summary>
        /// Displays error message.
        /// </summary>
        /// <param name="message">Error message.</param>
        public static void ErrorMessage(string message) => Console.WriteLine(IsHexColorEnabled ? $"`f00`!!!` {message}" : $"!!! {message}");

        /// <summary>
        /// Returns value as string with optional error color format.
        /// </summary>
        /// <param name="value">A value convertible to string.</param>
        /// <returns>Formatted value.</returns>
        public static string ErrorValue(object value) => IsHexColorEnabled ? $"`[`f00`{value}`]" : $"[{value}]";

        /// <summary>
        /// Returns value as string with optional correct color format.
        /// </summary>
        /// <param name="value">A value convertible to string.</param>
        /// <returns>Formatted value.</returns>
        public static string CorrectValue(object value) => IsHexColorEnabled ? $"`[`070`{value}`]" : $"[{value}]";

        /// <summary>
        /// Displays horizontal line separator across the console window.
        /// </summary>
        public static void Separator() => Console.WriteLine(String.Empty.PadRight(Console.WindowWidth - 1, '-'));

        /// <summary>
        /// Displays start message and returns progress object used to complete the action.
        /// </summary>
        /// <param name="message">Message to display.</param>
        /// <returns>Progress object used to complete the action.</returns>
        public static ConsoleProgress StartMessage(string message) {
            Console.Write(IsHexColorEnabled ? $"`444`  * {message}..." : $"  * {message}...");
            return new ConsoleProgress();
        }

        /// <summary>
        /// Display item as bullet point.
        /// </summary>
        /// <param name="item">Item.</param>
        public static void ItemMessage(string item) => Console.WriteLine(IsHexColorEnabled ? $"`444`  * {item}" : $"  * {item}");

        /// <summary>
        /// Completes the action started with the <see cref="StartMessage(string)"/>.
        /// </summary>
        /// <param name="progress">Progress object.</param>
        /// <param name="success">Status of the operation performed after <see cref="StartMessage(string)"/>.</param>
        public static void Complete(ConsoleProgress progress, bool success = true) {
            const string ok = "OK";
            const string fail = "FAIL";
            progress.Done(
                success
                    ? (IsHexColorEnabled ? $"`444`[`070`{ok}`444`]`" : $"[{ok}]")
                    : (IsHexColorEnabled ? $"`444`[`f00`{fail}`444`]`" : $"[{fail}]")
            );
        }

        /// <summary>
        /// Displays a message and waits until Ctrl+C is pressed.
        /// </summary>
        /// <param name="message">Optional alternative message to display.</param>
        public static void WaitForCtrlC(string message = "Press Ctrl+C to exit") {
            using (var semaphore = new ManualResetEventSlim()) {
                void handler(object s, ConsoleCancelEventArgs e) { e.Cancel = true; semaphore.Set(); }
                Console.CancelKeyPress += handler;
                Console.WriteLine(IsHexColorEnabled ? $"`fff`{message}.`" : message);
                semaphore.Wait();
                Console.CancelKeyPress -= handler;
            }
        }

    }

    /// <summary>
    /// Console size enumeration.
    /// </summary>
    public enum ConsoleSize {
        /// <summary>
        /// 80x25
        /// </summary>
        Normal,
        /// <summary>
        /// 160x50
        /// </summary>
        Double,
        /// <summary>
        /// Takes all available space
        /// </summary>
        Max
    }

    /// <summary>
    /// Console state data.
    /// </summary>
    public class ConsoleState {

        /// <summary>
        /// Console background color.
        /// </summary>
        public ConsoleColor Background;

        /// <summary>
        /// Console foreground color.
        /// </summary>
        public ConsoleColor Foreground;

        /// <summary>
        /// Cursor X coordinate.
        /// </summary>
        public int X;

        /// <summary>
        /// Cursor Y coordinate.
        /// </summary>
        public int Y;

        /// <summary>
        /// Window to buffer X coordinate.
        /// </summary>
        public int WinX;

        /// <summary>
        /// Window to buffer Y coorinate.
        /// </summary>
        public int WinY;

    }

}