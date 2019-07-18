// ----------------------------------------------------------------------------
// <copyright file="ObjectNameImporterException.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Data
{
	using System;
	using System.Globalization;
	using System.Runtime.Serialization;

	using Relativity.DataExchange.Resources;

	/// <summary>
	/// Represents an exception that occured while attempting to import an object name. This class cannot be inherited.
	/// </summary>
	[Serializable]
	public sealed class ObjectNameImporterException : ImporterException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectNameImporterException" /> class.
		/// </summary>
		public ObjectNameImporterException()
	    {
	    }

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectNameImporterException"/> class.
		/// </summary>
		/// <param name="message">
		/// The message that describes the error.
		/// </param>
		public ObjectNameImporterException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectNameImporterException"/> class.
		/// </summary>
		/// <param name="message">
		/// The error message that explains the reason for the exception.
		/// </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.
		/// </param>
		public ObjectNameImporterException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectNameImporterException"/> class.
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
		public ObjectNameImporterException(long row, int column, int length, string fieldName)
			: base(row, column, fieldName, CreateAdditionalInfoMessage(length, fieldName))
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ObjectNameImporterException"/> class.
		/// </summary>
		/// <param name="info">
		/// The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.
		/// </param>
		private ObjectNameImporterException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		/// <summary>
		/// Gets the error message.
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
		internal static string CreateAdditionalInfoMessage(int length, string fieldName)
		{
			return string.Format(CultureInfo.CurrentCulture, Strings.ObjectNameImporterMaxLengthErrorAdditionalInfo, length, fieldName);
		}
	}
}