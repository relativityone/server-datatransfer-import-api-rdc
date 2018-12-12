Namespace kCura.WinEDDS.Helpers
	Public Interface ISystemIoFileWrapper
		Function Exists(path As String) As Boolean
		Function ChangeExtension(path As String, extension As String) As String
		Function GetExtension(path As String) As String
	End Interface
End NameSpace