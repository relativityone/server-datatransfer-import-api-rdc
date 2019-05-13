Namespace kCura.WinEDDS.Exceptions

	''' <summary>
	''' The exception thrown when a failure occurs performing an export operation.
	''' </summary>
	<Serializable>
	Public MustInherit Class ExportBaseException
		Inherits System.Exception

		''' <summary>
		''' Initializes a new instance of the <see cref="ExportBaseException"/> class.
		''' </summary>
		''' <param name="message">
		''' The error message that explains the reason for the exception.
		''' </param>
		''' <param name="innerException">
		''' The exception that Is the cause of the current exception, Or a null reference (Nothing in Visual Basic) if no inner exception Is specified.
		''' </param>
		Protected Sub New(ByVal message As String, ByVal innerException As System.Exception)
			MyBase.new(message, innerException)
		End Sub

		''' <inheritdoc />
		<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
		Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
			MyBase.New(info, context)
		End Sub
	End Class
End Namespace