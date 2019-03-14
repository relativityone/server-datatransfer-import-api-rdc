// ----------------------------------------------------------------------------
// <copyright file="RecordNumberEventArgs.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Process
{
	using System;

	/// <summary>
	/// Represents the record number event argument data.
	/// </summary>
	public sealed class RecordNumberEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RecordNumberEventArgs"/> class.
		/// </summary>
		/// <param name="recordNumber">
		/// The record number.
		/// </param>
		public RecordNumberEventArgs(long recordNumber)
		{
			this.RecordNumber = recordNumber;
		}

		/// <summary>
		/// Gets the record number.
		/// </summary>
		/// <value>
		/// The record number.
		/// </value>
		public long RecordNumber
		{
			get;
		}
	}
}