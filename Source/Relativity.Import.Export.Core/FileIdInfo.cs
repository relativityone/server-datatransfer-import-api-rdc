// ----------------------------------------------------------------------------
// <copyright file="FileIdInfo.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	using System;

	/// <summary>
	/// Represents a class object that describes the file identification information for a specific file.
	/// </summary>
	public sealed class FileIdInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FileIdInfo"/> class.
		/// </summary>
		/// <param name="file">
		/// The full path to the file.
		/// </param>
		/// <param name="id">
		/// The file identifier.
		/// </param>
		/// <param name="description">
		/// The file type description.
		/// </param>
		/// <exception cref="System.ArgumentNullException">
		/// Thrown when <paramref name="file" /> is <see langword="null" /> or empty.
		/// </exception>
		public FileIdInfo(string file, int id, string description)
		{
			if (string.IsNullOrEmpty(file))
			{
				throw new ArgumentNullException(nameof(file));
			}

			this.File = file;
			this.Id = id;
			this.Description = description;
		}

		/// <summary>
		/// Gets the full path to the file used to obtain the file identification information.
		/// </summary>
		/// <value>
		/// The full path.
		/// </value>
		public string File
		{
			get;
		}

		/// <summary>
		/// Gets the file identifier.
		/// </summary>
		/// <value>
		/// The identifier.
		/// </value>
		public int Id
		{
			get;
		}

		/// <summary>
		/// Gets the file type description.
		/// </summary>
		/// <value>
		/// The description.
		/// </value>
		public string Description
		{
			get;
		}
	}
}