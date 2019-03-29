Namespace kCura.WinEDDS
	Public Class NewFolderEvent
		Inherits kCura.WinEDDS.AppEvent

		Public ParentFolderID As Int32
		Public FolderID As Int32
		Public Name As String

		Public ReadOnly Property ExportFriendlyName() As String
			Get
				Return Service.FolderManager.GetExportFriendlyFolderName(Me.Name)
			End Get
		End Property

		Public Sub New(ByVal parentFolderID As Int32, ByVal folderId As Int32, ByVal name As String)
			MyBase.New(AppEvent.AppEventType.NewFolder)
			Me.ParentFolderID = parentFolderID
			Me.FolderID = folderId
			Me.Name = name
		End Sub
	End Class
End Namespace