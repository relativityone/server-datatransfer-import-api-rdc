using System;


namespace kCura.WinEDDS.TApi
{
    /// <summary>
    /// Robust IO reporter
    /// </summary>
    public interface IIoReporter
    {
        /// <summary>
        /// Get file length
        /// </summary>
        long GetFileLength(string fileName, int lineNumberInParentFile);


        /// <summary>
        /// Property to expose IoWarningPublisher
        /// </summary>
        IoWarningPublisher IOWarningPublisher { get; }

        /// <summary>
        /// This method prepares meaningful warning message.
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        string BuildIOReporterWarningMessage(Exception ex);
    }
}
