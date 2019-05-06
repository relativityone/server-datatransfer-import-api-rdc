// ----------------------------------------------------------------------------
// <copyright file="FileTypeInfo.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Io
{
	/// <summary>
	/// Represents a class object that describes file type information.
	/// </summary>
	internal class FileTypeInfo : IFileTypeInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FileTypeInfo"/> class.
		/// </summary>
		/// <param name="id">
		/// The file identifier.
		/// </param>
		/// <param name="description">
		/// The file type description.
		/// </param>
		public FileTypeInfo(int id, string description)
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