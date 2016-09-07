Namespace kCura.WinEDDS.Exporters
	<Serializable()> Public Class VolumeInfo
		Implements System.Runtime.Serialization.ISerializable

#Region " Member and Accessors "

		Private _subdirectoryImagePrefix As String
		Private _subdirectoryNativePrefix As String
		Private _subdirectoryTextPrefix As String

		Public Property VolumePrefix As String

		Public Property VolumeStartNumber As Int32

		Public Property VolumeMaxSize() As Int64

		Public Property SubdirectoryStartNumber() As Int32

		Public Property SubdirectoryMaxSize() As Int64

		Public Property CopyNativeFilesFromRepository() As Boolean = True

		Public Property CopyImageFilesFromRepository As Boolean = True

		Public Property SubdirectoryImagePrefix(Optional ByVal includeTopFolder As Boolean = True) As String
			Get
				Dim result As String = _subdirectoryImagePrefix
				If includeTopFolder Then
					result = "IMAGES\" + result
				End If
				Return result
			End Get
			Set(ByVal value As String)
				_subdirectoryImagePrefix = value
			End Set
		End Property

		Public Property SubdirectoryNativePrefix(Optional ByVal includeTopFolder As Boolean = True) As String
			Get
				Dim result As String = _subdirectoryNativePrefix
				If includeTopFolder Then
					result = "NATIVES\" + result
				End If
				Return result
			End Get
			Set(ByVal value As String)
				_subdirectoryNativePrefix = value
			End Set
		End Property

		Public Property SubdirectoryFullTextPrefix(Optional ByVal includeTopFolder As Boolean = True) As String
			Get
				Dim result As String = _subdirectoryTextPrefix
				If includeTopFolder Then
					result = "TEXT\" + result
				End If
				Return result
			End Get
			Set(ByVal value As String)
				_subdirectoryTextPrefix = value
			End Set
		End Property

#End Region

#Region " Constructors "
		Public Sub New()
			MyBase.New()
			Me.CopyImageFilesFromRepository = True
			Me.CopyNativeFilesFromRepository = True
		End Sub
#End Region

#Region " Serialization "

		Public Sub GetObjectData(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext) Implements System.Runtime.Serialization.ISerializable.GetObjectData
			info.AddValue("CopyNativeFilesFromRepository", Me.CopyNativeFilesFromRepository, GetType(Boolean))
			info.AddValue("CopyImageFilesFromRepository", Me.CopyImageFilesFromRepository, GetType(Boolean))
			info.AddValue("SubdirectoryMaxSize", Me.SubdirectoryMaxSize, GetType(Int64))
			info.AddValue("SubdirectoryStartNumber", Me.SubdirectoryStartNumber, GetType(Int32))
			info.AddValue("SubdirectoryFullTextPrefix", Me.SubdirectoryFullTextPrefix(False), GetType(String))
			info.AddValue("SubdirectoryNativePrefix", Me.SubdirectoryNativePrefix(False), GetType(String))
			info.AddValue("SubdirectoryImagePrefix", Me.SubdirectoryImagePrefix(False), GetType(String))
			info.AddValue("VolumeMaxSize", Me.VolumeMaxSize, GetType(Int64))
			info.AddValue("VolumeStartNumber", Me.VolumeStartNumber, GetType(Int32))
			info.AddValue("VolumePrefix", Me.VolumePrefix, GetType(String))
		End Sub
		Private Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal Context As System.Runtime.Serialization.StreamingContext)
			With info
				Dim readCopyImageFilesSettingFrom As Boolean = False
				Dim readCopyNativeFilesSettingFrom As Boolean = False
				Try
					Me.CopyImageFilesFromRepository = .GetBoolean("CopyImageFilesFromRepository")
				Catch
					readCopyImageFilesSettingFrom = True
				End Try
				Try
					Me.CopyNativeFilesFromRepository = .GetBoolean("CopyNativeFilesFromRepository")
				Catch
					readCopyNativeFilesSettingFrom = True
				End Try
				If readCopyImageFilesSettingFrom Or readCopyNativeFilesSettingFrom Then
					Dim aggregateSetting As Boolean = .GetBoolean("CopyFilesFromRepository")
					If readCopyImageFilesSettingFrom Then Me.CopyImageFilesFromRepository = aggregateSetting
					If readCopyImageFilesSettingFrom Then Me.CopyNativeFilesFromRepository = aggregateSetting
				End If
				Me.SubdirectoryMaxSize = .GetInt64("SubdirectoryMaxSize")
				Me.SubdirectoryStartNumber = .GetInt32("SubdirectoryStartNumber")
				Me.SubdirectoryFullTextPrefix = .GetString("SubdirectoryFullTextPrefix")
				Me.SubdirectoryNativePrefix = .GetString("SubdirectoryNativePrefix")
				Me.SubdirectoryImagePrefix = .GetString("SubdirectoryImagePrefix")
				Me.VolumeMaxSize = .GetInt64("VolumeMaxSize")
				Me.VolumeStartNumber = .GetInt32("VolumeStartNumber")
				Me.VolumePrefix = .GetString("VolumePrefix")
			End With
		End Sub
#End Region

	End Class
End Namespace