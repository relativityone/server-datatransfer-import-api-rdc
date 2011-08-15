Imports System.Configuration
Namespace kCura.WinEDDS
	Public Class Config

#Region " ConfigSettings "

		Private Shared _configDictionary As System.Collections.IDictionary
		Public Shared ReadOnly Property ConfigSettings() As System.Collections.IDictionary
			Get
				If _configDictionary Is Nothing Then
					_configDictionary = DirectCast(System.Configuration.ConfigurationManager.GetSection("kCura.WinEDDS"), System.Collections.IDictionary)
					If _configDictionary Is Nothing Then _configDictionary = New System.Collections.Hashtable
					If Not _configDictionary.Contains("ImportBatchSize") Then _configDictionary.Add("ImportBatchSize", "1000")
					If Not _configDictionary.Contains("AutoBatchOn") Then _configDictionary.Add("AutoBatchOn", "False")
					If Not _configDictionary.Contains("MinimumBatchSize") Then _configDictionary.Add("MinimumBatchSize", "100")
					If Not _configDictionary.Contains("WaitTimeBetweenRetryAttempts") Then _configDictionary.Add("WaitTimeBetweenRetryAttempts", "30")
					If Not _configDictionary.Contains("ImportBatchMaxVolume") Then _configDictionary.Add("ImportBatchMaxVolume", "10485760") '10(2^20) - don't know what 10MB standard is
					If Not _configDictionary.Contains("ExportBatchSize") Then _configDictionary.Add("ExportBatchSize", "1000")
					If Not _configDictionary.Contains("EnableSingleModeImport") Then _configDictionary.Add("EnableSingleModeImport", "False")
					If Not _configDictionary.Contains("CreateErrorForEmptyNativeFile") Then _configDictionary.Add("CreateErrorForEmptyNativeFile", "False")
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

		Public Const PREVIEW_THRESHOLD As Int32 = 1000

		Public Shared ReadOnly Property FileTransferModeExplanationText(ByVal includeBulk As Boolean) As String
			Get
				Dim sb As New System.Text.StringBuilder
				sb.Append("FILE TRANSFER MODES:" & vbNewLine)
				sb.Append(" • Web • ")
				sb.Append(vbNewLine & "The document repository is accessed through the Relativity web service API.  This is the slower of the two methods, but is globally available.")
				sb.Append(vbNewLine & vbNewLine)
				sb.Append(" • Direct • ")
				sb.Append(vbNewLine)
				sb.Append("Direct mode is significantly faster than Web mode.  To use Direct mode, you must:")
				sb.Append(vbNewLine & vbNewLine)
				sb.Append(" - Have Windows rights to the document repository.")
				sb.Append(vbNewLine)
				sb.Append(" - Be logged into the document repository’s network.")
				sb.Append(vbNewLine & vbNewLine & "If you meet the above criteria, Relativity will automatically load in Direct mode.  If you are loading in Web mode and think you should have Direct mode, contact your Relativity Administrator to establish the correct rights.")
				sb.Append(vbNewLine & vbNewLine)
				If includeBulk Then
					sb.Append("SQL INSERT MODES:" & vbNewLine)
					sb.Append(" • Bulk • " & vbNewLine)
					sb.Append("The upload process has access to the SQL share on the appropriate case database.  This ensures the fastest transfer of information between the desktop client and the relativity servers.")
					sb.Append(vbNewLine & vbNewLine)
					sb.Append(" • Single •" & vbNewLine)
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
			Return Nothing

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
				Try
					Return CType(ConfigSettings("ImportBatchMaxVolume"), Int32)
				Catch
					Return 1000000
				End Try
			End Get
		End Property

		Public Shared ReadOnly Property ImportBatchSize() As Int32		'Number of records
			Get
				Try
					Return CType(ConfigSettings("ImportBatchSize"), Int32)
				Catch ex As Exception
					Return 500
				End Try
			End Get
		End Property

		Public Shared ReadOnly Property AutoBatchOn() As Boolean		'Allow or not to automatically decrease import batch size while import is in progress
			Get
				Return CType(ConfigSettings("AutoBathcOn"), Boolean)
			End Get
		End Property

		Public Shared ReadOnly Property MinimumBatchSize() As Int32		'When AutoBatch is on. This is the lower ceiling up to which batch will decrease
			Get
				Return CType(ConfigSettings("MinimumBatchSize"), Int32)
			End Get
		End Property

		Public Shared ReadOnly Property WaitTimeBetweenRetryAttempts() As Int32
			Get
				Return CType(ConfigSettings("WaitTimeBetweenRetryAttempts"), Int32)
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

		Public Shared ReadOnly Property DisableNativeValidation() As Boolean
			Get
				Return CType(ConfigSettings("DisableNativeValidation"), Boolean)
			End Get
		End Property

		Public Shared ReadOnly Property DisableNativeLocationValidation() As Boolean
			Get
				Return CType(ConfigSettings("DisableNativeLocationValidation"), Boolean)
			End Get
		End Property

		Public Shared ReadOnly Property CreateErrorForEmptyNativeFile() As Boolean
			Get
				Return CType(ConfigSettings("CreateErrorForEmptyNativeFile"), Boolean)
			End Get
		End Property



		Public Shared Property ForceFolderPreview() As Boolean
			Get
				Dim registryValue As String = Config.GetRegistryKeyValue("ForceFolderPreview")
				If String.IsNullOrEmpty(registryValue) Then
					Config.SetRegistryKeyValue("ForceFolderPreview", "true")
					Return True
				End If
				Return registryValue.ToLower.Equals("true")
			End Get
			Set(ByVal value As Boolean)
				Dim registryValue As String = "false"
				If value Then registryValue = "true"
				Config.SetRegistryKeyValue("ForceFolderPreview", registryValue)
			End Set
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
				Try
					Return CType(ConfigSettings("EnableSingleModeImport"), Boolean)
				Catch ex As Exception
					Return False
				End Try
			End Get
		End Property

		Public Shared ReadOnly Property WebBasedFileDownloadChunkSize() As Int32
			Get
				If Not ConfigSettings.Contains("WebBasedFileDownloadChunkSize") Then
					ConfigSettings.Add("WebBasedFileDownloadChunkSize", 1048576)
				End If
				Return System.Math.Max(CType(ConfigSettings("WebBasedFileDownloadChunkSize"), Int32), 1024)
			End Get
		End Property
	End Class
End Namespace
