Imports System.Collections.Generic

Namespace Monitoring
    Public MustInherit class MetricBase

        Public Sub New()
            CustomData = New Dictionary(Of String, Object)
        End Sub

        ''' <summary>
        ''' Gets the bucket name of a metric.
        ''' </summary>
        ''' <returns>Bucket name</returns>
        Public MustOverride ReadOnly Property BucketName As String

        ''' <summary>
        ''' Gets or sets dictionary containing additional properties of a metric.
        ''' </summary>
        ''' <returns>Dictionary with metric properties</returns>
        Public Property CustomData As Dictionary(Of String, Object)

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
        ''' Gets or sets value indicating whether old export was used to perform job
        ''' </summary>
        ''' <returns>True if old export was used, False otherwise</returns>
        Public Property UseOldExport As Boolean
            Get
                Return GetValueOrDefault(Of Boolean)(TelemetryConstants.KeyName.USE_OLD_EXPORT)
            End Get
            Set
                CustomData.Item(TelemetryConstants.KeyName.USE_OLD_EXPORT) = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets correlation ID - unique job identifier.
        ''' </summary>
        ''' <returns>Unique ID of a job</returns>
        Public Property CorrelationID As String
            Get
                Return GetValueOrDefault(Of String)(TelemetryConstants.KeyName.CORRELATION_ID)
            End Get
            Set
                CustomData.Item(TelemetryConstants.KeyName.CORRELATION_ID) = Value
            End Set
        End Property
        
        ''' <summary>
        ''' Gets or sets ID of workspace on which job is executed.
        ''' </summary>
        ''' <returns>Workspace identifier</returns>
        Public Property WorkspaceID As Integer
            Get
                Return GetValueOrDefault(Of Integer)(TelemetryConstants.KeyName.WORKSPACE_ID)
            End Get
            Set
                CustomData.Item(TelemetryConstants.KeyName.WORKSPACE_ID) = Value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets a string value to describe what the metric's value property is.
        ''' </summary>
        ''' <returns>Unit of measure</returns>
        Public Property UnitOfMeasure As String
            Get
                Return GetValueOrDefault(Of String)(TelemetryConstants.KeyName.UNIT_OF_MEASURE)
            End Get
            Set
                CustomData.Item(TelemetryConstants.KeyName.UNIT_OF_MEASURE) = Value
            End Set
        End Property

        Protected Function GetValueOrDefault(Of T)(ByVal key As String) As T
            Dim value As Object = Nothing
            Return If(Me.CustomData.TryGetValue(key, value), CType(value, T), Nothing)
        End Function
    end class
End NameSpace