Namespace kCura.WinEDDS.Exceptions
	Public Class CodeCreationFailedException
		Inherits System.Exception
		Private _originalExceptionText As String
		Public Sub New(ByVal originalExceptionText As String)
			MyBase.New("Error creating new code")
			_originalExceptionText = originalExceptionText
		End Sub
		Public Overrides Function ToString() As String
			Return MyBase.ToString & vbNewLine & _originalExceptionText
		End Function
	End Class
End Namespace


