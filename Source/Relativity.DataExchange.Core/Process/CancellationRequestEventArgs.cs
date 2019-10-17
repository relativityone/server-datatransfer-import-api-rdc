// ----------------------------------------------------------------------------
// <copyright file="CancellationRequestEventArgs.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Process
{
	using System;

	/// <summary>
	/// Represents the cancellation request event argument data. This class cannot be inherited.
	/// </summary>
	[Serializable]
	public sealed class CancellationRequestEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CancellationRequestEventArgs"/> class. This assumes cancellation is requested by the user.
		/// </summary>
		/// <param name="processId">
		/// The process unique identifier to cancel.
		/// </param>
		public CancellationRequestEventArgs(Guid processId)
			: this(processId, true)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="CancellationRequestEventArgs"/> class.
		/// </summary>
		/// <param name="processId">
		/// The process unique identifier to cancel.
		/// </param>
		/// <param name="requestByUser">
		/// <see langword="true" /> when cancellation is requested by the user; otherwise, <see langword="false" /> when requested to terminate the process.
		/// </param>
		public CancellationRequestEventArgs(Guid processId, bool requestByUser)
		{
			this.ProcessId = processId;
			this.RequestByUser = requestByUser;
		}

		/// <summary>
		/// Gets the process unique identifier to cancel.
		/// </summary>
		/// <value>
		/// The <see cref="Guid"/> value.
		/// </value>
		public Guid ProcessId
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether cancellation is requested by the user.
		/// </summary>
		/// <value>
		/// <see langword="true" /> when cancellation is requested by the user; otherwise, <see langword="false" /> when requested to terminate the process.
		/// </value>
		public bool RequestByUser
		{
			get;
		}
	}
}