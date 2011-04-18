Imports kCura.EDDS.WebAPI

Public Class ApplicationOutputForm
	Inherits System.Windows.Forms.Form

	Public WithEvents observer As kCura.Windows.Process.Generic.ProcessObserver(Of TemplateManagerBase.ApplicationInstallationResult)

	Private success As Boolean

	Private Sub observer_OnProcessEvent(ByVal evt As kCura.Windows.Process.Generic.ProcessEvent(Of TemplateManagerBase.ApplicationInstallationResult)) Handles observer.OnProcessEvent
		Me.Invoke(Sub() updateArtifactStatusTable(evt))
	End Sub

	Private Sub updateArtifactStatusTable(ByVal evt As kCura.Windows.Process.Generic.ProcessEvent(Of TemplateManagerBase.ApplicationInstallationResult))
		success = evt.Result.Success

		If success Then
			InformationLabel.Text = "Installation complete."

			Dim artifactTable = New DataTable()
			artifactTable.Columns.Add("Name", GetType(String))
			artifactTable.Columns.Add("Artifact Type", GetType(String))
			artifactTable.Columns.Add("Artifact ID", GetType(Integer))
			artifactTable.Columns.Add("Status", GetType(String))

			For Each art As TemplateManagerBase.ApplicationArtifact In evt.Result.NewApplicationArtifacts
				artifactTable.Rows.Add(New Object() {art.Name, art.Type.ToString, art.ArtifactId, "Created"})
			Next

			For Each art As TemplateManagerBase.ApplicationArtifact In evt.Result.UpdatedApplicationArtifacts
				artifactTable.Rows.Add(New Object() {art.Name, art.Type.ToString, art.ArtifactId, "Updated"})
			Next

			ArtifactStatusTable.DataSource = artifactTable

			ColorTable()

		Else
			InformationLabel.Text = "Installation failed. For detailed information on how to resolve errors, refer to the Relativity Applications documentation." & Environment.NewLine & Environment.NewLine & _
	"The following errors occurred while installing the application:" & Environment.NewLine & Environment.NewLine & evt.Result.Message
			InformationLabel.Links.Add(85, 37, "http://kcura.com/relativity/support/documentation/documentation-6.10")

			Dim artifactTable = New DataTable()
			artifactTable.Columns.Add("Name", GetType(String))
			artifactTable.Columns.Add("Object Type", GetType(String))
			artifactTable.Columns.Add("Artifact Type", GetType(String))
			artifactTable.Columns.Add("Conflict Artifact Name", GetType(String))
			artifactTable.Columns.Add("Conflict Artifact ID", GetType(Integer))
			artifactTable.Columns.Add("Locked Applications", GetType(String))
			artifactTable.Columns.Add("Error", GetType(String))
			artifactTable.Columns.Add("Error Details", GetType(String))


			For Each art As TemplateManagerBase.ApplicationArtifact In evt.Result.StatusApplicationArtifacts
				Dim parentName As String = ""
				Dim conflictName As String = ""
				Dim conflictID As Integer = Nothing
				Dim conflictApps As New System.Text.StringBuilder()
				If art.ParentArtifact IsNot Nothing Then
					parentName = art.ParentArtifact.Name
				End If
				If art.ConflictArtifact IsNot Nothing Then
					conflictID = art.ConflictArtifact.ArtifactId
					conflictName = art.ConflictArtifact.Name
					If art.ConflictArtifact.Applications IsNot Nothing Then
						Dim sepString As String = ""
						For Each app As TemplateManagerBase.Application In art.ConflictArtifact.Applications
							conflictApps.Append(sepString)
							conflictApps.Append(app.Name)
							sepString = ", "
						Next
					End If
				End If

				artifactTable.Rows.Add(New Object() { _
				 art.Name, _
				 parentName, _
				 art.Type.ToString, _
				 conflictName, _
				conflictID, _
				 conflictApps, _
				 StatusToString(art.Status), _
				 art.StatusMessage})
			Next

			ArtifactStatusTable.DataSource = artifactTable
			'ArtifactStatusTable.Columns("Error").Visible = False

			ColorTable()
		End If

	End Sub

	Private Sub ArtifactStatusTable_sort(ByVal sender As Object, ByVal e As System.EventArgs) Handles ArtifactStatusTable.Sorted
		ColorTable()
	End Sub

	Private Sub InformationLabel_LinkClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles InformationLabel.LinkClicked
		System.Diagnostics.Process.Start(CType(e.Link.LinkData, String))
	End Sub

	Private Sub ColorTable()
		If success Then
			For Each row As DataGridViewRow In ArtifactStatusTable.Rows
				row.Cells("Status").Style.BackColor = Color.PaleGreen
			Next
		Else
			For Each row As DataGridViewRow In ArtifactStatusTable.Rows
				row.Cells("Error").ToolTipText = row.Cells("Error Details").Value.ToString
				row.Cells("Error").Style.BackColor = Color.LightPink
				row.Cells("Error Details").Style.BackColor = Color.LightPink
			Next
		End If
	End Sub

	Private Function StatusToString(ByVal stat As TemplateManagerBase.StatusCode) As String
		Select Case stat
			Case TemplateManagerBase.StatusCode.FriendlyNameConflict
				Return "Friendly Name Conflict"
			Case TemplateManagerBase.StatusCode.MultipleFileField
				Return "Multiple File Fields"
			Case TemplateManagerBase.StatusCode.NameConflict
				Return "Name Conflict"
			Case TemplateManagerBase.StatusCode.SharedByLockedApp
				Return "Shared By A Locked App"
			Case TemplateManagerBase.StatusCode.UnknownError
				Return "Unknown Error"
			Case Else
				Return stat.ToString
		End Select
	End Function

End Class