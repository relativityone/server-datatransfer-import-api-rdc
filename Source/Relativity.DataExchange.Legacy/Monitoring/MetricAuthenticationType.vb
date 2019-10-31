Namespace Monitoring
	Public Class MetricAuthenticationType
		Inherits MetricBase

		''' <inheritdoc/>
		Public Overrides ReadOnly Property BucketName As String = TelemetryConstants.BucketName.METRIC_AUTHENTICATION_TYPE

		''' <summary>
		''' Gets or sets authentication method used by ImportAPI client to login.
		''' </summary>
		''' <returns>Authentication method.</returns>
		Public Property AuthenticationMethod As TelemetryConstants.AuthenticationMethod
			Get
				Return GetValueOrDefault(Of TelemetryConstants.AuthenticationMethod)(TelemetryConstants.KeyName.AUTHENTICATION_METHOD)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.AUTHENTICATION_METHOD) = Value
		    End Set
		End Property

		''' <summary>
		''' Gets or sets system from which ImportAPI client login.
		''' </summary>
		''' <returns>System from which client login.</returns>
		Public Property SystemType As String
			Get
				Return GetValueOrDefault(Of String)(TelemetryConstants.KeyName.SYSTEM_TYPE)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.SYSTEM_TYPE) = Value
			End Set
		End Property

		''' <summary>
		''' Gets or sets sub system from which ImportAPI client login.
		''' </summary>
		''' <returns>Sub system from which client login.</returns>
		Public Property SubSystemType As String
			Get
				Return GetValueOrDefault(Of String)(TelemetryConstants.KeyName.SUB_SYSTEM_TYPE)
			End Get
			Set
				CustomData.Item(TelemetryConstants.KeyName.SUB_SYSTEM_TYPE) = Value
			End Set
		End Property
	End Class
End NameSpace