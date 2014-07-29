Namespace kCura.WinEDDS
	Public Class AppEvent

		Private _appEventType As AppEventType

		Public Enum AppEventType
			LoadCase = 0
			ExitApplication = 1
			LogOn = 2
			CaseFolderSelected = 3
			NewFolder = 4
		End Enum

		Public ReadOnly Property EventType() As AppEventType
			Get
				Return _appEventType
			End Get
		End Property

		Public Sub New(ByVal type As AppEventType)
			_appEventType = type
		End Sub

	End Class
End Namespace