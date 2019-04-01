Namespace kCura.WinEDDS.Exceptions

	''' <summary>
	''' The exception thrown when attempting to download a file from the server and the expected file length doesn't match the actual file length.
	''' </summary>
	<Serializable>
	Public Class WebDownloadCorruptException
		Inherits System.Exception

		''' <summary>
		''' Initializes a new instance of the <see cref="WebDownloadCorruptException"/> class.
		''' </summary>
		''' <param name="message">
		''' The error message that explains the reason for the exception.
		''' </param>
		Public Sub New(ByVal message As String)
			MyBase.New(message)
		End Sub

		''' <inheritdoc />
		<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
		Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
			MyBase.New(info, context)
		End Sub
	End Class
End Namespace