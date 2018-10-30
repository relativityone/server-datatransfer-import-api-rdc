Imports System.Collections.Generic

Namespace kCura.WinEDDS
	Public Class ExtendedExportFile
		Inherits ExportFile

		Public SelectedNativesNameViewFields As IList(Of ViewFieldInfo)

		Public Sub New(artifactTypeID As Integer)
			MyBase.New(artifactTypeID)
			SelectedNativesNameViewFields = New List(Of ViewFieldInfo)
		End Sub
	End Class
End Namespace
