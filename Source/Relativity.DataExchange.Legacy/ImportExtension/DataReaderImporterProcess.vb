Imports Monitoring.Sinks
Imports Relativity.DataExchange
Imports Relativity.DataExchange.Io
Imports Relativity.DataExchange.Service

Namespace kCura.WinEDDS.ImportExtension
	Public Class DataReaderImporterProcess
		Inherits kCura.WinEDDS.ImportLoadFileProcess

		Public Sub New(ByVal sourceData As System.Data.IDataReader, ByVal metricService As IMetricService)
			MyBase.New(metricService)

			' Use the default value for the delimiter because as a public class,
			' users of this class may not know what value to set for this
			BulkLoadFileFieldDelimiter = ServiceConstants.DEFAULT_FIELD_DELIMITER
		End Sub

		Public Property OnBehalfOfUserToken As String
		Public Property TimeKeeperManager As ITimeKeeperManager

		Private Function AddColumnIndexToName(ByVal dr As System.Data.IDataReader, ByVal columnName As String) As String
			'function: convert [columnName] to [columnName]([index]) if [columnName] is a column in DR.  Example: FileLocation -> FileLocation(3)
			'*Note: I want the result to be a 1-based index. DR uses a 0-based index.  Computers, man, computers.
			Dim retColumnName As String = columnName
			If Not retColumnName Is Nothing Then
				retColumnName = retColumnName.ToLower
				Try
					Dim iIndex As Integer = dr.GetOrdinal(retColumnName)
					retColumnName = String.Concat(retColumnName, "(", iIndex + 1, ")")
				Catch ex As IndexOutOfRangeException
					Throw New IndexOutOfRangeException($"ERROR: {columnName} must be a field in the source data", ex)
				End Try
			End If
			Return retColumnName
		End Function

		Public Overrides Function GetImporter() As kCura.WinEDDS.BulkLoadFileImporter
			_ioReporterContext = New IoReporterContext(Me.FileSystem, Me.AppSettings, New WaitAndRetryPolicy(Me.AppSettings))
			Dim reporter As IIoReporter = Me.CreateIoReporter(_ioReporterContext)
            LoadFile.OIFileIdColumnName = OIFileIdColumnName
			LoadFile.OIFileIdMapped = OIFileIdMapped
			LoadFile.OIFileTypeColumnName = OIFileTypeColumnName
			LoadFile.FileSizeColumn = FileSizeColumn
			LoadFile.FileSizeMapped = FileSizeMapped
			LoadFile.FileNameColumn = FileNameColumn
			LoadFile.SupportedByViewerColumn = SupportedByViewerColumn
			
            'Avoid initializing the Artifact Reader in the constructor because it calls back to a virtual method (GetArtifactReader).  
			Dim importer As DataReaderImporter = New DataReaderImporter(
				DirectCast(Me.LoadFile, kCura.WinEDDS.ImportExtension.DataReaderLoadFile), _
				Me.Context, _
				reporter, _
				Me.Logger, _
				BulkLoadFileFieldDelimiter, _
				_temporaryLocalDirectory, _
				Me.CancellationTokenSource, _
				initializeArtifactReader:=False, _
				executionSource := ExecutionSource) With
				    {
						.OnBehalfOfUserToken = Me.OnBehalfOfUserToken,
						.Timekeeper = Me.TimeKeeperManager
				    }
			importer.Initialize()

			Dim dr As System.Data.IDataReader = importer.SourceData
			'These settings need to have [columnName]([index])
			LoadFile.FolderStructureContainedInColumn = AddColumnIndexToName(dr, LoadFile.FolderStructureContainedInColumn)
			LoadFile.NativeFilePathColumn = AddColumnIndexToName(dr, LoadFile.NativeFilePathColumn)
			LoadFile.DataGridIDColumn = AddColumnIndexToName(dr, LoadFile.DataGridIDColumn)
			importer.DestinationFolder = AddColumnIndexToName(dr, importer.DestinationFolder)
			Return DirectCast(importer, kCura.WinEDDS.BulkLoadFileImporter)
		End Function

		Dim _temporaryLocalDirectory As String = Nothing

		Protected Overrides Sub OnExecute()
			_temporaryLocalDirectory = System.IO.Path.GetTempPath() & "FlexMigrationFiles-" & System.Guid.NewGuid().ToString() & System.IO.Path.DirectorySeparatorChar
			MyBase.OnExecute()
			If System.IO.Directory.Exists(_temporaryLocalDirectory) Then System.IO.Directory.Delete(_temporaryLocalDirectory, True)
		End Sub

	End Class
End Namespace