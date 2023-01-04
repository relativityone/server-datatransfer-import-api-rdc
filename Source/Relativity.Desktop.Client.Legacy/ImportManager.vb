Imports kCura.WinEDDS
Imports kCura.WinEDDS.Exporters
Imports kCura.WinEDDS.Service
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Export
Imports Relativity.DataExchange.Logger
Imports Relativity.DataExchange.Service

Namespace Relativity.Desktop.Client

	Public Class ImportManager
#Region " Input Validation "

		Private ReadOnly _logger As Relativity.Logging.ILog

		<Obsolete("This constructor is marked for deprecation. Please use the constructor that requires a logger instance.")>
		Public Sub New()
			Me.New(RelativityLogger.Instance)
		End Sub

		Public Sub New(logger As Relativity.Logging.ILog)
			_logger = logger.ThrowIfNull(NameOf(logger))
		End Sub

		Private Function EnsureEncoding(ByVal importOptions As ImportOptions) As Boolean
			Dim determinedEncoding As System.Text.Encoding = kCura.WinEDDS.Utility.DetectEncoding(importOptions.LoadFilePath, True).DeterminedEncoding
			If determinedEncoding Is Nothing Then Return True
			Return (determinedEncoding.Equals(importOptions.SourceFileEncoding))
		End Function

#End Region

#Region " Run Import "

		Friend Async Function RunExport(options As ExportFile) As Task
			Dim exporter As New ExportSearchProcess(New ExportFileFormatterFactory(), New ExportConfig(), Await _application.SetupMetricService(), _application.RunningContext, _logger, AddressOf _application.GetCorrelationId)
			exporter.UserNotificationFactory = Function(e) New EventBackedUserNotification(e)
			exporter.ExportFile = options
			Dim executor As New CommandLineProcessRunner(exporter.Context, Nothing, Nothing)
			_application.StartProcess(exporter)
		End Function

		Friend Sub RunDynamicObjectImport(ByVal importOptions As ImportOptions)
			Dim importer As New Processes.RdcImportLoadFileProcess(_application.SetupMetricService().GetAwaiter().GetResult(), _application.RunningContext, _logger, AddressOf _application.GetCorrelationId)
			importOptions.SelectedNativeLoadFile.SourceFileEncoding = importOptions.SourceFileEncoding
			importOptions.SelectedNativeLoadFile.ExtractedTextFileEncoding = importOptions.ExtractedTextFileEncoding
			importOptions.SelectedNativeLoadFile.SelectedCasePath = importOptions.SelectedCasePath
			importOptions.SelectedNativeLoadFile.CopyFilesToDocumentRepository = importOptions.CopyFilesToDocumentRepository
			importOptions.SelectedNativeLoadFile.DestinationFolderID = importOptions.DestinationFolderID
			importOptions.SelectedNativeLoadFile.StartLineNumber = importOptions.StartLineNumber
			WebApiCredentialSetter.PopulateNativeLoadFile(importOptions.SelectedNativeLoadFile)
			importer.LoadFile = importOptions.SelectedNativeLoadFile
			importer.TimeZoneOffset = _application.TimeZoneOffset
			importer.BulkLoadFileFieldDelimiter = Config.BulkLoadFileFieldDelimiter
			importer.CloudInstance = Config.CloudInstance
			_application.SetWorkingDirectory(importOptions.SelectedNativeLoadFile.FilePath)
			Dim executor As New CommandLineProcessRunner(importer.Context, importOptions.ErrorLoadFileLocation, importOptions.ErrorReportFileLocation)
			_application.StartProcess(importer)
		End Sub

		Friend Async Function RunNativeImport(ByVal importOptions As ImportOptions) As Task
			If EnsureEncoding(importOptions) Then
				Dim folderManager As kCura.WinEDDS.Service.Replacement.IFolderManager = ManagerFactory.CreateFolderManager(Await _application.GetCredentialsAsync(), _application.CookieContainer, AddressOf _application.GetCorrelationId)
				If folderManager.Exists(importOptions.SelectedCaseInfo.ArtifactID, importOptions.SelectedCaseInfo.RootFolderID) Then
					Dim importer As New Processes.RdcImportLoadFileProcess(Await _application.SetupMetricService(), _application.RunningContext, _logger, AddressOf _application.GetCorrelationId)
					WebApiCredentialSetter.PopulateNativeLoadFile(importOptions.SelectedNativeLoadFile)
					importOptions.SelectedNativeLoadFile.SourceFileEncoding = importOptions.SourceFileEncoding
					importOptions.SelectedNativeLoadFile.ExtractedTextFileEncoding = importOptions.ExtractedTextFileEncoding
					importOptions.SelectedNativeLoadFile.SelectedCasePath = importOptions.SelectedCasePath
					importOptions.SelectedNativeLoadFile.CopyFilesToDocumentRepository = importOptions.CopyFilesToDocumentRepository
					importOptions.SelectedNativeLoadFile.DestinationFolderID = importOptions.DestinationFolderID
					importOptions.SelectedNativeLoadFile.StartLineNumber = importOptions.StartLineNumber
					importer.LoadFile = importOptions.SelectedNativeLoadFile
					importer.TimeZoneOffset = _application.TimeZoneOffset
					importer.BulkLoadFileFieldDelimiter = Config.BulkLoadFileFieldDelimiter
					importer.CloudInstance = Config.CloudInstance
					importOptions.SelectedNativeLoadFile.ArtifactTypeID = ArtifactType.Document
					_application.SetWorkingDirectory(importOptions.SelectedNativeLoadFile.FilePath)
					Dim executor As New CommandLineProcessRunner(importer.Context, importOptions.ErrorLoadFileLocation, importOptions.ErrorReportFileLocation)
					_application.StartProcess(importer)
				End If
			Else
				Throw New EncodingMisMatchException(importOptions.SourceFileEncoding.CodePage, kCura.WinEDDS.Utility.DetectEncoding(importOptions.LoadFilePath, True).DeterminedEncoding.CodePage)
			End If
		End Function

		Friend Async Function RunImageImport(ByVal importOptions As ImportOptions) As Task
			If EnsureEncoding(importOptions) Then
				Dim importer As New Processes.RdcImportImageFileProcess(Await _application.SetupMetricService(), _application.RunningContext, _logger, AddressOf _application.GetCorrelationId)
				importOptions.SelectedImageLoadFile.CookieContainer = _application.CookieContainer
				importOptions.SelectedImageLoadFile.Credential = Await _application.GetCredentialsAsync()
				WebApiCredentialSetter.PopulateImageLoadFile(importOptions.SelectedImageLoadFile)
				importOptions.SelectedImageLoadFile.SelectedCasePath = importOptions.SelectedCasePath
				importOptions.SelectedImageLoadFile.CaseDefaultPath = importOptions.SelectedCaseInfo.DocumentPath
				importOptions.SelectedImageLoadFile.CopyFilesToDocumentRepository = importOptions.CopyFilesToDocumentRepository
				importOptions.SelectedImageLoadFile.FullTextEncoding = importOptions.ExtractedTextFileEncoding
				importOptions.SelectedImageLoadFile.StartLineNumber = importOptions.StartLineNumber
				importer.ImageLoadFile = importOptions.SelectedImageLoadFile
				importer.CloudInstance = Config.CloudInstance
				_application.SetWorkingDirectory(importOptions.SelectedImageLoadFile.FileName)
				Dim executor As New CommandLineProcessRunner(importer.Context, importOptions.ErrorLoadFileLocation, importOptions.ErrorReportFileLocation)
				_application.StartProcess(importer)
			Else
				Throw New EncodingMisMatchException(importOptions.SourceFileEncoding.CodePage, kCura.WinEDDS.Utility.DetectEncoding(importOptions.LoadFilePath, True).DeterminedEncoding.CodePage)
			End If
		End Function

#End Region

	End Class

End Namespace