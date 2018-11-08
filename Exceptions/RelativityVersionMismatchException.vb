Namespace kCura.WinEDDS

	''' <summary>
	''' The exception thrown when a failure occurs verifying support between the client API and Relativity.
	''' </summary>
	<Serializable>
	Public Class RelativityVersionMismatchException
		Inherits System.Exception

		''' <summary>
		''' Initializes a new instance of the <see cref="RelativityVersionMismatchException"/> class.
		''' </summary>
		''' <param name="relativityVersion">
		''' The mismatched Relativity version.
		''' </param>
        Public Sub New(ByVal relativityVersion As String)
            MyBase.New(String.Format("Your version of the Relativity Desktop Client ({0}) is out of date. Please make sure you are running correct RDC version ({1}) or specified correct WebService URL for Relativity.", System.Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString, relativityVersion))

			' Preserving the original constructor for compatibility purposes.
			Me.RelativityVersion = relativityVersion
			Me.ClientVersion = System.Reflection.Assembly.GetExecutingAssembly.GetName.Version.ToString()
        End Sub

		''' <summary>
		''' Initializes a new instance of the <see cref="RelativityVersionMismatchException"/> class.
		''' </summary>
		''' <param name="message">
		''' The error message that explains the reason for the exception.
		''' </param>
		''' <param name="relativityVersion">
		''' The mismatched Relativity version.
		''' </param>
		''' <param name="clientVersion">
		''' The mismatched client version.
		''' </param>
		Public Sub New(ByVal message As String, ByVal relativityVersion As String, ByVal clientVersion As String)
			MyBase.New(message)
			Me.RelativityVersion = relativityVersion
			Me.ClientVersion = clientVersion
		End Sub		

		''' <inheritdoc />
        <System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
        Protected Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)
	        MyBase.New(info, context)
	        Me.ClientVersion = info.GetString("ClientVersion")
	        Me.RelativityVersion = info.GetString("RelativityVersion")
        End Sub

		''' <summary>
		''' Gets the current Relativity version.
		''' </summary>
		''' <value>
		''' The version string.
		''' </value>
		Public ReadOnly Property RelativityVersion As String

		''' <summary>
		''' Gets the current client version.
		''' </summary>
		''' <value>
		''' The version string.
		''' </value>
		Public ReadOnly Property ClientVersion As String

        ''' <inheritdoc />
        <System.Security.Permissions.SecurityPermissionAttribute(System.Security.Permissions.SecurityAction.Demand, SerializationFormatter:=True)>
        Public Overrides Sub GetObjectData(info As System.Runtime.Serialization.SerializationInfo, context As System.Runtime.Serialization.StreamingContext)
	        info.AddValue("ClientVersion", Me.ClientVersion)
	        info.AddValue("RelativityVersion", Me.RelativityVersion)
	        MyBase.GetObjectData(info, context)
        End Sub
    End Class
End Namespace