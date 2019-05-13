// ----------------------------------------------------------------------------
// <copyright file="IFileTypeInfo.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Io
{
	/// <summary>
	/// Represents an abstract object that describes file type information.
	/// </summary>
	public interface IFileTypeInfo
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