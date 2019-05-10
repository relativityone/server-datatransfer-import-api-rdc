Imports System.Net
Imports System.Threading
Imports System.Threading.Tasks
Imports System.Web.Services.Protocols
Imports Relativity.Import.Export

Namespace kCura.WinEDDS.Service
	Friend Class ApplicationVersionService
		Implements IApplicationVersionService

		Private Const InstanceDetailsServiceRelPath As String = "/Relativity.Rest/api/Relativity.Services.InstanceDetails.IInstanceDetailsModule/InstanceDetailsService/GetRelativityVersionAsync"

		Private ReadOnly _appSettings As IAppSettings
		Private ReadOnly _logger As Global.Relativity.Logging.ILog
		Private ReadOnly _instance As RelativityInstanceInfo

		Public Sub New(instance As RelativityInstanceInfo, appSettings As IAppSettings, logger As Global.Relativity.Logging.ILog)
			_instance = instance
			_appSettings = appSettings
			_logger = logger
		End Sub

		Public Async Function GetRelativityVersionAsync(ByVal token As CancellationToken) As Task(Of Version) Implements IApplicationVersionService.GetRelativityVersionAsync
			Dim client As New RestClient(_instance, _logger, _appSettings.HttpTimeoutSeconds, _appSettings.IoErrorNumberOfRetries)
			Dim relVersionString As String = Await client.RequestPostStringAsync(
			InstanceDetailsServiceRelPath,
			String.Empty,
			Function(retryAttempt)
				Return TimeSpan.FromSeconds(_appSettings.HttpTimeoutSeconds)
			End Function,
			Sub(exception, timespan, context)
				_logger.LogError(exception, "Retry - {Timespan} - Failed to retrieve the Relativity Version.", timespan)
			End Sub,
			Function(code)
				Return "query Relativity get version"
			End Function,
			Function(code)
				Return My.Resources.Strings.GetRelativityVersionFailedExceptionMessage
			End Function,
			token).ConfigureAwait(False)

			' Relativity version is stored in additional quotation marks
			relVersionString = relVersionString.TrimStart(""""c).TrimEnd(""""c)
			Return Version.Parse(relVersionString)
		End Function

		Public Async Function GetImportExportWebApiVersionAsync(ByVal token As CancellationToken) As Task(Of Version) Implements IApplicationVersionService.GetImportExportWebApiVersionAsync
			Try
				Using relativityManager As New RelativityManager(
				_instance.Credentials,
				_instance.CookieContainer,
				_instance.WebApiServiceUrl.ToString(),
				_appSettings.WebApiOperationTimeout)
					Dim version As String = Await Task.FromResult(relativityManager.GetImportExportWebApiVersion()).ConfigureAwait(False)
					Return System.Version.Parse(version)
				End Using
			Catch e As SoapException
				If ExceptionHelper.IsEndpointNotFound(e, NameOf(RelativityManager.GetImportExportWebApiVersion)) Then
					Throw New HttpServiceException(My.Resources.Strings.GetImportExportWebApiVersionNotFoundExceptionMessage, e, HttpStatusCode.NotFound, False)
				End If

				Throw New HttpServiceException(My.Resources.Strings.GetImportExportWebApiVersionExceptionMessage, e, True)
			Catch e As Exception
				Throw New HttpServiceException(My.Resources.Strings.GetImportExportWebApiVersionExceptionMessage, e, True)
			End Try
		End Function
	End Class
End Namespace