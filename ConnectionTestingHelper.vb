Imports kCura.WinEDDS.Aspera.Diagnostics
Imports Relativity

Public Class ConnectionTestingHelper
	Public Sub New(application As kCura.EDDS.WinForm.Application)
		_application =  application
	End Sub

	Private WithEvents _application As kCura.EDDS.WinForm.Application

	Private Sub _application_OnEvent(ByVal appEvent As AppEvent) Handles _application.OnEvent
		Select Case appEvent.EventType
			Case appEvent.AppEventType.LoadCase
				TestCaseConnection(CType(appEvent, LoadCaseEvent).Case)
		End Select
	End Sub

	Private Sub TestCaseConnection(caseInfo As CaseInfo)
		Try
			ConnectionTester.RegisterWorkspaceForTesting(caseInfo.ArtifactID)
		Catch ex As Exception
			'ignore as we don't want to impact existing functionality
		End Try
	End Sub
End Class
