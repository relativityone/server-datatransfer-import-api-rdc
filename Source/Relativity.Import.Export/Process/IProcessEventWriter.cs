// ----------------------------------------------------------------------------
// <copyright file="IProcessEventWriter.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Process
{
	using System;

	/// <summary>
	/// Represents an abstract object that writes output events to a file.
	/// </summary>
	public interface IProcessEventWriter : IDisposable
	{
		/// <summary>
		/// Gets the full path to the event file.
		/// </summary>
		/// <value>
		/// The full path.
		/// </value>
		string File
		{
			get;
		}

		/// <summary>
		/// Gets a value indicating whether any events have been written.
		/// </summary>
		/// <value>
		/// <see langword="true" /> if at least 1 event has been written; otherwise, <see langword="false" />.
		/// </value>
		bool HasEvents
		{
			get;
		}

		/// <summary>
		/// Closes this instance.
		/// </summary>
		void Close();

		/// <summary>
		/// Saves the process event file to the target file.
		/// </summary>
		/// <param name="targetFile">
		/// The target file.
		/// </param>
		/// <exception cref="ArgumentNullException">
		/// Thrown when <paramref name="targetFile"/> is is <see langword="null" /> or empty.
		/// </exception>
		void Save(string targetFile);

		/// <summary>
		/// Writes the process event to a file.
		/// </summary>
		/// <param name="dto">
		/// The process event data transfer object to write.
		/// </param>
		void Write(ProcessEventDto dto);
	}
}