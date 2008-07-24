Imports System.Configuration
Namespace kCura.WinEDDS
	Public Class Config

#Region " ConfigSettings "

		Private Shared _configDictionary As System.Collections.IDictionary
		Public Shared ReadOnly Property ConfigSettings() As System.Collections.IDictionary
			Get
				If _configDictionary Is Nothing Then
					_configDictionary = DirectCast(System.Configuration.ConfigurationSettings.GetConfig("kCura.WinEDDS"), System.Collections.IDictionary)
					If Not _configDictionary.Contains("ImportBatchSize") Then _configDictionary.Add("ImportBatchSize", "1000")
					If Not _configDictionary.Contains("ImportBatchMaxVolume") Then _configDictionary.Add("ImportBatchMaxVolume", "10485760") '10(2^20) - don't know what 10MB standard is
					If Not _configDictionary.Contains("ExportBatchSize") Then _configDictionary.Add("ExportBatchSize", "1000")
				End If
				Return _configDictionary
			End Get
		End Property

#End Region

#Region " Unused or shouldn't be used "

		Public Shared ReadOnly Property UsesWebAPI() As Boolean
			Get
				Return True				'Return CType(ConfigSettings("UsesWebAPI"), Boolean)
			End Get
		End Property

		Public Shared ReadOnly Property OutlookImporterLocation() As String
			Get
				Return ""		 'CType(ConfigSettings("OutlookImporterLocation"), String)
			End Get
		End Property

#End Region

#Region " Constants "

		Public Shared ReadOnly Property MaxReloginTries() As Int32
			Get
				Return 20
			End Get
		End Property

		Public Shared ReadOnly Property WaitBeforeReconnect() As Int32		'Millisecodns
			Get
				Return 2000
			End Get
		End Property

#End Region

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

		Public Shared ReadOnly Property ImportBatchMaxVolume() As Int32		'Volume in bytes
			Get
				Return CType(ConfigSettings("ImportBatchMaxVolume"), Int32)
			End Get
		End Property

		Public Shared ReadOnly Property ImportBatchSize() As Int32		'Number of records
			Get
				Return CType(ConfigSettings("ImportBatchSize"), Int32)
			End Get
		End Property

		Public Shared ReadOnly Property ExportBatchSize() As Int32		'Number of records
			Get
				Return CType(ConfigSettings("ExportBatchSize"), Int32)
			End Get
		End Property

		Public Shared Property WebServiceURL() As String
			Get
				Return Config.GetRegistryKeyValue("WebServiceURL")
			End Get
			Set(ByVal value As String)
				Config.SetRegistryKeyValue("WebServiceURL", value)
			End Set
		End Property

	End Class
End Namespace
