// ----------------------------------------------------------------------------
// <copyright file="ConvertToMultiPageTiffException.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// The exception thrown when a serious failure occurs attempting to convert a multi-page TIFF. This class cannot be inherited.
	/// </summary>
	[Serializable]
	public sealed class ConvertToMultiPageTiffException : ImageRollupException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ConvertToMultiPageTiffException"/> class.
		/// </summary>
		public ConvertToMultiPageTiffException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConvertToMultiPageTiffException"/> class.
		/// </summary>
		/// <param name="message">
		/// The message that describes the error.
		/// </param>
		public ConvertToMultiPageTiffException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConvertToMultiPageTiffException"/> class.
		/// </summary>
		/// <param name="message">
		/// The error message that explains the reason for the exception.
		/// </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.
		/// </param>
		public ConvertToMultiPageTiffException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConvertToMultiPageTiffException"/> class.
		/// </summary>
		/// <param name="file">
		/// The full path to the image file that caused the exception.
		/// </param>
		/// <param name="pageNumber">
		/// The number of pages in the file.
		/// </param>
		/// <param name="pageCount">
		/// The image page count.
		/// </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.
		/// </param>
		public ConvertToMultiPageTiffException(string file, int pageNumber, int pageCount, Exception innerException)
			: base(file, pageNumber, pageCount, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ConvertToMultiPageTiffException"/> class.
		/// </summary>
		/// <param name="info">
		/// The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.
		/// </param>
		private ConvertToMultiPageTiffException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		/// <inheritdoc />
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}
	}
}