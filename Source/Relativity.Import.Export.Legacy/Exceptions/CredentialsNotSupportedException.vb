Namespace kCura.WinEDDS.Exceptions

	''' <summary>
	''' The exception thrown when a credential is not supported within the current runtime context
	''' For example, attempting to use an OAuth2 implicit flow within a non-interactive process is not supported.
	''' </summary>
	<Serializable>
	Public Class CredentialsNotSupportedException
		Inherits System.Exception

		''' <summary>
		''' Initializes a new instance of the <see cref="CredentialsNotSupportedException"/> class.
		''' </summary>
		''' <param name="message">
		''' The error message that explains the reason for the exception.
		''' </param>
		Public Sub New(message As String)
			MyBase.New(message)
		End Sub

		''' <inheritdoc />
		<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
		Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
			MyBase.New(info, context)
		End Sub
	End Class
End Namespace