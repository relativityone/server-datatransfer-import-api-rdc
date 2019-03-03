// ----------------------------------------------------------------------------
// <copyright file="NullProcessErrorWriter.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Process
{
	/// <summary>
	/// Represents a class object that provides standard null object behavior by skipping writing process errors to a file.
	/// </summary>
	public sealed class NullProcessErrorWriter : IProcessErrorWriter
	{
		/// <inheritdoc />
		public void Write(string key, string description)
		{
		}
	}
}