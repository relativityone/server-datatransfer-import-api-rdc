// ----------------------------------------------------------------------------
// <copyright file="IoReporterFactory.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents a factory to create <see cref="IoReporter"/> instances.
// </summary>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Io
{
    using System;
	using System.Threading;

    using Relativity.Logging;

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
		/// <see langword="true" /> to throw <see cref="FileInfoInvalidPathException"/> when illegal characters are found within the path.
		/// </param>
		/// <param name="options">
		/// The configurable retry options.
		/// </param>
		/// <param name="logger">
		/// The Relativity logger.
		/// </param>
		/// <param name="publisher">
		/// The warning publisher.
		/// </param>
		/// <param name="cancellationToken">
		/// A token which is used to cancel current task.
		/// </param>
		/// <returns>
		/// The <see cref="IoReporter"/> instance.
		/// </returns>
		public static IIoReporter CreateIoReporter(
		    int maxRetryAttempts,
		    int waitTimeSecondsBetweenRetryAttempts,
		    bool disableNativeLocationValidation,
			RetryOptions options,
			ILog logger,
		    IoWarningPublisher publisher,
		    CancellationToken cancellationToken)
	    {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

			if (publisher == null)
	        {
		        throw new ArgumentNullException(nameof(publisher));
	        }

	        IWaitAndRetryPolicy waitAndRetryPolicy = new WaitAndRetryPolicy(maxRetryAttempts, waitTimeSecondsBetweenRetryAttempts);
	        IFileSystem fileSystem = FileSystem.Instance.DeepCopy();
			return new IoReporter(
				fileSystem,
                waitAndRetryPolicy,
                logger,
                publisher,
				disableNativeLocationValidation,
				options,
	            cancellationToken);
        }
    }
}