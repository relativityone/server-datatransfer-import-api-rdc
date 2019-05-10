// ----------------------------------------------------------------------------
// <copyright file="CancellationRequestEventArgs.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Process
{
	using System;

	/// <summary>
	/// Represents the cancellation request event argument data. This class cannot be inherited.
	/// </summary>
	[Serializable]
	public sealed class CancellationRequestEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="CancellationRequestEventArgs"/> class.
		/// </summary>
		/// <param name="processId">
		/// The process unique identifier to cancel.
		/// </param>
		public CancellationRequestEventArgs(Guid processId)
		{
			this.ProcessId = processId;
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
	}
}