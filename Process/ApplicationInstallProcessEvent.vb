Namespace kCura.WinEDDS
	<Serializable()> Public Class ApplicationInstallProcessEvent
		Inherits kCura.Windows.Process.ProcessEvent

		Public InstallationResult As kCura.EDDS.WebAPI.TemplateManagerBase.ApplicationInstallationResult

		Public Sub New()

		End Sub

		Public Sub New(ByVal installResult As kCura.EDDS.WebAPI.TemplateManagerBase.ApplicationInstallationResult)
			Me.DateTime = System.DateTime.Now
			Me.InstallationResult = installResult
			If installResult.Success Then
				Me.Type = Windows.Process.ProcessEventTypeEnum.Status
				Me.Message = "Installation successful."
			Else
				Me.Type = Windows.Process.ProcessEventTypeEnum.[Error]
				Me.Message = "Installation failed."
			End If
			Me.RecordInfo = ""
		End Sub
	End Class
End Namespace
