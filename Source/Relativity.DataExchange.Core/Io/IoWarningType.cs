// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IoWarningType.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
// Represents the different I/O warning types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Io
{
	/// <summary>
	/// Represents the different I/O warning types.
	/// </summary>
	public enum IoWarningType
	{
		/// <summary>
		/// A message only warning.
		/// </summary>
		Message,

		/// <summary>
		/// The warning is instantly retried.
		/// </summary>
		InstantRetry,

		/// <summary>
		/// The current thread waits a period of time before being retried.
		/// </summary>
		WaitRetryError,
	}
}