Namespace kCura.WinEDDS.Exceptions

	''' <summary>
	''' The exception thrown when a failure occurs attempting to set a field value during an import operation.
	''' </summary>
	<Serializable>
	Public Class FieldValueImportException
		Inherits kCura.Utility.ImporterExceptionBase

		''' <summary>
		''' Initializes a new instance of the <see cref="FieldValueImportException"/> class.
		''' </summary>
		Public Sub New()
			MyBase.New()
		End Sub

		''' <summary>
		''' Initializes a new instance of the <see cref="FieldValueImportException"/> class.
		''' </summary>
		''' <param name="row">
		''' The row number where the error occurred.
		''' </param>
		''' <param name="fieldName">
		''' The field name where the error occurred.
		''' </param>
		Public Sub New(ByVal row As Int64, ByVal fieldName As String, ByVal additionalInfo As String)
			Me.New(Nothing, row, fieldName, additionalInfo)
		End Sub

		''' <summary>
		''' Initializes a new instance of the <see cref="FieldValueImportException"/> class.
		''' </summary>
		''' <param name="innerException">
		''' The exception that Is the cause of the current exception, Or a null reference (Nothing in Visual Basic) if no inner exception Is specified.
		''' </param>
		''' <param name="row">
		''' The row number where the error occurred.
		''' </param>
		''' <param name="fieldName">
		''' The field name associated with the error.
		''' </param>
		''' <param name="additionalInfo">
		''' Additional information included with the error.
		''' </param>
		Public Sub New(ByVal innerException As System.Exception, ByVal row As Int64, ByVal fieldName As String, ByVal additionalInfo As String)
			MyBase.New(innerException, row, fieldName, additionalInfo)
			Me.FieldName = fieldName
			Me.RowNumber = row
		End Sub

		''' <inheritdoc />
		<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
		Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
			MyBase.New(info, context)
			Me.FieldName = info.GetString("FieldName")
			Me.RowNumber = info.GetInt64("RowNumber")
		End Sub

		''' <summary>
		''' Gets the row number where the error occurred.
		''' </summary>
		''' <value>
		''' The row number.
		''' </value>
		Public ReadOnly Property RowNumber As Long

		''' <summary>
		''' Gets the field name associated with the error.
		''' </summary>
		''' <value>
		''' The row number.
		''' </value>
		Public ReadOnly Property FieldName As String

		''' <inheritdoc />
		<System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
		Public Overrides Sub GetObjectData(info As System.Runtime.Serialization.SerializationInfo, context As System.Runtime.Serialization.StreamingContext)
			info.AddValue("FieldName", Me.FieldName)
			info.AddValue("RowNumber", Me.RowNumber)
			MyBase.GetObjectData(info, context)
		End Sub
	End Class
End Namespace