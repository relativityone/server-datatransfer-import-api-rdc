Namespace kCura.WinEDDS.Exceptions

	''' <summary>
	''' The exception thrown when a failure occurs attempting to create a new code.
	''' </summary>
	<Serializable>
	Public Class CodeCreationFailedException
		Inherits System.Exception

		''' <summary>
		''' Initializes a new instance of the <see cref="CodeCreationFailedException"/> class.
		''' </summary>
		''' <param name="message">
		''' The error message that explains the reason for the exception.
		''' </param>
		Public Sub New(ByVal message As String)
			MyBase.New("Error creating new code")
			Me.OriginalExceptionText = message
		End Sub

		''' <inheritdoc />
		<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
		Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
			MyBase.New(info, context)
			Me.OriginalExceptionText = info.GetString("OriginalExceptionText")
		End Sub

		''' <summary>
		''' Gets or sets the original exception text.
		''' </summary>
		''' <value>
		''' The text.
		''' </value>
		Public ReadOnly Property OriginalExceptionText As String

		''' <inheritdoc />
		<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
		Public Overrides Sub GetObjectData(info As System.Runtime.Serialization.SerializationInfo, context As System.Runtime.Serialization.StreamingContext)
			info.AddValue("OriginalExceptionText", Me.OriginalExceptionText)
			MyBase.GetObjectData(info, context)
		End Sub

		Public Overrides Function ToString() As String
			Return MyBase.ToString & vbNewLine & Me.OriginalExceptionText
		End Function
	End Class
End Namespace