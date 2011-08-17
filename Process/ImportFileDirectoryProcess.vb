Namespace kCura.WinEDDS

	Public Class ImportFileDirectoryProcess
		Inherits kCura.Windows.Process.ProcessBase

		Protected WithEvents _fileDirectoryImporter As FileDirectoryImporter

		Public ImportFileDirectorySettings As ImportFileDirectorySettings
		Private _credential As Net.NetworkCredential
		Private _cookieContainer As System.Net.CookieContainer
		Private _serviceURL As String
		'Private _identity As Relativity.Core.EDDSIdentity

		Public Overridable Property ServiceURL As String
			Get
				Return _serviceURL
			End Get
			Set(value As String)
				_serviceURL = value
			End Set
		End Property

		Protected Overrides Sub Execute()
			'_fileDirectoryImporter = New kCura.WinEDDS.FileDirectoryImporter(ImportFileDirectorySettings, _credential, _cookieContainer, _identity)
			_fileDirectoryImporter = New kCura.WinEDDS.FileDirectoryImporter(ImportFileDirectorySettings, _credential, _cookieContainer, ServiceURL)
			_fileDirectoryImporter.Import()
			MyBase.ProcessObserver.RaiseProcessCompleteEvent()
		End Sub

		Private Sub _fileDirectoryImporter_OnStatusEvent(ByVal msgId As String, ByVal eventMessage As String) Handles _fileDirectoryImporter.OnStatusEvent
			MyBase.ProcessObserver.RaiseStatusEvent(msgId, eventMessage)
		End Sub
		Private Sub _fileDirectoryImporter_OnWarningEvent(ByVal msgid As String, ByVal eventMessage As String) Handles _fileDirectoryImporter.OnWarningEvent
			MyBase.ProcessObserver.RaiseWarningEvent(msgid, eventMessage)
		End Sub
		Private Sub _fileDirectoryImporter_OnErrorEvent(ByVal msgId As String, ByVal eventMessage As String) Handles _fileDirectoryImporter.OnErrorEvent
			MyBase.ProcessObserver.RaiseErrorEvent(msgId, eventMessage)
		End Sub
		Private Sub _fileDirectoryImporter_OnProgressStatus(ByVal totalRecords As Integer, ByVal totalRecordsProcessed As Integer, ByVal totalRecordsProcessedWithWarnings As Integer, ByVal totalRecordsProcessedWithErrors As Integer, ByVal startTime As Date, ByVal endTime As Date) Handles _fileDirectoryImporter.OnProgressStatus
			MyBase.ProcessObserver.RaiseProgressEvent(totalRecords, totalRecordsProcessed, totalRecordsProcessedWithWarnings, totalRecordsProcessedWithErrors, startTime, endTime)
		End Sub

		Public Sub New(ByVal credential As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer)
			Me.New(credential, cookieContainer, kCura.WinEDDS.Config.WebServiceURL)
		End Sub

		Public Sub New(ByVal credential As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer, ByVal webURL As String)
			'Public Sub New(ByVal credential As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer, ByVal identity As Relativity.Core.EDDSIdentity)
			MyBase.new()
			_credential = credential
			_cookieContainer = cookieContainer
			ServiceURL = webURL
			'_identity = identity
		End Sub
	End Class
End Namespace