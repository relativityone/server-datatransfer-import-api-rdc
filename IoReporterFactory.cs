using System;
using Relativity.Logging;
using Relativity.Transfer;


namespace kCura.WinEDDS.TApi
{
    /// <summary>
    /// Represents a class to create <see cref="IoReporter"/> instances.
    /// </summary>
    public static class IoReporterFactory
    {
        /// <summary>
        /// reates a <see cref="IoReporter"/> instance.
        /// </summary>
        /// <returns>The <see cref="IoReporter"/> instance.</returns>
        public static IIoReporter CreateIoReporter(int numberOfRetries, int waitTimeBetweenRetryAttempts, bool disableNativeLocationValidation, ILog logger, IoWarningPublisher ioWarningPublisher)
        {
            var fileSystemService = new FileSystemService();
            var waitAndRetryPolicy = new WaitAndRetryPolicy(numberOfRetries, waitTimeBetweenRetryAttempts);
            var ioReporter = new IoReporter(fileSystemService, waitAndRetryPolicy, logger, ioWarningPublisher, disableNativeLocationValidation);

            return ioReporter;
        }
    }
}
