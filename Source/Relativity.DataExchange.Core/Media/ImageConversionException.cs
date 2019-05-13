// ----------------------------------------------------------------------------
// <copyright file="ImageConversionException.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Media
{
	using System;
	using System.Globalization;
	using System.Runtime.Serialization;

	using Relativity.DataExchange.Resources;

	/// <summary>
	/// The exception thrown when a serious failure occurs during image rollup.
	/// </summary>
	[Serializable]
	public class ImageConversionException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ImageConversionException"/> class.
		/// </summary>
		public ImageConversionException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageConversionException"/> class.
		/// </summary>
		/// <param name="message">
		/// The message that describes the error.
		/// </param>
		public ImageConversionException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageConversionException"/> class.
		/// </summary>
		/// <param name="message">
		/// The error message that explains the reason for the exception.
		/// </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.
		/// </param>
		public ImageConversionException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageConversionException"/> class.
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
		public ImageConversionException(string file, int pageNumber, int pageCount, Exception innerException)
			: base(GetErrorMessage(file, pageNumber, pageCount), innerException)
		{
			this.File = file;
			this.PageCount = pageCount;
			this.PageNumber = pageNumber;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageConversionException"/> class.
		/// </summary>
		/// <param name="info">
		/// The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.
		/// </param>
		protected ImageConversionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.File = info.GetString(nameof(this.File));
			this.PageNumber = info.GetInt32(nameof(this.PageNumber));
			this.PageCount = info.GetInt32(nameof(this.PageCount));
		}

		/// <summary>
		/// Gets or sets the full path to the image file that caused the exception.
		/// </summary>
		/// <value>
		/// The full path.
		/// </value>
		public string File
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets or sets the number of pages in the file.
		/// </summary>
		/// <value>
		/// The total number of pages.
		/// </value>
		public int PageCount
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets or sets the page number on which the exception occurred.
		/// </summary>
		/// <value>
		/// The page number.
		/// </value>
		public int PageNumber
		{
			get;
			protected set;
		}

		/// <inheritdoc />
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}

			this.File = info.GetString(nameof(this.File));
			this.PageCount = info.GetInt32(nameof(this.PageCount));
			this.PageNumber = info.GetInt32(nameof(this.PageNumber));
			base.GetObjectData(info, context);
		}

		/// <summary>
		/// Gets a formatted error message.
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
		/// <returns>
		/// The error message.
		/// </returns>
		internal static string GetErrorMessage(string file, int pageNumber, int pageCount)
		{
			return string.Format(
				CultureInfo.CurrentCulture,
				Strings.ConvertToMultiPageTiffOrPdfError,
				file,
				pageNumber + 1,
				pageCount);
		}
	}
}