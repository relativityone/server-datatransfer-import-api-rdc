Namespace kCura.Relativity.DataReaderClient
	Public MustInherit Class LoadFileJobBase

#Region "Private Variables"
		Protected Enum LoadFileJobType As Integer
			Image
			Native
		End Enum

		Protected _controller As Windows.Process.Controller
		Protected _imgSettings As ImportSettingsBase
		Protected _imgDataReader As ImageSourceIDataReader
		Protected _nativeDataReader As SourceIDataReader
		Protected _nativeSettings As ImportSettingsBase
		Protected WithEvents _observer As Windows.Process.ProcessObserver
#End Region

#Region "Constructors"
		'
#End Region

#Region "Events"
		Public Event OnError(ByVal row As IDictionary)
		Public Event OnMessage(ByVal status As Status)
#End Region

#Region "Public Functions"
		'
#End Region

#Region "Public Routines"
		Public MustOverride Sub Execute()
#End Region

#Region "Private Functions"
		Protected MustOverride Function IsSettingsValid() As Boolean
#End Region

#Region "Private Routines"
		Protected MustOverride Sub SelectServiceURL()
#End Region

#Region "Properties"
		'TODO: As of now, the 2 inheriting classes use Settings() to refer to 2
		' different object types, so we can't "MustOverride" here with our more
		' generic type :(
		'Public MustOverride Property Settings() As ImportSettingsBase
		'
		'Public MustOverride Property SourceData() As ???
#End Region

	End Class
End Namespace