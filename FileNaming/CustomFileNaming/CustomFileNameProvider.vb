Imports System.Collections.Generic
Imports System.IO
Imports System.Text
Imports FileNaming.CustomFileNaming
Imports kCura.WinEDDS.Exporters

Namespace kCura.WinEDDS.FileNaming.CustomFileNaming
	Public Class CustomFileNameProvider
		Implements IFileNameProvider

		Private ReadOnly _fileNamePartDescriptors As List(Of DescriptorPart)
		Private ReadOnly _fileNamePartNameContainer As IFileNamePartProviderContainer
		Private ReadOnly _appendOriginalFileName As Boolean

		Public Sub New(fileNamePartDescriptors As List(Of DescriptorPart), fileNamePartNameContainer As IFileNamePartProviderContainer, appendOriginalFileName As Boolean)
			_fileNamePartDescriptors = fileNamePartDescriptors
			_fileNamePartNameContainer = fileNamePartNameContainer
			_appendOriginalFileName = appendOriginalFileName
		End Sub

		Public Function GetName(exportObjectInfo As ObjectExportInfo) As String Implements IFileNameProvider.GetName
			Dim name As StringBuilder = CreateFileName(exportObjectInfo)

			If _appendOriginalFileName Then
				name = AppendOriginalFileName(name, exportObjectInfo)
			Else
				name = GetNameWithNativeExtension(name, exportObjectInfo)
			End If
			Return kCura.Utility.File.Instance.ConvertIllegalCharactersInFilename(name.ToString())
		End Function

		Public Function GetTextName(exportObjectInfo As ObjectExportInfo) As String Implements IFileNameProvider.GetTextName
			Dim name As StringBuilder = CreateFileName(exportObjectInfo)
			Dim nameAsString As String
			If _appendOriginalFileName Then
				Dim nameWithOriginalFileNameAppended As String = AppendOriginalFileName(name, exportObjectInfo).ToString()
				nameAsString = GetNameWithTextExtension(nameWithOriginalFileNameAppended)
			Else
				nameAsString = GetNameWithTextExtension(name)
			End If

			Return kCura.Utility.File.Instance.ConvertIllegalCharactersInFilename(nameAsString)
		End Function

		Private Function AppendOriginalFileName(ByRef name As StringBuilder, exportObjectInfo As ObjectExportInfo) As StringBuilder
			Return name.Append("_" & exportObjectInfo.OriginalFileName)
		End Function
		Private Function CreateFileName(objectExportInfo As ObjectExportInfo) As StringBuilder
			Dim name As StringBuilder = New StringBuilder()
			Dim namePartsCount As Integer = CType(((_fileNamePartDescriptors.Count - 1) / 2), Integer)
			name.Append(GetFileNamePartName(_fileNamePartDescriptors(0), objectExportInfo))

			For i As Integer = 0 To namePartsCount - 1
				name.Append(BuildFileNamePart(objectExportInfo, i))
			Next
			Return name
		End Function

		Private Function BuildFileNamePart(objectExportInfo As ObjectExportInfo, partNumber As Integer) As String
			Const numberOfDescriptorsPerPartName As Integer = 2
			Const separatorPositionShift As Integer = 1
			Const textPositionShift As Integer = 2

			Dim separator As String =
					GetFileNamePartName(_fileNamePartDescriptors(numberOfDescriptorsPerPartName * partNumber + separatorPositionShift),
									objectExportInfo)
			Dim text As String =
					GetFileNamePartName(_fileNamePartDescriptors(numberOfDescriptorsPerPartName * partNumber + textPositionShift),
									objectExportInfo)

			Return BuildFileNamePartFromSeparatorAndText(separator, text)
		End Function

		Private Function BuildFileNamePartFromSeparatorAndText(separator As String, text As String) As String
			If text = "" Then
				Return ""
			End If
			Return separator & text
		End Function

		Private Function GetFileNamePartName(descriptorPart As DescriptorPart, exportObjectInfo As ObjectExportInfo) As String
			Dim fileNamePartProvider As IFileNamePartProvider = _fileNamePartNameContainer.GetProvider(descriptorPart)
			Return fileNamePartProvider.GetPartName(descriptorPart, exportObjectInfo)
		End Function


		Private Function GetNameWithNativeExtension(name As StringBuilder, exportObjectInfo As ObjectExportInfo) As StringBuilder
			If Not String.IsNullOrEmpty(exportObjectInfo.NativeExtension) Then
				name.Append($".{exportObjectInfo.NativeExtension}")
			End If
			Return name
		End Function
		
		Private Function GetNameWithTextExtension(name As String) As String
			Const textFileExtension As String = ".txt"
			Return FileNameHelper.AppendExtensionToFileWhenMissing(name, textFileExtension)
		End Function

		Private Function GetNameWithTextExtension(nameBuilder As StringBuilder) As String
			Return GetNameWithTextExtension(nameBuilder.ToString())
		End Function

	End Class
End Namespace