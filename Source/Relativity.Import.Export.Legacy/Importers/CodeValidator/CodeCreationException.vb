Namespace kCura.WinEDDS.CodeValidator

	''' <summary>
	''' The exception thrown when a failure occurs attempting to create a choice field.
	''' </summary>
	<Serializable>
	Public Class CodeCreationException
		Inherits System.Exception

		''' <summary>
		''' Initializes a new instance of the <see cref="CodeCreationException"/> class.
		''' </summary>
		''' <param name="isFatal">
		''' Specify whether the error is fatal.
		''' </param>
		''' <param name="message">
		''' The error message that explains the reason for the exception.
		''' </param>
		Public Sub New(ByVal isFatal As Boolean, ByVal message As String)
			MyBase.New(message)
			Me.IsFatal = isFatal
		End Sub

		''' <inheritdoc />
		<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
		Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
			MyBase.New(info, context)
			Me.IsFatal = info.GetBoolean("IsFatal")
		End Sub

		''' <summary>
		''' Gets a value indicating whether the error is fatal.
		''' </summary>
		''' <value>
		''' <see langword="true"/> if the error is considered fatal; otherwise, <see langword="false"/>.
		''' </value>
		Public ReadOnly Property IsFatal As Boolean

		''' <inheritdoc />
		<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
		Public Overrides Sub GetObjectData(info As System.Runtime.Serialization.SerializationInfo, context As System.Runtime.Serialization.StreamingContext)
			info.AddValue("IsFatal", Me.IsFatal)
			MyBase.GetObjectData(info, context)
		End Sub
	End Class
End Namespace

