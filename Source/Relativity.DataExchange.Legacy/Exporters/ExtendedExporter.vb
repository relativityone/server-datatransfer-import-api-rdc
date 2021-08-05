Imports System.Collections.Generic
Imports System.Threading
Imports kCura.WinEDDS.Exporters
Imports kCura.WinEDDS.FileNaming.CustomFileNaming
Imports kCura.WinEDDS.Service.Export
Imports Relativity.DataExchange.Process
Imports Relativity.Logging

Namespace kCura.WinEDDS
	Public Class ExtendedExporter
		Inherits Exporter

		Private _exportSettings As ExtendedExportFile = TryCast(MyBase.Settings, ExtendedExportFile)
		Private ReadOnly _nativeFileNameViewFieldHelper As INativeFileNameViewFieldsHelper = New NativeFileNameViewFieldsHelper()

		<Obsolete("This constructor is marked for deprecation. Please use the constructor that requires a logger instance.")>
		Public Sub New(exportFile As ExtendedExportFile, context As ProcessContext, loadFileFormatterFactory As ILoadFileHeaderFormatterFactory, correlationIdFunc As Func(Of String))
			MyBase.New(exportFile, context, loadFileFormatterFactory, correlationIdFunc)
		End Sub

		<Obsolete("This constructor is marked for deprecation. Please use the constructor that requires a logger instance.")>
		Public Sub New(exportFile As ExtendedExportFile, context As ProcessContext, serviceFactory As IServiceFactory, loadFileFormatterFactory As ILoadFileHeaderFormatterFactory, exportConfig As IExportConfig, correlationIdFunc As Func(Of String))
			MyBase.New(exportFile, context, serviceFactory, loadFileFormatterFactory, exportConfig, correlationIdFunc)
		End Sub

		Public Sub New(exportFile As ExtendedExportFile, context As ProcessContext, serviceFactory As IServiceFactory, loadFileFormatterFactory As ILoadFileHeaderFormatterFactory, exportConfig As IExportConfig, logger As ILog, cancellationToken As CancellationToken, correlationIdFunc As Func(Of String))
			MyBase.New(exportFile, context, serviceFactory, loadFileFormatterFactory, exportConfig, logger, cancellationToken, correlationIdFunc)
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

