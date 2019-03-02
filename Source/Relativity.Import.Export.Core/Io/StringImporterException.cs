// ----------------------------------------------------------------------------
// <copyright file="StringImporterException.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Importer
{
	using System;
	using System.Globalization;
	using System.Runtime.Serialization;

	using Relativity.Import.Export.Resources;

	/// <summary>
	/// Represents an exception that occured while attempting to import a string.
	/// </summary>
	[Serializable]
	public class StringImporterException : ImporterException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StringImporterException" /> class.
		/// </summary>
		public StringImporterException()
	    {
	    }

		/// <summary>
		/// Initializes a new instance of the <see cref="StringImporterException"/> class.
		/// </summary>
		/// <param name="message">
		/// The message that describes the error.
		/// </param>
		public StringImporterException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StringImporterException"/> class.
		/// </summary>
		/// <param name="message">
		/// The error message that explains the reason for the exception.
		/// </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.
		/// </param>
		public StringImporterException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StringImporterException"/> class.
		/// </summary>
		/// <param name="row">
		/// The index of the row causing the exception.
		/// </param>
		/// <param name="column">
		/// The index of the column causing the exception.
		/// </param>
		/// <param name="length">
		/// The length of the string value causing the exception.
		/// </param>
		public StringImporterException(long row, int column, int length)
			: base(row, column, GetAdditionalInfoMessage(length))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StringImporterException"/> class.
		/// </summary>
		/// <param name="row">
		/// The index of the row causing the exception.
		/// </param>
		/// <param name="column">
		/// The index of the column causing the exception.
		/// </param>
		/// <param name="length">
		/// The length of the string value causing the exception.
		/// </param>
		/// <param name="fieldName">
		/// The name of the field causing the exception.
		/// </param>
		public StringImporterException(long row, int column, int length, string fieldName)
			: base(row, column, fieldName, GetAdditionalInfoMessage(length, fieldName))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StringImporterException"/> class.
		/// </summary>
		/// <param name="row">
		/// The index of the row causing the exception.
		/// </param>
		/// <param name="column">
		/// The index of the column causing the exception.
		/// </param>
		/// <param name="sourceLength">
		/// The length of the source string value.
		/// </param>
		/// <param name="destinationMaxLength">
		/// The length of the destination string value causing the exception.
		/// </param>
		/// <param name="fieldName">
		/// The name of the field causing the exception.
		/// </param>
		public StringImporterException(
			long row,
			int column,
			int sourceLength,
			int destinationMaxLength,
			string fieldName)
			: base(row, column, fieldName, GetAdditionalInfoMessage(sourceLength, destinationMaxLength, fieldName))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="StringImporterException"/> class.
		/// </summary>
		/// <param name="info">
		/// The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.
		/// </param>
		protected StringImporterException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		/// <inheritdoc />
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		/// <summary>
		/// Gets the additional information message.
		/// </summary>
		/// <param name="length">
		/// The length of the string value causing the exception.
		/// </param>
		/// <returns>
		/// The error message.
		/// </returns>
		internal static string GetAdditionalInfoMessage(int length)
		{
			return string.Format(
				CultureInfo.CurrentCulture,
				Strings.StringImporterMaxLengthWithoutFieldErrorAdditionalInfo,
				length);
		}

		/// <summary>
		/// Gets the additional information message.
		/// </summary>
		/// <param name="length">
		/// The length of the string value causing the exception.
		/// </param>
		/// <param name="fieldName">
		/// The name of the field causing the exception.
		/// </param>
		/// <returns>
		/// The error message.
		/// </returns>
		internal static string GetAdditionalInfoMessage(int length, string fieldName)
		{
			return string.Format(
				CultureInfo.CurrentCulture,
				Strings.StringImporterMaxLengthWithFieldErrorAdditionalInfo,
				length,
				fieldName);
		}

		/// <summary>
		/// Gets the additional information message.
		/// </summary>
		/// <param name="sourceLength">
		/// The length of the source string value.
		/// </param>
		/// <param name="destinationMaxLength">
		/// The length of the destination string value causing the exception.
		/// </param>
		/// <param name="fieldName">
		/// The name of the field causing the exception.
		/// </param>
		/// <returns>
		/// The error message.
		/// </returns>
		internal static string GetAdditionalInfoMessage(int sourceLength, int destinationMaxLength, string fieldName)
		{
			return string.Format(
				CultureInfo.CurrentCulture,
				Strings.StringImporterMaxLengthExWithFieldErrorAdditionalInfo,
				fieldName,
				sourceLength,
				destinationMaxLength);
		}
	}
}