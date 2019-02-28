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
		/// <param name="options">
		/// The configurable retry options.
		/// </param>
		/// <param name="logger">
		/// The Relativity logger.
		/// </param>
		/// <param name="publisher">
		/// The warning publisher.
		/// </param>
		/// <param name="token">
		/// A token which is used to cancel current task.
		/// </param>
		/// <returns>
		/// The <see cref="IoReporter"/> instance.
		/// </returns>
		public static IIoReporter CreateIoReporter(
		    RetryOptions options,
		    ILog logger,
		    IoWarningPublisher publisher,
		    CancellationToken token)
	    {
		    return CreateIoReporter(
			    AppSettings.Instance,
			    FileSystem.Instance.DeepCopy(),
			    options,
			    logger,
			    publisher,
			    token);
	    }

		/// <summary>
		/// Create a new <see cref="IoReporter"/> instance.
		/// </summary>
		/// <param name="appSettings">
		/// The application settings.
		/// </param>
		/// <param name="fileSystem">
		/// The file system.
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
		/// <param name="token">
		/// A token which is used to cancel current task.
		/// </param>
		/// <returns>
		/// The <see cref="IoReporter"/> instance.
		/// </returns>
		public static IIoReporter CreateIoReporter(
		    IAppSettings appSettings,
			IFileSystem fileSystem,
			RetryOptions options,
			ILog logger,
		    IoWarningPublisher publisher,
		    CancellationToken token)
	    {
		    if (appSettings == null)
		    {
			    throw new ArgumentNullException(nameof(appSettings));
		    }

		    if (fileSystem == null)
		    {
			    throw new ArgumentNullException(nameof(fileSystem));
		    }

			if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

			if (publisher == null)
	        {
		        throw new ArgumentNullException(nameof(publisher));
	        }

			return new IoReporter(appSettings, fileSystem, publisher, options, logger, token);
	    }
    }
}