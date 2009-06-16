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
					If Not _configDictionary.Contains("EnableSingleModeImport") Then _configDictionary.Add("EnableSingleModeImport", "False")
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


		Public Shared ReadOnly Property FileTransferModeExplanationText(ByVal includeBulk As Boolean) As String
			Get
				Dim sb As New System.Text.StringBuilder
				sb.Append("FILE TRANSFER MODES:" & vbNewLine)
				sb.Append(" � Web � ")
				sb.Append(vbNewLine & "The document repository is accessed through the Relativity web service API.  This is the slower of the two methods, but is globally available.")
				sb.Append(vbNewLine & vbNewLine)
				sb.Append(" � Direct � ")
				sb.Append(vbNewLine)
				sb.Append("Direct mode is significantly faster than Web mode.  To use Direct mode, you must:")
				sb.Append(vbNewLine & vbNewLine)
				sb.Append(" - Have Windows rights to the document repository.")
				sb.Append(vbNewLine)
				sb.Append(" - Be logged into the document repository�s network.")
				sb.Append(vbNewLine & vbNewLine & "If you meet the above criteria, Relativity will automatically load in Direct mode.  If you are loading in Web mode and think you should have Direct mode, contact your Relativity Administrator to establish the correct rights.")
				sb.Append(vbNewLine & vbNewLine)
				If includeBulk Then
					sb.Append("SQL INSERT MODES:" & vbNewLine)
					sb.Append(" � Bulk � " & vbNewLine)
					sb.Append("The upload process has access to the SQL share on the appropriate case database.  This ensures the fastest transfer of information between the desktop client and the relativity servers.")
					sb.Append(vbNewLine & vbNewLine)
					sb.Append(" � Single �" & vbNewLine)
					sb.Append("The upload process has NO access to the SQL share on the appropriate case database.  This is a slower method of import. If the process is using single mode, contact your Relativity Database Administrator to see if a SQL share can be opened for the desired case.")
				End If
				Return sb.ToString
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

		Public Shared ReadOnly Property DisableImageTypeValidation() As Boolean
			Get
				Return CType(ConfigSettings("DisableImageTypeValidation"), Boolean)
			End Get
		End Property

		Public Shared ReadOnly Property DisableImageLocationValidation() As Boolean
			Get
				Return CType(ConfigSettings("DisableImageLocationValidation"), Boolean)
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

		Public Shared ReadOnly Property EnableSingleModeImport() As Boolean
			Get
				Return CType(ConfigSettings("EnableSingleModeImport"), Boolean)
			End Get
		End Property

	End Class
End Namespace
