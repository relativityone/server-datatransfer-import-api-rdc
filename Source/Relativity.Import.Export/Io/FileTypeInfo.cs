// ----------------------------------------------------------------------------
// <copyright file="FileTypeIdInfo.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Io
{
	/// <summary>
	/// Represents a class object that describes the file identification information for a specific file.
	/// </summary>
	internal class FileTypeIdInfo : IFileTypeIdInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FileTypeIdInfo"/> class.
		/// </summary>
		/// <param name="id">
		/// The file identifier.
		/// </param>
		/// <param name="description">
		/// The file type description.
		/// </param>
		public FileTypeIdInfo(int id, string description)
		{
			this.Id = id;
			this.Description = description;
		}

		/// <inheritdoc />
		public int Id
		{
			get;
		}

		/// <inheritdoc />
		public string Description
		{
			get;
		}
	}
}