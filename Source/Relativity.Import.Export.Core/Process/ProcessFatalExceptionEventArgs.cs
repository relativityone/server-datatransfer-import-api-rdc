// ----------------------------------------------------------------------------
// <copyright file="ProcessFatalExceptionEventArgs.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Process
{
	using System;

	/// <summary>
	/// Represents the process fatal exception event argument data.
	/// </summary>
	public sealed class ProcessFatalExceptionEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessFatalExceptionEventArgs"/> class.
		/// </summary>
		/// <param name="exception">
		/// The fatal exception.
		/// </param>
		public ProcessFatalExceptionEventArgs(Exception exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException(nameof(exception));
			}

			this.FatalException = exception;
		}

		/// <summary>
		/// Gets the fatal exception.
		/// </summary>
		/// <value>
		/// The <see cref="Exception"/> instance.
		/// </value>
		public Exception FatalException { get; }
	}
}