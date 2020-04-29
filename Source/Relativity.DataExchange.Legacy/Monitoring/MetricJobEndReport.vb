Imports Relativity.DataExchange.Transfer

Namespace Monitoring
	Public Class MetricJobEndReport
		Inherits MetricJobBase

		''' <inheritdoc/>
		Public Overrides ReadOnly Property BucketName As String = TelemetryConstants.BucketName.METRIC_JOB_END_REPORT

		''' <summary>
		''' Gets or sets job status - <see cref="TelemetryConstants.JobStatus"/>
		''' </summary>
		''' <returns>Job status</returns>
		Public Property JobStatus() As TelemetryConstants.JobStatus
			Get
				Return GetValueOrDefault (Of TelemetryConstants.JobStatus)(TelemetryConstants.KeyName.JOB_STATUS)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.JOB_STATUS) = Value
			End Set
		End Property

		''' <summary>
		''' Gets or sets total job size in bytes. This value is equal to sum of <see cref="MetadataSizeBytes"/> and <see cref="FileSizeBytes"/>.
		''' </summary>
		''' <returns>Total job size in bytes</returns>
		Public Property TotalSizeBytes() As Long
			Get
				Return GetValueOrDefault (Of Long)(TelemetryConstants.KeyName.TOTAL_SIZE_BYTES)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.TOTAL_SIZE_BYTES) = Value
			End Set
		End Property

		''' <summary>
		''' Gets or sets total size of all files in bytes.
		''' </summary>
		''' <returns>Total size of all files in bytes</returns>
		Public Property FileSizeBytes() As Long
			Get
				Return GetValueOrDefault (Of Long)(TelemetryConstants.KeyName.FILE_SIZE_BYTES)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.FILE_SIZE_BYTES) = Value
			End Set
		End Property

		''' <summary>
		''' Gets or sets metadata size in bytes.
		''' </summary>
		''' <returns>Metadata size in bytes</returns>
		Public Property MetadataSizeBytes As Long
			Get
				Return GetValueOrDefault (Of Long)(TelemetryConstants.KeyName.METADATA_SIZE_BYTES)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.METADATA_SIZE_BYTES) = Value
			End Set
		End Property

		''' <summary>
		''' Gets or sets total number of records to process in a job
		''' </summary>
		''' <returns>Total number of records</returns>
		Public Property TotalRecords As Long
			Get
				Return GetValueOrDefault (Of Long)(TelemetryConstants.KeyName.TOTAL_RECORDS)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.TOTAL_RECORDS) = Value
			End Set
		End Property

		''' <summary>
		''' Gets or sets the number of successfully processed records
		''' </summary>
		''' <returns>Number of completed records</returns>
		Public Property CompletedRecords As Long
			Get
				Return GetValueOrDefault (Of Long)(TelemetryConstants.KeyName.COMPLETED_RECORDS)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.COMPLETED_RECORDS) = Value
			End Set
		End Property

		''' <summary>
		''' Gets or sets total number of records processed with errors.
		''' </summary>
		''' <returns>The total number of records processed with errors.</returns>
		Public Property RecordsWithErrors As Long
			Get
				Return GetValueOrDefault (Of Long)(TelemetryConstants.KeyName.RECORDS_WITH_ERRORS)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.RECORDS_WITH_ERRORS) = Value
			End Set
		End Property

		''' <summary>
		''' Gets or sets total transfer rate in bytes per second
		''' </summary>
		''' <returns>Transfer rate in bytes per second</returns>
		Public Property ThroughputBytesPerSecond As Double
			Get
				Return GetValueOrDefault (Of Double)(TelemetryConstants.KeyName.THROUGHPUT_BYTES_PER_SECOND)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.THROUGHPUT_BYTES_PER_SECOND) = Value
			End Set
		End Property

		''' <summary>
		''' Gets or sets total transfer rate in records per second
		''' </summary>
		''' <returns>Transfer rate in records per second</returns>
		Public Property ThroughputRecordsPerSecond As Double
			Get
				Return GetValueOrDefault (Of Double)(TelemetryConstants.KeyName.THROUGHPUT_RECORDS_PER_SECOND)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.THROUGHPUT_RECORDS_PER_SECOND) = Value
			End Set
		End Property

		''' <summary>
		''' Gets or sets sql bulk load throughput in records per second.
		''' </summary>
		''' <returns>Sql bulk load throughput in records per second.</returns>
		Public Property SqlBulkLoadThroughputRecordsPerSecond As Double
			Get
				Return GetValueOrDefault (Of Double)(TelemetryConstants.KeyName.SQL_THROUGHPUT)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.SQL_THROUGHPUT) = Value
			End Set
		End Property

		''' <summary>
		''' Gets or sets job duration in seconds.
		''' </summary>
		''' <returns>Job duration in seconds.</returns>
		Public Property JobDurationInSeconds As Double
			Get
				Return GetValueOrDefault(Of Double)(TelemetryConstants.KeyName.JOB_DURATION)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.JOB_DURATION) = Value
			End Set
		End Property

		''' <summary>
		''' Gets or sets initial transfer mode.
		''' </summary>
		''' <returns>Initial transfer mode.</returns>
		Public Property InitialTransferMode As TapiClient
			Get
				Return GetValueOrDefault(Of TapiClient)(TelemetryConstants.KeyName.INITIAL_TRANSFER_MODE)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.INITIAL_TRANSFER_MODE) = Value
			End Set
		End Property

		''' <summary>
		''' Gets or sets start timestamp of the job.
		''' </summary>
		''' <returns>Start timestamp in epoch format.</returns>
		Public Property JobStartTimeStamp As Double
			Get
				Return GetValueOrDefault(Of Double)(TelemetryConstants.KeyName.JOB_START_TIMESTAMP)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.JOB_START_TIMESTAMP) = Value
			End Set
		End Property

		''' <summary>
		''' Gets or sets end timestamp of the job.
		''' </summary>
		''' <returns>End timestamp in epoch format.</returns>
		Public Property JobEndTimeStamp As Double
			Get
				Return GetValueOrDefault(Of Double)(TelemetryConstants.KeyName.JOB_END_TIMESTAMP)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.JOB_END_TIMESTAMP) = Value
			End Set
		End Property

		''' <summary>
		''' Gets or sets the server side job run ID.
		''' </summary>
		''' <returns>Job run ID.</returns>
		Public Property JobRunId As String
			Get
				Return GetValueOrDefault(Of String)(TelemetryConstants.KeyName.JOB_RUN_ID)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.JOB_RUN_ID) = Value
			End Set
		End Property
	End Class
End NameSpace