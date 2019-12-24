Imports Monitoring
Imports Relativity.DataExchange

Namespace kCura.WinEDDS
	Public Class Statistics
		Public Const BatchCountKey As String = "Batches"
		Public Const DocsCreatedKey As String = "DocsCreated"
		Public Const DocsCountKey As String = "Docs"
		Public Const DocsUpdatedKey As String = "DocsUpdated"
		Public Const FilesProcessedKey As String = "MassImportFilesProcessed"
		Public Const MetadataBytesKey As String = "MetadataBytes"
		Public Const MetadataFilesTransferredKey As String = "MetadataFilesTransferred"
		Public Const MetadataThroughputKey As String = "MetadataThroughput"
		Public Const MetadataTimeKey As String = "MetadataTime"
		Public Const NativeFileBytesKey As String = "NativeFileBytes"
		Public Const NativeFileThroughputKey As String = "NativeFileThroughput"
		Public Const NativeFileTimeKey As String = "NativeFileTime"
		Public Const NativeFilesTransferredKey As String = "NativeFilesTransferred"

		Private _metadataBytes As Int64 = 0
		Private _metadataTime As Int64 = 0
		Private _metadataThroughput As Double = 0
		Private _fileBytes As Int64 = 0
		Private _fileTime As Int64 = 0
		Private _fileThroughput As Double = 0
		Private _fileWaitTime As Int64 = 0
		Private _metadataWaitTime As Int64 = 0
		Private _sqlTime As Int64 = 0
		Private _docCount As Int32 = 0
		Private _lastAccessed As System.DateTime
		Private _documentsCreated As Int32 = 0
		Private _documentsUpdated As Int32 = 0
		Private _filesProcessed As Int32 = 0

		Public ReadOnly Property LastAccessed() As System.DateTime
			Get
				Return _lastAccessed
			End Get
		End Property

		Public ReadOnly Property DocumentsCreated() As Int32
			Get
				Return _documentsCreated
			End Get
		End Property

		Public ReadOnly Property DocumentsUpdated() As Int32
			Get
				Return _documentsUpdated
			End Get
		End Property

		Public ReadOnly Property FilesProcessed() As Int32
			Get
				Return _filesProcessed
			End Get
		End Property

		''' <summary>
		''' Gets total transferred bytes. This value is equal to sum of <see cref="FileBytes"/> and <see cref="MetadataBytes"/>.
		''' </summary>
		''' <returns>Total number of transferred bytes.</returns>
		Public ReadOnly Property TotalBytes() As Int64
			Get
				Return _fileBytes + _metadataBytes
			End Get
		End Property

		''' <summary>
		''' Gets or sets the total number of import or export batches.
		''' </summary>
		''' <value>
		''' The total number of batches.
		''' </value>
		Public Property BatchCount As Int32 = 0
		
		Public Property BatchSize As Int32 = 0

		Public Property ImportObjectType As TelemetryConstants.ImportObjectType = TelemetryConstants.ImportObjectType.NotApplicable

		''' <summary>
		'''  Gets or sets transferred metadata bytes.
		''' </summary>
		''' <returns>Transferred metadata bytes.</returns>
		Public Property MetadataBytes() As Int64
			Get
				Return _metadataBytes
			End Get
			Set(ByVal value As Int64)
				_lastAccessed = System.DateTime.Now
				_metadataBytes = value
			End Set
		End Property

		''' <summary>
		''' Gets or sets metadata transfer time in ticks.
		''' </summary>
		''' <returns>Metadata transfer time in ticks.</returns>
		Public Property MetadataTime() As Int64
			Get
				Return _metadataTime
			End Get
			Set(ByVal value As Int64)
				_lastAccessed = System.DateTime.Now
				_metadataTime = value
			End Set
		End Property

		''' <summary>
		''' Gets or sets metadata transfer rate in bytes per second.
		''' </summary>
		''' <returns>Metadata transfer rate in bytes per second.</returns>
		Public Property MetadataThroughput() As Double
			Get
				Return _metadataThroughput
			End Get
			Set(ByVal value As Double)
				_lastAccessed = System.DateTime.Now
				_metadataThroughput = value
			End Set
		End Property

		''' <summary>
		'''  Gets or sets transferred file bytes.
		''' </summary>
		''' <returns>Transferred file bytes.</returns>
		Public Property FileBytes() As Int64
			Get
				Return _fileBytes
			End Get
			Set(ByVal value As Int64)
				_lastAccessed = System.DateTime.Now
				_fileBytes = value
			End Set
		End Property

		''' <summary>
		''' Gets or sets file transfer time in ticks.
		''' </summary>
		''' <returns>File transfer time in ticks.</returns>
		Public Property FileTime() As Int64
			Get
				Return _fileTime
			End Get
			Set(ByVal value As Int64)
				_lastAccessed = System.DateTime.Now
				_fileTime = value
			End Set
		End Property
		
		''' <summary>
		''' Gets or sets file transfer rate in bytes per second.
		''' </summary>
		''' <returns>File transfer rate in bytes per second.</returns>
		Public Property FileThroughput() As Double
			Get
				Return _fileThroughput
			End Get
			Set(ByVal value As Double)
				_lastAccessed = System.DateTime.Now
				_fileThroughput = value
			End Set
		End Property

		Public Property FileWaitTime() As Int64
			Get
				Return _fileWaitTime
			End Get
			Set(ByVal value As Int64)
				_lastAccessed = System.DateTime.Now
				_fileWaitTime = value
			End Set
		End Property

		''' <summary>
		''' Gets or sets the total number of metadata load and extracted text files transferred for import and export respectively.
		''' </summary>
		''' <value>
		''' The total number of files.
		''' </value>
		Public Property MetadataFilesTransferredCount As Int32 = 0

		Public Property MetadataWaitTime() As Int64
			Get
				Return _metadataWaitTime
			End Get
			Set(ByVal value As Int64)
				_lastAccessed = System.DateTime.Now
				_metadataWaitTime = value
			End Set
		End Property

		''' <summary>
		''' Gets or sets the total number of native files transferred for both import and export.
		''' </summary>
		''' <value>
		''' The total number of files.
		''' </value>
		Public Property NativeFilesTransferredCount As Int32 = 0
		
		''' <summary>
		''' Gets or sets client side time of SQL bulk insert operation in ticks.
		''' </summary>
		''' <returns>Client side time of SQL bulk insert operation in ticks.</returns>
		Public Property SqlTime() As Int64
			Get
				Return _sqlTime
			End Get
			Set(ByVal value As Int64)
				_lastAccessed = System.DateTime.Now
				_sqlTime = value
			End Set
		End Property

		Public Property DocCount() As Int32
			Get
				Return _docCount
			End Get
			Set(ByVal value As Int32)
				_lastAccessed = System.DateTime.Now
				_docCount = value
			End Set
		End Property
		
		Public Sub ProcessRunResults(ByVal results As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults)
			_documentsCreated += results.ArtifactsCreated
			_documentsUpdated += results.ArtifactsUpdated
			_filesProcessed += results.FilesProcessed
		End Sub

		''' <summary>
		''' General function calculating throughput in units per second.
		''' </summary>
		''' <param name="size">Size in any unit.</param>
		''' <param name="timeSeconds">Time in seconds.</param>
		''' <returns>Throughput in units per second if timeSeconds is not equal to zero; Zero otherwise.</returns>
		Public Shared Function CalculateThroughput(size As Long, timeSeconds As Double) As Double
			Return CDbl(IIf(timeSeconds.Equals(0.0), 0.0, size/timeSeconds))
		End Function

		Public Function ToFileSizeSpecification(ByVal value As Double) As String
			Return ByteSize.FromBytes(value).ToString("0.##")
		End Function

		''' <summary>
		''' Converts this instance into a dictionary containing limited name/value pairs.
		''' </summary>
		''' <remarks>
		''' The NV pairs are displayed in the RDC. Do NOT modify unless you intend to change the RDC display!
		''' </remarks>
		''' <returns>
		''' The <see cref="IDictionary"/> instance.
		''' </returns>
		Public Overridable Function ToDictionary() As IDictionary
			Dim retval As New System.Collections.Specialized.HybridDictionary
			If Not Me.FileTime = 0 Then
				Dim fileTime As Int64 = Me.FileTime - Me.FileWaitTime
				If fileTime < 0 Then
					fileTime = Me.FileTime
				End If

				retval.Add("Average file transfer rate", ToFileSizeSpecification(Me.FileBytes / (fileTime / TimeSpan.TicksPerSecond)) & "/sec")
			End If
			If Not Me.MetadataTime = 0 Then
				Dim metadataTime As Int64 = Me.MetadataTime - Me.MetadataWaitTime
				If metadataTime < 0 Then
					metadataTime = Me.MetadataTime
				End If

				retval.Add("Average metadata transfer rate", ToFileSizeSpecification(Me.MetadataBytes / (metadataTime / TimeSpan.TicksPerSecond)) & "/sec")
			End If
			If Not Me.SqlTime = 0 Then retval.Add("Average SQL process rate", (Me.DocCount / (Me.SqlTime / TimeSpan.TicksPerSecond)).ToString("N0") & " Documents/sec")
			If Not Me.BatchSize = 0 Then retval.Add("Current batch size", (Me.BatchSize).ToString("N0"))
			Return retval
		End Function

		''' <summary>
		''' Converts this instance into a dictionary containing relevant name/value pairs for logging purposes.
		''' </summary>
		''' <returns>
		''' The <see cref="IDictionary"/> instance.
		''' </returns>
		Public Overridable Function ToDictionaryForLogs() As System.Collections.Generic.IDictionary(Of String, Object)

			Dim statisticsDict As System.Collections.Generic.Dictionary(Of String, Object) = New System.Collections.Generic.Dictionary(Of String, Object) From
				    {
					    {BatchCountKey, Me.BatchCount},
					    {DocsCountKey, Me.DocCount},
					    {DocsCreatedKey, Me.DocumentsCreated},
					    {DocsUpdatedKey, Me.DocumentsUpdated},
					    {FilesProcessedKey, Me.FilesProcessed},
					    {MetadataBytesKey, Me.MetadataBytes},
					    {MetadataFilesTransferredKey, Me.MetadataFilesTransferredCount},
					    {MetadataThroughputKey, Me.MetadataThroughput},
					    {MetadataTimeKey, TimeSpan.FromTicks(Me.MetadataTime)},
					    {NativeFileBytesKey, Me.FileBytes},
					    {NativeFileThroughputKey, Me.FileThroughput},
					    {NativeFileTimeKey, TimeSpan.FromTicks(Me.FileTime)},
					    {NativeFilesTransferredKey, Me.NativeFilesTransferredCount}
				    }

			Dim pair As DictionaryEntry
			For Each pair In Me.ToDictionary()
				statisticsDict.Add(pair.Key.ToString(), pair.Value)
			Next

			Return statisticsDict
		End Function
	End Class
End Namespace