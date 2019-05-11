// ----------------------------------------------------------------------------
// <copyright file="FileTypeIdentifyException.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange.Io
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// The exception thrown when a serious file type identification error occurs. This class cannot be inherited.
	/// </summary>
	[Serializable]
	public sealed class FileTypeIdentifyException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FileTypeIdentifyException"/> class.
		/// </summary>
		public FileTypeIdentifyException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FileTypeIdentifyException"/> class.
		/// </summary>
		/// <param name="message">
		/// The message that describes the error.
		/// </param>
		public FileTypeIdentifyException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FileTypeIdentifyException"/> class.
		/// </summary>
		/// <param name="message">
		/// The error message that explains the reason for the exception.
		/// </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.
		/// </param>
		public FileTypeIdentifyException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FileTypeIdentifyException"/> class.
		/// </summary>
		/// <param name="message">
		/// The error message that explains the reason for the exception.
		/// </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.
		/// </param>
		/// <param name="error">
		/// The file identification error.
		/// </param>
		public FileTypeIdentifyException(string message, Exception innerException, FileTypeIdentifyError error)
			: base(message, innerException)
		{
			this.Error = error;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FileTypeIdentifyException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
		private FileTypeIdentifyException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.Error = (FileTypeIdentifyError)info.GetValue(nameof(this.Error), typeof(FileTypeIdentifyError));
		}

		/// <summary>
		/// Gets the file identification error.
		/// </summary>
		/// <value>
		/// The <see cref="FileTypeIdentifyError"/> value.
		/// </value>
		public FileTypeIdentifyError Error
		{
			get;
		}

		/// <inheritdoc />
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}

			info.AddValue("Error", this.Error);
			base.GetObjectData(info, context);
		}
	}
}