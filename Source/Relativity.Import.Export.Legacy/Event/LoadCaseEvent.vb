Namespace kCura.WinEDDS
	Public Class LoadCaseEvent
		Inherits kCura.WinEDDS.AppEvent
		Public [Case] As Relativity.CaseInfo
		Public Sub New(ByVal [case] As Relativity.CaseInfo)
			MyBase.New(AppEvent.AppEventType.LoadCase)
			Me.Case = [case]
		End Sub
	End Class
End Namespace