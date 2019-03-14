// ----------------------------------------------------------------------------
// <copyright file="ParentFormClosingEventArgs.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Process
{
	using System;

	/// <summary>
	/// Represents the parent form closing event argument data.
	/// </summary>
	[Serializable]
	public sealed class ParentFormClosingEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ParentFormClosingEventArgs"/> class.
		/// </summary>
		/// <param name="processId">
		/// The process unique identifier that is closing.
		/// </param>
		public ParentFormClosingEventArgs(Guid processId)
		{
			this.ProcessId = processId;
		}

		/// <summary>
		/// Gets the process unique identifier that is closing.
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