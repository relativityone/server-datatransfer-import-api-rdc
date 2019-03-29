﻿Imports System.Collections.Generic
Imports kCura.WinEDDS.Exporters
Imports kCura.WinEDDS.FileNaming.CustomFileNaming
Imports kCura.WinEDDS.Service.Export
Imports Relativity.Import.Export.Process

Namespace kCura.WinEDDS
	Public Class ExtendedExporter
		Inherits Exporter

		Private _exportSettings As ExtendedExportFile = TryCast(MyBase.Settings, ExtendedExportFile)
		Private ReadOnly _nativeFileNameViewFieldHelper As INativeFileNameViewFieldsHelper = New NativeFileNameViewFieldsHelper()


		Public Sub New(exportFile As ExtendedExportFile, context As ProcessContext, loadFileFormatterFactory As ILoadFileHeaderFormatterFactory)
			MyBase.New(exportFile, context, loadFileFormatterFactory)
		End Sub
		Public Sub New(exportFile As ExtendedExportFile, context As ProcessContext, serviceFactory As IServiceFactory, loadFileFormatterFactory As ILoadFileHeaderFormatterFactory, exportConfig As IExportConfig)
			MyBase.New(exportFile, context, serviceFactory, loadFileFormatterFactory, exportConfig)
		End Sub


		Protected Overrides Function CreateObjectExportInfo() As ObjectExportInfo
			Return New ExtendedObjectExportInfo(FieldLookupService) With
				{.SelectedNativeFileNameViewFields = _exportSettings.SelectedNativesNameViewFields.ToList()}
		End Function

		Protected Overrides Function GetAvfIds() As List(Of Integer)
			_nativeFileNameViewFieldHelper.PopulateNativeFileNameViewFields(_exportSettings)
			Dim selectedAvfIds As HashSet(Of Integer) = New HashSet(Of Integer)(MyBase.GetAvfIds())
			selectedAvfIds.UnionWith(_exportSettings.SelectedNativesNameViewFields.[Select](Function(item) item.AvfId))
			Return selectedAvfIds.ToList()
		End Function
	End Class
End Namespace

