Public Class BatchReport
	Public Sub New(ordinalNumber As Integer, numberOfFiles As Integer, numberOfRecords As Integer, numberOfRecordsWithErrors As Integer)
		Me.OrdinalNumber = ordinalNumber
		Me.NumberOfFiles = numberOfFiles
		Me.NumberOfRecords = numberOfRecords
		Me.NumberOfRecordsWithErrors = numberOfRecordsWithErrors
	End Sub

	''' <summary>
	''' Gets or sets batch ordinal number.
	''' </summary>
	''' <returns>Batch ordinal number.</returns>
	Public Property OrdinalNumber As Integer

	''' <summary>
	''' Gets or sets number of files in batch.
	''' </summary>
	''' <returns>Number of files.</returns>
	Public Property NumberOfFiles As Integer

	''' <summary>
	''' Gets or sets number of records in batch.
	''' </summary>
	''' <returns>Number of records.</returns>
	Public Property NumberOfRecords As Integer

	''' <summary>
	''' Gets or sets number of records with errors in batch.
	''' </summary>
	''' <returns>Number of records with errors.</returns>
	Public Property NumberOfRecordsWithErrors As Integer

	Public Overrides Function ToString() As String
		Return $"Batch #{Me.OrdinalNumber}; Files: {Me.NumberOfFiles}; Records: {Me.NumberOfRecords}; Records with errors: {Me.NumberOfRecordsWithErrors}"
	End Function
End Class