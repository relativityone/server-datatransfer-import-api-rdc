Namespace kCura.EDDS.WinForm
	Public Class Config

		Private Shared _configDictionary As System.Collections.IDictionary
		Private Shared _dataConfigDictionary As System.Collections.IDictionary
		Private Shared _eddsConfigDictionary As System.Collections.IDictionary

		Public Shared ReadOnly Property ConfigSettings() As System.Collections.IDictionary
			Get
				If _configDictionary Is Nothing Then
					_configDictionary = kCura.Config.Manager.GetConfig("kCura.EDDS.WinForm", New ConfigDictionaryFactory)
				End If
				Return _configDictionary
			End Get
		End Property

		''' <summary>
		''' Gets the collection of configuration settings for data transfer with a server. These settings are
		''' necessary for the WinEDDS client to correctly communicate with the server.
		''' </summary>
		Public Shared ReadOnly Property DataConfigSettings() As System.Collections.IDictionary
			Get
				If _dataConfigDictionary Is Nothing Then
					_dataConfigDictionary = kCura.Config.Manager.GetConfig("Relativity.Data", New ConfigDictionaryFactory)
				End If
				Return _dataConfigDictionary
			End Get
		End Property

		''' <summary>
		''' Gets the collection of configuration settings for data transfer with a server. These settings are
		''' necessary for the WinEDDS client to correctly communicate with the server.
		''' </summary>
		Public Shared ReadOnly Property EddsConfigSettings() As System.Collections.IDictionary
			Get
				If _eddsConfigDictionary Is Nothing Then
					_eddsConfigDictionary = kCura.Config.Manager.GetConfig("Relativity.Core", New ConfigDictionaryFactory)
				End If
				Return _eddsConfigDictionary
			End Get
		End Property

		Public Shared Sub FlushEddsConfigSettings()
			_eddsConfigDictionary = kCura.Config.Manager.GetConfig("Relativity.Core", New ConfigDictionaryFactory)
		End Sub

#Region " ConfigDictionaryFactory "

		Public Class ConfigDictionaryFactory
			Implements kCura.Config.IConfigDictionaryFactory
			Public Function GetDictionary(ByVal sectionName As String, ByVal collection As kCura.Config.Collection) As kCura.Config.DictionaryBase Implements kCura.Config.IConfigDictionaryFactory.GetDictionary
				Return New ConfigDictionary(sectionName, collection)
			End Function
		End Class

#End Region

		''' <summary>
		''' Gets a string that is used to split fields in bulk load files. Line delimiters are this
		''' value plus a line feed.
		''' </summary>
		''' <exception cref="kCura.Config.ConfigurationException">
		''' Thrown if the BulkLoadFileFieldDelimiter configuration setting is not set
		''' </exception>
		Public Shared ReadOnly Property BulkLoadFileFieldDelimiter() As String
			Get
				Dim delimiter As String = CType(DataConfigSettings("BulkLoadFileFieldDelimiter"), String)

				If (String.IsNullOrEmpty(delimiter)) Then
					Throw New kCura.Config.ConfigurationException("BulkLoadFileFieldDelimiter is not set")
				Else
					Return delimiter
				End If
			End Get
		End Property

		Public Shared ReadOnly Property ExportVolumeDigitPadding() As Int32
			Get
				Return CType(ConfigSettings("ExportVolumeDigitPadding"), Int32)
			End Get
		End Property

		Public Shared ReadOnly Property CloudInstance() As Boolean
			Get
				Return CType(EddsConfigSettings("CloudInstance"), Boolean)
			End Get
		End Property

		Public Shared ReadOnly Property EnforceDocumentLimit() As Boolean
			Get
				Return CType(EddsConfigSettings("EnforceDocumentLimit"), Boolean)
			End Get
		End Property

		Public Shared ReadOnly Property ExportSubdirectoryDigitPadding() As Int32
			Get
				Return CType(ConfigSettings("ExportSubdirectoryDigitPadding"), Int32)
			End Get
		End Property

		Public Shared ReadOnly Property CopyFilesToRepository() As Boolean
			Get
				Return CType(ConfigSettings("CopyFilesToRepository"), Boolean)
			End Get
		End Property

		Public Shared ReadOnly Property SendNotificationOnImportCompletionByDefault() As Boolean
			Get
				Return CType(ConfigSettings("SendNotificationOnImportCompletionByDefault"), Boolean)
			End Get
		End Property

		Private Shared _rdcMetricsThrottlingSeconds As String = "MetricsThrottlingSeconds"

		Public Shared ReadOnly Property RdcMetricsThrottlingSeconds() As Int32
			Get
				Return GetConfigWithDefault(_RdcMetricsThrottlingSeconds, 30)
			End Get
		End Property
		
		Private Shared _rdcMetricsConfiguration As String = "MetricsConfiguration"

		Public Shared ReadOnly Property SendLiveApmMetrics As Boolean
			Get
				Dim metricsConfig As Integer = GetConfigWithDefault(_rdcMetricsConfiguration, Integer.MaxValue)
				Return (metricsConfig And Metrics.LiveApmMetrics) > 0
			End Get
		End Property

		Public Shared ReadOnly Property SendSumMetrics As Boolean
			Get
				Dim metricsConfig As Integer = GetConfigWithDefault(_rdcMetricsConfiguration, Integer.MaxValue)
				Return (metricsConfig And Metrics.SumMetrics) > 0
			End Get
		End Property

		Public Shared ReadOnly Property SendSummaryApmMetrics As Boolean
			Get
				Dim metricsConfig As Integer = GetConfigWithDefault(_rdcMetricsConfiguration, Integer.MaxValue)
				Return (metricsConfig And Metrics.SummaryApmMetrics) > 0
			End Get
		End Property

		Private Shared Function GetConfigWithDefault(Of T)(name As String, defaultValue As T) As T
			If Not ConfigSettings.Contains(name) Then
				ConfigSettings.Add(name, defaultValue)
			End If
			Return CType(ConfigSettings(name), T)
		End Function

		Private Enum Metrics
			LiveApmMetrics = 1
			SummaryApmMetrics = 2
			SumMetrics = 4
		End Enum


#If EnableInjections Then

		Private Shared _config As IDictionary
		''' <summary>
		''' For testing purposes only: this value, if populated, will be used to pad out the workspace list in the CaseSelectForm so that functional testing can happen with a large number of workspaces w/o actually creating a large number of workspaces in an instance.
		''' </summary>
		Public Shared ReadOnly Property NumberOfFakeWorkspacesToAdd As Int32?
			Get
				Dim retval As Int32?
				Try
					If _config Is Nothing Then
						_config = DirectCast(System.Configuration.ConfigurationManager.GetSection("kCura.WinEDDS"), IDictionary)
					End If
					retval = CInt(_config("NumberOfFakeWorkspacesToAdd"))
				Catch
					_config = New System.Collections.Generic.Dictionary(Of String, Int32)
					_config.Add("NumberOfFakeWorkspacesToAdd", 0)
					retval = 0
				End Try
				Return retval
			End Get
		End Property
#End If

	End Class
End Namespace
