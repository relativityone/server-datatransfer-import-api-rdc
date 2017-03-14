Imports System.Collections.Concurrent
Imports System.IO
Imports System.Collections.Generic
Imports System.Configuration
Imports System.Threading
Imports kCura.EDDS.WebAPI.ExportManagerBase
Imports System.Web.Services.Protocols
Imports kCura.Utility.Extensions
Imports kCura.WinEDDS.Exporters
Imports System.Threading.Tasks

Namespace kCura.WinEDDS
	Public Class Exporter
		Implements IExporterStatusNotification
		Implements IExporter

#Region "Members"

		Private _loadFileFormatterFactory As ILoadFileHeaderFormatterFactory
		Private _fieldProviderCache As IFieldProviderCache
		Private _searchManager As Service.Export.ISearchManager
		Private _productionManager As Service.Export.IProductionManager
		Private _auditManager As Service.Export.IAuditManager
		Private _fieldManager As Service.Export.IFieldManager
		Public Property ExportManager As Service.Export.IExportManager
		Private _exportFile As kCura.WinEDDS.ExportFile
		Private _columns As System.Collections.ArrayList
		Public TotalExportArtifactCount As Int32
		Private WithEvents _processController As kCura.Windows.Process.Controller
		Private WithEvents _downloadHandler As Service.Export.IExportFileDownloader
		Private _halt As Boolean
		Private _volumeManager As VolumeManager
		Private _exportNativesToFileNamedFrom As kCura.WinEDDS.ExportNativeWithFilenameFrom
		Private _beginBatesColumn As String = ""
		Private _timekeeper As New kCura.Utility.Timekeeper
		Private _productionArtifactIDs As Int32()
		Private _lastStatusMessageTs As Long = System.DateTime.Now.Ticks
		Private _lastDocumentsExportedCountReported As Int32 = 0
		Private _statistics As New kCura.WinEDDS.ExportStatistics
		Private _lastStatisticsSnapshot As IDictionary
		Private _start As System.DateTime
		Private _warningCount As Int32 = 0
		Private _errorCount As Int32 = 0
		Private _fileCount As Int64 = 0
		Private _productionExportProduction As kCura.EDDS.WebAPI.ProductionManagerBase.ProductionInfo
		Private _productionLookup As New System.Collections.Generic.Dictionary(Of Int32, kCura.EDDS.WebAPI.ProductionManagerBase.ProductionInfo)
		Private _productionPrecedenceIds As Int32()
		Private _tryToNameNativesAndTextFilesAfterPrecedenceBegBates As Boolean = False
		Private _linesToWriteDat As ConcurrentDictionary(Of Int32, String)
		Private _linesToWriteOpt As ConcurrentDictionary(Of String, String)

#End Region

#Region "Accessors"
		''' <summary>
		''' This is not moved over to the Settings object yet
		''' </summary>
		''' <returns></returns>
		Public Property NameTextAndNativesAfterBegBates() As Boolean = False

		Public Property Settings() As kCura.WinEDDS.ExportFile
			Get
				Return _exportFile
			End Get
			Set(ByVal value As kCura.WinEDDS.ExportFile)
				_exportFile = value
			End Set
		End Property

		Public Property Columns() As System.Collections.ArrayList
			Get
				Return _columns
			End Get
			Set(ByVal value As System.Collections.ArrayList)
				_columns = value
			End Set
		End Property

		Public Property ExportNativesToFileNamedFrom() As kCura.WinEDDS.ExportNativeWithFilenameFrom
			Get
				Return _exportNativesToFileNamedFrom
			End Get
			Set(ByVal value As kCura.WinEDDS.ExportNativeWithFilenameFrom)
				_exportNativesToFileNamedFrom = value
			End Set
		End Property

		Public ReadOnly Property ErrorLogFileName() As String
			Get
				If Not _volumeManager Is Nothing Then
					Return _volumeManager.ErrorLogFileName
				Else
					Return Nothing
				End If
			End Get
		End Property

		Protected Overridable ReadOnly Property NumberOfRetries() As Int32
			Get
				Return kCura.Utility.Config.ExportErrorNumberOfRetries
			End Get
		End Property

		Protected Overridable ReadOnly Property WaitTimeBetweenRetryAttempts() As Int32
			Get
				Return kCura.Utility.Config.ExportErrorWaitTimeInSeconds
			End Get
		End Property

#End Region

		Public Event ShutdownEvent()
		Public Sub Shutdown()
			RaiseEvent ShutdownEvent()
		End Sub

#Region "Constructors"

		Public Sub New(ByVal exportFile As kCura.WinEDDS.ExportFile, ByVal processController As kCura.Windows.Process.Controller, loadFileFormatterFactory As ILoadFileHeaderFormatterFactory)
			Me.New(exportFile, processController, New Service.Export.WebApiServiceFactory(exportFile), loadFileFormatterFactory)
		End Sub

		Public Sub New(ByVal exportFile As kCura.WinEDDS.ExportFile, ByVal processController As kCura.Windows.Process.Controller, serviceFactory As Service.Export.IServiceFactory,
					   loadFileFormatterFactory As ILoadFileHeaderFormatterFactory)
			_searchManager = serviceFactory.CreateSearchManager()
			_downloadHandler = serviceFactory.CreateExportFileDownloader()
			_productionManager = serviceFactory.CreateProductionManager()
			_auditManager = serviceFactory.CreateAuditManager()
			_fieldManager = serviceFactory.CreateFieldManager()
			ExportManager = serviceFactory.CreateExportManager()

			_fieldProviderCache = New FieldProviderCache(exportFile.Credential, exportFile.CookieContainer)
			_halt = False
			_processController = processController
			DocumentsExported = 0
			TotalExportArtifactCount = 1
			Settings = exportFile
			Settings.FolderPath = Me.Settings.FolderPath + "\"
			ExportNativesToFileNamedFrom = exportFile.ExportNativesToFileNamedFrom
			_loadFileFormatterFactory = loadFileFormatterFactory
		End Sub

#End Region

		Public Property DocumentsExported As Integer Implements IExporter.DocumentsExported
		Public Property InteractionManager As Exporters.IUserNotification = New Exporters.NullUserNotification Implements IExporter.InteractionManager

		Public Function ExportSearch() As Boolean Implements IExporter.ExportSearch
			Try
				_start = System.DateTime.Now
				Me.Search()
			Catch ex As System.Exception
				Me.WriteFatalError(String.Format("A fatal error occurred on document #{0}", Me.DocumentsExported), ex)
				If Not _volumeManager Is Nothing Then
					_volumeManager.Close()
				End If
			End Try
			Return Me.ErrorLogFileName = ""
		End Function

		Protected Overridable Function GetHeaderColName(fieldInfo As ViewFieldInfo) As String
			return fieldInfo.DisplayName
		End Function


		Private Function IsExtractedTextSelected() As Boolean
			For Each vfi As ViewFieldInfo In Me.Settings.SelectedViewFields
				If vfi.Category = Relativity.FieldCategory.FullText Then Return True
			Next
			Return False
		End Function

		Private Function ExtractedTextField() As ViewFieldInfo
			For Each v As ViewFieldInfo In Me.Settings.AllExportableFields
				If v.Category = Relativity.FieldCategory.FullText Then Return v
			Next
			Throw New System.Exception("Full text field somehow not in all fields")
		End Function

		Private Sub InitializeExportProcess()
			_productionPrecedenceIds = (From p In If(Settings.ImagePrecedence, New Pair() {}) Where Not String.IsNullOrEmpty(p.Value) Select CInt(p.Value)).ToArray()
			_tryToNameNativesAndTextFilesAfterPrecedenceBegBates = ShouldTextAndNativesBeNamedAfterPrecedenceBegBates()
			InitializeFieldIndentifier()
		End Sub

		Private Function Search() As Boolean
			InitializeExportProcess()
			Dim tries As Int32 = 0
			Dim maxTries As Int32 = NumberOfRetries + 1

			Dim typeOfExportDisplayString As String = ""
			Dim errorOutputFilePath As String = _exportFile.FolderPath & "\" & _exportFile.LoadFilesPrefix & "_img_errors.txt"
			If System.IO.File.Exists(errorOutputFilePath) AndAlso _exportFile.Overwrite Then kCura.Utility.File.Instance.Delete(errorOutputFilePath)
			Me.WriteUpdate("Retrieving export data from the server...")
			Dim startTicks As Int64 = System.DateTime.Now.Ticks
			Dim exportInitializationArgs As kCura.EDDS.WebAPI.ExportManagerBase.InitializationResults = Nothing
			Dim columnHeaderString As String = Me.LoadColumns
			Dim allAvfIds As New List(Of Int32)
			For i As Int32 = 0 To _columns.Count - 1
				If Not TypeOf _columns(i) Is CoalescedTextViewField Then
					allAvfIds.Add(Me.Settings.SelectedViewFields(i).AvfId)
				End If
			Next
			Dim production As kCura.EDDS.WebAPI.ProductionManagerBase.ProductionInfo = Nothing

			If Me.Settings.TypeOfExport = ExportFile.ExportType.Production Then

				tries = 0
				While tries < maxTries
					tries += 1
					Try
						production = _productionManager.Read(Me.Settings.CaseArtifactID, Me.Settings.ArtifactID)
						Exit While
					Catch ex As System.Exception
						If tries < maxTries AndAlso Not (TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("Need To Re Login") <> -1) Then
							Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, "Error occurred, attempting retry number " & tries & ", in " & WaitTimeBetweenRetryAttempts & " seconds...", True)
							System.Threading.Thread.CurrentThread.Join(WaitTimeBetweenRetryAttempts * 1000)
						Else
							Throw
						End If
					End Try
				End While

				_productionExportProduction = production
				With _fieldManager.Read(Me.Settings.CaseArtifactID, production.BeginBatesReflectedFieldId)
					_beginBatesColumn = Relativity.SqlNameHelper.GetSqlFriendlyName(.DisplayName)
					If Not allAvfIds.Contains(.ArtifactViewFieldID) Then allAvfIds.Add(.ArtifactViewFieldID)
				End With
			End If

			If Me.Settings.ExportImages AndAlso Me.Settings.LogFileFormat = LoadFileType.FileFormat.IPRO_FullText Then
				If Not Me.IsExtractedTextSelected Then
					allAvfIds.Add(Me.ExtractedTextField.AvfId)
				End If
			End If
			tries = 0
			Select Case Me.Settings.TypeOfExport
				Case ExportFile.ExportType.ArtifactSearch
					typeOfExportDisplayString = "search"
					exportInitializationArgs = CallServerWithRetry(Function() Me.ExportManager.InitializeSearchExport(_exportFile.CaseInfo.ArtifactID, Me.Settings.ArtifactID, allAvfIds.ToArray, Me.Settings.StartAtDocumentNumber + 1), maxTries)

				Case ExportFile.ExportType.ParentSearch
					typeOfExportDisplayString = "folder"
					exportInitializationArgs = CallServerWithRetry(Function() Me.ExportManager.InitializeFolderExport(Me.Settings.CaseArtifactID, Me.Settings.ViewID, Me.Settings.ArtifactID, False, allAvfIds.ToArray, Me.Settings.StartAtDocumentNumber + 1, Me.Settings.ArtifactTypeID), maxTries)

				Case ExportFile.ExportType.AncestorSearch
					typeOfExportDisplayString = "folder and subfolder"
					exportInitializationArgs = CallServerWithRetry(Function() Me.ExportManager.InitializeFolderExport(Me.Settings.CaseArtifactID, Me.Settings.ViewID, Me.Settings.ArtifactID, True, allAvfIds.ToArray, Me.Settings.StartAtDocumentNumber + 1, Me.Settings.ArtifactTypeID), maxTries)

				Case ExportFile.ExportType.Production
					typeOfExportDisplayString = "production"
					exportInitializationArgs = CallServerWithRetry(Function() Me.ExportManager.InitializeProductionExport(_exportFile.CaseInfo.ArtifactID, Me.Settings.ArtifactID, allAvfIds.ToArray, Me.Settings.StartAtDocumentNumber + 1), maxTries)

			End Select
			Me.TotalExportArtifactCount = CType(exportInitializationArgs.RowCount, Int32)
			If Me.TotalExportArtifactCount - 1 < Me.Settings.StartAtDocumentNumber Then
				Dim msg As String = String.Format("The chosen start item number ({0}) exceeds the number of {2} items in the export ({1}).  Export halted.", Me.Settings.StartAtDocumentNumber + 1, Me.TotalExportArtifactCount, vbNewLine)
				InteractionManager.AlertCriticalError(msg)
				Me.Shutdown()
				Return False
			Else
				Me.TotalExportArtifactCount -= Me.Settings.StartAtDocumentNumber
			End If
			_statistics.MetadataTime += System.Math.Max(System.DateTime.Now.Ticks - startTicks, 1)
			RaiseEvent FileTransferModeChangeEvent(_downloadHandler.UploaderType.ToString)
			_volumeManager = New VolumeManager(Me.Settings, Me.Settings.FolderPath, Me.Settings.Overwrite, Me.TotalExportArtifactCount, Me, _downloadHandler, _timekeeper, exportInitializationArgs.ColumnNames, _statistics)
			Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, "Created search log file.", True)
			_volumeManager.ColumnHeaderString = columnHeaderString
			Me.WriteUpdate("Data retrieved. Beginning " & typeOfExportDisplayString & " export...")

			Dim records As Object() = Nothing
			Dim start, realStart As Int32
			Dim lastRecordCount As Int32 = -1
			While lastRecordCount <> 0
				realStart = start + Me.Settings.StartAtDocumentNumber
				_timekeeper.MarkStart("Exporter_GetDocumentBlock")
				startTicks = System.DateTime.Now.Ticks
				Dim textPrecedenceAvfIds As Int32() = Nothing
				If Not Me.Settings.SelectedTextFields Is Nothing AndAlso Me.Settings.SelectedTextFields.Count > 0 Then textPrecedenceAvfIds = Me.Settings.SelectedTextFields.Select(Of Int32)(Function(f As ViewFieldInfo) f.AvfId).ToArray

				If Me.Settings.TypeOfExport = ExportFile.ExportType.Production Then
					records = CallServerWithRetry(Function() Me.ExportManager.RetrieveResultsBlockForProduction(Me.Settings.CaseInfo.ArtifactID, exportInitializationArgs.RunId, Me.Settings.ArtifactTypeID, allAvfIds.ToArray, Config.ExportBatchSize, Me.Settings.MulticodesAsNested, Me.Settings.MultiRecordDelimiter, Me.Settings.NestedValueDelimiter, textPrecedenceAvfIds, Me.Settings.ArtifactID), maxTries)
				Else
					records = CallServerWithRetry(Function() Me.ExportManager.RetrieveResultsBlock(Me.Settings.CaseInfo.ArtifactID, exportInitializationArgs.RunId, Me.Settings.ArtifactTypeID, allAvfIds.ToArray, Config.ExportBatchSize, Me.Settings.MulticodesAsNested, Me.Settings.MultiRecordDelimiter, Me.Settings.NestedValueDelimiter, textPrecedenceAvfIds), maxTries)
				End If


				If records Is Nothing Then Exit While
				If Me.Settings.TypeOfExport = ExportFile.ExportType.Production AndAlso production IsNot Nothing AndAlso production.DocumentsHaveRedactions Then
					WriteStatusLineWithoutDocCount(kCura.Windows.Process.EventType.Warning, "Please Note - Documents in this production were produced with redactions applied.  Ensure that you have exported text that was generated via OCR of the redacted documents.", True)
				End If
				lastRecordCount = records.Length
				_statistics.MetadataTime += System.Math.Max(System.DateTime.Now.Ticks - startTicks, 1)
				_timekeeper.MarkEnd("Exporter_GetDocumentBlock")
				Dim artifactIDs As New ArrayList
				Dim artifactIdOrdinal As Int32 = _volumeManager.OrdinalLookup("ArtifactID")
				If records.Length > 0 Then
					For Each artifactMetadata As Object() In records
						artifactIDs.Add(artifactMetadata(artifactIdOrdinal))
					Next
					ExportChunk(DirectCast(artifactIDs.ToArray(GetType(Int32)), Int32()), records)
					artifactIDs.Clear()
					records = Nothing
				End If
				If _halt Then Exit While
			End While

			Me.WriteStatusLine(Windows.Process.EventType.Status, kCura.WinEDDS.FileDownloader.TotalWebTime.ToString, True)
			_timekeeper.GenerateCsvReportItemsAsRows()
			_volumeManager.Finish()
			Me.AuditRun(True)
			Return Nothing
		End Function


		Private Function CallServerWithRetry(Of T)(f As Func(Of T), ByVal maxTries As Int32) As T
			Dim tries As Integer
			Dim records As T

			tries = 0
			While tries < maxTries
				tries += 1
				Try
					records = f()
					Exit While
				Catch ex As System.Exception
					If TypeOf (ex) Is System.InvalidOperationException AndAlso ex.Message.Contains("empty response") Then
						Throw New Exception("Communication with the WebAPI server has failed, possibly because values for MaximumLongTextSizeForExportInCell and/or MaximumTextVolumeForExportChunk are too large.  Please lower them and try again.", ex)
					ElseIf tries < maxTries AndAlso Not (TypeOf ex Is System.Web.Services.Protocols.SoapException AndAlso ex.ToString.IndexOf("Need To Re Login") <> -1) Then
						Me.WriteStatusLine(kCura.Windows.Process.EventType.Status, "Error occurred, attempting retry number " & tries & ", in " & WaitTimeBetweenRetryAttempts & " seconds...", True)
						System.Threading.Thread.CurrentThread.Join(WaitTimeBetweenRetryAttempts * 1000)
					Else
						Throw
					End If
				End Try
			End While
			Return records
		End Function

#Region "Private Helper Functions"

		Friend Class BatesEntry
			Friend Property ProductionArtifactID As Int32
			Friend Property BegBates As String
		End Class

		Private Function GenerateBatesLookup(source As Object()()) As Dictionary(Of Int32, List(Of BatesEntry))
			Dim lookup As New Dictionary(Of Int32, List(Of BatesEntry))()
			For Each entry As Object() In source
				Dim documentId As Int32 = CInt(entry(0))
				Dim documentEntries As List(Of BatesEntry)
				If Not lookup.ContainsKey(documentId) Then
					documentEntries = New List(Of BatesEntry)
					lookup.Add(documentId, documentEntries)
				Else
					documentEntries = lookup(documentId)
				End If
				documentEntries.Add(New BatesEntry() With {.BegBates = entry(1).ToString(), .ProductionArtifactID = CInt(entry(2))})
			Next
			Return lookup
		End Function

		Private Sub InitializeFieldIndentifier()

			If String.IsNullOrEmpty(Settings.IdentifierColumnName) Then
				'This case is for Exporter class usage outside of RDC (WinForms project)

				Dim docFieldCollection As DocumentFieldCollection = _fieldProviderCache.CurrentFields(Settings.ArtifactTypeID,
																									  Settings.CaseArtifactID,
																									  True)
				Dim identifierList As String() = docFieldCollection.IdentifierFieldNames()
				If identifierList.IsNullOrEmpty() Then
					Dim errMsg As String = String.Format("Export process failed. Cannot find identifier field for workspace: {0}!", Settings.CaseInfo.Name)
					WriteError(errMsg)
				End If
				Settings.IdentifierColumnName = identifierList(0)
			End If
		End Sub

		Private Sub ExportChunk(ByVal documentArtifactIDs As Int32(), ByVal records As Object())
			Dim tries As Int32 = 0
			Dim maxTries As Int32 = NumberOfRetries + 1
			Dim natives As New System.Data.DataView
			Dim images As New System.Data.DataView
			Dim productionImages As New System.Data.DataView
			Dim i As Int32 = 0
			Dim productionArtifactID As Int32 = 0
	
			If Me.Settings.TypeOfExport = ExportFile.ExportType.Production Then productionArtifactID = Settings.ArtifactID

			Dim retrieveThreads As Task(Of System.Data.DataView)() = New Task(Of System.Data.DataView)(2){}

			retrieveThreads(0) = RetrieveNatives(natives,productionArtifactID,documentArtifactIDs,maxTries)
			retrieveThreads(1) = RetrieveImages(images,documentArtifactIDs,maxTries)
			retrieveThreads(2) = RetrieveProductions(productionImages,documentArtifactIDs,maxTries)

			Task.WaitAll(retrieveThreads)

			natives = retrieveThreads(0).Result()
			images = retrieveThreads(1).Result()
			productionImages = retrieveThreads(2).Result()

			Dim beginBatesColumnIndex As Int32 = -1
			If Me.ExportNativesToFileNamedFrom = ExportNativeWithFilenameFrom.Production AndAlso _volumeManager.OrdinalLookup.ContainsKey(_beginBatesColumn) Then
				beginBatesColumnIndex = _volumeManager.OrdinalLookup(_beginBatesColumn)
			End If
			Dim identifierColumnName As String = Relativity.SqlNameHelper.GetSqlFriendlyName(Me.Settings.IdentifierColumnName)
			Dim identifierColumnIndex As Int32 = _volumeManager.OrdinalLookup(identifierColumnName)
			'TODO: come back to this
			Dim productionPrecedenceArtifactIds As Int32() = Settings.ImagePrecedence.Select(Function(pair) CInt(pair.Value)).ToArray()
			Dim lookup As New Lazy(Of Dictionary(Of Int32, List(Of BatesEntry)))(Function() GenerateBatesLookup(_productionManager.RetrieveBatesByProductionAndDocument(Me.Settings.CaseArtifactID, productionPrecedenceArtifactIds, documentArtifactIDs)))

			_linesToWriteDat = New ConcurrentDictionary(Of Int32, String)
			_linesToWriteOpt = New ConcurrentDictionary(Of String, String)

			Dim artifacts(documentArtifactIDs.Length - 1) As Exporters.ObjectExportInfo

			Dim threadCount As Integer = Config.ExportThreadCount - 1
			Dim threads As Task() = New Task(threadCount) {}
			For i = 0 To threadCount
				threads(i) = Task.FromResult(0)
			Next

			For i = 0 To documentArtifactIDs.Length - 1
				Dim record As Object() = DirectCast(records(i), Object())
				Dim nativeRow As System.Data.DataRowView = GetNativeRow(natives, documentArtifactIDs(i))
				Dim artifact As ObjectExportInfo = CreateArtifact(record, documentArtifactIDs(i), nativeRow, images, productionImages, beginBatesColumnIndex, identifierColumnIndex, lookup)
				artifacts(i) = artifact
			Next

			For i = 0 To documentArtifactIDs.Length - 1
				If _halt Then Exit For
				Dim openThreadNumber As Integer = Task.WaitAny(threads, TimeSpan.FromDays(1))
				threads(openThreadNumber) = ExportArtifactAsync(artifacts(i), maxTries,i,documentArtifactIDs.Length, openThreadNumber)
				DocumentsExported += 1
			Next

			Task.WaitAll(threads)
			_volumeManager.WriteDatFile(_linesToWriteDat, artifacts)
			_volumeManager.WriteOptFile(_linesToWriteOpt, artifacts)

		End Sub

		Private Async Function RetrieveNatives(ByVal natives As System.Data.DataView, ByVal productionArtifactID As Int32, ByVal documentArtifactIDs As Int32(), ByVal maxTries As Integer) As Task(Of System.Data.DataView)
			return Await Task.Run(
					Function() As System.Data.DataView
						If Me.Settings.ExportNative Then
							_timekeeper.MarkStart("Exporter_GetNativesForDocumentBlock")
							Dim start As Int64
							start = System.DateTime.Now.Ticks
							If Me.Settings.TypeOfExport = ExportFile.ExportType.Production Then
								natives.Table = CallServerWithRetry(Function() _searchManager.RetrieveNativesForProduction(Me.Settings.CaseArtifactID, productionArtifactID, kCura.Utility.Array.ToCsv(documentArtifactIDs)).Tables(0), maxTries)
							ElseIf Me.Settings.ArtifactTypeID = Relativity.ArtifactType.Document Then
								natives.Table = CallServerWithRetry(Function() _searchManager.RetrieveNativesForSearch(Me.Settings.CaseArtifactID, kCura.Utility.Array.ToCsv(documentArtifactIDs)).Tables(0), maxTries)
							Else
								Dim dt As System.Data.DataTable = CallServerWithRetry(Function() _searchManager.RetrieveFilesForDynamicObjects(Me.Settings.CaseArtifactID, Me.Settings.FileField.FieldID, documentArtifactIDs).Tables(0), maxTries)
								If dt Is Nothing Then
									natives = Nothing
								Else
									natives.Table = dt
								End If
							End If
							_statistics.MetadataTime += System.Math.Max(System.DateTime.Now.Ticks - start, 1)
							_timekeeper.MarkEnd("Exporter_GetNativesForDocumentBlock")
						End If
						return natives
					End Function
				)
		End Function

		Private Async Function RetrieveImages(ByVal images As System.Data.DataView, ByVal documentArtifactIDs As Int32(), ByVal maxTries As Integer) As Task(Of System.Data.DataView)
			return Await Task.Run(
				Function()
					If Me.Settings.ExportImages Then
						Dim start As Int64
						_timekeeper.MarkStart("Exporter_GetImagesForDocumentBlock")
						start = System.DateTime.Now.Ticks

						images.Table = CallServerWithRetry(Function() Me.RetrieveImagesForDocuments(documentArtifactIDs, Me.Settings.ImagePrecedence), maxTries)

						_statistics.MetadataTime += System.Math.Max(System.DateTime.Now.Ticks - start, 1)
						_timekeeper.MarkEnd("Exporter_GetImagesForDocumentBlock")
					End If
					return images
					End Function
				)
		End Function

		Private Async Function RetrieveProductions(ByVal productionImages As System.Data.DataView, ByVal documentArtifactIDs As Int32(), ByVal maxTries As Integer) As Task(Of System.Data.DataView)
			return Await Task.Run(
				Function()
					If Me.Settings.ExportImages Then
						Dim start As Int64
						_timekeeper.MarkStart("Exporter_GetProductionsForDocumentBlock")
						start = System.DateTime.Now.Ticks

						productionImages.Table = CallServerWithRetry(Function() Me.RetrieveProductionImagesForDocuments(documentArtifactIDs, Me.Settings.ImagePrecedence), maxTries)

						_statistics.MetadataTime += System.Math.Max(System.DateTime.Now.Ticks - start, 1)
						_timekeeper.MarkEnd("Exporter_GetProductionsForDocumentBlock")
					End If
					return productionImages
					End Function
				)
		End Function

		Private Async Function ExportArtifactAsync(ByVal artifact As ObjectExportInfo, ByVal maxTries As Integer, ByVal docNum As Integer , ByVal numDocs As Integer, ByVal threadNumber As Integer) As Task
			 Await Task.Run(
					Sub()
					    _fileCount += CallServerWithRetry(Function()
							Dim retval As Long
							retval = _volumeManager.ExportArtifact(artifact, _linesToWriteDat, _linesToWriteOpt, threadNumber)
							If retval >= 0 Then
								WriteUpdate("Exported document " & docNum + 1, docNum = numDocs - 1)
								_lastStatisticsSnapshot = _statistics.ToDictionary
								Return retval
							Else
								Return 0
							End If
						End Function, maxTries)
					End Sub
				)
		End Function

		Private Function CreateArtifact(ByVal record As Object(), ByVal documentArtifactID As Int32, ByVal nativeRow As System.Data.DataRowView, ByVal images As System.Data.DataView, ByVal productionImages As System.Data.DataView, ByVal beginBatesColumnIndex As Int32, ByVal identifierColumnIndex As Int32, ByRef lookup As Lazy(Of Dictionary(Of Int32, List(Of BatesEntry)))) As Exporters.ObjectExportInfo
			Dim artifact As New Exporters.ObjectExportInfo
			If Me.ExportNativesToFileNamedFrom = ExportNativeWithFilenameFrom.Production AndAlso beginBatesColumnIndex <> -1 Then
				artifact.ProductionBeginBates = record(beginBatesColumnIndex).ToString
			End If
			artifact.IdentifierValue = record(identifierColumnIndex).ToString
			artifact.Images = Me.PrepareImages(images, productionImages, documentArtifactID, artifact.IdentifierValue, artifact, Me.Settings.ImagePrecedence)
			If nativeRow Is Nothing Then
				artifact.NativeFileGuid = ""
				artifact.OriginalFileName = ""
				artifact.NativeSourceLocation = ""
			Else
				artifact.OriginalFileName = nativeRow("Filename").ToString
				artifact.NativeSourceLocation = nativeRow("Location").ToString
				If Me.Settings.ArtifactTypeID = Relativity.ArtifactType.Document Then
					artifact.NativeFileGuid = nativeRow("Guid").ToString
				Else
					artifact.FileID = CType(nativeRow("FileID"), Int32)
				End If
			End If

			If nativeRow Is Nothing Then
				artifact.NativeExtension = ""
			ElseIf nativeRow("Filename").ToString.IndexOf(".") <> -1 Then
				artifact.NativeExtension = nativeRow("Filename").ToString.Substring(nativeRow("Filename").ToString.LastIndexOf(".") + 1)
			Else
				artifact.NativeExtension = ""
			End If
			artifact.ArtifactID = documentArtifactID
			artifact.Metadata = DirectCast(record, Object())
			SetProductionBegBatesFileName(artifact, lookup)

			Return artifact
		End Function

		Private Function ShouldTextAndNativesBeNamedAfterPrecedenceBegBates() As Boolean
			Return Me.NameTextAndNativesAfterBegBates AndAlso
					Settings.TypeOfExport <> ExportFile.ExportType.Production AndAlso
					ExportNativesToFileNamedFrom = ExportNativeWithFilenameFrom.Production AndAlso
					_productionPrecedenceIds.Any(Function(prodID) prodID > 0)
		End Function

		Private Sub SetProductionBegBatesFileName(artifact As ObjectExportInfo, bateslookup As Lazy(Of Dictionary(Of Int32, List(Of BatesEntry))))
			If Not _tryToNameNativesAndTextFilesAfterPrecedenceBegBates Then Return

			Dim lookup As Dictionary(Of Int32, List(Of BatesEntry)) = bateslookup.Value
			If Not lookup.ContainsKey(artifact.ArtifactID) Then Return 'Nothing to set up
			Dim documentProductions As List(Of BatesEntry) = lookup(artifact.ArtifactID)
			If artifact.CoalescedProductionID.HasValue Then
				artifact.ProductionBeginBates = (From r In documentProductions Where r.ProductionArtifactID = artifact.CoalescedProductionID.Value).First().BegBates
			Else
				For Each prodId As Int32 In _productionPrecedenceIds
					For Each entry As BatesEntry In documentProductions
						If entry.ProductionArtifactID = prodId Then
							artifact.ProductionBeginBates = entry.BegBates
							Return
						End If
					Next
				Next
			End If
		End Sub

		Private Function PrepareImagesForProduction(ByVal imagesView As System.Data.DataView, ByVal documentArtifactID As Int32, ByVal batesBase As String, ByVal artifact As Exporters.ObjectExportInfo) As System.Collections.ArrayList
			Dim retval As New System.Collections.ArrayList
			If Not Me.Settings.ExportImages Then Return retval
			Dim matchingRows As DataRow() = imagesView.Table.Select("DocumentArtifactID = " & documentArtifactID.ToString)
			Dim i As Int32 = 0
			'DAS034 There is at least one case where all production images for a document will end up with the same filename.
			'This happens when the production uses Existing production numbering, and the base production used Document numbering.
			'This case cannot be detected using current available information about the Production that we get from WebAPI.
			'To be on the safe side, keep track of the first image filename, and if another image has the same filename, add i + 1 onto it.
			Dim firstImageFileName As String = Nothing
			If matchingRows.Count > 0 Then
				Dim dr As System.Data.DataRow
				For Each dr In matchingRows
					Dim image As New Exporters.ImageExportInfo
					image.FileName = dr("ImageFileName").ToString
					image.FileGuid = dr("ImageGuid").ToString
					image.ArtifactID = documentArtifactID
					image.PageOffset = kCura.Utility.NullableTypesHelper.DBNullConvertToNullable(Of Int32)(dr("ByteRange"))
					image.BatesNumber = dr("BatesNumber").ToString
					image.SourceLocation = dr("Location").ToString
					Dim filenameExtension As String = ""
					If image.FileName.IndexOf(".") <> -1 Then
						filenameExtension = "." & image.FileName.Substring(image.FileName.LastIndexOf(".") + 1)
					End If
					Dim filename As String = image.BatesNumber
					If i = 0 Then
						firstImageFileName = filename
					End If
					If (i > 0) AndAlso
						(IsDocNumberOnlyProduction(_productionExportProduction) OrElse
						 filename.Equals(firstImageFileName, StringComparison.OrdinalIgnoreCase)) Then
						filename &= "_" & (i + 1).ToString
					End If
					image.FileName = filename & filenameExtension
					If Not image.FileGuid = "" Then
						retval.Add(image)
					End If
					i += 1
				Next
			End If
			Return retval
		End Function

		Private Function GetProduction(ByVal productionArtifactId As String) As kCura.EDDS.WebAPI.ProductionManagerBase.ProductionInfo
			Dim id As Int32 = CInt(productionArtifactId)
			If Not _productionLookup.ContainsKey(id) Then
				_productionLookup.Add(id, _productionManager.Read(Me.Settings.CaseArtifactID, id))
			End If
			Return _productionLookup(id)
		End Function

		Private Function IsDocNumberOnlyProduction(ByVal production As kCura.EDDS.WebAPI.ProductionManagerBase.ProductionInfo) As Boolean
			Return Not production Is Nothing AndAlso production.BatesNumbering = False AndAlso production.UseDocumentLevelNumbering AndAlso Not production.IncludeImageLevelNumberingForDocumentLevelNumbering
		End Function

		Private Function PrepareImages(ByVal imagesView As System.Data.DataView, ByVal productionImagesView As System.Data.DataView, ByVal documentArtifactID As Int32, ByVal batesBase As String, ByVal artifact As Exporters.ObjectExportInfo, ByVal productionOrderList As Pair()) As System.Collections.ArrayList
			Dim retval As New System.Collections.ArrayList
			If Not Me.Settings.ExportImages Then Return retval
			If Me.Settings.TypeOfExport = ExportFile.ExportType.Production Then
				productionImagesView.Sort = "DocumentArtifactID ASC"
				Return Me.PrepareImagesForProduction(productionImagesView, documentArtifactID, batesBase, artifact)
			End If
			Dim item As Pair
			For Each item In productionOrderList
				If item.Value = "-1" Then
					Return Me.PrepareOriginalImages(imagesView, documentArtifactID, batesBase, artifact)
				Else
					productionImagesView.RowFilter = String.Format("DocumentArtifactID = {0} AND ProductionArtifactID = {1}", documentArtifactID, item.Value)
					Dim firstImageFileName As String = Nothing
					If productionImagesView.Count > 0 Then
						Dim drv As System.Data.DataRowView
						Dim i As Int32 = 0
						For Each drv In productionImagesView
							Dim image As New Exporters.ImageExportInfo
							image.FileName = drv("ImageFileName").ToString
							image.FileGuid = drv("ImageGuid").ToString
							If image.FileGuid <> "" Then
								image.ArtifactID = documentArtifactID
								image.BatesNumber = drv("BatesNumber").ToString
								image.PageOffset = kCura.Utility.NullableTypesHelper.DBNullConvertToNullable(Of Int32)(drv("ByteRange"))
								Dim filenameExtension As String = ""
								If image.FileName.IndexOf(".") <> -1 Then
									filenameExtension = "." & image.FileName.Substring(image.FileName.LastIndexOf(".") + 1)
								End If
								Dim filename As String = image.BatesNumber
								If i = 0 Then
									firstImageFileName = filename
								End If
								If (IsDocNumberOnlyProduction(Me.GetProduction(item.Value)) OrElse filename.Equals(firstImageFileName, StringComparison.OrdinalIgnoreCase)) AndAlso i > 0 Then filename &= "_" & (i + 1).ToString
								image.FileName = filename & filenameExtension
								image.SourceLocation = drv("Location").ToString
								retval.Add(image)
								i += 1
							End If
						Next
						artifact.CoalescedProductionID = CInt(item.Value)
						Return retval
					End If
				End If
			Next
			Return retval
		End Function

		Private Function PrepareOriginalImages(ByVal imagesView As System.Data.DataView, ByVal documentArtifactID As Int32, ByVal batesBase As String, ByVal artifact As Exporters.ObjectExportInfo) As System.Collections.ArrayList
			Dim retval As New System.Collections.ArrayList
			If Not Me.Settings.ExportImages Then Return retval
			imagesView.RowFilter = "DocumentArtifactID = " & documentArtifactID.ToString
			Dim i As Int32 = 0
			If imagesView.Count > 0 Then
				Dim drv As System.Data.DataRowView
				For Each drv In imagesView
					Dim image As New Exporters.ImageExportInfo
					image.FileName = drv("Filename").ToString
					image.FileGuid = drv("Guid").ToString
					image.ArtifactID = documentArtifactID
					image.PageOffset = kCura.Utility.NullableTypesHelper.DBNullConvertToNullable(Of Int32)(drv("ByteRange"))
					If i = 0 Then
						image.BatesNumber = artifact.IdentifierValue
					Else
						image.BatesNumber = drv("Identifier").ToString
						If image.BatesNumber.IndexOf(image.FileGuid) <> -1 Then
							image.BatesNumber = artifact.IdentifierValue & "_" & i.ToString.PadLeft(imagesView.Count.ToString.Length + 1, "0"c)
						End If
					End If
					'image.BatesNumber = drv("Identifier").ToString
					Dim filenameExtension As String = ""
					If image.FileName.IndexOf(".") <> -1 Then
						filenameExtension = "." & image.FileName.Substring(image.FileName.LastIndexOf(".") + 1)
					End If
					image.FileName = kCura.Utility.File.Instance.ConvertIllegalCharactersInFilename(image.BatesNumber.ToString & filenameExtension)
					image.SourceLocation = drv("Location").ToString
					retval.Add(image)
					i += 1
				Next
			End If
			Return retval
		End Function

		Private Function GetNativeRow(ByVal dv As System.Data.DataView, ByVal artifactID As Int32) As System.Data.DataRowView
			If Not Me.Settings.ExportNative Then Return Nothing
			If Me.Settings.ArtifactTypeID = 10 Then
				dv.RowFilter = "DocumentArtifactID = " & artifactID.ToString
			Else
				dv.RowFilter = "ObjectArtifactID = " & artifactID.ToString
			End If
			If dv.Count > 0 Then
				Return dv(0)
			Else
				Return Nothing
			End If
		End Function

		''' <summary>
		''' Sets the member variable _columns to contain an array of each Field which will be exported.
		''' _columns is an array of ViewFieldInfo, but for the "Text Precedence" column, the array item is
		''' a CoalescedTextViewField (a subclass of ViewFieldInfo).
		''' </summary>
		''' <returns>A string containing the contents of the export file header.  For example, if _exportFile.LoadFile is false,
		''' and the fields selected to export are (Control Number and ArtifactID), along with the Text Precendence which includes
		''' Extracted Text, then the following string would be returned: ""Control Number","Artifact ID","Text Precedence" "
		''' </returns>
		''' <remarks></remarks>
		Private Function LoadColumns() As String
			For Each field As WinEDDS.ViewFieldInfo In Me.Settings.SelectedViewFields
				Me.Settings.ExportFullText = Me.Settings.ExportFullText OrElse field.Category = Relativity.FieldCategory.FullText
			Next
			_columns = New System.Collections.ArrayList(Me.Settings.SelectedViewFields)
			If Not Me.Settings.SelectedTextFields Is Nothing AndAlso Me.Settings.SelectedTextFields.Count > 0 Then
				Dim longTextSelectedViewFields As New List(Of ViewFieldInfo)()
				longTextSelectedViewFields.AddRange(Me.Settings.SelectedViewFields.Where(Function(f As ViewFieldInfo) f.FieldType = Relativity.FieldTypeHelper.FieldType.Text OrElse f.FieldType = Relativity.FieldTypeHelper.FieldType.OffTableText))
				If (Me.Settings.SelectedTextFields.Count = 1) AndAlso longTextSelectedViewFields.Exists(Function(f As ViewFieldInfo) f.Equals(Me.Settings.SelectedTextFields.First)) Then
					Dim selectedViewFieldToRemove As ViewFieldInfo = longTextSelectedViewFields.Find(Function(f As ViewFieldInfo) f.Equals(Me.Settings.SelectedTextFields.First))
					If selectedViewFieldToRemove IsNot Nothing Then
						Dim indexOfSelectedViewFieldToRemove As Int32 = _columns.IndexOf(selectedViewFieldToRemove)
						_columns.RemoveAt(indexOfSelectedViewFieldToRemove)
						_columns.Insert(indexOfSelectedViewFieldToRemove, New CoalescedTextViewField(Me.Settings.SelectedTextFields.First, True))
					Else
						_columns.Add(New CoalescedTextViewField(Me.Settings.SelectedTextFields.First, False))
					End If
				Else
					_columns.Add(New CoalescedTextViewField(Me.Settings.SelectedTextFields.First, False))
				End If
			End If

			Dim header As String = _loadFileFormatterFactory.Create(Settings).GetHeader(_columns.Cast(Of ViewFieldInfo)().ToList())
			Return header
		End Function

		Private Function RetrieveImagesForDocuments(ByVal documentArtifactIDs As Int32(), ByVal productionOrderList As Pair()) As System.Data.DataTable
			Select Case Me.Settings.TypeOfExport
				Case ExportFile.ExportType.Production
					Return Nothing
				Case Else
					Return _searchManager.RetrieveImagesForDocuments(Me.Settings.CaseArtifactID, documentArtifactIDs).Tables(0)
			End Select
		End Function

		Private Function RetrieveProductionImagesForDocuments(ByVal documentArtifactIDs As Int32(), ByVal productionOrderList As Pair()) As System.Data.DataTable
			Select Case Me.Settings.TypeOfExport
				Case ExportFile.ExportType.Production
					Return _searchManager.RetrieveImagesForProductionDocuments(Me.Settings.CaseArtifactID, documentArtifactIDs, Int32.Parse(productionOrderList(0).Value)).Tables(0)
				Case Else
					Dim productionIDs As Int32() = Me.GetProductionArtifactIDs(productionOrderList)
					If productionIDs.Length > 0 Then Return _searchManager.RetrieveImagesByProductionIDsAndDocumentIDsForExport(Me.Settings.CaseArtifactID, productionIDs, documentArtifactIDs).Tables(0)
			End Select
			Return Nothing
		End Function

		Private Function GetProductionArtifactIDs(ByVal productionOrderList As Pair()) As Int32()
			If _productionArtifactIDs Is Nothing Then
				Dim retval As New System.Collections.ArrayList
				Dim item As Pair
				For Each item In productionOrderList
					If item.Value <> "-1" Then
						retval.Add(Int32.Parse(item.Value))
					End If
				Next
				_productionArtifactIDs = DirectCast(retval.ToArray(GetType(Int32)), Int32())
			End If
			Return _productionArtifactIDs
		End Function

#End Region


#Region "Messaging"

		Private Sub AuditRun(ByVal success As Boolean)
			Dim args As New kCura.EDDS.WebAPI.AuditManagerBase.ExportStatistics
			args.AppendOriginalFilenames = Me.Settings.AppendOriginalFileName
			args.Bound = Me.Settings.QuoteDelimiter
			args.ArtifactTypeID = Me.Settings.ArtifactTypeID
			Select Case Me.Settings.TypeOfExport
				Case ExportFile.ExportType.AncestorSearch
					args.DataSourceArtifactID = Me.Settings.ViewID
				Case ExportFile.ExportType.ArtifactSearch
					args.DataSourceArtifactID = Me.Settings.ArtifactID
				Case ExportFile.ExportType.ParentSearch
					args.DataSourceArtifactID = Me.Settings.ViewID
				Case ExportFile.ExportType.Production
					args.DataSourceArtifactID = Me.Settings.ArtifactID
			End Select
			args.Delimiter = Me.Settings.RecordDelimiter
			args.DestinationFilesystemFolder = Me.Settings.FolderPath
			args.DocumentExportCount = Me.DocumentsExported
			args.ErrorCount = _errorCount
			If Not Me.Settings.SelectedTextFields Is Nothing Then args.ExportedTextFieldID = Me.Settings.SelectedTextFields(0).FieldArtifactId
			If Me.Settings.ExportFullTextAsFile Then
				args.ExportedTextFileEncodingCodePage = Me.Settings.TextFileEncoding.CodePage
				args.ExportTextFieldAsFiles = True
			Else
				args.ExportTextFieldAsFiles = False
			End If
			Dim fields As New System.Collections.ArrayList
			For Each field As ViewFieldInfo In Me.Settings.SelectedViewFields
				If Not fields.Contains(field.FieldArtifactId) Then fields.Add(field.FieldArtifactId)
			Next
			args.Fields = DirectCast(fields.ToArray(GetType(Int32)), Int32())
			args.ExportNativeFiles = Me.Settings.ExportNative
			If args.Fields.Length > 0 OrElse Me.Settings.ExportNative Then
				args.MetadataLoadFileEncodingCodePage = Me.Settings.LoadFileEncoding.CodePage
				Select Case Me.Settings.LoadFileExtension.ToLower
					Case "txt"
						args.MetadataLoadFileFormat = EDDS.WebAPI.AuditManagerBase.LoadFileFormat.Custom
					Case "csv"
						args.MetadataLoadFileFormat = EDDS.WebAPI.AuditManagerBase.LoadFileFormat.Csv
					Case "dat"
						args.MetadataLoadFileFormat = EDDS.WebAPI.AuditManagerBase.LoadFileFormat.Dat
					Case "html"
						args.MetadataLoadFileFormat = EDDS.WebAPI.AuditManagerBase.LoadFileFormat.Html
				End Select
				args.MultiValueDelimiter = Me.Settings.MultiRecordDelimiter
				args.ExportMultipleChoiceFieldsAsNested = Me.Settings.MulticodesAsNested
				args.NestedValueDelimiter = Me.Settings.NestedValueDelimiter
				args.NewlineProxy = Me.Settings.NewlineDelimiter
			End If
			Try
				args.FileExportCount = CType(_fileCount, Int32)
			Catch
			End Try
			Select Case Me.Settings.TypeOfExportedFilePath
				Case ExportFile.ExportedFilePathType.Absolute
					args.FilePathSettings = "Use Absolute Paths"
				Case ExportFile.ExportedFilePathType.Prefix
					args.FilePathSettings = "Use Prefix: " & Me.Settings.FilePrefix
				Case ExportFile.ExportedFilePathType.Relative
					args.FilePathSettings = "Use Relative Paths"
			End Select
			If Me.Settings.ExportImages Then
				args.ExportImages = True
				Select Case Me.Settings.TypeOfImage
					Case ExportFile.ImageType.MultiPageTiff
						args.ImageFileType = EDDS.WebAPI.AuditManagerBase.ImageFileExportType.MultiPageTiff
					Case ExportFile.ImageType.Pdf
						args.ImageFileType = EDDS.WebAPI.AuditManagerBase.ImageFileExportType.PDF
					Case ExportFile.ImageType.SinglePage
						args.ImageFileType = EDDS.WebAPI.AuditManagerBase.ImageFileExportType.SinglePage
				End Select
				Select Case Me.Settings.LogFileFormat
					Case LoadFileType.FileFormat.IPRO
						args.ImageLoadFileFormat = EDDS.WebAPI.AuditManagerBase.ImageLoadFileFormatType.Ipro
					Case LoadFileType.FileFormat.IPRO_FullText
						args.ImageLoadFileFormat = EDDS.WebAPI.AuditManagerBase.ImageLoadFileFormatType.IproFullText
					Case LoadFileType.FileFormat.Opticon
						args.ImageLoadFileFormat = EDDS.WebAPI.AuditManagerBase.ImageLoadFileFormatType.Opticon
				End Select
				Dim hasOriginal As Boolean = False
				Dim hasProduction As Boolean = False
				For Each pair As WinEDDS.Pair In Me.Settings.ImagePrecedence
					If pair.Value <> "-1" Then
						hasProduction = True
					Else
						hasOriginal = True
					End If
				Next
				If hasProduction AndAlso hasOriginal Then
					args.ImagesToExport = EDDS.WebAPI.AuditManagerBase.ImagesToExportType.Both
				ElseIf hasProduction Then
					args.ImagesToExport = EDDS.WebAPI.AuditManagerBase.ImagesToExportType.Produced
				Else
					args.ImagesToExport = EDDS.WebAPI.AuditManagerBase.ImagesToExportType.Original
				End If
			Else
				args.ExportImages = False
			End If
			args.OverwriteFiles = Me.Settings.Overwrite
			Dim preclist As New System.Collections.ArrayList
			For Each pair As WinEDDS.Pair In Me.Settings.ImagePrecedence
				preclist.Add(Int32.Parse(pair.Value))
			Next
			args.ProductionPrecedence = DirectCast(preclist.ToArray(GetType(Int32)), Int32())
			args.RunTimeInMilliseconds = CType(System.Math.Min(System.DateTime.Now.Subtract(_start).TotalMilliseconds, Int32.MaxValue), Int32)
			If Me.Settings.TypeOfExport = ExportFile.ExportType.AncestorSearch OrElse Me.Settings.TypeOfExport = ExportFile.ExportType.ParentSearch Then
				args.SourceRootFolderID = Me.Settings.ArtifactID
			End If
			args.SubdirectoryImagePrefix = Me.Settings.VolumeInfo.SubdirectoryImagePrefix(False)
			args.SubdirectoryMaxFileCount = Me.Settings.VolumeInfo.SubdirectoryMaxSize
			args.SubdirectoryNativePrefix = Me.Settings.VolumeInfo.SubdirectoryNativePrefix(False)
			args.SubdirectoryStartNumber = Me.Settings.VolumeInfo.SubdirectoryStartNumber
			args.SubdirectoryTextPrefix = Me.Settings.VolumeInfo.SubdirectoryFullTextPrefix(False)
			'args.TextAndNativeFilesNamedAfterFieldID = Me.ExportNativesToFileNamedFrom
			If Me.ExportNativesToFileNamedFrom = ExportNativeWithFilenameFrom.Identifier Then
				For Each field As ViewFieldInfo In Me.Settings.AllExportableFields
					If field.Category = Relativity.FieldCategory.Identifier Then
						args.TextAndNativeFilesNamedAfterFieldID = field.FieldArtifactId
						Exit For
					End If
				Next
			Else
				For Each field As ViewFieldInfo In Me.Settings.AllExportableFields
					If field.AvfColumnName.ToLower = _beginBatesColumn.ToLower Then
						args.TextAndNativeFilesNamedAfterFieldID = field.FieldArtifactId
						Exit For
					End If
				Next
			End If
			args.TotalFileBytesExported = _statistics.FileBytes
			args.TotalMetadataBytesExported = _statistics.MetadataBytes
			Select Case Me.Settings.TypeOfExport
				Case ExportFile.ExportType.AncestorSearch
					args.Type = "Folder and Subfolders"
				Case ExportFile.ExportType.ArtifactSearch
					args.Type = "Saved Search"
				Case ExportFile.ExportType.ParentSearch
					args.Type = "Folder"
				Case ExportFile.ExportType.Production
					args.Type = "Production Set"
			End Select
			args.VolumeMaxSize = Me.Settings.VolumeInfo.VolumeMaxSize
			args.VolumePrefix = Me.Settings.VolumeInfo.VolumePrefix
			args.VolumeStartNumber = Me.Settings.VolumeInfo.VolumeStartNumber
			args.StartExportAtDocumentNumber = Me.Settings.StartAtDocumentNumber + 1
			args.CopyFilesFromRepository = Me.Settings.VolumeInfo.CopyNativeFilesFromRepository OrElse Settings.VolumeInfo.CopyNativeFilesFromRepository
			args.WarningCount = _warningCount
			Try
				_auditManager.AuditExport(Me.Settings.CaseInfo.ArtifactID, Not success, args)
			Catch
			End Try
		End Sub

		Friend Sub WriteFatalError(ByVal line As String, ByVal ex As System.Exception)
			Me.AuditRun(False)
			RaiseEvent FatalErrorEvent(line, ex)
		End Sub

		Friend Sub WriteStatusLine(ByVal e As kCura.Windows.Process.EventType, ByVal line As String, ByVal isEssential As Boolean)
			Dim now As Long = System.DateTime.Now.Ticks
			If now - _lastStatusMessageTs > 10000000 OrElse isEssential Then
				_lastStatusMessageTs = now
				Dim appendString As String = " ... " & Me.DocumentsExported - _lastDocumentsExportedCountReported & " document(s) exported."
				_lastDocumentsExportedCountReported = Me.DocumentsExported
				RaiseEvent StatusMessage(New ExportEventArgs(Me.DocumentsExported, Me.TotalExportArtifactCount, line & appendString, e, _lastStatisticsSnapshot))
			End If
		End Sub

		Friend Sub WriteStatusLineWithoutDocCount(ByVal e As kCura.Windows.Process.EventType, ByVal line As String, ByVal isEssential As Boolean)
			Dim now As Long = System.DateTime.Now.Ticks
			If now - _lastStatusMessageTs > 10000000 OrElse isEssential Then
				_lastStatusMessageTs = now
				_lastDocumentsExportedCountReported = Me.DocumentsExported
				RaiseEvent StatusMessage(New ExportEventArgs(Me.DocumentsExported, Me.TotalExportArtifactCount, line, e, _lastStatisticsSnapshot))
			End If
		End Sub

		Friend Sub WriteError(ByVal line As String)
			_errorCount += 1
			WriteStatusLine(kCura.Windows.Process.EventType.Error, line, True)
		End Sub

		Friend Sub WriteImgProgressError(ByVal artifact As Exporters.ObjectExportInfo, ByVal imageIndex As Int32, ByVal ex As System.Exception, Optional ByVal notes As String = "")
			Dim sw As New System.IO.StreamWriter(_exportFile.FolderPath & "\" & _exportFile.LoadFilesPrefix & "_img_errors.txt", True, _exportFile.LoadFileEncoding)
			sw.WriteLine(System.DateTime.Now.ToString("s"))
			sw.WriteLine(String.Format("DOCUMENT: {0}", artifact.IdentifierValue))
			If imageIndex > -1 AndAlso artifact.Images.Count > 0 Then
				sw.WriteLine(String.Format("IMAGE: {0} ({1} of {2})", artifact.Images(imageIndex), imageIndex + 1, artifact.Images.Count))
			End If
			If Not notes = "" Then sw.WriteLine("NOTES: " & notes)
			sw.WriteLine("ERROR: " & ex.ToString)
			sw.WriteLine("")
			sw.Flush()
			sw.Close()
			Dim errorLine As String = String.Format("Error processing images for document {0}: {1}. Check {2}_img_errors.txt for details", artifact.IdentifierValue, ex.Message.TrimEnd("."c), _exportFile.LoadFilesPrefix)
			Me.WriteError(errorLine)
		End Sub

		Friend Sub WriteWarning(ByVal line As String)
			_warningCount += 1
			WriteStatusLine(kCura.Windows.Process.EventType.Warning, line, True)
		End Sub

		Friend Sub WriteUpdate(ByVal line As String, Optional ByVal isEssential As Boolean = True)
			WriteStatusLine(kCura.Windows.Process.EventType.Progress, line, isEssential)
		End Sub

#End Region

#Region "Public Events"

		Public Event FatalErrorEvent(ByVal message As String, ByVal ex As System.Exception) Implements IExporterStatusNotification.FatalErrorEvent
		Public Event StatusMessage(ByVal exportArgs As ExportEventArgs) Implements IExporterStatusNotification.StatusMessage
		Public Event FileTransferModeChangeEvent(ByVal mode As String) Implements IExporterStatusNotification.FileTransferModeChangeEvent
		Public Event DisableCloseButton()
		Public Event EnableCloseButton()

#End Region

		Private Sub _processController_HaltProcessEvent(ByVal processID As System.Guid) Handles _processController.HaltProcessEvent
			_halt = True
			If Not _volumeManager Is Nothing Then _volumeManager.Halt = True
		End Sub

		Public Event UploadModeChangeEvent(ByVal mode As String)

		Private Sub _downloadHandler_UploadModeChangeEvent(ByVal mode As String) Handles _downloadHandler.UploadModeChangeEvent
			RaiseEvent FileTransferModeChangeEvent(_downloadHandler.UploaderType.ToString)
		End Sub
	End Class
End Namespace