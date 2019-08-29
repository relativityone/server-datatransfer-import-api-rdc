Namespace Monitoring
	Public Class TransferJobProgressMessage
		Inherits TransferJobMessageBase

        Public Overrides ReadOnly Property BucketName As String = TelemetryConstants.BucketName.PROGRESS

		Public Property FileThroughput As Double
			Get
				Return GetValueOrDefault (Of Double)(TelemetryConstants.KeyName.FILE_THROUGHPUT)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.FILE_THROUGHPUT) = Value
			End Set
		End Property

		Public Property MetadataThroughput As Double
			Get
				Return GetValueOrDefault (Of Double)(TelemetryConstants.KeyName.METADATA_THROUGHPUT)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.METADATA_THROUGHPUT) = Value
			End Set
		End Property
	End Class
End Namespace