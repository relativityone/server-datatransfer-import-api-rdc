Namespace kCura.WinEDDS.Api
	Public Class ImageRecord
		Private _batesNumber As String
		Private _fileLocation As String
		Private _isNewDoc As Boolean
		Private _originalIndex As Int64

		Public Property BatesNumber() As String
			Get
				Return _batesNumber
			End Get
			Set(ByVal value As String)
				_batesNumber = value
			End Set
		End Property

		Public Property FileLocation() As String
			Get
				Return _fileLocation
			End Get
			Set(ByVal value As String)
				_fileLocation = value
			End Set
		End Property

		Public Property IsNewDoc() As Boolean
			Get
				Return _isNewDoc
			End Get
			Set(ByVal value As Boolean)
				_isNewDoc = value
			End Set
		End Property

		Friend Property OriginalIndex() As Int64
			Get
				Return _originalIndex
			End Get
			Set(ByVal value As Int64)
				_originalIndex = value
			End Set
		End Property
	End Class
End Namespace
