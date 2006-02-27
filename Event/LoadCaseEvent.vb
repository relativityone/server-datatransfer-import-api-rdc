Namespace kCura.WinEDDS
	Public Class LoadCaseEvent
		Inherits kCura.WinEDDS.AppEvent
		Public [Case] As kCura.EDDS.Types.CaseInfo
		Public Sub New(ByVal [case] As kCura.EDDS.Types.CaseInfo)
			MyBase.New(AppEvent.AppEventType.LoadCase)
			Me.Case = [case]
		End Sub
	End Class
End Namespace