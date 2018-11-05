Namespace kCura.WinEDDS.Exceptions

	''' <summary>
	''' The exception thrown when a credential is not supported within the current runtime context
	''' For example, attempting to use an OAuth2 implicit flow within a non-interactive process is not supported.
	''' </summary>
	<Serializable()>
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

		''' <summary>
		''' Initializes a new instance of the <see cref="CredentialsNotSupportedException"/> class.
		''' </summary>
		''' <param name="message">
		''' The error message that explains the reason for the exception.
		''' </param>
		''' <param name="innerException">
		''' The exception that Is the cause of the current exception, Or a null reference (Nothing in Visual Basic) if no inner exception Is specified.
		''' </param>
		Public Sub New(message As String, innerException As Exception)
			MyBase.New(message, innerException)
		End Sub

		''' <summary>
		''' Initializes a New instance of the <see cref="CredentialsNotSupportedException"/> class.
		''' </summary>
		''' <param name="info">
		''' The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> that holds the serialized object data about the exception being thrown.
		''' </param>
		''' <param name="context">
		''' The <see cref="T:System.Runtime.Serialization.StreamingContext" /> that contains contextual information about the source Or destination.
		''' </param>
		<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
		Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
			MyBase.New(info, context)
		End Sub

		''' <inheritdoc />
		<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
		Overrides Sub GetObjectData(info As System.Runtime.Serialization.SerializationInfo, context As System.Runtime.Serialization.StreamingContext)
			MyBase.GetObjectData(info, context)
		End Sub
	End Class
End Namespace