Namespace kCura.EDDS.WinForm
	Public Class Config

		Private Shared _configDictionary As System.Collections.IDictionary

		Public Shared ReadOnly Property ConfigSettings() As System.Collections.IDictionary
			Get
				If _configDictionary Is Nothing Then
					_configDictionary = kCura.Config.Manager.GetConfig("kCura.EDDS.WinForm", New ConfigDictionaryFactory)
				End If
				Return _configDictionary
			End Get
		End Property

#Region " ConfigDictionaryFactory "

		Public Class ConfigDictionaryFactory
			Implements kCura.Config.IConfigDictionaryFactory
			Public Function GetDictionary(ByVal sectionName As String, ByVal collection As kCura.Config.Collection) As kCura.Config.DictionaryBase Implements kCura.Config.IConfigDictionaryFactory.GetDictionary
				Return New ConfigDictionary(sectionName, collection)
			End Function
		End Class

#End Region

		Public Shared ReadOnly Property ExportVolumeDigitPadding() As Int32
			Get
				Return CType(ConfigSettings("ExportVolumeDigitPadding"), Int32)
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
				Dim retval As Boolean = CType(ConfigSettings("SendNotificationOnImportCompletionByDefault"), Boolean)
			End Get
		End Property

	End Class
End Namespace
