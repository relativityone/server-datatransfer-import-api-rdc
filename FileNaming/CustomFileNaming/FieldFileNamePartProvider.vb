Imports System.Collections.Concurrent
Imports FileNaming.CustomFileNaming
Imports kCura.WinEDDS.Exporters
Imports kCura.WinEDDS.Helpers

Namespace kCura.WinEDDS.FileNaming.CustomFileNaming
	Public Class FieldFileNamePartProvider
		Inherits FileNamePartProvider(Of FieldDescriptorPart)

		Private ReadOnly _cache As ConcurrentDictionary(Of Integer, ViewFieldInfo) = New ConcurrentDictionary(Of Integer, ViewFieldInfo)()

		Public Overrides Function GetPartName(descriptorPart As FieldDescriptorPart, exportObject As ObjectExportInfo) As String
			Dim extExportObject As ExtendedObjectExportInfo = TryCast(exportObject, ExtendedObjectExportInfo)
			Dim viewFieldInfo As ViewFieldInfo = GetViewField(descriptorPart, extExportObject)
			Dim fieldValue As String = ConvertToString(extExportObject.GetFieldValue(viewFieldInfo.AvfColumnName), viewFieldInfo, " "c)
			If viewFieldInfo.FieldType = Relativity.FieldTypeHelper.FieldType.Boolean
				GetYesNoFieldStatus(viewFieldInfo, fieldValue)
			End If
			Return kCura.Utility.File.Instance.ConvertIllegalCharactersInFilename(fieldValue)
		End Function

		Private Sub GetYesNoFieldStatus(viewFieldInfo As ViewFieldInfo, ByRef fieldValue As String)
			If fieldValue = "True"
				fieldValue = viewFieldInfo.DisplayName
			Else
				fieldValue = ""
			End If
		End Sub

		Private Function GetViewField(descriptorPart As FieldDescriptorPart, exportObject As ExtendedObjectExportInfo) As ViewFieldInfo
			If Not _cache.ContainsKey(descriptorPart.Value) Then
				Dim foundViewField As ViewFieldInfo = exportObject.SelectedNativeFileNameViewFields.ToList().
						Find(Function(item) item.FieldArtifactId = descriptorPart.Value)
				If (foundViewField Is Nothing) Then
					Throw New ArgumentOutOfRangeException($"Can not find field id: {descriptorPart.Value} in selection list!")
				End If
				_cache(descriptorPart.Value) = foundViewField
			End If
			Return _cache(descriptorPart.Value)
		End Function
	End Class
End Namespace
