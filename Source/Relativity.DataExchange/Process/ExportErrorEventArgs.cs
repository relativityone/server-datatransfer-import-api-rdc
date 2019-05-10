// ----------------------------------------------------------------------------
// <copyright file="ExportErrorEventArgs.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Process
{
	using System;

	/// <summary>
	/// Represents export error event argument data. This class cannot be inherited.
	/// </summary>
	public sealed class ExportErrorEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ExportErrorEventArgs"/> class.
		/// </summary>
		/// <param name="path">
		/// The full path to the directory or file.
		/// </param>
		public ExportErrorEventArgs(string path)
		{
			this.Path = path;
		}

		/// <summary>
		/// Gets the full path to the directory or file.
		/// </summary>
		/// <value>
		/// The full path.
		/// </value>
		public string Path
		{
			get;
		}
	}
}