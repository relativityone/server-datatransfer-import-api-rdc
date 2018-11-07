Imports System.Collections.Generic
Imports FileNaming.CustomFileNaming

Namespace kCura.WinEDDS.FileNaming.CustomFileNaming
	Public Class NativeFileNameViewFieldsHelper
		Implements INativeFileNameViewFieldsHelper

		Private Function FilterFields(exportFile As ExportFile, fieldsIds As List(Of Integer)) As ViewFieldInfo() Implements INativeFileNameViewFieldsHelper.FilterFields
			Return _
				exportFile.AllExportableFields.Where(Function(x) fieldsIds.Any(Function(fieldId) fieldId = x.FieldArtifactId)).ToArray().OrderBy(
					Function(x)
						Dim index As Integer = fieldsIds.IndexOf(x.FieldArtifactId)
						Return If(index < 0, Integer.MaxValue, index)
					End Function).ToArray()
		End Function


		Public Sub PopulateNativeFileNameViewFields(ExportSettings As ExtendedExportFile) Implements INativeFileNameViewFieldsHelper.PopulateNativeFileNameViewFields
			Dim fieldDescriptorParts As IEnumerable(Of FieldDescriptorPart) = If(ExportSettings.CustomFileNaming.DescriptorParts IsNot Nothing, ExportSettings.CustomFileNaming.DescriptorParts.OfType(Of FieldDescriptorPart)(), Enumerable.Empty(Of FieldDescriptorPart)())

			If fieldDescriptorParts.Any() Then
				ExportSettings.SelectedNativesNameViewFields = FilterFields(ExportSettings, fieldDescriptorParts.[Select](Function(item) item.Value).ToList()).ToList()
			End If
		End Sub

	End Class
End Namespace