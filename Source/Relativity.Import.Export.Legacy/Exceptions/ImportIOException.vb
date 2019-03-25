Namespace kCura.WinEDDS.Exceptions

	''' <summary>
	''' The exception thrown when a non-fatal IO error occurs importing a document.
	''' </summary>
	<Serializable>
	Public Class ImportIOException
		Inherits kCura.Utility.ImporterExceptionBase

		''' <summary>
		''' Initializes a new instance of the <see cref="ImportIOException"/> class.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Initializes a new instance of the <see cref="ImportIOException"/> class.
		''' </summary>
		''' <param name="message">
		''' The error message that explains the reason for the exception.
		''' </param>
		Public Sub New (message As String)
			MyBase.New(message)
		End Sub

		''' <summary>
		''' Initializes a new instance of the <see cref="ImportIOException"/> class.
		''' </summary>
		''' <param name="message">
		''' The error message that explains the reason for the exception.
		''' </param>
		''' <param name="innerException">
		''' The exception that Is the cause of the current exception, Or a null reference (Nothing in Visual Basic) if no inner exception Is specified.
		''' </param>
		Public Sub New (message As String, innerException As Exception)
			MyBase.New(message, innerException)
		End Sub

		''' <inheritdoc />
		<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
		Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
			MyBase.New(info, context)
		End Sub
	End Class
End Namespace