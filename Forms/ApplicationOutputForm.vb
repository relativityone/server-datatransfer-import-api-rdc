Imports System.Globalization
Imports kCura.EDDS.WebAPI

Public Class ApplicationOutputForm
	Inherits System.Windows.Forms.Form

	Public WithEvents observer As kCura.Windows.Process.Generic.ProcessObserver(Of TemplateManagerBase.ApplicationInstallationResult)

	Private artifactTable As New DataTable()
	Private result As TemplateManagerBase.ApplicationInstallationResult
	Private errorExpanded As Boolean

	Private Const ExpandedText As String = "More Detail"
	Private Const CollapseText As String = "Less Detail"
	Private Const HelpLink As String = "http://help.kcura.com/relativity/Relativity Applications/Using a Relativity Application.pdf#installhelp"

	Private Sub UpdateArtifactStatusTable(ByVal evt As kCura.Windows.Process.Generic.ProcessEvent(Of TemplateManagerBase.ApplicationInstallationResult))
		result = evt.Result
		errorExpanded = False

		If result.Success Then
			InformationText.Text = "Installation complete."

			artifactTable = CreateSucessTable(result)
		Else
			InformationText.Text = "Installation failed. For detailed information on how to resolve errors, refer to the Relativity Applications documentation." & Environment.NewLine & Environment.NewLine & _
			"The following errors occurred while installing the application:" & Environment.NewLine & Environment.NewLine
			InformationText.Links.Add(84, 38, HelpLink)

			If Not String.IsNullOrEmpty(result.Message) Then
				InformationText.Text = String.Format(CultureInfo.CurrentCulture, "{0} {1} {2}", InformationText.Text, ExpandedText, result.Message)
				InformationText.Links.Add(195, ExpandedText.Length, "Details")
			End If

			artifactTable = CreateFailedTable(result)
		End If

		ArtifactStatusTable.DataSource = artifactTable
		ArtifactStatusTable.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
		ArtifactStatusTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
		ArtifactStatusTable.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing
		ArtifactStatusTable.AllowUserToResizeColumns = True
		ArtifactStatusTable.AllowUserToOrderColumns = True
		ArtifactStatusTable.ReadOnly = True

		ColorTable()

		ExportButton.Enabled = True
	End Sub

	Private Function CreateSucessTable(ByVal result As TemplateManagerBase.ApplicationInstallationResult) As DataTable
		Dim successTable As New DataTable()

		successTable.Columns.Add("Name", GetType(String))
		successTable.Columns.Add("Artifact Type", GetType(String))
		successTable.Columns.Add("Artifact ID", GetType(Integer))
		successTable.Columns.Add("Status", GetType(String))

		For Each art As TemplateManagerBase.ApplicationArtifact In result.NewApplicationArtifacts
			successTable.Rows.Add(New Object() {art.Name, TypeToString(art.Type), art.ArtifactId, "Created"})
		Next

		For Each art As TemplateManagerBase.ApplicationArtifact In result.UpdatedApplicationArtifacts
			successTable.Rows.Add(New Object() {art.Name, TypeToString(art.Type), art.ArtifactId, "Updated"})
		Next

		Return successTable
	End Function

	Private Function CreateFailedTable(ByVal result As TemplateManagerBase.ApplicationInstallationResult) As DataTable
		Dim failedTable As New DataTable()

		failedTable.Columns.Add("Name", GetType(String))
		failedTable.Columns.Add("Object Type", GetType(String))
		failedTable.Columns.Add("Artifact Type", GetType(String))
		failedTable.Columns.Add("Conflicting Artifact Name", GetType(String))
		failedTable.Columns.Add("Conflicting Artifact ID", GetType(Integer))
		failedTable.Columns.Add("Locked Applications", GetType(String))
		failedTable.Columns.Add("Error", GetType(String))
		failedTable.Columns.Add("Details", GetType(String))

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

			failedTable.Rows.Add(New Object() { _
			 art.Name, _
			 parentName, _
			 TypeToString(art.Type), _
			 conflictName, _
			 conflictID, _
			 conflictApps, _
			 StatusToString(art.Status), _
			 art.StatusMessage})
		Next

		Return failedTable
	End Function

	Private Sub ColorTable()
		If result.Success Then
			For Each row As DataGridViewRow In ArtifactStatusTable.Rows
				row.Cells("Status").Style.BackColor = Color.PaleGreen
			Next
		Else
			For Each row As DataGridViewRow In ArtifactStatusTable.Rows
				row.Cells("Error").ToolTipText = row.Cells("Details").Value.ToString
				row.Cells("Error").Style.BackColor = Color.LightPink
			Next
		End If
	End Sub

#Region " Event handlers "

	Private Sub Observer_OnProcessEvent(ByVal evt As kCura.Windows.Process.Generic.ProcessEvent(Of TemplateManagerBase.ApplicationInstallationResult)) Handles observer.OnProcessEvent
		Me.Invoke(Sub() UpdateArtifactStatusTable(evt))
	End Sub

	Private Sub ArtifactStatusTable_sort(ByVal sender As Object, ByVal e As System.EventArgs) Handles ArtifactStatusTable.Sorted
		ColorTable()
	End Sub

	Private Sub InformationLabel_LinkClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles InformationText.LinkClicked
		If String.Equals(CType(e.Link.LinkData, String), "Details") Then
			If errorExpanded Then
				InformationText.Text = "Installation failed. For detailed information on how to resolve errors, refer to the Relativity Applications documentation." & Environment.NewLine & Environment.NewLine & _
				 "The following errors occurred while installing the application:" & Environment.NewLine & Environment.NewLine & " More Detail " & result.Message
				errorExpanded = False
				InformationText.Parent.Height = InformationText.Height
				InformationText.Parent.Width = InformationText.Width
			Else
				InformationText.Text = "Installation failed. For detailed information on how to resolve errors, refer to the Relativity Applications documentation." & Environment.NewLine & Environment.NewLine & _
				 "The following errors occurred while installing the application:" & Environment.NewLine & Environment.NewLine & " Less Detail " & result.Message & _
				 Environment.NewLine & result.Details
				errorExpanded = True
				InformationText.Parent.Height = InformationText.Height
				InformationText.Parent.Width = InformationText.Width
			End If
		Else
			System.Diagnostics.Process.Start(CType(e.Link.LinkData, String))
		End If
	End Sub

	Private Sub CopyErrorToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyErrorToolStripMenuItem.Click
		If errorExpanded Then
			Clipboard.SetText("Message:" & Environment.NewLine & result.Message & Environment.NewLine & Environment.NewLine & "Details:" & Environment.NewLine & result.Details)
		Else
			Clipboard.SetText("Message:" & Environment.NewLine & result.Message)
		End If
	End Sub

	Private Sub ExportButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ExportButton.Click
		Debug.Assert(artifactTable IsNot Nothing)

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

		Dim saveAsCsvDialog As New SaveFileDialog()
		saveAsCsvDialog.Filter = "csv files (*.csv)|*.csv"
		saveAsCsvDialog.FileName = String.Format("RA_{0}_{1}.csv", "Import", System.DateTime.Now.ToString("yyyyMMddHHmmss"))

		If saveAsCsvDialog.ShowDialog() = DialogResult.OK Then
			Dim myStream As IO.Stream = saveAsCsvDialog.OpenFile()
			If (myStream IsNot Nothing) Then
				Dim encoding As New System.Text.UTF8Encoding()
				myStream.Write(encoding.GetBytes(csvBuilder.ToString), 0, encoding.GetByteCount(csvBuilder.ToString))
				myStream.Close()
			End If
		End If
	End Sub

	Private Sub CloseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CloseButton.Click
		Me.Close()
	End Sub

#End Region

	Private Function TypeToString(ByVal type As TemplateManagerBase.ApplicationArtifactType) As String
		If type = TemplateManagerBase.ApplicationArtifactType.Object Then
			Return "Object Type"
		Else
			Return type.ToString
		End If
	End Function

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