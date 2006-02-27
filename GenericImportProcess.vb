Namespace kCura.WinEDDS
	Public Class GenericImportProcess
    Inherits kCura.Windows.Process.ProcessBase

		Protected WithEvents _importer As kCura.EDDS.Import.ImporterBase

		Public Settings As Object

		Private _credential As Net.NetworkCredential
		Private _winEDDSGateway As kCura.EDDS.Import.HostGatewayBase

		Protected Overrides Sub Execute()
			_importer = Me.GetImporter
			_importer.Settings = Settings
			_importer.HostGateway = _winEDDSGateway
			_importer.Import()
		End Sub

		Private Function GetImporter() As kCura.EDDS.Import.ImporterBase
			Dim importerAssembly As System.Reflection.Assembly
			importerAssembly = System.Reflection.Assembly.LoadFrom("c:\sourcecode\edds\trunk\kCura.EDDS.Import.Outlook\bin\kCura.EDDS.Import.Outlook.dll")
			Dim importer As kCura.EDDS.Import.ImporterBase
			importer = CType(importerAssembly.CreateInstance("kCura.EDDS.Import.Outlook.OutlookImporter"), kCura.EDDS.Import.ImporterBase)
			Return importer
		End Function

		Private Sub _importer_OnStatusEvent(ByVal msgId As String, ByVal eventMessage As String) Handles _importer.OnStatusEvent
			MyBase.ProcessObserver.RaiseStatusEvent(msgId, eventMessage)
		End Sub

		Private Sub _importer_OnWarningEvent(ByVal msgid As String, ByVal eventMessage As String) Handles _importer.OnWarningEvent
			MyBase.ProcessObserver.RaiseWarningEvent(msgid, eventMessage)
		End Sub

		Private Sub _importer_OnErrorEvent(ByVal msgId As String, ByVal eventMessage As String) Handles _importer.OnErrorEvent
			MyBase.ProcessObserver.RaiseErrorEvent(msgId, eventMessage)
		End Sub

		Private Sub _importer_OnProgressStatus(ByVal totalRecords As Integer, ByVal totalRecordsProcessed As Integer, ByVal totalRecordsProcessedWithWarnings As Integer, ByVal totalRecordsProcessedWithErrors As Integer, ByVal startTime As Date, ByVal endTime As Date) Handles _importer.OnProgressStatus
			MyBase.ProcessObserver.RaiseProgressEvent(totalRecords, totalRecordsProcessed, totalRecordsProcessedWithWarnings, totalRecordsProcessedWithErrors, startTime, endTime)
		End Sub

		Public Sub New(ByVal winEDDSGateway As kCura.EDDS.Import.HostGatewayBase)
			MyBase.New()
			_winEDDSGateway = winEDDSGateway
		End Sub

	End Class
End Namespace