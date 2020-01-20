Namespace Monitoring

	Public Class MetricJobBatch
		Inherits MetricJobBase

		Public Overrides ReadOnly Property BucketName As String = TelemetryConstants.BucketName.METRIC_JOB_BATCH

		''' <summary>
		''' Gets or sets ordinal number of batch.
		''' </summary>
		''' <returns>Batch ordinal number.</returns>
		Public Property BatchNumber As Integer
			Get
				Return GetValueOrDefault(Of Integer)(TelemetryConstants.KeyName.BATCH_NUMBER)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.BATCH_NUMBER) = Value
			End Set
		End Property

		''' <summary>
		''' Gets or sets time of mass import in milliseconds.
		''' </summary>
		''' <returns>Time of mass import in milliseconds.</returns>
		Public Property MassImportDurationMilliseconds As Double
			Get
				Return GetValueOrDefault(Of Double)(TelemetryConstants.KeyName.MASS_IMPORT_DURATION)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.MASS_IMPORT_DURATION) = Value
			End Set
		End Property

		''' <summary>
		''' Gets or sets number of records in batch.
		''' </summary>
		''' <returns>Number of records in batch.</returns>
		Public Property NumberOfRecords As Integer
			Get
				Return GetValueOrDefault(Of Integer)(TelemetryConstants.KeyName.NUMBER_OF_RECORDS)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.NUMBER_OF_RECORDS) = Value
			End Set
		End Property
	End Class
End NameSpace