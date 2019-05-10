// ----------------------------------------------------------------------------
// <copyright file="ProcessEventType.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Process
{
	/// <summary>
	/// Represents all supported process events that are published by a <see cref="IRunnable"/> process.
	/// </summary>
	public enum ProcessEventType
	{
		/// <summary>
		/// The process event includes a status message.
		/// </summary>
		Status,

		/// <summary>
		/// The process event includes a warning message.
		/// </summary>
		Warning,

		/// <summary>
		/// The process event includes an error message.
		/// </summary>
		Error,
	}
}