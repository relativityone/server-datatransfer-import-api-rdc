Imports kCura.EDDS.WebAPI.TemplateManagerBase
Namespace kCura.WinEDDS
	Public Class ApplicationDeploymentProcess
		Inherits kCura.Windows.Process.Generic.ProcessBase(Of ApplicationInstallationResult)

#Region " Constructors "

		Public Sub New(ByVal application As Xml.XmlDocument, ByVal credential As Net.NetworkCredential, ByVal cookieContainer As Net.CookieContainer, ByVal caseInfos As Generic.IEnumerable(Of Relativity.CaseInfo))
			MyBase.New(Function(res As ApplicationInstallationResult) res.Message, Function(res As ApplicationInstallationResult) res.Details)
			_application = application
			_credential = credential
			_cookieContainer = cookieContainer
			_caseInfos = caseInfos
		End Sub

#End Region

#Region " Protected Methods "

		Protected Overrides Sub Execute()
			For Each caseInfo As Relativity.CaseInfo In _caseInfos
				Try
					Dim installationParameters As New ApplicationInstallationParameters
					installationParameters.CaseId = caseInfo.ArtifactID

					Dim applicationDeploymentSystem As New WinEDDS.Service.TemplateManager(_credential, Me._cookieContainer)
					Dim installationResult As ApplicationInstallationResult = applicationDeploymentSystem.InstallTemplate(_application, installationParameters)
					installationResult.TotalWorkspaces = _caseInfos.Count
					installationResult.WorkspaceID = caseInfo.ArtifactID
					installationResult.WorkspaceName = caseInfo.Name
					Me.ProcessObserver.RaiseStatusEvent(installationResult)

				Catch ex As System.Web.Services.Protocols.SoapException
					Try
						Dim MrSoapy As System.Web.Services.Protocols.SoapException = CType(ex, System.Web.Services.Protocols.SoapException)
						WriteError(MrSoapy.Detail("ExceptionMessage").InnerText)
						Return
					Catch ex2 As System.Exception	'MrSoapy not soapified :(
						WriteError(String.Format("Application Installation Error: {0}", ex.Message))
					End Try
					Exit For
				Catch ex As Exception
					WriteError(String.Format("Application Installation Error: {0}", ex.Message))
					Exit For
				End Try
			Next
		End Sub

#End Region

#Region " Private Fields "

		Private _application As Xml.XmlDocument
		Private _caseInfos As Generic.IEnumerable(Of Relativity.CaseInfo)
		Private _cookieContainer As Net.CookieContainer
		Private _credential As Net.NetworkCredential

#End Region

#Region " Private Methods "


		Private Sub WriteError(ByVal message As String)	'TODO: KFM
			Dim res As New ApplicationInstallationResult()
			res.Message = message
			res.Details = ""
			Me.ProcessObserver.RaiseErrorEvent(res)
		End Sub

#End Region

	End Class
End Namespace