Namespace kCura.WinEDDS.Helpers
	Public Class CaseInsensitiveFilePathHelper
		Inherits CaseSensitiveFilePathHelper

		Public Sub New(systemIoWrapper As ISystemIoFileWrapper)
			MyBase.New(systemIoWrapper)
		End Sub

		Protected Overrides Function TryToSearchInCaseSensitivePaths(path As String) As String
			Return Nothing
		End Function
	End Class
End NameSpace