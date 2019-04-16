Imports Relativity.Import.Export.Services

Namespace kCura.WinEDDS
	Public Class LoadCaseEvent
		Inherits kCura.WinEDDS.AppEvent
		Public [Case] As CaseInfo
		Public Sub New(ByVal [case] As CaseInfo)
			MyBase.New(AppEvent.AppEventType.LoadCase)
			Me.Case = [case]
		End Sub
	End Class
End Namespace