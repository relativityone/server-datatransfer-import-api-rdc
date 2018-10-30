Imports System.Collections.Generic
Imports System.Text
Imports FileNaming.CustomFileNaming
Imports kCura.WinEDDS.Exporters

Namespace kCura.WinEDDS.FileNaming.CustomFileNaming
	Public Class CustomFileNameProvider
		Implements IFileNameProvider

		Private ReadOnly _fileNamePartDescriptors As List(Of DescriptorPart)
		Private ReadOnly _fileNamePartNameContainer As IFileNamePartProviderContainer

		Public Sub New(fileNamePartDescriptors As List(Of DescriptorPart), fileNamePartNameContainer As IFileNamePartProviderContainer)
			_fileNamePartDescriptors = fileNamePartDescriptors
			_fileNamePartNameContainer = fileNamePartNameContainer
		End Sub

		Public Function GetName(exportObjectInfo As ObjectExportInfo) As String Implements IFileNameProvider.GetName
			Dim name As StringBuilder = CreateFileName(exportObjectInfo)
			Return GetNameWithNativeExtension(name, exportObjectInfo)
		End Function

		Private Function CreateFileName(exportObjectInfo As ObjectExportInfo) As StringBuilder
			Dim name As StringBuilder = New StringBuilder()
			For Each descriptor As DescriptorPart In _fileNamePartDescriptors
				Dim fileNamePartProvider As IFileNamePartProvider = _fileNamePartNameContainer.GetProvider(descriptor)
				name.Append(fileNamePartProvider.GetPartName(descriptor, exportObjectInfo))
			Next
			Return name
		End Function

		Private Function GetNameWithNativeExtension(name As StringBuilder, exportObjectInfo As ObjectExportInfo) As String
			If Not String.IsNullOrEmpty(exportObjectInfo.NativeExtension) Then
				name.Append($".{exportObjectInfo.NativeExtension}")
			End If
			Return name.ToString()
		End Function

		Public Function GetTextName(exportedObjectInfo As ObjectExportInfo) As String Implements IFileNameProvider.GetTextName
			Dim name As StringBuilder = CreateFileName(exportedObjectInfo)
			Return GetNameWithTextExtension(name)
		End Function

		Private Function GetNameWithTextExtension(name As StringBuilder) As String
			name.Append(".txt")
			Return name.ToString()
		End Function

	End Class
End Namespace