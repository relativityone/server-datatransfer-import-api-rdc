Namespace Monitoring

	''' <summary>
	''' Contains information about single batch.
	''' </summary>
	Public Class BatchInformation
		
		''' <summary>
		''' Gets or sets batch ordinal number.
		''' </summary>
		''' <returns>Batch ordinal number.</returns>
		Public Property OrdinalNumber As Integer

		''' <summary>
		''' Gets or sets number of records in batch. This value is
		''' a sum of all created and updated artifacts in the batch.
		''' </summary>
		''' <returns>Number of records.</returns>
		Public Property NumberOfRecords As Integer

		''' <summary>
		''' Gets or sets number of files in batch.
		''' </summary>
		''' <returns>Number of files.</returns>
		Public Property NumberOfFilesProcessed As Integer

		''' <summary>
		''' Gets or sets number of records with errors in batch.
		''' </summary>
		''' <returns>Number of records with errors.</returns>
		Public Property NumberOfRecordsWithErrors As Integer

		''' <summary>
		''' Gets or sets duration of mass import.
		''' </summary>
		''' <returns>Mass import duration.</returns>
		Public Property MassImportDuration As TimeSpan
	End Class
End NameSpace