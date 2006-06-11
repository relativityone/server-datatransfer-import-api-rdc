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
				Return String.Format("{1}://{0}/", Host, protocol)
      End Get
    End Property

    Public Shared ReadOnly Property SearchExportChunkSize() As Int32
      Get
        Return 50   'TODO: Put this in App.Config
      End Get
    End Property

		Public Shared ReadOnly Property Protocol() As String
			Get
				Return String.Format(CType(ConfigSettings("Protocol"), String), Host)
			End Get
		End Property
	End Class
End Namespace
