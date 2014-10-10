Namespace kCura.WinEDDS.CodeValidator
	Public Class CodeCreationException
		Inherits System.Exception
		Private _isFatal As Boolean
		Public ReadOnly Property IsFatal() As Boolean
			Get
				Return _isFatal
			End Get
		End Property
		Public Sub New(ByVal isFatal As Boolean, ByVal message As String)
			MyBase.New(message)
			_isFatal = isFatal
		End Sub
	End Class
End Namespace

