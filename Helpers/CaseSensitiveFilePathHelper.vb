Imports System.Runtime.InteropServices

Namespace kCura.WinEDDS.Helpers
	Public Class CaseSensitiveFilePathHelper
		Implements IFilePathHelper

		Private ReadOnly _systemIoWrapper As ISystemIoFileWrapper

		Public Sub New(systemIoWrapper As ISystemIoFileWrapper)
			_systemIoWrapper = systemIoWrapper
		End Sub

		Public Function GetExistingFilePath(path As String) As String Implements IFilePathHelper.GetExistingFilePath
			If String.IsNullOrEmpty(path) Then
				Throw New ArgumentException("Path cannot be empty", NameOf(path))
			End If

			If _systemIoWrapper.Exists(path)
				Return path
			End If

			Return TryToSearchInCaseSensitivePaths(path)
		End Function

		Protected Overridable Function TryToSearchInCaseSensitivePaths(path As String) As String
			Dim extension As String = _systemIoWrapper.GetExtension(path)
			
			If String.IsNullOrEmpty(extension)
				Return Nothing
			End If

			Dim foundFileLocation As String = Nothing

			If (Not IsExtensionLowercase(extension)) AndAlso FileExistsWithNewExtension(path, extension.ToLower(), foundFileLocation)
				Return foundFileLocation
			End If

			If (Not IsExtensionUppercase(extension)) AndAlso FileExistsWithNewExtension(path, extension.ToUpper(), foundFileLocation)
				Return foundFileLocation
			End If

			Return Nothing
		End Function

		Private Function FileExistsWithNewExtension(path As String, extension As String, <Out> ByRef updatedPath As String) As Boolean
			updatedPath = _systemIoWrapper.ChangeExtension(path, extension)

			Return _systemIoWrapper.Exists(updatedPath)
		End Function

		Private Function IsExtensionLowercase(extension As String) As Boolean
			Return extension.Equals(extension.ToLower())
		End Function

		Private Function IsExtensionUppercase(extension As String) As Boolean
			Return extension.Equals(extension.ToUpper())
		End Function
	End Class
End NameSpace