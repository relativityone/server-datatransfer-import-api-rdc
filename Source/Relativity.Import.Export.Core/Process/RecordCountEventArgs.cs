// ----------------------------------------------------------------------------
// <copyright file="RecordCountEventArgs.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Process
{
	using System;

	/// <summary>
	/// Represents the record count event argument data.
	/// </summary>
	public sealed class RecordCountEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RecordCountEventArgs"/> class.
		/// </summary>
		/// <param name="count">
		/// The record count.
		/// </param>
		public RecordCountEventArgs(int count)
		{
			this.Count = count;
		}

		/// <summary>
		/// Gets the record count.
		/// </summary>
		/// <value>
		/// The total number of records.
		/// </value>
		public int Count
		{
			get;
		}
	}
}