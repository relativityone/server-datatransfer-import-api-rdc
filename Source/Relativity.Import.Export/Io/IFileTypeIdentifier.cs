// ----------------------------------------------------------------------------
// <copyright file="IFileTypeIdentifier.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Io
{
	using System;

	/// <summary>
	/// Represents an abstract object to identify file types.
	/// </summary>
	public interface IFileTypeIdentifier : IDisposable
	{
		/// <summary>
		/// Gets the file identification configuration.
		/// </summary>
		/// <value>
		/// The <see cref="FileTypeIdConfiguration"/> instance.
		/// </value>
		IFileTypeIdConfiguration Configuration
		{
			get;
		}

		/// <summary>
		/// Identifies the specified file.
		/// </summary>
		/// <param name="file">The file.</param>
		/// <returns>FileIdInfo.</returns>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown when <paramref name="file" /> is <see langword="null" /> or empty.
		/// </exception>
		/// <exception cref="System.IO.FileNotFoundException">
		/// The exception thrown when the file doesn't exist.
		/// </exception>
		/// <exception cref="FileTypeIdException">
		/// The exception thrown when the source file cannot be identified.
		/// </exception>
		IFileTypeIdInfo Identify(string file);

		/// <summary>
		/// Reinitialize the Outside In runtime. This should only be done in cases where OI is failing and this instance cannot be easily disposed.
		/// </summary>
		void Reinitialize();
	}
}