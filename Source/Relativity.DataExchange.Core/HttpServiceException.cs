// ----------------------------------------------------------------------------
// <copyright file="HttpServiceException.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.DataExchange
{
	using System;
	using System.Net;
	using System.Runtime.Serialization;

	/// <summary>
	/// The exception thrown when call to Http server failed (e.g. WebApi or Kepler). This class cannot be inherited.
	/// </summary>
	[Serializable]
	public sealed class HttpServiceException : Exception
	{
		/// <summary>
		/// The default fatal value.
		/// </summary>
		public const bool DefaultFatalValue = false;

		/// <summary>
		/// The default HTTP status code when it's unknown or not supplied.
		/// </summary>
		public const HttpStatusCode DefaultHttpStatusCode = HttpStatusCode.InternalServerError;

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpServiceException"/> class.
		/// </summary>
		public HttpServiceException()
		{
			this.Fatal = DefaultFatalValue;
			this.StatusCode = DefaultHttpStatusCode;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpServiceException"/> class.
		/// </summary>
		/// <param name="message">
		/// The message that describes the error.
		/// </param>
		public HttpServiceException(string message)
			: base(message)
		{
			this.Fatal = DefaultFatalValue;
			this.StatusCode = DefaultHttpStatusCode;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpServiceException"/> class.
		/// </summary>
		/// <param name="message">
		/// The message that describes the error.
		/// </param>
		/// <param name="fatal">
		/// Specify whether the error is fatal.
		/// </param>
		public HttpServiceException(string message, bool fatal)
			: this(message, DefaultHttpStatusCode, fatal)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpServiceException"/> class.
		/// </summary>
		/// <param name="message">
		/// The message that describes the error.
		/// </param>
		/// <param name="statusCode">
		/// The HTTP response status code.
		/// </param>
		/// <param name="fatal">
		/// Specify whether the error is fatal.
		/// </param>
		public HttpServiceException(string message, HttpStatusCode statusCode, bool fatal)
			: base(message)
		{
			this.StatusCode = statusCode;
			this.Fatal = fatal;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpServiceException"/> class.
		/// </summary>
		/// <param name="message">
		/// The error message that explains the reason for the exception.
		/// </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.
		/// </param>
		public HttpServiceException(string message, Exception innerException)
			: base(message, innerException)
		{
			this.StatusCode = DefaultHttpStatusCode;
			this.Fatal = DefaultFatalValue;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpServiceException"/> class.
		/// </summary>
		/// <param name="message">
		/// The error message that explains the reason for the exception.
		/// </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.
		/// </param>
		/// <param name="fatal">
		/// Specify whether the error is fatal.
		/// </param>
		public HttpServiceException(string message, Exception innerException, bool fatal)
			: this(message, innerException, DefaultHttpStatusCode, fatal)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpServiceException"/> class.
		/// </summary>
		/// <param name="message">
		/// The error message that explains the reason for the exception.
		/// </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.
		/// </param>
		/// <param name="statusCode">
		/// The HTTP response status code.
		/// </param>
		/// <param name="fatal">
		/// Specify whether the error is fatal.
		/// </param>
		public HttpServiceException(string message, Exception innerException, HttpStatusCode statusCode, bool fatal)
			: base(message, innerException)
		{
			this.Fatal = fatal;
			this.StatusCode = statusCode;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HttpServiceException"/> class.
		/// </summary>
		/// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
		/// <param name="context">The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source or destination.</param>
		private HttpServiceException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			this.Fatal = info.GetBoolean(nameof(this.Fatal));
			this.StatusCode = (HttpStatusCode)info.GetValue(nameof(this.StatusCode), typeof(HttpStatusCode));
		}

		/// <summary>
		/// Gets a value indicating whether the error is considered fatal.
		/// </summary>
		/// <value>
		/// <see langword="true"/> if the error is considered fatal; otherwise, <see langword="false"/>.
		/// </value>
		public bool Fatal
		{
			get;
		}

		/// <summary>
		/// Gets the HTTP response status code.
		/// </summary>
		/// <value>
		/// The <see cref="HttpStatusCode"/> value.
		/// </value>
		public HttpStatusCode StatusCode
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

			info.AddValue(nameof(this.Fatal), this.Fatal);
			info.AddValue(nameof(this.StatusCode), this.StatusCode);
			base.GetObjectData(info, context);
		}
	}
}