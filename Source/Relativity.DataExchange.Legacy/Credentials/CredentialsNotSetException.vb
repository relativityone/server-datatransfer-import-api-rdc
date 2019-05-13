Namespace kCura.WinEDDS.Credentials

	''' <summary>
	''' The exception thrown when a credential operation is called but the credentials provider has not been assigned.
	''' </summary>
	''' <remarks>
	''' This represents a logic error.
	''' </remarks>
	<Serializable>
	Public Class CredentialsNotSetException
		Inherits Exception

		''' <summary>
		''' Initializes a new instance of the <see cref="CredentialsNotSetException"/> class.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <inheritdoc />
		<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
		Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
			MyBase.New(info, context)
		End Sub
	End Class
End Namespace