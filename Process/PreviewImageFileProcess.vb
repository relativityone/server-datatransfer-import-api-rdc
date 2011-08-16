Namespace kCura.WinEDDS

	Public Class PreviewImageFileProcess
		Inherits kCura.Windows.Process.ProcessBase

		Public LoadFile As ImageLoadFile
		Protected WithEvents _imageFilePreviewer As ImageFilePreviewer
		Private _startTime As System.DateTime
		Private _errorCount As Int32
		Private _errorsOnly As Boolean
		Private _warningCount As Int32
		Private _timeZoneOffset As Int32
		Private StartTime As System.DateTime
		Private WithEvents _valueThrower As ValueThrower
		Public ReturnValueCollection As Hashtable
		Public ReturnValueKey As Guid
		Private _serviceURL As String

		Public Sub New()
			Me.New(kCura.WinEDDS.Config.WebServiceURL)
		End Sub

		Public Sub New(ByVal webURL As String)
			MyBase.New()
			ServiceURL = webURL
		End Sub

		Public Property ServiceURL As String
			Get
				Return _serviceURL
			End Get
			Set(value As String)
				_serviceURL = value
				If Not _imageFilePreviewer Is Nothing Then
					_imageFilePreviewer.ServiceURL = value
				End If
			End Set
		End Property

		Public Property TimeZoneOffset() As Int32
			Get
				Return _timeZoneOffset
			End Get
			Set(ByVal value As Int32)
				_timeZoneOffset = value
			End Set
		End Property

		Public Property ErrorsOnly() As Boolean
			Get
				Return _errorsOnly
			End Get
			Set(ByVal value As Boolean)
				_errorsOnly = value
			End Set
		End Property

		Public Property Thrower() As ValueThrower
			Get
				Return _valueThrower
			End Get
			Set(ByVal value As ValueThrower)
				_valueThrower = value
			End Set
		End Property

		Protected Overrides Sub Execute()
			_startTime = DateTime.Now
			_warningCount = 0
			_errorCount = 0
			_imageFilePreviewer = New kCura.WinEDDS.ImageFilePreviewer(LoadFile, ProcessController, True)
			_imageFilePreviewer.ServiceURL = ServiceURL

			_imageFilePreviewer.ReadFile(LoadFile.FileName)
			Me.ProcessObserver.RaiseProcessCompleteEvent()
		End Sub

		Protected Overrides Sub Execute(ByVal webServiceURL As String)
			If String.IsNullOrEmpty(webServiceURL) Then
				Throw New ArgumentNullException("webServiceURL")
			End If

			Me.Execute()
		End Sub

		Private Sub _imageFileImporter_StatusMessage(ByVal e As kCura.Windows.Process.StatusEventArgs) Handles _imageFilePreviewer.StatusMessage
			Select Case e.EventType
				Case kCura.Windows.Process.EventType.Error
					_errorCount += 1
					Me.ProcessObserver.RaiseErrorEvent(e.CurrentRecordIndex.ToString, e.Message)
				Case kCura.Windows.Process.EventType.Progress
					Me.ProcessObserver.RaiseStatusEvent(e.CurrentRecordIndex.ToString, e.Message)
					Me.ProcessObserver.RaiseProgressEvent(e.TotalRecords, e.CurrentRecordIndex, _warningCount, _errorCount, _startTime, DateTime.Now)
				Case kCura.Windows.Process.EventType.Status
					Me.ProcessObserver.RaiseStatusEvent(e.CurrentRecordIndex.ToString, e.Message)
				Case kCura.Windows.Process.EventType.Warning
					_warningCount += 1
					Me.ProcessObserver.RaiseWarningEvent(e.CurrentRecordIndex.ToString, e.Message)
			End Select
		End Sub

	End Class

End Namespace