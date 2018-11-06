Imports System.Collections.Generic
Imports FileNaming.CustomFileNaming
Imports kCura.Windows.Process
Imports kCura.WinEDDS.Exporters
Imports kCura.WinEDDS.FileNaming.CustomFileNaming
Imports kCura.WinEDDS.Service.Export

Namespace kCura.WinEDDS
	Public Class ExtendedExporter
		Inherits Exporter
#Region "Members"
		Private ExportSettings As ExtendedExportFile = TryCast(MyBase.Settings, ExtendedExportFile)

#End Region
#Region "Constructors"
		Public Sub New(exportFile As ExtendedExportFile, processController As Controller, loadFileFormatterFactory As ILoadFileHeaderFormatterFactory)
			MyBase.New(exportFile, processController, loadFileFormatterFactory)
		End Sub
		Public Sub New(exportFile As ExtendedExportFile, processController As Controller, serviceFactory As IServiceFactory, loadFileFormatterFactory As ILoadFileHeaderFormatterFactory, exportConfig As IExportConfig)
			MyBase.New(exportFile, processController, serviceFactory, loadFileFormatterFactory, exportConfig)
		End Sub
#End Region

		Protected Overrides Function CreateObjectExportInfo() As ObjectExportInfo
			Return New ExtendedObjectExportInfo(FieldLookupService) With
				{.SelectedNativeFileNameViewFields = ExportSettings.SelectedNativesNameViewFields.ToList()}
		End Function

		Private Shared Function FilterFields(exportFile As ExportFile, fieldsIds As List(Of Integer)) As ViewFieldInfo()
			Return _
				exportFile.AllExportableFields.Where(Function(x) fieldsIds.Any(Function(fieldId) fieldId = x.FieldArtifactId)).ToArray().OrderBy(
					Function(x)
						Dim index As Integer = fieldsIds.IndexOf(x.FieldArtifactId)
						Return If(index < 0, Integer.MaxValue, index)
					End Function).ToArray()
		End Function


		Private Sub PopulateNativeFileNameViewFields()
			Dim fieldDescriptorParts As IEnumerable(Of FieldDescriptorPart) = If(ExportSettings.CustomFileNaming.DescriptorParts IsNot Nothing, ExportSettings.CustomFileNaming.DescriptorParts.OfType(Of FieldDescriptorPart)(), Enumerable.Empty(Of FieldDescriptorPart)())

			If fieldDescriptorParts.Any() Then
				ExportSettings.SelectedNativesNameViewFields = FilterFields(ExportSettings, fieldDescriptorParts.[Select](Function(item) item.Value).ToList()).ToList()
			End If
		End Sub


		Protected Overrides Function GetAvfIds() As List(Of Integer)
			PopulateNativeFileNameViewFields()
			Dim selectedAvfIds As HashSet(Of Integer) = New HashSet(Of Integer)(MyBase.GetAvfIds())
			selectedAvfIds.UnionWith(ExportSettings.SelectedNativesNameViewFields.[Select](Function(item) item.AvfId))
			Return selectedAvfIds.ToList()
		End Function
	End Class
End Namespace

