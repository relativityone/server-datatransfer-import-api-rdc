Imports System.Collections.Generic
Imports FileNaming.CustomFileNaming

Namespace kCura.WinEDDS.FileNaming.CustomFileNaming
	Public Class NativeFileNameViewFieldsHelper
		Implements INativeFileNameViewFieldsHelper

		Private Function FilterAndSortFields(exportFile As ExportFile, fieldsIds As List(Of Integer)) As ViewFieldInfo() _
			Implements INativeFileNameViewFieldsHelper.FilterAndSortFields
			Return _
				exportFile.AllExportableFields.Where(Function(x) fieldsIds.Contains(x.FieldArtifactId)).OrderBy(
					Function(x)
						Dim index As Integer = fieldsIds.IndexOf(x.FieldArtifactId)
						Return index
					End Function).ToArray()
		End Function


		Public Sub PopulateNativeFileNameViewFields(ExportSettings As ExtendedExportFile) _
			Implements INativeFileNameViewFieldsHelper.PopulateNativeFileNameViewFields
			If ExportSettings.CustomFileNaming Is Nothing OrElse ExportSettings.CustomFileNaming.DescriptorParts Is Nothing
				Return
			End If

			Dim fieldDescriptorParts As IEnumerable(Of FieldDescriptorPart) =
				ExportSettings.CustomFileNaming.DescriptorParts.OfType (Of FieldDescriptorPart)()

			ExportSettings.SelectedNativesNameViewFields =
				FilterAndSortFields(ExportSettings, fieldDescriptorParts.[Select](Function(item) item.Value).ToList()).ToList()
		End Sub
	End Class
End Namespace