Imports System.Collections.Generic
Imports Relativity.Telemetry.DataContracts.Shared

Namespace Monitoring
	Public Class MetricJobProgress
		Inherits MetricBase

        ''' <inheritdoc/>
        Public Overrides ReadOnly Property BucketName As String = TelemetryConstants.BucketName.METRIC_JOB_PROGRESS

        ''' <inheritdoc/>
        Public Overrides Function GenerateSumMetrics() As List(Of MetricRef)
            Return New List(Of MetricRef)
        End Function

        ''' <summary>
        ''' Gets or sets active file transfer rate in bytes per second
        ''' </summary>
        ''' <returns>File throughput in bytes per second</returns>
		Public Property FileThroughput As Double
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