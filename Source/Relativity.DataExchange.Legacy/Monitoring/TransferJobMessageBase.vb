Imports System.Collections.Generic
Imports Relativity.DataTransfer.MessageService
Imports Relativity.DataTransfer.MessageService.MetricsManager.APM

Namespace Monitoring
    Public MustInherit class TransferJobMessageBase
        Implements IMessage, IMetricMetadata

        Public Sub New()
            CustomData = New Dictionary(Of String, Object)
        End Sub

        Public MustOverride ReadOnly Property BucketName As String

        Public Property JobType As String
            Get
                Return GetValueOrDefault(Of String)(TelemetryConstants.KeyName.JOB_TYPE)
            End Get
            Set
                CustomData.Item(TelemetryConstants.KeyName.JOB_TYPE) = Value
            End Set
        End Property

        Public Property TransferMode As String
            Get
                Return GetValueOrDefault(Of String)(TelemetryConstants.KeyName.TRANSFER_MODE)
            End Get
            Set
                CustomData.Item(TelemetryConstants.KeyName.TRANSFER_MODE) = Value
            End Set
        End Property

        Public Property ApplicationName As String
            Get
                Return GetValueOrDefault(Of String)(TelemetryConstants.KeyName.APPLICATION_NAME)
            End Get
            Set
                CustomData.Item(TelemetryConstants.KeyName.APPLICATION_NAME) = Value
            End Set
        End Property

        Public Property CorrelationID As String Implements IMetricMetadata.CorrelationID
        Public Property CustomData As Dictionary(Of String, Object) Implements IMetricMetadata.CustomData
        Public Property WorkspaceID As Integer Implements IMetricMetadata.WorkspaceID
        Public Property UnitOfMeasure As String Implements IMetricMetadata.UnitOfMeasure

    end class
End NameSpace