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
	public class FileIdInfo
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FileIdInfo"/> class.
		/// </summary>
		/// <param name="id">
		/// The file identifier.
		/// </param>
		/// <param name="description">
		/// The file type description.
		/// </param>
		public FileIdInfo(int id, string description)
		{
			this.Id = id;
			this.Description = description;
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