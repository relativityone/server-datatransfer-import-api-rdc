Imports Monitoring
Imports Relativity.DataExchange

Namespace kCura.WinEDDS

	Public Class ImportStatistics
		Inherits Statistics
		
		Public Const DocsCreatedKey As String = "DocsCreated"
		Public Const DocsUpdatedKey As String = "DocsUpdated"
		Public Const FilesProcessedKey As String = "MassImportFilesProcessed"
		Public Const ImportObjectTypeKey As String = "ImportObjectType"

		''' <summary>
		''' Gets number of documents created in mass import.
		''' </summary>
		''' <value>The number of documents created in mass import.</value>
		Public Property DocumentsCreated As Integer = 0

		''' <summary>
		''' Gets number of documents updated in mass import.
		''' </summary>
		''' <value>The number of documents updated in mass import.</value>
		Public Property DocumentsUpdated As Integer = 0

		''' <summary>
		''' Gets total number of records processed in mass import.
		''' </summary>
		''' <value>The total number of records processed in mass import.</value>
		Public Property FilesProcessed As Integer = 0
		Public Property ImportObjectType As TelemetryConstants.ImportObjectType = TelemetryConstants.ImportObjectType.NotApplicable

		Public Sub ProcessMassImportResults(ByVal results As kCura.EDDS.WebAPI.BulkImportManagerBase.MassImportResults)
			DocumentsCreated += results.ArtifactsCreated
			DocumentsUpdated += results.ArtifactsUpdated
			FilesProcessed += results.FilesProcessed
		End Sub

		''' <summary>
		''' Converts this instance into a dictionary containing relevant name/value pairs for logging purposes.
		''' </summary>
		''' <returns>
		''' The <see cref="IDictionary"/> instance.
		''' </returns>
		Public Overrides Function ToDictionaryForLogs() As System.Collections.Generic.IDictionary(Of String, Object)

			Dim statisticsDict As System.Collections.Generic.IDictionary(Of String, Object) = MyBase.ToDictionaryForLogs()

			statisticsDict.Add(DocsCreatedKey, Me.DocumentsCreated)
			statisticsDict.Add(DocsUpdatedKey, Me.DocumentsUpdated)
			statisticsDict.Add(FilesProcessedKey, Me.FilesProcessed)
			statisticsDict.Add(ImportObjectTypeKey, Me.ImportObjectType)

			Dim pair As DictionaryEntry
			For Each pair In Me.ToDictionaryForProgress()
				statisticsDict.Add(pair.Key.ToString(), pair.Value)
			Next

			Return statisticsDict
		End Function

	End Class

End Namespace