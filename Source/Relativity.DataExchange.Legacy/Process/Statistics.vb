Imports Monitoring
Imports Relativity.DataExchange

Namespace kCura.WinEDDS
	Public Class Statistics
		Public Const BatchSizeKey As String = "BatchSize"
		Public Const BatchCountKey As String = "Batches"
		Public Const DocsCountKey As String = "Docs"
		Public Const MetadataBytesKey As String = "MetadataBytes"
		Public Const MetadataFilesTransferredKey As String = "MetadataFilesTransferred"
		Public Const MetadataThroughputKey As String = "MetadataThroughput"
		Public Const MetadataTimeKey As String = "MetadataTime"
		Public Const NativeFileBytesKey As String = "NativeFileBytes"
		Public Const NativeFileThroughputKey As String = "NativeFileThroughput"
		Public Const NativeFileTimeKey As String = "NativeFileTime"
		Public Const NativeFilesTransferredKey As String = "NativeFilesTransferred"
		Public Const DocsErrorsCountKey As String = "DocsErrorsCount"

		Public Const FileTransferRateKey As String = "Average file transfer rate"
		Public Const MetadataTransferRateKey As String = "Average metadata transfer rate"
		Public Const SqlProcessRateKey As String = "Average SQL process rate"
		Public Const CurrentBatchSizeKey As String = "Current batch size"

		''' <summary>
		''' Gets or sets the total number of import or export batches.
		''' </summary>
		''' <value>The total number of batches.</value>
		Public Property BatchCount As Integer = 0
		
		''' <summary>
		''' Gets or sets the maximum number of documents or objects before the metadata is imported.
		''' </summary>
		''' <value>The maximum number of documents or objects in batch.</value>
		Public Property BatchSize As Integer = 0

		''' <summary>
		''' Gets or sets number of documents processed with errors.
		''' </summary>
		''' <value>The total number of records with errors.</value>
		Public Property DocsErrorsCount As Integer = 0

		''' <summary>
		''' Gets or sets the total number of metadata load and extracted text files transferred for import and export respectively.
		''' </summary>
		''' <value>The total number of files.</value>
		Public Property MetadataFilesTransferredCount As Integer = 0

		''' <summary>
		'''  Gets or sets transferred metadata bytes.
		''' </summary>
		''' <value>Transferred metadata bytes.</value>
		Public Property MetadataTransferredBytes As Long = 0

		''' <summary>
		''' Gets or sets metadata transfer duration.
		''' </summary>
		''' <value>Metadata transfer duration.</value>
		Public Property MetadataTransferDuration As New TimeSpan

		Public Property MetadataWaitDuration As New TimeSpan

		''' <summary>
		''' Gets or sets metadata transfer rate in bytes per second.
		''' </summary>
		''' <value>Metadata transfer rate in bytes per second.</value>
		Public Property MetadataTransferThroughput As Double = 0

		''' <summary>
		''' Gets or sets the total number of native files transferred for both import and export.
		''' </summary>
		''' <value>The total number of files.</value>
		Public Property NativeFilesTransferredCount As Integer = 0

		''' <summary>
		'''  Gets or sets transferred file bytes.
		''' </summary>
		''' <value>Transferred file bytes.</value>
		Public Property FileTransferredBytes As Long = 0

		''' <summary>
		''' Gets or sets file transfer duration.
		''' </summary>
		''' <value>File transfer duration.</value>
		Public Property FileTransferDuration As New TimeSpan

		Public Property FileWaitDuration As New TimeSpan

		''' <summary>
		''' Gets or sets file transfer rate in bytes per second.
		''' </summary>
		''' <value>File transfer rate in bytes per second.</value>
		Public Property FileTransferThroughput As Double = 0

		''' <summary>
		''' Gets or sets client side duration of mass import operation.
		''' </summary>
		''' <value>Client side duration of mass import operation.</value>
		Public Property MassImportDuration As New TimeSpan

		Public Property DocumentsCount As Integer = 0
		

		''' <summary>
		''' General function calculating throughput in units per second.
		''' </summary>
		''' <param name="size">Size in any unit.</param>
		''' <param name="timeSeconds">Time in seconds.</param>
		''' <returns>Throughput in units per second if timeSeconds is not equal to zero; Zero otherwise.</returns>
		Public Shared Function CalculateThroughput(size As Long, timeSeconds As Double) As Double
			Return CDbl(IIf(timeSeconds.Equals(0.0), 0.0, size/timeSeconds))
		End Function

		Friend Shared Function ToFileSizeSpecification(value As Double) As String
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
		Public Overridable Function ToDictionaryForProgress() As IDictionary
			Dim retval As New System.Collections.Specialized.HybridDictionary
			If Not Me.FileTransferDuration.Equals(TimeSpan.Zero) Then
				Dim fileDuration As TimeSpan = Me.FileTransferDuration - Me.FileWaitDuration
				If fileDuration <= TimeSpan.Zero Then
					fileDuration = Me.FileTransferDuration
				End If

				retval.Add(FileTransferRateKey, ToFileSizeSpecification(Me.FileTransferredBytes / fileDuration.TotalSeconds) & "/sec")
			End If
			If Not Me.MetadataTransferDuration.Equals(TimeSpan.Zero) Then
				Dim metadataDuration As TimeSpan = Me.MetadataTransferDuration - Me.MetadataWaitDuration
				If metadataDuration <= TimeSpan.Zero Then
					metadataDuration = Me.MetadataTransferDuration
				End If

				retval.Add(MetadataTransferRateKey, ToFileSizeSpecification(Me.MetadataTransferredBytes / metadataDuration.TotalSeconds) & "/sec")
			End If
			If Not Me.MassImportDuration.Equals(TimeSpan.Zero) Then retval.Add(SqlProcessRateKey, (Me.DocumentsCount / Me.MassImportDuration.TotalSeconds).ToString("N0") & " Documents/sec")
			If Not Me.BatchSize = 0 Then retval.Add(CurrentBatchSizeKey, (Me.BatchSize).ToString("N0"))
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
						{DocsErrorsCountKey, Me.DocsErrorsCount},
						{BatchSizeKey, Me.BatchSize},
					    {BatchCountKey, Me.BatchCount},
					    {DocsCountKey, Me.DocumentsCount},
					    {MetadataBytesKey, Me.MetadataTransferredBytes},
					    {MetadataFilesTransferredKey, Me.MetadataFilesTransferredCount},
					    {MetadataThroughputKey, Me.MetadataTransferThroughput},
					    {MetadataTimeKey, Me.MetadataTransferDuration},
					    {NativeFileBytesKey, Me.FileTransferredBytes},
					    {NativeFileThroughputKey, Me.FileTransferThroughput},
					    {NativeFileTimeKey, Me.FileTransferDuration},
					    {NativeFilesTransferredKey, Me.NativeFilesTransferredCount}
				    }

			Return statisticsDict
		End Function
	End Class
End Namespace