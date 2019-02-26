// ----------------------------------------------------------------------------
// <copyright file="FileInfoInvalidPathException.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export.Io
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// Represent the error that occurs when there are illegal characters in the file path.
	/// </summary>
	[Serializable]
	public class FileInfoInvalidPathException : Exception
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="FileInfoInvalidPathException" /> class.
		/// </summary>
		public FileInfoInvalidPathException()
	    {
	    }

		/// <summary>
		/// Initializes a new instance of the <see cref="FileInfoInvalidPathException"/> class.
		/// </summary>
		/// <param name="message">
		/// The message that describes the error.
		/// </param>
		public FileInfoInvalidPathException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FileInfoInvalidPathException"/> class.
		/// </summary>
		/// <param name="message">
		/// The error message that explains the reason for the exception.
		/// </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.
		/// </param>
		public FileInfoInvalidPathException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FileInfoInvalidPathException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
		protected FileInfoInvalidPathException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}