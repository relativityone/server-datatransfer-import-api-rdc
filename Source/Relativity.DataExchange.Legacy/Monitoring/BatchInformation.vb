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
		''' Gets or sets number of records in batch.
		''' </summary>
		''' <returns>Number of records</returns>
		Public Property NumberOfRecords As Integer

		''' <summary>
		''' Gets or sets duration of mass import.
		''' </summary>
		''' <returns>Mass import duration.</returns>
		Public Property MassImportDuration As TimeSpan
	End Class
End NameSpace