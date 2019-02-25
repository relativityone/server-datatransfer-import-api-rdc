// ----------------------------------------------------------------------------
// <copyright file="IProcessErrorWriter.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Process
{
	/// <summary>
	/// Represents an abstract object that writes errors to a file.
	/// </summary>
	public interface IProcessErrorWriter
	{
		/// <summary>
		/// Writes the error information to a file.
		/// </summary>
		/// <param name="key">
		/// The error key.
		/// </param>
		/// <param name="description">
		/// The error description.
		/// </param>
		void Write(string key, string description);
	}
}