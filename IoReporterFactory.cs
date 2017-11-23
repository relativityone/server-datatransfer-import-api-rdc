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
        public static IIoReporter CreateIoReporter(int numberOfRetries, int waitTimeBetweenRetryAttempts, bool disableNativeLocationValidation, IFileInfoFailedExceptionHelper fileInfoFailedExceptionHelper, ILog logger)
        {
            var fileSystemService = new FileSystemService();
            var waitAndRetryPolicy = new WaitAndRetryPolicy(numberOfRetries, waitTimeBetweenRetryAttempts);
            var ioWarningPublisher = new IoWarningPublisher();
            var ioReporter = new IoReporter(fileSystemService, waitAndRetryPolicy, logger, ioWarningPublisher, fileInfoFailedExceptionHelper, disableNativeLocationValidation);

            return ioReporter;
        }
    }
}
