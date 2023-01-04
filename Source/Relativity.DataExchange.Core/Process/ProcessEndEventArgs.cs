// ----------------------------------------------------------------------------
// <copyright file="ProcessEndEventArgs.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Process
{
	using System;

	/// <summary>
	/// Represents the process end event argument data. This class cannot be inherited.
	/// </summary>
	[Serializable]
	public sealed class ProcessEndEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ProcessEndEventArgs"/> class.
		/// </summary>
		/// <param name="nativeFileBytes">
		/// The total number of native file bytes that were transferred.
		/// </param>
		/// <param name="metadataBytes">
		/// The total number of metadata bytes that were transferred.
		/// </param>
		/// <param name="sqlProcessRate">
		/// Server/SQL Process rate (docs / sec).
		/// </param>
		public ProcessEndEventArgs(long nativeFileBytes, long metadataBytes, double sqlProcessRate)
		{
			this.NativeFileBytes = nativeFileBytes;
			this.MetadataBytes = metadataBytes;
			this.SqlProcessRate = sqlProcessRate;
		}

		/// <summary>
		/// Gets the total number of native file bytes that were transferred.
		/// </summary>
		/// <value>
		/// The total number of bytes.
		/// </value>
		public long NativeFileBytes { get; }

		/// <summary>
		/// Gets the total number of metadata bytes that were transferred.
		/// </summary>
		/// <value>
		/// The total number of bytes.
		/// </value>
		public long MetadataBytes { get; }

		/// <summary>
		/// Gets the Server/SQL Process rate summary.
		/// </summary>
		/// <value>
		/// processed documents /second.
		/// </value>
		public double SqlProcessRate { get; }
	}
}