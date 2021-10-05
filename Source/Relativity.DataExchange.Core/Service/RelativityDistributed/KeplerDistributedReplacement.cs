// <copyright file="KeplerDistributedReplacement.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Service.RelativityDistributed
{
	using System;
	using System.IO;
	using System.Threading.Tasks;

	using Relativity.DataExchange.Io;
	using Relativity.DataTransfer.Legacy.SDK.ImportExport.V1;
	using Relativity.Kepler.Transport;
	using Relativity.Logging;
	using Relativity.Services.Exceptions;

	/// <summary>
	/// Kepler facade which provides methods for downloading a file.
	/// </summary>
	internal sealed class KeplerDistributedReplacement : IRelativityDistributedFacade
	{
		private readonly IKeplerProxy keplerProxy;
		private readonly IFile fileHelper;
		private readonly ILog logger;
		private readonly Func<string> correlationIdFunc;

		/// <summary>
		/// Initializes a new instance of the <see cref="KeplerDistributedReplacement"/> class.
		/// </summary>
		/// <param name="keplerProxy">Kepler proxy.</param>
		/// <param name="fileHelper">Utility class for file operations.</param>
		/// <param name="logger">Logger.</param>
		/// <param name="correlationIdFunc">Function resolving correlation id related with Kepler request.</param>
		public KeplerDistributedReplacement(IKeplerProxy keplerProxy, IFile fileHelper, ILog logger, Func<string> correlationIdFunc)
		{
			this.keplerProxy = keplerProxy;
			this.fileHelper = fileHelper;
			this.logger = logger;
			this.correlationIdFunc = correlationIdFunc;
		}

		/// <summary>
		/// Downloads file.
		/// </summary>
		/// <param name="request">File download request.</param>
		/// <returns>File download response.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1031:DoNotCatchGeneralExceptionTypes",
			Justification = "Exception is included in the response")]
		public FileDownloadResponse DownloadFile(FileDownloadRequest request)
		{
			if (!int.TryParse(request.WorkspaceId, out var workspaceId))
			{
				throw new ArgumentException("WorkspaceID is not integer", nameof(request));
			}

			if (!Guid.TryParse(request.RemoteFileGuid, out var remoteFileGuid))
			{
				throw new ArgumentException("Remote File Guid is not valid guid", nameof(request));
			}

			async Task DownloadFileOrThrowException(IServiceProxyFactory serviceProxyFactory)
			{
				using (IWebDistributedService service = serviceProxyFactory.CreateProxyInstance<IWebDistributedService>())
				{
					using (IKeplerStream keplerStream = await service.DownloadTempFileAsync(workspaceId, remoteFileGuid, this.correlationIdFunc?.Invoke()).ConfigureAwait(false))
					{
						using (var stream = await keplerStream.GetStreamAsync().ConfigureAwait(false))
						{
							using (var fileStream = this.GetDestinationFileStream(request.DestinationFilePath))
							{
								await stream.CopyToAsync(fileStream).ConfigureAwait(false);
							}
						}
					}
				}
			}

			try
			{
				this.keplerProxy.ExecuteAsync(DownloadFileOrThrowException).GetAwaiter().GetResult();

				return new FileDownloadResponse();
			}
			catch (NotAuthorizedException ex)
			{
				LogError(ex, request);
				return new FileDownloadResponse(RelativityDistributedErrorType.Authentication, ex);
			}
			catch (ConflictException ex)
			{
				LogError(ex, request);
				return new FileDownloadResponse(RelativityDistributedErrorType.NotFound, ex);
			}
			catch (ServiceException ex)
			{
				LogError(ex, request);
				return new FileDownloadResponse(RelativityDistributedErrorType.InternalServerError, ex);
			}
			catch (Exception ex)
			{
				LogError(ex, request);
				return new FileDownloadResponse(RelativityDistributedErrorType.Unknown, ex);
			}
		}

		private void LogError(Exception exception, FileDownloadRequest request)
		{
			this.logger.LogError(exception, "An error occurred when downloading the error file. Request: {@request}", request);
		}

		private Stream GetDestinationFileStream(string destinationFilePath, bool shouldRetry = true)
		{
			try
			{
				return this.fileHelper.Create(destinationFilePath);
			}
			catch (Exception ex)
			{
				this.logger.LogError(
					ex,
					"An error occurred when creating destination file: {destinationFilePath}. Will retry: {shouldRetry}",
					destinationFilePath,
					shouldRetry);
				if (shouldRetry)
				{
					return this.GetDestinationFileStream(destinationFilePath, false);
				}

				throw;
			}
		}
	}
}