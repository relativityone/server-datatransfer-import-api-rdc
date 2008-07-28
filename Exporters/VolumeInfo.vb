Namespace kCura.WinEDDS.Exporters
	Public Class VolumeInfo
		Private _volumePrefix As String
		Private _volumeStartNumber As Int32
		Private _volumeMaxSize As Int64
		Private _subdirectoryImagePrefix As String
		Private _subdirectoryNativePrefix As String
		Private _subdirectoryTextPrefix As String
		Private _subdirectoryStartNumber As Int32
		Private _subdirectoryMaxSize As Int64

		Public Property VolumePrefix() As String
			Get
				Return _volumePrefix
			End Get
			Set(ByVal value As String)
				_volumePrefix = value
			End Set
		End Property

		Public Property VolumeStartNumber() As Int32
			Get
				Return _volumeStartNumber
			End Get
			Set(ByVal value As Int32)
				_volumeStartNumber = value
			End Set
		End Property

		Public Property VolumeMaxSize() As Int64
			Get
				Return _volumeMaxSize
			End Get
			Set(ByVal value As Int64)
				_volumeMaxSize = value
			End Set
		End Property

		Public Property SubdirectoryImagePrefix() As String
			Get
				Return _subdirectoryImagePrefix
			End Get
			Set(ByVal value As String)
				_subdirectoryImagePrefix = value
			End Set
		End Property

		Public Property SubdirectoryNativePrefix() As String
			Get
				Return _subdirectoryNativePrefix
			End Get
			Set(ByVal value As String)
				_subdirectoryNativePrefix = value
			End Set
		End Property

		Public Property SubdirectoryFullTextPrefix() As String
			Get
				Return _subdirectoryTextPrefix
			End Get
			Set(ByVal value As String)
				_subdirectoryTextPrefix = value
			End Set
		End Property

		Public Property SubdirectoryStartNumber() As Int32
			Get
				Return _subdirectoryStartNumber
			End Get
			Set(ByVal value As Int32)
				_subdirectoryStartNumber = value
			End Set
		End Property

		Public Property SubdirectoryMaxSize() As Int64
			Get
				Return _subdirectoryMaxSize
			End Get
			Set(ByVal value As Int64)
				_subdirectoryMaxSize = value
			End Set
		End Property

	End Class
End Namespace