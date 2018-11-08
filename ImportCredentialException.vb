''' <summary>
''' The exception thrown when invalid credentials are found within the credential cache or a failure occurs attempting to login.
''' </summary>
<Serializable>
Public Class ImportCredentialException
	Inherits System.Exception

	''' <summary>
	''' Initializes a new instance of the <see cref="ImportCredentialException"/> class.
	''' </summary>
	''' <param name="message">
	''' The error message that explains the reason for the exception.
	''' </param>
	''' <param name="userName">
	''' The user name used that failed.
	''' </param>
	''' <param name="URL">
	''' The web service URL used that failed.
	''' </param>
	Public Sub New(ByVal message As String, ByVal userName As String, ByVal URL As String)
		MyBase.new(String.Format("{0}" + vbCrLf + "Username: {1}" + vbCrLf + "URL: {2}", message, userName, URL))
	End Sub

	''' <inheritdoc />
	<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
	Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
		MyBase.New(info, context)
	End Sub
End Class