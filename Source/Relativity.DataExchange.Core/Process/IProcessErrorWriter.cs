// ----------------------------------------------------------------------------
// <copyright file="IProcessErrorWriter.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Process
{
	using System;
	using System.Threading;

	/// <summary>
	/// Represents an abstract object that writes errors to a file.
	/// </summary>
	public interface IProcessErrorWriter : IDisposable
	{
		/// <summary>
		/// Gets a value indicating whether any errors have been written.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if at least 1 error has been written; otherwise, <see langword="false" />.
		/// </value>
		bool HasErrors
		{
			get;
		}

		/// <summary>
		/// Builds the error report.
		/// </summary>
		/// <param name="token">
		/// The cancellation token.
		/// </param>
		/// <returns>
		/// The <see cref="ProcessErrorReport"/> instance.
		/// </returns>
		ProcessErrorReport BuildErrorReport(CancellationToken token);

		/// <summary>
		/// Closes this instance.
		/// </summary>
		void Close();

		/// <summary>
		/// Writes the error information to a file.
		/// </summary>
		/// <param name="key">
		/// The error key.
		/// </param>
		/// <param name="description">
		/// The error description.
		/// </param>
		void Write(string key, string description);
	}
}