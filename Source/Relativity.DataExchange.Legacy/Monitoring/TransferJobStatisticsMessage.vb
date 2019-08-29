Namespace Monitoring
	Public Class TransferJobStatisticsMessage
		Inherits TransferJobMessageBase

        Public Overrides ReadOnly Property BucketName As String = TelemetryConstants.BucketName.JOB_STATISTICS
        
		Public Property JobSizeInBytes As Double
			Get
				Return GetValueOrDefault (Of Double)(TelemetryConstants.KeyName.JOB_SIZE_IN_BYTES)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.JOB_SIZE_IN_BYTES) = Value
			End Set
		End Property

		Public Property MetadataBytes As Long
			Get
				Return GetValueOrDefault (Of Long)(TelemetryConstants.KeyName.METADATA_BYTES)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.METADATA_BYTES) = Value
			End Set
		End Property

		Public Property FileBytes As Long
			Get
				Return GetValueOrDefault (Of Long)(TelemetryConstants.KeyName.FILE_BYTES)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.FILE_BYTES) = Value
			End Set
		End Property

	End Class
End Namespace