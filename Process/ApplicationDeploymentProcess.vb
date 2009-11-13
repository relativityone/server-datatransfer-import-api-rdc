Namespace kCura.WinEDDS
	Public Class ApplicationDeploymentProcess
		Inherits kCura.Windows.Process.ProcessBase

#Region " Constructors "

		Public Sub New(ByVal application As Xml.XmlDocument, ByVal credential As Net.NetworkCredential, ByVal cookieContainer As Net.CookieContainer, ByVal caseInfo As kCura.EDDS.Types.CaseInfo)
			MyBase.New()
			_application = application
			_credential = credential
			_cookieContainer = cookieContainer
			_caseInfo = caseInfo
		End Sub

#End Region

#Region " Protected Methods "

		Protected Overrides Sub Execute()
			Try
				Dim installationParameters As New kCura.EDDS.WebAPI.TemplateManagerBase.ApplicationInstallationParameters
				installationParameters.CaseId = _caseInfo.ArtifactID

				Dim applicationDeploymentSystem As New WinEDDS.Service.TemplateManager(_credential, Me._cookieContainer)
				Dim installationResult As kCura.EDDS.WebAPI.TemplateManagerBase.ApplicationInstallationResult = applicationDeploymentSystem.InstallTemplate(_application, installationParameters)

				If installationResult.Success Then
					Dim installedArtifacts As New System.Text.StringBuilder
					For Each applicationArtifact As kCura.EDDS.WebAPI.TemplateManagerBase.ApplicationArtifact In installationResult.NewApplicationArtifacts
						installedArtifacts.AppendFormat(System.Globalization.CultureInfo.CurrentCulture, "Created {0}: {1} (ID = {2}){3}", applicationArtifact.Type, applicationArtifact.Name, applicationArtifact.ArtifactId, System.Environment.NewLine)
					Next
					For Each applicationArtifact As kCura.EDDS.WebAPI.TemplateManagerBase.ApplicationArtifact In installationResult.UpdatedApplicationArtifacts
						installedArtifacts.AppendFormat(System.Globalization.CultureInfo.CurrentCulture, "Updated {0}: {1} (ID = {2}){3}", applicationartifact.Type, applicationartifact.Name, applicationartifact.ArtifactId, System.Environment.NewLine)
					Next
					WriteStatus(String.Format(System.Globalization.CultureInfo.CurrentCulture, "Installation successful.{0}{0}{1}", System.Environment.NewLine, installedArtifacts))
				Else
					WriteStatus(String.Format(System.Globalization.CultureInfo.CurrentCulture, "Error installing Application: {0}", installationResult.ExceptionMessage))
				End If
			Catch ex As Exception
				WriteStatus(String.Format("Application Installation Error: {0}", ex.Message))
			End Try
		End Sub

#End Region

#Region " Private Fields "

		Private _application As Xml.XmlDocument
		Private _caseInfo As kCura.EDDS.Types.CaseInfo
		Private _cookieContainer As Net.CookieContainer
		Private _credential As Net.NetworkCredential

#End Region

#Region " Private Methods "

		Private Sub WriteStatus(ByVal message As String)
			Me.ProcessObserver.RaiseStatusEvent("", message)
		End Sub

#End Region

	End Class
End Namespace