Namespace kCura.WinEDDS

	Public Class ImportFileDirectoryProcess
    Inherits kCura.Windows.Process.ProcessBase

		Protected WithEvents _fileDirectoryImporter As FileDirectoryImporter

		Public ImportFileDirectorySettings As ImportFileDirectorySettings
		Private _credential As Net.NetworkCredential
		Private _cookieContainer As System.Net.CookieContainer
		'Private _identity As kCura.EDDS.EDDSIdentity

		Protected Overrides Sub Execute()
			'_fileDirectoryImporter = New kCura.WinEDDS.FileDirectoryImporter(ImportFileDirectorySettings, _credential, _cookieContainer, _identity)
			_fileDirectoryImporter = New kCura.WinEDDS.FileDirectoryImporter(ImportFileDirectorySettings, _credential, _cookieContainer)
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
			'Public Sub New(ByVal credential As Net.NetworkCredential, ByVal cookieContainer As System.Net.CookieContainer, ByVal identity As kCura.EDDS.EDDSIdentity)
			MyBase.new()
			_credential = credential
			_cookieContainer = cookieContainer
			'_identity = identity
		End Sub
	End Class
End Namespace