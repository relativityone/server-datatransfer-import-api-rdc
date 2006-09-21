Imports System.Configuration
Namespace kCura.WinEDDS
	Public Class Config

		Private Shared _configDictionary As System.Collections.IDictionary

		Public Shared ReadOnly Property ConfigSettings() As System.Collections.IDictionary
			Get
				If _configDictionary Is Nothing Then
					_configDictionary = DirectCast(System.Configuration.ConfigurationSettings.GetConfig("kCura.WinEDDS"), System.Collections.IDictionary)
				End If
				Return _configDictionary
			End Get
		End Property

		Public Shared ReadOnly Property UsesWebAPI() As Boolean
			Get
				Return CType(ConfigSettings("UsesWebAPI"), Boolean)
			End Get
		End Property

		Public Shared ReadOnly Property OutlookImporterLocation() As String
			Get
				Return CType(ConfigSettings("OutlookImporterLocation"), String)
			End Get
		End Property

		Public Shared ReadOnly Property URI() As String
			Get
				Return String.Format(CType(ConfigSettings("uriFormatString"), String), Host, Protocol)
			End Get
		End Property

		Public Shared ReadOnly Property Host() As String
			Get
				Return CType(ConfigSettings("Host"), String)
			End Get
		End Property

		Public Shared ReadOnly Property HostURL() As String
			Get
				Return String.Format("{1}://{0}/", Host, Protocol)
			End Get
		End Property

		Public Shared ReadOnly Property SearchExportChunkSize() As Int32
			Get
				Return CType(ConfigSettings("SearchExportChunkSize"), Int32)
			End Get
		End Property

		Public Shared ReadOnly Property Protocol() As String
			Get
				Return String.Format(CType(ConfigSettings("Protocol"), String), Host)
			End Get
		End Property

		Public Shared Property WebServiceURL() As String
			Get
				Dim value As String = Config.GetRegistryKeyValue("WebServiceURL")
				'If value = "" Then
				'	Config.SetRegistryKeyValue("WebServiceURL", CType(ConfigSettings("WebServiceURL"), String))
				'	value = Config.GetRegistryKeyValue("WebServiceURL")
				'End If
				'Return value
				Return value
			End Get
			Set(ByVal Value As String)
				Config.SetRegistryKeyValue("WebServiceURL", Value)
			End Set
		End Property

#Region "Registry Helpers"

		Private Shared Function GetRegistryKeyValue(ByVal keyName As String) As String
			Dim regKey As Microsoft.Win32.RegistryKey = Config.GetRegistryKey(False)
			Dim value As String = CType(regKey.GetValue(keyName, ""), String)
			regKey.Close()
			Return value
		End Function

		Private Shared Function SetRegistryKeyValue(ByVal keyName As String, ByVal keyVal As String) As String
			Dim regKey As Microsoft.Win32.RegistryKey = Config.GetRegistryKey(True)
			regKey.SetValue(keyName, keyVal)
			regKey.Close()
		End Function

		Private Shared ReadOnly Property GetRegistryKey(ByVal write As Boolean) As Microsoft.Win32.RegistryKey
			Get
				Dim regKey As Microsoft.Win32.RegistryKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("software\kCura\Relativity", write)
				If regKey Is Nothing Then
					Microsoft.Win32.Registry.CurrentUser.CreateSubKey("software\\kCura\\Relativity")
					regKey = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("software\kCura\Relativity", write)
				End If
				Return regKey
			End Get
		End Property

#End Region

	End Class
End Namespace
