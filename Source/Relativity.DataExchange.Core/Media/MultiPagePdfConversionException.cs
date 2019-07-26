// ----------------------------------------------------------------------------
// <copyright file="MultiPagePdfConversionException.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Media
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// The exception thrown when a serious failure occurs attempting to convert a multi-page PDF. This class cannot be inherited.
	/// </summary>
	[Serializable]
	public sealed class MultiPagePdfConversionException : ImageConversionException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MultiPagePdfConversionException"/> class.
		/// </summary>
		public MultiPagePdfConversionException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MultiPagePdfConversionException"/> class.
		/// </summary>
		/// <param name="message">
		/// The message that describes the error.
		/// </param>
		public MultiPagePdfConversionException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MultiPagePdfConversionException"/> class.
		/// </summary>
		/// <param name="message">
		/// The error message that explains the reason for the exception.
		/// </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.
		/// </param>
		public MultiPagePdfConversionException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MultiPagePdfConversionException"/> class.
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
		public MultiPagePdfConversionException(string file, int pageNumber, int pageCount, Exception innerException)
			: base(file, pageNumber, pageCount, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MultiPagePdfConversionException"/> class.
		/// </summary>
		/// <param name="info">
		/// The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.
		/// </param>
		private MultiPagePdfConversionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}