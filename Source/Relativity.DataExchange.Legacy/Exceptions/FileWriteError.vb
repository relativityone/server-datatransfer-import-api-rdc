Namespace kCura.WinEDDS.Exceptions
	
	''' <summary>
	''' The exception thrown when a failure occurs attempting to perform file write, delete, or related I/O operations.
	''' </summary>
	<Serializable>
	Public Class FileWriteException
		Inherits ExportBaseException

		Public Enum DestinationFile
			Errors
			Load
			Image
			Generic
		End Enum

		''' <summary>
		''' Initializes a new instance of the <see cref="FileWriteException"/> class.
		''' </summary>
		''' <param name="destination">
		''' The destination file type.
		''' </param>
		''' <param name="innerException">
		''' The exception that Is the cause of the current exception, Or a null reference (Nothing in Visual Basic) if no inner exception Is specified.
		''' </param>
		Public Sub New(ByVal destination As DestinationFile, ByVal innerException As System.Exception)
			MyBase.new("Error writing to " & destination.ToString & " output file", innerException)
		End Sub

		''' <inheritdoc />
		<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
		Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
			MyBase.New(info, context)
		End Sub
	End Class
End Namespace