Imports System.Net
Imports kCura.WinEDDS.Service
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Service
Imports Relativity.DataExchange.Service.RelativityDistributed
Imports Relativity.Logging

Namespace kCura.WinEDDS
	Public Class ErrorFileService

		Private ReadOnly _fileIoService As FileIO
		Private ReadOnly _relativityDistributedFacade As IRelativityDistributedFacade

		Public Sub New(credentials As NetworkCredential, downloadHandlerUrl As String, cookieContainer As CookieContainer)
			_fileIoService = New FileIO(credentials, cookieContainer) With {
				.Timeout = Int32.MaxValue
			}

			_relativityDistributedFacade = CreateRelativityDistributedFacade(credentials, downloadHandlerUrl, cookieContainer)
		End Sub

		Public Function DownloadErrorFile(ByVal localFilePath As String, ByVal remoteFileGuid As String, ByVal caseInfo As CaseInfo) As Boolean
			Return DownloadErrorFile(localFilePath, remoteFileGuid, caseInfo, True)
		End Function

		Public Function DownloadErrorFile(ByVal localFilePath As String, ByVal remoteFileGuid As String, ByVal caseInfo As CaseInfo, ByVal removeErrorFileOnServer As Boolean) As Boolean
			Dim request As FileDownloadRequest = New FileDownloadRequest(localFilePath, caseInfo.ArtifactID.ToString(), remoteFileGuid)
			Dim response As FileDownloadResponse = _relativityDistributedFacade.DownloadFile(request)
			If Not response.IsSuccess Then
				HandleFileDownloadFailureAndThrowException(response)
			End If

			If removeErrorFileOnServer Then
				_fileIoService.RemoveTempFile(caseInfo.ArtifactID, remoteFileGuid)
			End If

			Return True
		End Function

		Public Sub RemoveErrorFile(ByVal remoteFileGuid As String, ByVal caseInfo As CaseInfo)
			_fileIoService.RemoveTempFile(caseInfo.ArtifactID, remoteFileGuid)
		End Sub

		Public Event UploadStatusEvent(ByVal message As String)

		Private Sub HandleFileDownloadFailureAndThrowException(fileDownloadResponse As FileDownloadResponse) ' TODO can/should it be in the different class???
			Const genericErrorMessage As String = "Error Downloading File"
			
			Dim specificErrorMessage As String = genericErrorMessage
			If fileDownloadResponse.ErrorType = RelativityDistributedErrorType.Authentication Then
				specificErrorMessage = "Error Downloading File: unable to authenticate against Distributed server"
			Else If fileDownloadResponse.ErrorType = RelativityDistributedErrorType.DataCorrupted Then
				specificErrorMessage = "Error Downloading File: received different number of bytes than expected"
			Else If fileDownloadResponse.ErrorType = RelativityDistributedErrorType.NotFound Then
				specificErrorMessage = "Error Downloading File: the file cannot be found"
			End If

			RaiseEvent UploadStatusEvent(genericErrorMessage)
			Throw New ApplicationException(specificErrorMessage & vbNewLine, fileDownloadResponse.Exception)
		End Sub

		Private Shared Function CreateRelativityDistributedFacade(credentials As NetworkCredential, downloadHandlerUrl As String, cookieContainer As CookieContainer) As IRelativityDistributedFacade
			Dim reLoginService As IReLoginService = New UserManager(credentials, cookieContainer)
			Dim factory As RelativityDistributedFacadeFactory = New RelativityDistributedFacadeFactory(
				Log.Logger,
				AppSettings.Instance,
				reLoginService,
				Global.Relativity.DataExchange.Io.FileSystem.Instance.File,
				authenticationTokenProvider:=Function() Settings.AuthenticationToken
			)

			Return factory.Create(
				downloadHandlerUrl,
				credentials,
				cookieContainer
			)
		End Function
	End Class
End Namespace
