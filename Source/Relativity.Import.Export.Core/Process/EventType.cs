// ----------------------------------------------------------------------------
// <copyright file="EventType.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Process
{
	/// <summary>
	/// Represents all supported events.
	/// </summary>
	public enum EventType
	{
		/// <summary>
		/// The status event.
		/// </summary>
		Status,

		/// <summary>
		/// The progress event.
		/// </summary>
		Progress,

		/// <summary>
		/// The end event.
		/// </summary>
		End,

		/// <summary>
		/// The warning event.
		/// </summary>
		Warning,

		/// <summary>
		/// The error event.
		/// </summary>
		Error,

		/// <summary>
		/// The reset start time event.
		/// </summary>
		ResetStartTime,

		/// <summary>
		/// The count event.
		/// </summary>
		Count,

		/// <summary>
		/// The reset progress event.
		/// </summary>
		ResetProgress,

		/// <summary>
		/// The statistics event.
		/// </summary>
		Statistics
	}
}