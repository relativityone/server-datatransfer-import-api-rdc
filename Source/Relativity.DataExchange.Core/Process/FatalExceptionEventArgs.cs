// ----------------------------------------------------------------------------
// <copyright file="FatalExceptionEventArgs.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Process
{
	using System;

	/// <summary>
	/// Represents the fatal exception event argument data. This class cannot be inherited.
	/// </summary>
	public sealed class FatalExceptionEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FatalExceptionEventArgs"/> class.
		/// </summary>
		/// <param name="exception">
		/// The fatal exception.
		/// </param>
		public FatalExceptionEventArgs(Exception exception)
		{
			this.FatalException = exception ?? throw new ArgumentNullException(nameof(exception));
		}

		/// <summary>
		/// Gets the fatal exception.
		/// </summary>
		/// <value>
		/// The <see cref="Exception"/> instance.
		/// </value>
		public Exception FatalException
		{
			get;
		}
	}
}