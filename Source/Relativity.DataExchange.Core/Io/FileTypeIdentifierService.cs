// ----------------------------------------------------------------------------
// <copyright file="FileTypeIdentifierService.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Io
{
	using System;

	/// <summary>
	/// Provides a way to access the FileTypeIdentifier.
	/// </summary>
	public static class FileTypeIdentifierService
	{
		/// <summary>
		/// A singleton instance of the file identifier service.
		/// </summary>
		private static readonly Lazy<OutsideInFileTypeIdentifierService> LazyInstance = new Lazy<OutsideInFileTypeIdentifierService>();

		/// <summary>
		/// Gets the file identification singleton instance.
		/// </summary>
		public static IFileTypeIdentifier Instance => LazyInstance.Value;
	}
}
