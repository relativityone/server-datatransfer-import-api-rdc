// <copyright file="RelativityDistributedFacade.cs" company="Relativity ODA LLC">
// © Relativity All Rights Reserved.
// </copyright>

namespace Relativity.DataExchange.Service.RelativityDistributed
{
	using System;
	using System.IO;
	using System.Net;

	using Relativity.DataExchange.Io;
	using Relativity.Logging;

	/// <summary>
	/// This is a facade for the Relativity.Distributed service.
	/// </summary>
	internal class RelativityDistributedFacade : IRelativityDistributedFacade
	{
		private const string AccessDeniedMarker = "KCURAACCESSDENIEDMARKER";

		private readonly ILog logger;
		private readonly IAppSettings settings;
		private readonly IFile fileHelper;

		private readonly string downloadUrl;
		private readonly NetworkCredential credentials;
		private readonly CookieContainer cookieContainer;

		private readonly Func<string> authenticationTokenProvider;

		/// <summary>
		/// Initializes a new instance of the <see cref="RelativityDistributedFacade"/> class.
		/// </summary>
		/// <param name="logger">logger.</param>
		/// <param name="settings">Application settings.</param>
		/// <param name="fileHelper">File helper.</param>
		/// <param name="relativityDistributedUrl">Relativity.Distributed URL.</param>
		/// <param name="credentials">Credentials.</param>
		/// <param name="cookieContainer">Cookie container.</param>
		/// <param name="authenticationTokenProvider">Returns authentication token for Relativity.Distributed.</param>
		public RelativityDistributedFacade(
			ILog logger,
			IAppSettings settings,
			IFile fileHelper,
			string relativityDistributedUrl,
			NetworkCredential credentials,
			CookieContainer cookieContainer,
			Func<string> authenticationTokenProvider)
		{
			this.logger = logger;
			this.settings = settings;
			this.fileHelper = fileHelper;

			this.downloadUrl = UrlHelper.GetBaseUrlAndCombine(this.settings.WebApiServiceUrl, relativityDistributedUrl);
			this.cookieContainer = cookieContainer;
			this.credentials = credentials;
			this.authenticationTokenProvider = authenticationTokenProvider;
		}

		/// <inheritdoc/>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "This method uses result instead of exceptions.")]
		public FileDownloadResponse DownloadFile(FileDownloadRequest request)
		{
			this.logger.LogInformation("Attempting to download an error file using Relativity.Distributed. Request: {@request}", request);
			try
			{
				HttpWebRequest httpWebRequest = this.CreateDownloadRequest(request);

				long length = 0;
				using (var webResponse = httpWebRequest.GetResponse())
				{
					length = Math.Max(webResponse.ContentLength, 0);
					using (var responseStream = webResponse.GetResponseStream())
					using (var localFileStream = this.GetDestinationFileStream(request.DestinationFilePath))
					{
						responseStream?.CopyTo(localFileStream, this.settings.WebBasedFileDownloadChunkSize);
					}
				}

				long actualLength = this.fileHelper.GetFileSize(request.DestinationFilePath);
				if (length != actualLength && length > 0)
				{
					this.logger.LogError("Error retrieving data from distributed server; expecting " + length + " bytes and received " + actualLength);
					return new FileDownloadResponse(RelativityDistributedErrorType.DataCorrupted);
				}

				return new FileDownloadResponse();
			}
			catch (WebException ex)
			{
				if (ex.Response is HttpWebResponse response)
				{
					switch (response.StatusCode)
					{
						case HttpStatusCode.Forbidden when response.StatusDescription.ToUpperInvariant() == AccessDeniedMarker:
							return new FileDownloadResponse(RelativityDistributedErrorType.Authentication, ex);
						case HttpStatusCode.Conflict: // 409
							return new FileDownloadResponse(RelativityDistributedErrorType.NotFound, ex);
						case HttpStatusCode.InternalServerError: // 500
							return new FileDownloadResponse(RelativityDistributedErrorType.InternalServerError, ex);
					}
				}

				this.LogError(ex, request);
				return new FileDownloadResponse(RelativityDistributedErrorType.Unknown, ex);
			}
			catch (Exception ex)
			{
				this.LogError(ex, request);
				return new FileDownloadResponse(RelativityDistributedErrorType.Unknown, ex);
			}
		}

		private HttpWebRequest CreateDownloadRequest(FileDownloadRequest request)
		{
			string remoteUri = this.BuildDownloadUrl(request);

			var httpWebRequest = (HttpWebRequest)WebRequest.Create(remoteUri);
			httpWebRequest.Credentials = this.credentials;
			httpWebRequest.CookieContainer = this.cookieContainer;
			httpWebRequest.UnsafeAuthenticatedConnectionSharing = true;

			return httpWebRequest;
		}

		private string BuildDownloadUrl(FileDownloadRequest request)
		{
			string baseUrl = this.downloadUrl.TrimEnd('/') + "/";
			string remoteUri = $"{baseUrl}Download.aspx?ArtifactID=-1&GUID={request.RemoteFileGuid}&AppID={request.WorkspaceId}";
			string authenticationToken = this.authenticationTokenProvider();
			if (!string.IsNullOrEmpty(authenticationToken))
			{
				remoteUri += $"&AuthenticationToken={authenticationToken}";
			}

			return remoteUri;
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
					"An error occured when creating destination file: {destinationFilePath}. Will retry: {shouldRetry}",
					destinationFilePath,
					shouldRetry);
				if (shouldRetry)
				{
					return this.GetDestinationFileStream(destinationFilePath, false);
				}

				throw;
			}
		}

		private void LogError(Exception exception, FileDownloadRequest request)
		{
			this.logger.LogError(exception, "An error occured when downloading the error file. Request: {@request}", request);
		}
	}
}
