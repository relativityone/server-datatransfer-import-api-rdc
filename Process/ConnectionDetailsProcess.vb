Namespace kCura.WinEDDS
	Public Class ConnectionDetailsProcess
		Inherits kCura.Windows.Process.ProcessBase

		Private ReadOnly _credential As Net.NetworkCredential
		Private ReadOnly _cookieContainer As Net.CookieContainer
		Private ReadOnly _caseInfo As Relativity.CaseInfo

		Public Sub New(ByVal credential As Net.NetworkCredential, ByVal cookieContainer As Net.CookieContainer, ByVal caseInfo As Relativity.CaseInfo)
			MyBase.New()

			_credential = credential
			_cookieContainer = cookieContainer
			_caseInfo = caseInfo
		End Sub

		Protected Overrides Sub Execute()
			Me.CheckBcp()
			Me.WriteStatus("")
			Me.CheckDownloadHandlerURL()
			Me.WriteStatus("")

			' TAPI is now responsible to perform uniform connection checks against all available clients.
			Dim connectionInfo As Relativity.Transfer.RelativityConnectionInfo =
				kCura.WinEDDS.TApi.TapiWinEddsHelper.CreateRelativityConnectionInfo(
					WinEDDS.Config.WebServiceURL,
					_caseInfo.ArtifactID,
					_credential.UserName,
					_credential.Password)
			Using transferLog As New kCura.WinEDDS.TApi.RelativityTransferLog
				Using transferHost As New Relativity.Transfer.RelativityTransferHost(connectionInfo, transferLog)
					Dim context As New Relativity.Transfer.DiagnosticsContext
					Dim configuration As New Relativity.Transfer.DiagnosticsConfiguration(context, _cookieContainer)

					' Reducing these values to more quickly publish error information to the user.
					configuration.MaxHttpRetryAttempts = 1
					configuration.MaxJobRetryAttempts = 2
					Try
						AddHandler context.DiagnosticMessage, AddressOf DiagnosticsContext_OnDiagnosticMessage
						transferHost.ConnectionChecksAsync(configuration, System.Threading.CancellationToken.None).GetAwaiter().GetResult()
					Catch e As Exception
						Me.WriteStatus($"A fatal error occurred performing the connection check. Error: {e.Message}")
					Finally
						RemoveHandler context.DiagnosticMessage, AddressOf DiagnosticsContext_OnDiagnosticMessage
					End Try
				End Using
			End Using
		End Sub

		Private Sub DiagnosticsContext_OnDiagnosticMessage(sender As Object, e As Relativity.Transfer.DiagnosticMessageEventArgs)
			Me.WriteStatus(e.Message)
		End Sub
		
		Private Sub CheckDownloadHandlerURL()
			Me.WriteStatus("Validate Download URL:")
			Dim downloadUrl As String = kCura.Utility.URI.GetFullyQualifiedPath(_caseInfo.DownloadHandlerURL, New System.Uri(kCura.WinEDDS.Config.WebServiceURL))
			Me.WriteStatus(downloadUrl)
			Dim myReq As System.Net.HttpWebRequest = DirectCast(System.Net.WebRequest.Create(downloadUrl & "AccessDenied.aspx"), System.Net.HttpWebRequest)
	  Try
		'SF00204217: Set credentials to avoid http 401 when IIS is using Integrated Windows Authentication.
		myReq.UseDefaultCredentials = True
		myReq.GetResponse()
		Me.WriteStatus("URL validated")
	  Catch ex As System.Net.WebException
		With DirectCast(ex.Response, System.Net.HttpWebResponse)
		  If .StatusCode = Net.HttpStatusCode.Forbidden AndAlso .StatusDescription = "kcuraaccessdeniedmarker" Then
			Me.WriteStatus("URL validated")
		  Else
			Me.WriteStatus("Cannot find URL")
			Me.WriteStatus(ex.ToString)
		  End If
		End With
	  Catch ex As System.Exception
		Me.WriteStatus("Cannot find URL")
		Me.WriteStatus(ex.ToString)
	  End Try
		End Sub

		Private Sub CheckBcp()
			Me.WriteStatus("Checking Bulk Share Configuration")
			Dim gateway As New kCura.WinEDDS.Service.FileIO(_credential, _cookieContainer)

			Dim bcpPath As String
			Try
				bcpPath = gateway.GetBcpSharePath(_caseInfo.ArtifactID)
				Me.WriteStatus("Selected BCP folder: " & bcpPath)
			Catch ex As System.Exception
				Me.WriteStatus("Error retrieving BCP folder - WebAPI error")
				Dim ensure As String = "Ensure"
				If WinEDDS.Config.EnableSingleModeImport Then
					Me.WriteStatus("Loads will happen in Single mode - this is less than optimal.  To upgrade,")
					ensure = "ensure"
				End If
				Me.WriteStatus(ensure & " that relativity service account has rights to create/delete files and subdirectories in the BCP folder")
				Me.WriteStatus("Exact error: " & ex.ToString)
				Return
			End Try
			Try
				If Not System.IO.Directory.Exists(bcpPath) Then
					System.IO.Directory.CreateDirectory(bcpPath)
				End If
				Dim path As String = bcpPath.TrimEnd("\"c) & "\"
				System.IO.File.Create(path & "123").Close()
				System.IO.File.Delete(path & "123")
				Me.WriteStatus("Your account (" & System.Security.Principal.WindowsIdentity.GetCurrent.Name & ") has direct access to BCP folder")
			Catch ex As Exception
				Me.WriteStatus("Valid: No direct access to BCP folder from direct account - using Relativity service account and Web Service for access")
				'Me.WriteStatus("Exact error: " & ex.ToString)
			End Try
			Try
				Me.WriteStatus("Validating bulk insert rights")
				If gateway.ValidateBcpShare(_caseInfo.ArtifactID) Then
					Me.WriteStatus("Bulk insert rights validated")
				Else
					Me.WriteStatus("Bulk share configured incorrectly or does not exist")
				End If
			Catch ex As Exception
				Me.WriteStatus("Error running bulk insert")
				Me.WriteStatus("Ensure that EDDSDBO login has bulk admin rights on case DB")
				Me.WriteStatus("Ensure that the security account that SQL Server is running under has rights to the selected BCP folder")
				Dim text As String = ex.ToString
				If TypeOf ex Is System.Web.Services.Protocols.SoapException Then text = System.Web.HttpUtility.HtmlDecode(text)
				Me.WriteStatus("Exact error: " & text)
				Return
			End Try
			Try
				Me.WriteStatus("Retrieving bulk directory drive information:")
				Dim s As String()() = gateway.GetBcpShareSpaceReport(_caseInfo.ArtifactID)
				Me.WriteStatus("Success - report follows:")
				Me.WriteOutReportString(s)
			Catch ex As Exception
				Me.WriteStatus("Error retrieving space information on bulk share - make sure relativity service account has full rights to the share if you want to see this report")
				Me.WriteStatus("Exact error: " & ex.ToString)
				Return
			End Try
		End Sub

		Private Sub WriteOutReportString(ByVal input As String()())
			For Each line As String() In input
				Me.WriteStatus(vbTab & line(0) & ": " & line(1))
			Next
		End Sub

		Private Sub WriteStatus(ByVal message As String)
			Me.ProcessObserver.RaiseStatusEvent("", message)
		End Sub

	End Class
End Namespace

