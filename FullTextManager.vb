Namespace kCura.WinEDDS
	Public Class FullTextManager

		Public Enum ConnectionType
			Web
			Direct
		End Enum

		Private _gateway As kCura.WinEDDS.Service.FileIO
		Private _credentials As Net.NetworkCredential
		'Private _cookieContainer As System.Net.CookieContainer
		Private _type As ConnectionType
		Private _destinationFolderPath As String

		Public Sub SetDesintationFolderName(ByVal value As String)
			_destinationFolderPath = value
		End Sub

		Public Sub New(ByVal credentials As Net.NetworkCredential, ByVal sourceFolderPath As String, ByVal cookieContainer As System.Net.CookieContainer)
			_gateway = New kCura.WinEDDS.Service.FileIO(credentials, cookieContainer)
			_gateway.Credentials = credentials
			_gateway.Timeout = Int32.MaxValue
			_credentials = credentials
			_destinationFolderPath = sourceFolderPath
			'Dim documentManager As kCura.EDDS.WebAPI.DocumentManagerBase.DocumentManager
			Try
				System.IO.File.Create(sourceFolderPath & "123").Close()
				System.IO.File.Delete(sourceFolderPath & "123")
				Me.Type = ConnectionType.Direct
			Catch ex As System.Exception
				Me.Type = ConnectionType.Web
			End Try
		End Sub

		Public Property Type() As ConnectionType
			Get
				Return _type
			End Get
			Set(ByVal value As ConnectionType)
				_type = value
				RaiseEvent UploadModeChangeEvent(value.ToString)
			End Set
		End Property

		Private ReadOnly Property Gateway() As kCura.WinEDDS.Service.FileIO
			Get
				Return _gateway
			End Get
		End Property

		Friend Class Settings

			Friend Shared ReadOnly Property ChunkSize() As Int32
				Get
					Return 1024000
				End Get
			End Property
		End Class

		Public Function ReadFullTextFile(ByVal filePath As String) As String
			If Me.Type = ConnectionType.Direct Then
				Dim sr As New System.IO.StreamReader(filePath)
				Dim retval As String = sr.ReadToEnd
				sr.Close()
				Return retval
			Else
				Return WebReadFullTextFile(filePath)
			End If
		End Function


		Public Function WebReadFullTextFile(ByVal filePath As String) As String
			Return System.Text.Encoding.UTF8.GetString(_gateway.ReadFileAsString(filePath))
		End Function

		Public Event UploadStatusEvent(ByVal message As String)
		Public Event UploadModeChangeEvent(ByVal mode As String)


	End Class
End Namespace
