// ----------------------------------------------------------------------------
// <copyright file="FileTypeIdentifierService.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Io
{
	/// <summary>
	/// Provides a way to access the FileTypeIdentifier.
	/// </summary>
	public static class FileTypeIdentifierService
	{
		/// <summary>
		/// An instance of the OutsideIn file identifier service.
		/// </summary>
		private static OutsideInFileTypeIdentifierService instance;

		/// <summary>
		/// Gets an instance of the OutsideIn file identifier service.
		/// </summary>
		public static IFileTypeIdentifier Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new OutsideInFileTypeIdentifierService();
				}

				return instance;
			}
		}
	}
}
