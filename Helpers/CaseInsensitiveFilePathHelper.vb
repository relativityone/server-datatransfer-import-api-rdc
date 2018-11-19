Namespace kCura.WinEDDS.Helpers
	Public Class CaseInsensitiveFilePathHelper
		Implements IFilePathHelper

		Private ReadOnly _systemIoWrapper As ISystemIoFileWrapper

		Public Sub New(systemIoWrapper As ISystemIoFileWrapper)
			_systemIoWrapper = systemIoWrapper
		End Sub

		Public Function GetExistingFilePath(path As String) As String Implements IFilePathHelper.GetExistingFilePath
			If String.IsNullOrEmpty(path) Then
				Return Nothing
			End If

			Try
				Return If(_systemIoWrapper.Exists(path), path, Nothing)
			Catch ex As Exception
				Return Nothing
			End Try
		End Function
	End Class
End NameSpace