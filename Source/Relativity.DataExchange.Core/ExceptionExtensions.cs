// <copyright file="ExceptionExtensions.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange
{
	using System;
	using System.Threading;

	/// <summary>
	/// Represents extension methods for common exception-based operations.
	/// </summary>
	internal static class ExceptionExtensions
	{
		/// <summary>
		/// Check if source of the exception was user cancellation request.
		/// </summary>
		/// <param name="ex">the exception.</param>
		/// <param name="token">the token to check.</param>
		/// <returns>true if exception was caused by user cancellation request.</returns>
		public static bool IsCanceledByUser(this Exception ex, CancellationToken token)
		{
			return ex is OperationCanceledException && token.IsCancellationRequested;
		}
	}
}
