Namespace kCura.WinEDDS.Helpers
	Public Class SystemIoFileWrapper
		Implements ISystemIoFileWrapper

		Public Function Exists(path As String) As Boolean Implements ISystemIoFileWrapper.Exists
			Return System.IO.File.Exists(path)
		End Function

		Public Function ChangeExtension(path As String, extension As String) As String Implements ISystemIoFileWrapper.ChangeExtension
			Return System.IO.Path.ChangeExtension(path, extension)
		End Function

		Public Function GetExtension(path As String) As String Implements ISystemIoFileWrapper.GetExtension
			Return System.IO.Path.GetExtension(path)
		End Function
	End Class
End NameSpace