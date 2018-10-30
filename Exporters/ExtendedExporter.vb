Imports System.Collections.Generic
Imports kCura.Windows.Process
Imports kCura.WinEDDS.Core.Model.Export
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

		Protected Overrides Function GetAvfIds() As List(Of Integer)
			Dim selectedAvfIds As HashSet(Of Integer) = New HashSet(Of Integer)(MyBase.GetAvfIds())
			selectedAvfIds.UnionWith(ExportSettings.SelectedNativesNameViewFields.[Select](Function(item) item.AvfId))
			Return selectedAvfIds.ToList()
		End Function
	End Class
End Namespace

