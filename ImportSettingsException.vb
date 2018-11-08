''' <summary>
''' The exception thrown when attempting to set a configurable setting with an invalid or out of range value.
''' </summary>
Public Class ImportSettingsException
	Inherits Exception

	''' <summary>
	''' Initializes a new instance of the <see cref="ImportSettingsException"/> class.
	''' </summary>
	''' <param name="setting">
	''' The invalid configurable setting name.
	''' </param>
	Public Sub New(ByVal setting As String)
		Me.New(setting, String.Empty)
	End Sub

	''' <summary>
	''' Initializes a new instance of the <see cref="ImportSettingsException"/> class.
	''' </summary>
	''' <param name="setting">
	''' The invalid configurable setting name.
	''' </param>
	''' <param name="additionalInfo">
	''' Additional information about the invalid configuration setting value.
	''' </param>
	Public Sub New(ByVal setting As String, ByVal additionalInfo As String)
		MyBase.new(GenerateMessage(setting, additionalInfo))
		Me.Setting = setting
		Me.AdditionalInfo = additionalInfo
	End Sub

	''' <inheritdoc />
	<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
	Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
		MyBase.New(info, context)
		Me.Setting = info.GetString("Setting")
		Me.AdditionalInfo = info.GetString("AdditionalInfo")
	End Sub

	''' <summary>
	''' Gets the invalid configurable setting name.
	''' </summary>
	''' <value>
	''' The setting name.
	''' </value>
	Public ReadOnly Property Setting As String

	''' <summary>
	''' Gets additional information about the invalid configuration setting value.
	''' </summary>
	''' <value>
	''' Additional information.
	''' </value>
	Public ReadOnly Property AdditionalInfo As String

	''' <inheritdoc />
	<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
	Public Overrides Sub GetObjectData(info As System.Runtime.Serialization.SerializationInfo, context As System.Runtime.Serialization.StreamingContext)
		info.AddValue("Setting", Me.Setting)
		info.AddValue("AdditionalInfo", Me.AdditionalInfo)
		MyBase.GetObjectData(info, context)
	End Sub

	Private Shared Function GenerateMessage(ByVal setting As String, ByVal additionalInfo As String) As String
		Return String.Format("{0} must be set.  {1}", setting, additionalInfo)
	End Function
End Class

''' <summary>
''' The exception thrown when attempting to set a configurable setting with an invalid or out of range value due to a conflict with another configurable setting.
''' </summary>
Public Class ImportSettingsConflictException
	Inherits Exception

	''' <summary>
	''' Initializes a new instance of the <see cref="ImportSettingsConflictException"/> class.
	''' </summary>
	''' <param name="setting">
	''' The invalid configurable setting name.
	''' </param>
	''' <param name="conflictingSetting">
	''' The conflicting configurable setting name.
	''' </param>
	''' <param name="message">
	''' The error message that explains the reason for the exception.
	''' </param>
	Public Sub New(ByVal setting As String, ByVal conflictingSetting As String, ByVal message As String)
		MyBase.New(message)
		Me.Setting = setting
		Me.ConflictSetting = conflictingSetting
	End Sub

	''' <inheritdoc />
	<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
	Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
		MyBase.New(info, context)
		Me.Setting = info.GetString("Setting")
		Me.ConflictSetting = info.GetString("ConflictSetting")
	End Sub

	''' <summary>
	''' Gets the invalid configurable setting name.
	''' </summary>
	''' <value>
	''' The setting name.
	''' </value>
	Public ReadOnly Property Setting As String

	''' <summary>
	''' Gets the conflicting configurable setting name.
	''' </summary>
	''' <value>
	''' The setting name.
	''' </value>
	Public ReadOnly Property ConflictSetting As String

	''' <inheritdoc />
	<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
	Public Overrides Sub GetObjectData(info As System.Runtime.Serialization.SerializationInfo, context As System.Runtime.Serialization.StreamingContext)
		info.AddValue("Setting", Me.Setting)
		info.AddValue("ConflictSetting", Me.ConflictSetting)
		MyBase.GetObjectData(info, context)
	End Sub
End Class