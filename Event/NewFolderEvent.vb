Namespace kCura.WinEDDS
	Public Class NewFolderEvent
		Inherits kCura.WinEDDS.AppEvent

		Public ParentFolderID As Int32
		Public FolderID As Int32
		Public Name As String

		Public Sub New(ByVal parentFolderID As Int32, ByVal folderId As Int32, ByVal name As String)
			MyBase.New(AppEvent.AppEventType.newFolder)
			Me.ParentFolderID = parentFolderID
			Me.FolderID = folderId
			Me.Name = name
		End Sub
	End Class
End Namespace