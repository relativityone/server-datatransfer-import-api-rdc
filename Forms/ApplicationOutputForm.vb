Imports kCura.EDDS.WebAPI

Public Class ApplicationOutputForm
	Inherits System.Windows.Forms.Form

	Public WithEvents observer As kCura.Windows.Process.Generic.ProcessObserver(Of TemplateManagerBase.ApplicationInstallationResult)

	Private result As TemplateManagerBase.ApplicationInstallationResult
	Private artifactTable As DataTable
	Private errorExpanded As Boolean
	Private advancedMode As Boolean

	Private Sub observer_OnProcessEvent(ByVal evt As kCura.Windows.Process.Generic.ProcessEvent(Of TemplateManagerBase.ApplicationInstallationResult)) Handles observer.OnProcessEvent
		Me.Invoke(Sub() updateArtifactStatusTable(evt))
	End Sub

	Private Sub updateArtifactStatusTable(ByVal evt As kCura.Windows.Process.Generic.ProcessEvent(Of TemplateManagerBase.ApplicationInstallationResult))
		result = evt.Result
		errorExpanded = False
		advancedMode = False

		If result.Success Then
			InformationText.Text = "Installation complete."

			artifactTable = New DataTable()
			artifactTable.Columns.Add("Name", GetType(String))
			artifactTable.Columns.Add("Artifact Type", GetType(String))
			artifactTable.Columns.Add("Artifact ID", GetType(Integer))
			artifactTable.Columns.Add("Status", GetType(String))

			For Each art As TemplateManagerBase.ApplicationArtifact In result.NewApplicationArtifacts
				artifactTable.Rows.Add(New Object() {art.Name, TypeToString(art.Type), art.ArtifactId, "Created"})
			Next

			For Each art As TemplateManagerBase.ApplicationArtifact In result.UpdatedApplicationArtifacts
				artifactTable.Rows.Add(New Object() {art.Name, TypeToString(art.Type), art.ArtifactId, "Updated"})
			Next

			ArtifactStatusTable.DataSource = artifactTable

			ColorTable()

		Else
			InformationText.Text = "Installation failed. For detailed information on how to resolve errors, refer to the Relativity Applications documentation." & Environment.NewLine & Environment.NewLine & _
			"The following errors occurred while installing the application:" & Environment.NewLine & Environment.NewLine
			InformationText.Links.Add(84, 38, "http://help.kcura.com/relativity/Relativity Applications/Using a Relativity Application.pdf#installhelp")

			If Not String.IsNullOrEmpty(result.Message) Then
				InformationText.Text = InformationText.Text & " + " & result.Message
				InformationText.Links.Add(195, 1, "Details")
			End If

			artifactTable = New DataTable()
			artifactTable.Columns.Add("Name", GetType(String))
			artifactTable.Columns.Add("Object Type", GetType(String))
			artifactTable.Columns.Add("Artifact Type", GetType(String))
			artifactTable.Columns.Add("Conflict Artifact Name", GetType(String))
			artifactTable.Columns.Add("Conflict Artifact ID", GetType(Integer))
			artifactTable.Columns.Add("Locked Applications", GetType(String))
			artifactTable.Columns.Add("Error", GetType(String))
			artifactTable.Columns.Add("Details", GetType(String))


			For Each art As TemplateManagerBase.ApplicationArtifact In result.StatusApplicationArtifacts
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
				 TypeToString(art.Type), _
				 conflictName, _
				conflictID, _
				 conflictApps, _
				 StatusToString(art.Status), _
				 art.StatusMessage})
			Next

			ArtifactStatusTable.DataSource = artifactTable
			ArtifactStatusTable.Columns(0).Width = 5
			ColorTable()
			End If

	End Sub

	Private Function TypeToString(ByVal type As TemplateManagerBase.ApplicationArtifactType) As String
		If type = TemplateManagerBase.ApplicationArtifactType.Object Then
			Return "Object Type"
		Else
			Return type.ToString
		End If
	End Function

	Private Sub updateTableForMode()
		If Not result.Success Then
			If advancedMode Then
				ArtifactStatusTable.Columns("Artifact Type").Visible = True
				ArtifactStatusTable.Columns("Object Type").Visible = True
				ArtifactStatusTable.Columns("Name").Visible = True
				ArtifactStatusTable.Columns("Conflict Artifact Name").Visible = True
				ArtifactStatusTable.Columns("Conflict Artifact ID").Visible = True
				ArtifactStatusTable.Columns("Locked Applications").Visible = True
				ArtifactStatusTable.Columns("Error").Visible = True
				ArtifactStatusTable.Columns("Details").Visible = False
			Else
				ArtifactStatusTable.Columns("Artifact Type").Visible = True
				ArtifactStatusTable.Columns("Object Type").Visible = True
				ArtifactStatusTable.Columns("Name").Visible = True
				ArtifactStatusTable.Columns("Conflict Artifact Name").Visible = False
				ArtifactStatusTable.Columns("Conflict Artifact ID").Visible = False
				ArtifactStatusTable.Columns("Locked Applications").Visible = False
				ArtifactStatusTable.Columns("Error").Visible = False
				ArtifactStatusTable.Columns("Details").Visible = True
			End If
		End If
	End Sub

	Private Sub ArtifactStatusTable_sort(ByVal sender As Object, ByVal e As System.EventArgs) Handles ArtifactStatusTable.Sorted
		ColorTable()
	End Sub

	Private Sub InformationLabel_LinkClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles InformationText.LinkClicked
		If String.Equals(CType(e.Link.LinkData, String), "Details") Then
			If errorExpanded Then
				InformationText.Text = "Installation failed. For detailed information on how to resolve errors, refer to the Relativity Applications documentation." & Environment.NewLine & Environment.NewLine & _
				 "The following errors occurred while installing the application:" & Environment.NewLine & Environment.NewLine & " + " & result.Message
				errorExpanded = False
				InformationText.Parent.Height = InformationText.Height
				InformationText.Parent.Width = InformationText.Width
			Else
				InformationText.Text = "Installation failed. For detailed information on how to resolve errors, refer to the Relativity Applications documentation." & Environment.NewLine & Environment.NewLine & _
				 "The following errors occurred while installing the application:" & Environment.NewLine & Environment.NewLine & " - " & result.Message & _
				 Environment.NewLine & result.Details
				errorExpanded = True
				InformationText.Parent.Height = InformationText.Height
				InformationText.Parent.Width = InformationText.Width
			End If
		Else
			System.Diagnostics.Process.Start(CType(e.Link.LinkData, String))
		End If
	End Sub

	Private Sub ColorTable()
		If result.Success Then
			For Each row As DataGridViewRow In ArtifactStatusTable.Rows
				row.Cells("Status").Style.BackColor = Color.PaleGreen
			Next
		Else
			For Each row As DataGridViewRow In ArtifactStatusTable.Rows
				row.Cells("Error").ToolTipText = row.Cells("Details").Value.ToString
				row.Cells("Error").Style.BackColor = Color.LightPink
				'row.Cells("Details").Style.BackColor = Color.LightPink
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



	Private Sub CopyErrorToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyErrorToolStripMenuItem.Click
		If errorExpanded Then
			Clipboard.SetText("Message:" & Environment.NewLine & result.Message & Environment.NewLine & Environment.NewLine & "Details:" & Environment.NewLine & result.Details)
		Else
			Clipboard.SetText("Message:" & Environment.NewLine & result.Message)
		End If
	End Sub

	Private Sub ExportButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExportButton.Click
		Dim csvBuilder As New System.Text.StringBuilder()
		Dim prepend As String = """"
		For Each col As DataColumn In artifactTable.Columns
			csvBuilder.Append(prepend)
			csvBuilder.Append(col.ColumnName.Replace("""", """"""))
			csvBuilder.Append("""")
			prepend = ","""
		Next
		csvBuilder.Append(Environment.NewLine)

		For Each row As DataRow In artifactTable.Rows
			prepend = """"
			For Each cell In row.ItemArray
				csvBuilder.Append(prepend)
				csvBuilder.Append(cell.ToString.Replace("""", """"""))
				csvBuilder.Append("""")
				prepend = ","""
			Next
			csvBuilder.Append(Environment.NewLine)
		Next

		Dim myStream As IO.Stream
		Dim saveAsCsvDialog As New SaveFileDialog()
		saveAsCsvDialog.Filter = "csv files (*.csv)|*.csv"
		If result.Success Then
			saveAsCsvDialog.FileName = "Application_Import_Log"
		Else
			saveAsCsvDialog.FileName = "Application_Import_Error_Log"
		End If

		If saveAsCsvDialog.ShowDialog() = DialogResult.OK Then
			myStream = saveAsCsvDialog.OpenFile()
			If (myStream IsNot Nothing) Then
				Dim encoding As New System.Text.UTF8Encoding()
				myStream.Write(encoding.GetBytes(csvBuilder.ToString), 0, encoding.GetByteCount(csvBuilder.ToString))
				myStream.Close()
			End If
		End If

	End Sub

	Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CloseButton.Click
		Me.Close()
	End Sub
End Class