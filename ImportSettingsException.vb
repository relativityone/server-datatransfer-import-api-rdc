

Public Class ImportSettingsException
	Inherits Exception

	Private _setting As String
	Private _additionalInfo As String


	Public Sub New(ByVal SettingName As String, ByVal AdditionalInfo As String)
		MyBase.new(GenerateMessage(SettingName, AdditionalInfo))
		_setting = Setting
		_additionalInfo = AdditionalInfo
	End Sub

	Public ReadOnly Property Setting As String
		Get
			Return _setting
		End Get
	End Property

	Public ReadOnly Property AdditionalInfo As String
		Get
			Return _additionalInfo
		End Get
	End Property

	Private Shared Function GenerateMessage(ByVal settingName As String, ByVal additionalInfoString As String) As String
		Return String.Format("{0} must be set.  {1}", settingName, additionalInfoString)
	End Function

End Class
