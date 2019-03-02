// ----------------------------------------------------------------------------
// <copyright file="DecimalImporterException.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Io
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// Represents an exception that occured while attempting to import a boolean.
	/// </summary>
	[Serializable]
	public class DecimalImporterException : ImporterException
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DecimalImporterException" /> class.
		/// </summary>
		public DecimalImporterException()
	    {
	    }

		/// <summary>
		/// Initializes a new instance of the <see cref="DecimalImporterException"/> class.
		/// </summary>
		/// <param name="message">
		/// The message that describes the error.
		/// </param>
		public DecimalImporterException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DecimalImporterException"/> class.
		/// </summary>
		/// <param name="message">
		/// The error message that explains the reason for the exception.
		/// </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.
		/// </param>
		public DecimalImporterException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DecimalImporterException"/> class.
		/// </summary>
		/// <param name="row">
		/// The index of the row causing the exception.
		/// </param>
		/// <param name="column">
		/// The index of the column causing the exception.
		/// </param>
		/// <param name="innerException">
		/// The exception that caused this exception.
		/// </param>
		public DecimalImporterException(long row, int column, Exception innerException)
			: base(row, column, "Invalid decimal.", innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DecimalImporterException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
		protected DecimalImporterException(SerializationInfo info, StreamingContext context)
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