using System;
using System.Net;

namespace Woof.SystemEx {

    /// <summary>
    /// HTTP responsive file downloader.
    /// </summary>
    public class Download : IDisposable {

        /// <summary>
        /// Occurs when download progress is changed by at least 1%.
        /// </summary>
        public event EventHandler<PercentEventArgs> DownloadProgressChanged;

        /// <summary>
        /// Occurs when download completes.
        /// </summary>
        public event EventHandler DownloadCompleted;

        /// <summary>
        /// Gets the current download <see cref="Uri"/>.
        /// </summary>
        public Uri Uri { get; }

        /// <summary>
        /// True if this instance is already disposed.
        /// </summary>
        public bool IsDisposed;

        /// <summary>
        /// True if the instance is being disposed;
        /// </summary>
        private bool IsDisposing;

        /// <summary>
        /// Creates new HTTP file downloader.
        /// </summary>
        /// <param name="uri"><see cref="Uri"/> requested.</param>
        public Download(Uri uri) => Uri = uri;

        /// <summary>
        /// Gets the <see cref="DiagnosticStream"/> for the downloaded file. Returns null if the response status is not OK.
        /// </summary>
        /// <returns><see cref="DiagnosticStream"/> or null for error responses.</returns>
        public DiagnosticStream GetStream() {
            Request = (HttpWebRequest)WebRequest.Create(Uri);
            Request.UserAgent = UA;
            Response = (HttpWebResponse)Request.GetResponse();
            if (Response.StatusCode != HttpStatusCode.OK) return null;
            Stream = new DiagnosticStream(Response.GetResponseStream(), Response.ContentLength);
            Stream.EndOfContent += (s, e) => DownloadCompleted?.Invoke(this, e);
            Stream.Closed += (s, e) => Dispose();
            if (DownloadProgressChanged != null) {
                Stream.ReadCompleted += (s, e) => {
                    var progress = (int)(100 * Stream.BytesRead / Stream.Length);
                    if (progress > Progress) DownloadProgressChanged.Invoke(this, new PercentEventArgs(Progress = progress));
                };
            }
            return Stream;
        }

        /// <summary>
        /// Disposes HTTP response and the download stream if needed.
        /// </summary>
        public void Dispose() {
            if (!IsDisposed && !IsDisposing) {
                IsDisposing = true;
                Response?.Dispose();
                Stream?.Dispose();
                IsDisposed = true;
            }
        }

        #region Private data

        /// <summary>
        /// User agent string used to indicate the Edge browser used.
        /// </summary>
        const string UA = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/52.0.2743.116 Safari/537.36 Edge/15.15063";
        
        /// <summary>
        /// HTTP request instance.
        /// </summary>
        HttpWebRequest Request;

        /// <summary>
        /// HTTP response instance.
        /// </summary>
        HttpWebResponse Response;

        /// <summary>
        /// Diagnostinc stream used to report progress.
        /// </summary>
        DiagnosticStream Stream;

        /// <summary>
        /// Download progress in percents.
        /// </summary>
        int Progress;

        #endregion

    }

}