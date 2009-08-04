Namespace kCura.WinEDDS
	Public Class Statistics
		Private _metadataBytes As Int64 = 0
		Private _metadataTime As Int64 = 0
		Private _fileBytes As Int64 = 0
		Private _fileTime As Int64 = 0
		Private _sqlTime As Int64 = 0
		Private _docCount As Int64 = 0
		Private _lastAccessed As System.DateTime

		Public Property MetadataBytes() As Int64
			Get
				Return _metadataBytes
			End Get
			Set(ByVal value As Int64)
				_lastAccessed = System.DateTime.Now
				_metadataBytes = value
			End Set
		End Property

		Public Property MetadataTime() As Int64
			Get
				Return _metadataTime
			End Get
			Set(ByVal value As Int64)
				_lastAccessed = System.DateTime.Now
				_metadataTime = value
			End Set
		End Property

		Public Property FileBytes() As Int64
			Get
				Return _fileBytes
			End Get
			Set(ByVal value As Int64)
				_lastAccessed = System.DateTime.Now
				_fileBytes = value
			End Set
		End Property

		Public Property FileTime() As Int64
			Get
				Return _fileTime
			End Get
			Set(ByVal value As Int64)
				_lastAccessed = System.DateTime.Now
				_fileTime = value
			End Set
		End Property

		Public Property SqlTime() As Int64
			Get
				Return _sqlTime
			End Get
			Set(ByVal value As Int64)
				_lastAccessed = System.DateTime.Now
				_sqlTime = value
			End Set
		End Property

		Public Property DocCount() As Int64
			Get
				Return _docCount
			End Get
			Set(ByVal value As Int64)
				_lastAccessed = System.DateTime.Now
				_docCount = value
			End Set
		End Property

		Public ReadOnly Property LastAccessed() As System.DateTime
			Get
				Return _lastAccessed
			End Get
		End Property

		Public Function ToFileSizeSpecification(ByVal value As Double) As String
			Dim log As Double = System.Math.Ceiling(System.Math.Log10(value))
			Dim prefix As String
			Dim k As Int32 = CType(System.Math.Floor(log / 3), Int32)
			k = CType(System.Math.Floor(System.Math.Log(value, 1000)), Int32)
			Select Case k
				Case 0
					prefix = ""
				Case 1
					prefix = "K"
				Case 2
					prefix = "M"
				Case 3
					prefix = "G"
				Case 4
					prefix = "T"
				Case 5
					prefix = "P"
				Case 6
					prefix = "E"
				Case 7
					prefix = "B"
				Case 8
					prefix = "Y"
			End Select
			Return (value / Math.Pow(1000, k)).ToString("N2") & " " & prefix & "B"
		End Function

		Public Function ToDictionary() As IDictionary
			Dim retval As New System.Collections.Specialized.HybridDictionary
			If Not Me.FileTime = 0 Then retval.Add("Average file transfer rate", ToFileSizeSpecification(Me.FileBytes / (Me.FileTime / 10000000)) & "/sec")
			If Not Me.MetadataTime = 0 Then retval.Add("Average metadata transfer rate", ToFileSizeSpecification(Me.MetadataBytes / (Me.MetadataTime / 10000000)) & "/sec")
			If Not Me.SqlTime = 0 Then retval.Add("Average SQL process rate", (Me.DocCount / (Me.SqlTime / 10000000)).ToString("N0") & " Documents/sec")
			Return retval
		End Function
	End Class
End Namespace
