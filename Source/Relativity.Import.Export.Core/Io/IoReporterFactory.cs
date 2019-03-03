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
    internal static class IoReporterFactory
    {
		/// <summary>
		/// Create a new <see cref="IoReporter"/> instance.
		/// </summary>
		/// <param name="logger">
		/// The Relativity logger.
		/// </param>
		/// <param name="token">
		/// The cancellation token used to stop the process upon request.
		/// </param>
		/// <returns>
		/// The <see cref="IoReporter"/> instance.
		/// </returns>
		public static IIoReporter CreateIoReporter(ILog logger, CancellationToken token)
	    {
		    return CreateIoReporter(new IoReporterContext(), logger, token);
	    }

		/// <summary>
		/// Create a new <see cref="IoReporter"/> instance.
		/// </summary>
		/// <param name="context">
		/// The I/O reporter context.
		/// </param>
		/// <param name="logger">
		/// The Relativity logger.
		/// </param>
		/// <param name="token">
		/// The cancellation token used to stop the process upon request.
		/// </param>
		/// <returns>
		/// The <see cref="IoReporter"/> instance.
		/// </returns>
		public static IIoReporter CreateIoReporter(
			IoReporterContext context,
		    ILog logger,
			CancellationToken token)
	    {
			if (context == null)
			{
				throw new ArgumentNullException(nameof(context));
			}

			if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

			return new IoReporter(context, logger, token);
	    }
    }
}