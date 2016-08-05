Imports kCura.EDDS.WebAPI.TemplateManagerBase
Namespace kCura.WinEDDS
	Public Class ApplicationDeploymentProcess
		Inherits kCura.Windows.Process.Generic.ProcessBase(Of ApplicationInstallationResult)

#Region " Constructors "

		Public Sub New(ByVal appsToOverride As Int32(), ByVal resolveArtifacts()() As ResolveArtifact, packageData As Byte(), ByVal credential As Net.NetworkCredential, ByVal cookieContainer As Net.CookieContainer, ByVal caseInfos As Generic.IEnumerable(Of Relativity.CaseInfo))
			MyBase.New(Function(res As ApplicationInstallationResult) res.Message, Function(res As ApplicationInstallationResult) res.Details)
			_packageData = packageData
			_appsToOverride = appsToOverride
			_resolveArtifacts = resolveArtifacts
			_credential = credential
			_cookieContainer = cookieContainer
			_caseInfos = caseInfos
		End Sub

#End Region

#Region " Protected Methods "

		Protected Overrides Sub Execute()
			For i As Int32 = 0 To _caseInfos.Count - 1
				Dim caseInfo As Relativity.CaseInfo = _caseInfos(i)
				Try
					Dim installationParameters As New ApplicationInstallationParameters
					installationParameters.CaseId = caseInfo.ArtifactID

					Dim applicationDeploymentSystem As New WinEDDS.Service.TemplateManager(_credential, Me._cookieContainer)

					Dim resolutionResult As ApplicationInstallationResult = Nothing
					If _resolveArtifacts IsNot Nothing AndAlso _resolveArtifacts(i).Count > 0 Then
						resolutionResult = applicationDeploymentSystem.ResolveConflicts(_appsToOverride, _resolveArtifacts(i), installationParameters)
						If Not resolutionResult.Success Then
							resolutionResult.TotalWorkspaces = _caseInfos.Count
							resolutionResult.WorkspaceID = caseInfo.ArtifactID
							resolutionResult.WorkspaceName = caseInfo.Name
							Me.ProcessObserver.RaiseStatusEvent(resolutionResult)
							Exit Sub
						End If
					End If

					Dim installationResult As ApplicationInstallationResult = applicationDeploymentSystem.InstallTemplate(_appsToOverride, _packageData, installationParameters)
					Dim tempArr As New System.Collections.Generic.List(Of ApplicationArtifact)(installationResult.StatusApplicationArtifacts)
					If resolutionResult IsNot Nothing Then
						tempArr.AddRange(resolutionResult.StatusApplicationArtifacts)
					End If
					installationResult.StatusApplicationArtifacts = tempArr.ToArray()
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

		Private _packageData As Byte()
		Private _appsToOverride As Int32()
		Private _resolveArtifacts As ResolveArtifact()()
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