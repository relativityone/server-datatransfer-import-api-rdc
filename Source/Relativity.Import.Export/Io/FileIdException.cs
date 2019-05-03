// ----------------------------------------------------------------------------
// <copyright file="FileIdException.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// The exception thrown when a serious file identification error occurs. This class cannot be inherited.
	/// </summary>
	[Serializable]
	public sealed class FileIdException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FileIdException"/> class.
		/// </summary>
		public FileIdException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FileIdException"/> class.
		/// </summary>
		/// <param name="message">
		/// The message that describes the error.
		/// </param>
		public FileIdException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FileIdException"/> class.
		/// </summary>
		/// <param name="message">
		/// The error message that explains the reason for the exception.
		/// </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.
		/// </param>
		public FileIdException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FileIdException"/> class.
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
		public FileIdException(string message, Exception innerException, FileIdError error)
			: base(message, innerException)
		{
			this.Error = error;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FileIdException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
		private FileIdException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.Error = (FileIdError)info.GetValue(nameof(this.Error), typeof(FileIdError));
		}

		/// <summary>
		/// Gets the file identification error.
		/// </summary>
		/// <value>
		/// The <see cref="FileIdError"/> value.
		/// </value>
		public FileIdError Error
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