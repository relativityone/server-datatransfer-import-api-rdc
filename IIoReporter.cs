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
        long GetFileLength(string fileName);
    }
}
