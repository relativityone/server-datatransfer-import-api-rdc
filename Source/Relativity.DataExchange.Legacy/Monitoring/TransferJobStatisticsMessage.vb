Namespace Monitoring
	Public Class TransferJobStatisticsMessage
		Inherits TransferJobMessageBase

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property BucketName As String = TelemetryConstants.BucketName.JOB_STATISTICS
        
        ''' <summary>
        ''' Gets or sets total job size in bytes. This value is equal to sum of <see cref="MetadataBytes"/> and <see cref="FileBytes"/>.
        ''' </summary>
        ''' <returns>Total job size in bytes</returns>
		Public Property JobSizeInBytes As Double
			Get
				Return GetValueOrDefault (Of Double)(TelemetryConstants.KeyName.JOB_SIZE_IN_BYTES)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.JOB_SIZE_IN_BYTES) = Value
			End Set
		End Property

        ''' <summary>
        ''' Gets or sets metadata size in bytes.
        ''' </summary>
        ''' <returns>Metadata size in bytes</returns>
		Public Property MetadataBytes As Long
			Get
				Return GetValueOrDefault (Of Long)(TelemetryConstants.KeyName.METADATA_BYTES)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.METADATA_BYTES) = Value
			End Set
		End Property

        ''' <summary>
        ''' Gets or sets files size in bytes.
        ''' </summary>
        ''' <returns>Files size in bytes</returns>
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