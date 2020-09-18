// <copyright file="FileDownloadResponse.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Service.RelativityDistributed
{
	using System;

	/// <summary>
	/// This type represents Relativity.Distributed response for the file download request.
	/// </summary>
	internal class FileDownloadResponse
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="FileDownloadResponse"/> class.
		/// Which represents successful response.
		/// </summary>
		public FileDownloadResponse()
		{
			IsSuccess = true;
			ErrorType = RelativityDistributedErrorType.Unknown;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FileDownloadResponse"/> class.
		/// Which represent unsuccessful response.
		/// </summary>
		/// <param name="errorType">Type of an error.</param>
		public FileDownloadResponse(RelativityDistributedErrorType errorType)
			: this(errorType, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="FileDownloadResponse"/> class.
		/// Which represent unsuccessful response.
		/// </summary>
		/// <param name="errorType">Type of an error.</param>
		/// <param name="exception">Exception thrown when accessing Relativity Distributed service.</param>
		public FileDownloadResponse(RelativityDistributedErrorType errorType, Exception exception)
		{
			IsSuccess = false;
			ErrorType = errorType;
			Exception = exception;
		}

		/// <summary>
		/// Gets a value indicating whether response was successful.
		/// </summary>
		public bool IsSuccess { get; }

		/// <summary>
		/// Gets a type of an error for unsuccessful response, <see cref="RelativityDistributedErrorType.Undefined"/> otherwise.
		/// </summary>
		public RelativityDistributedErrorType ErrorType { get; }

		/// <summary>
		/// Gets a exception which was thrown for unsuccessful response, null otherwise.
		/// </summary>
		public Exception Exception { get; }
	}
}
