// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoReporterFactory.cs" company="kCura Corp">
//   kCura Corp (C) 2017 All Rights Reserved.
// </copyright>
// <summary>
//   Represents a factory to create <see cref="IoReporter"/> instances.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace kCura.WinEDDS.TApi
{
    using System;
    using Relativity.Logging;
    using Relativity.Transfer;

    /// <summary>
    /// Represents a factory to create <see cref="IoReporter"/> instances.
    /// </summary>
    public static class IoReporterFactory
    {
        /// <summary>
        /// Create a new <see cref="IoReporter"/> instance.
        /// </summary>
        /// <param name="maxRetryAttempts">
        /// The maximum number of retry attempts.
        /// </param>
        /// <param name="waitTimeSecondsBetweenRetryAttempts">
        /// The total number of seconds to wait between each retry attempty.
        /// </param>
        /// <param name="disableNativeLocationValidation">
        /// Specify whether to disable checks to determine whether a file exists.
        /// </param>
        /// <param name="logger">
        /// The Relativity logger.
        /// </param>
        /// <param name="publisher">
        /// The warning publisher.
        /// </param>
        /// <returns>
        /// The <see cref="IoReporter"/> instance.
        /// </returns>
        public static IIoReporter CreateIoReporter(
            int maxRetryAttempts,
            int waitTimeSecondsBetweenRetryAttempts,
            bool disableNativeLocationValidation,
            ILog logger,
            IoWarningPublisher publisher)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if (publisher == null)
            {
                throw new ArgumentNullException(nameof(publisher));
            }

            var fileSystemService = new FileSystemService();
            var waitAndRetryPolicy = new WaitAndRetryPolicy(maxRetryAttempts, waitTimeSecondsBetweenRetryAttempts);
            return new IoReporter(
                fileSystemService,
                waitAndRetryPolicy,
                logger,
                publisher,
                disableNativeLocationValidation);
        }
    }
}
