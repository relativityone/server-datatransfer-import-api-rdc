Imports System.Collections.Generic
Imports Relativity.DataTransfer.MessageService
Imports Relativity.DataTransfer.MessageService.MetricsManager.APM

Namespace Monitoring
    Public MustInherit class TransferJobMessageBase
        Implements IMessage, IMetricMetadata

        Public Sub New()
            CustomData = New Dictionary(Of String, Object)
        End Sub

        ''' <summary>
        ''' Gets a bucket name of a metric.
        ''' </summary>
        ''' <returns>Bucket name</returns>
        Public MustOverride ReadOnly Property BucketName As String

        ''' <summary>
        ''' Gets or sets job type - Import or Export.
        ''' </summary>
        ''' <returns>Job type</returns>
        Public Property JobType As String
            Get
                Return GetValueOrDefault(Of String)(TelemetryConstants.KeyName.JOB_TYPE)
            End Get
            Set
                CustomData.Item(TelemetryConstants.KeyName.JOB_TYPE) = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets transfer mode - Aspera, Web or Direct.
        ''' </summary>
        ''' <returns>Transfer mode</returns>
        Public Property TransferMode As String
            Get
                Return GetValueOrDefault(Of String)(TelemetryConstants.KeyName.TRANSFER_MODE)
            End Get
            Set
                CustomData.Item(TelemetryConstants.KeyName.TRANSFER_MODE) = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets name of application that executes a job.
        ''' </summary>
        ''' <returns>Application name</returns>
        Public Property ApplicationName As String
            Get
                Return GetValueOrDefault(Of String)(TelemetryConstants.KeyName.APPLICATION_NAME)
            End Get
            Set
                CustomData.Item(TelemetryConstants.KeyName.APPLICATION_NAME) = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets correlation ID - unique job identifier.
        ''' </summary>
        ''' <returns>Unique ID of a job</returns>
        Public Property CorrelationID As String Implements IMetricMetadata.CorrelationID

        ''' <summary>
        ''' Gets or sets dictionary containing additional properties of a metric.
        ''' </summary>
        ''' <returns>Dictionary with metric properties</returns>
        Public Property CustomData As Dictionary(Of String, Object) Implements IMetricMetadata.CustomData

        ''' <summary>
        ''' Gets or sets ID of workspace on which job is executed.
        ''' </summary>
        ''' <returns>Workspace identifier</returns>
        Public Property WorkspaceID As Integer Implements IMetricMetadata.WorkspaceID

        ''' <summary>
        ''' Gets or sets a string value to describe what the metric's Value property is.
        ''' </summary>
        ''' <returns>Unit of measure</returns>
        Public Property UnitOfMeasure As String Implements IMetricMetadata.UnitOfMeasure
    end class
End NameSpace