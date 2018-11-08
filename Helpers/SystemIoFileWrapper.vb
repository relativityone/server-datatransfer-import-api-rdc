Imports System.Windows.Forms

Namespace kCura.WinEDDS.Helpers
	Public Class SystemIoFileWrapper
		Implements ISystemIoFileWrapper

		Public Function Exists(path As String) As Boolean Implements ISystemIoFileWrapper.Exists
			Try
				Dim fileInfo As System.IO.FileInfo = new System.IO.FileInfo(path)
				System.IO.File.Exists(path)
				Return fileInfo.Exists
			Catch ex As Exception
				Return False
			End Try
		End Function

		Public Function ChangeExtension(path As String, extension As String) As String Implements ISystemIoFileWrapper.ChangeExtension
			Return System.IO.Path.ChangeExtension(path, extension)
		End Function

		Public Function GetExtension(path As String) As String Implements ISystemIoFileWrapper.GetExtension
			Return System.IO.Path.GetExtension(path)
		End Function
	End Class
End NameSpace