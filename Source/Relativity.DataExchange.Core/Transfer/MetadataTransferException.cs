// ----------------------------------------------------------------------------
// <copyright file="MetadataTransferException.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Transfer
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// Represents an exception that occured while attempting to transfer metadata load files to the server. This class cannot be inherited.
	/// </summary>
	[Serializable]
	public sealed class MetadataTransferException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="MetadataTransferException" /> class.
		/// </summary>
		public MetadataTransferException()
	    {
	    }

		/// <summary>
		/// Initializes a new instance of the <see cref="MetadataTransferException"/> class.
		/// </summary>
		/// <param name="message">
		/// The message that describes the error.
		/// </param>
		public MetadataTransferException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MetadataTransferException"/> class.
		/// </summary>
		/// <param name="message">
		/// The error message that explains the reason for the exception.
		/// </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.
		/// </param>
		public MetadataTransferException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MetadataTransferException"/> class.
		/// </summary>
		/// <param name="info">
		/// The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.
		/// </param>
		private MetadataTransferException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}