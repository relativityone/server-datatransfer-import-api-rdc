Namespace Monitoring
	Public Class MetricJobProgress
		Inherits MetricBase

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property BucketName As String = TelemetryConstants.BucketName.METRIC_JOB_PROGRESS

        ''' <summary>
        ''' Gets or sets active file transfer rate in bytes per second
        ''' </summary>
        ''' <returns>File throughput in bytes per second</returns>
		Public Property FileThroughputBytesPerSecond As Double
			Get
				Return GetValueOrDefault (Of Double)(TelemetryConstants.KeyName.FILE_THROUGHPUT)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.FILE_THROUGHPUT) = Value
			End Set
		End Property

        ''' <summary>
        ''' Gets or sets active metadata transfer rate in bytes per second
        ''' </summary>
        ''' <returns>Metadata throughput in bytes per second</returns>
		Public Property MetadataThroughputBytesPerSecond As Double
			Get
				Return GetValueOrDefault (Of Double)(TelemetryConstants.KeyName.METADATA_THROUGHPUT)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.METADATA_THROUGHPUT) = Value
			End Set
		End Property

		''' <summary>
		''' Gets or sets active sql bulk load throughput in records per second.
		''' </summary>
		''' <returns>Sql bulk load throughput in records per second.</returns>
		Public Property SqlBulkLoadThroughputRecordsPerSecond As Double
			Get
				Return GetValueOrDefault (Of Double)(TelemetryConstants.KeyName.SQL_THROUGHPUT)
			End Get
		    Set
				CustomData(TelemetryConstants.KeyName.SQL_THROUGHPUT) = Value
		    End Set
		End Property
	End Class
End Namespace