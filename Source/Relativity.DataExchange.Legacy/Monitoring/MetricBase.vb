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

		''' <summary>
		''' Gets or sets version of Import API.
		''' </summary>
		''' <returns>Version of Import API as string.</returns>
        Public Property ImportApiVersion As String
            Get
                Return GetValueOrDefault(Of String)(TelemetryConstants.KeyName.IMPORT_API_VERSION)
            End Get
            Set
                CustomData.Item(TelemetryConstants.KeyName.IMPORT_API_VERSION) = value
            End Set
        End Property

        ''' <summary>
        ''' Gets or sets version Relativity.
        ''' </summary>
        ''' <returns>Version of Relativity as string.</returns>
        Public Property RelativityVersion As String
            Get
                Return GetValueOrDefault(Of String)(TelemetryConstants.KeyName.RELATIVITY_VERSION)
            End Get
            Set
                CustomData.Item(TelemetryConstants.KeyName.RELATIVITY_VERSION) = value
            End Set
        End Property

        Protected Function GetValueOrDefault(Of T)(ByVal key As String) As T
            Dim value As Object = Nothing
            Return If(Me.CustomData.TryGetValue(key, value), CType(value, T), Nothing)
        End Function
    end class
End NameSpace