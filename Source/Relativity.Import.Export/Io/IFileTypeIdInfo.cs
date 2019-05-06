// ----------------------------------------------------------------------------
// <copyright file="IFileTypeIdInfo.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Io
{
	/// <summary>
	/// Represents an abstract object that describes the file identification information for a specific file.
	/// </summary>
	public interface IFileTypeIdInfo
	{
		/// <summary>
		/// Gets the file identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		int Id
		{
			get;
		}

		/// <summary>
		/// Gets the file type description.
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		string Description
		{
			get;
		}
	}
}