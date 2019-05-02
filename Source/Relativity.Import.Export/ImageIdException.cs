﻿// ----------------------------------------------------------------------------
// <copyright file="ImageIdException.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Import.Export
{
	using System;
	using System.Runtime.Serialization;

	/// <summary>
	/// The exception thrown when a serious image identification error occurs. This class cannot be inherited.
	/// </summary>
	[Serializable]
	public sealed class ImageIdException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ImageIdException"/> class.
		/// </summary>
		public ImageIdException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageIdException"/> class.
		/// </summary>
		/// <param name="message">
		/// The message that describes the error.
		/// </param>
		public ImageIdException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageIdException"/> class.
		/// </summary>
		/// <param name="message">
		/// The error message that explains the reason for the exception.
		/// </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.
		/// </param>
		public ImageIdException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ImageIdException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
		private ImageIdException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}

		/// <inheritdoc />
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException(nameof(info));
			}

			base.GetObjectData(info, context);
		}
	}
}