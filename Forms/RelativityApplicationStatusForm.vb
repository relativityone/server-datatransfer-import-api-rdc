
Imports System.Globalization
Imports kCura.EDDS.WebAPI

Public Class RelativityApplicationStatusForm
	Inherits System.Windows.Forms.Form

	Public WithEvents observer As kCura.Windows.Process.Generic.ProcessObserver(Of TemplateManagerBase.ApplicationInstallationResult)

	Private Const workspaceIDColumnName As String = "Workspace ID"
	Private Const workspaceNameColumnName As String = "Workspace Name"
	Private Const workspaceStatusColumnName As String = "Install Status"

	Private Const WorkspaceSuccessString As String = "Completed"
	Private Const WorkspaceErrorString As String = "Error"

	Private artifactTable As New DataTable()
	Private results As Generic.List(Of TemplateManagerBase.ApplicationInstallationResult)
	Private globalSuccess As Boolean = True
	Private currentResultIndex As Int32
	Private _workspaceView As Boolean
	Private errorExpanded As Boolean

	Private Const ExpandText As String = "[+]"
	Private Const CollapseText As String = "[-]"
	Private Const HelpLink As String = "http://help.kcura.com/relativity/Relativity Applications/Using a Relativity Application.pdf#installhelp"
	Private ErrorMessagePart1 As String = "Installation failed. For details on potential resolutions to the errors you may have encountered here, please refer to the "
	Private ErrorMessageLink As String = "Relativity Applications documentation."
	Private ErrorMessagePart2 As String = Environment.NewLine & " - this link takes you to the most recent version of the document, which may not match with your version of Relativity" & Environment.NewLine & Environment.NewLine & "The following errors occurred while installing the application:" & Environment.NewLine & Environment.NewLine

	Private Property WorkspaceView As Boolean
		Get
			Return _workspaceView
		End Get
		Set(ByVal value As Boolean)
			If value Then
				DetailsButton.Text = "View Details"
			Else
				DetailsButton.Text = "Back to Workspaces"
			End If
			_workspaceView = value
		End Set
	End Property

	Private Sub Observer_OnProcessEvent(ByVal evt As kCura.Windows.Process.Generic.ProcessEvent(Of TemplateManagerBase.ApplicationInstallationResult)) Handles observer.OnProcessEvent
		Me.Invoke(Sub() ProcessEvent(evt))
	End Sub

	Private Sub ProcessEvent(ByVal evt As kCura.Windows.Process.Generic.ProcessEvent(Of TemplateManagerBase.ApplicationInstallationResult))
		If results Is Nothing Then
			results = New Generic.List(Of TemplateManagerBase.ApplicationInstallationResult)(evt.Result.TotalWorkspaces)
		End If

		results.Add(evt.Result)
		globalSuccess = globalSuccess And evt.Result.Success
		If results.Capacity > 1 Then
			DetailsButton.Visible = True
			UpdateWorkspaceStatusView()
		Else
			DetailsButton.Visible = False
			currentResultIndex = 0
			UpdateArtifactStatusView()
		End If

	End Sub

	Private Sub UpdateWorkspaceStatusView()
		WorkspaceView = True
		StatusHeader.Text = "Installation Status Report"
		If results.Count < results.Capacity Then
			InformationText.Text = String.Format("Installing... ({0}/{1})", results.Count, results.Capacity)
		Else
			DetailsButton.Enabled = True
			ExportButton.Enabled = True
			If globalSuccess Then
				InformationText.Text = String.Format("Installaiton complete. Select a workspace, then click the ""View Details"" button for more information.")
			Else
				InformationText.Text = String.Format(CultureInfo.CurrentCulture, "{0}{1}{2} Select a workspace, then click the ""View Details"" button for more information.", ErrorMessagePart1, ErrorMessageLink, ErrorMessagePart2)
				InformationText.Links.Clear()
				InformationText.Links.Add(ErrorMessagePart1.Length, ErrorMessageLink.Length, HelpLink)
			End If
		End If
		InformationText.Parent.Height = InformationText.Height
		InformationText.Parent.Width = InformationText.Width

		artifactTable = CreateWorkspaceTable()
		UpdateArtifactStatusTableProperties()

	End Sub

	Private Sub UpdateArtifactStatusView()
		WorkspaceView = False
		errorExpanded = False
		Dim result As TemplateManagerBase.ApplicationInstallationResult = results(currentResultIndex)
		StatusHeader.Text = String.Format("Installation Status Report -- {0} ({1})", result.WorkspaceName, result.WorkspaceID)

		If result.Success Then
			InformationText.Text = "Installation complete."

			artifactTable = CreateSucessTable(result)
		Else
			InformationText.Text = String.Format(CultureInfo.CurrentCulture, "{0}{1}{2}", ErrorMessagePart1, ErrorMessageLink, ErrorMessagePart2)
			InformationText.Links.Clear()
			InformationText.Links.Add(ErrorMessagePart1.Length, ErrorMessageLink.Length, HelpLink)

			If Not String.IsNullOrEmpty(result.Message) Then
				InformationText.Text = String.Format(CultureInfo.CurrentCulture, "{0}{1} {2}", InformationText.Text, ExpandText, result.Message)
				InformationText.Links.Add(ErrorMessagePart1.Length + ErrorMessageLink.Length + ErrorMessagePart2.Length, ExpandText.Length, "Details")
			End If

			artifactTable = CreateFailedTable(result)
		End If
		InformationText.Parent.Height = InformationText.Height
		InformationText.Parent.Width = InformationText.Width

		UpdateArtifactStatusTableProperties()

		ExportButton.Enabled = True
	End Sub

	Private Sub UpdateArtifactStatusTableProperties()
		ArtifactStatusTable.DataSource = artifactTable
		ArtifactStatusTable.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None
		ArtifactStatusTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
		ArtifactStatusTable.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing
		ArtifactStatusTable.AllowUserToResizeColumns = True
		ArtifactStatusTable.AllowUserToOrderColumns = True
		ArtifactStatusTable.ReadOnly = True
		ArtifactStatusTable.MultiSelect = False

		ColorTable()
	End Sub

	Private Function CreateWorkspaceTable() As DataTable
		Dim workspaceTable As New DataTable()

		workspaceTable.Columns.Add(workspaceIDColumnName, GetType(Integer))
		workspaceTable.Columns.Add(workspaceNameColumnName, GetType(String))
		workspaceTable.Columns.Add(workspaceStatusColumnName, GetType(String))

		For Each res As TemplateManagerBase.ApplicationInstallationResult In results
			workspaceTable.Rows.Add(New Object() {res.WorkspaceID, res.WorkspaceName, WorkspaceResultToString(res.Success)})
		Next

		Return workspaceTable

	End Function

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
		If WorkspaceView Then
			For Each row As DataGridViewRow In ArtifactStatusTable.Rows
				If row.Cells(workspaceStatusColumnName).Value.ToString.Equals(WorkspaceSuccessString) Then
					row.Cells(workspaceStatusColumnName).Style.BackColor = Color.PaleGreen
				Else
					row.Cells(workspaceStatusColumnName).Style.BackColor = Color.LightPink
				End If
			Next
		Else
			If results(currentResultIndex).Success Then
				For Each row As DataGridViewRow In ArtifactStatusTable.Rows
					row.Cells("Status").Style.BackColor = Color.PaleGreen
				Next
			Else
				For Each row As DataGridViewRow In ArtifactStatusTable.Rows
					row.Cells("Error").ToolTipText = row.Cells("Details").Value.ToString
					row.Cells("Error").Style.BackColor = Color.LightPink
				Next
			End If
		End If
		
	End Sub

#Region " Event handlers "

	Private Sub ArtifactStatusTable_Sort(ByVal sender As Object, ByVal e As System.EventArgs) Handles ArtifactStatusTable.Sorted
		ColorTable()
	End Sub

	Private Sub ArtifactStatusTable_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles ArtifactStatusTable.DoubleClick
		If WorkspaceView Then
			GoToDetailsView()
		End If

	End Sub

	Private Sub DetailsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DetailsButton.Click
		If WorkspaceView Then
			GoToDetailsView()
		Else
			UpdateWorkspaceStatusView()
		End If
	End Sub

	Private Sub GoToDetailsView()
		If ArtifactStatusTable.SelectedRows.Count > 0 Then
			currentResultIndex = findResultByID(CInt(ArtifactStatusTable.SelectedRows(0).Cells(workspaceIDColumnName).Value))
		ElseIf ArtifactStatusTable.SelectedCells.Count > 0 Then
			currentResultIndex = findResultByID(CInt(ArtifactStatusTable.Rows(ArtifactStatusTable.SelectedCells(0).RowIndex).Cells(workspaceIDColumnName).Value))
		Else
			Return 'The user has done something asinine, like selecting a column. Do nothing.
		End If
		UpdateArtifactStatusView()
	End Sub

	Private Sub InformationLabel_LinkClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles InformationText.LinkClicked
		Dim result As TemplateManagerBase.ApplicationInstallationResult = results(currentResultIndex)
		If String.Equals(CType(e.Link.LinkData, String), "Details") Then
			InformationText.Links.Clear()
			If errorExpanded Then
				InformationText.Text = String.Format(CultureInfo.CurrentCulture, "{0}{1}{2}{3} {4}", ErrorMessagePart1, ErrorMessageLink, ErrorMessagePart2, ExpandText, result.Message)
				errorExpanded = False
				InformationText.Parent.Height = InformationText.Height
				InformationText.Parent.Width = InformationText.Width


				If Not String.IsNullOrEmpty(result.Message) Then
					InformationText.Links.Add(ErrorMessagePart1.Length + ErrorMessageLink.Length + ErrorMessagePart2.Length, ExpandText.Length, "Details")
				End If

			Else
				InformationText.Text = String.Format(CultureInfo.CurrentCulture, "{0}{1}{2}{3} {4}{5}{6}", ErrorMessagePart1, ErrorMessageLink, ErrorMessagePart2, CollapseText, result.Message, Environment.NewLine, result.Details)
				errorExpanded = True
				InformationText.Parent.Height = InformationText.Height
				InformationText.Parent.Width = InformationText.Width

				If Not String.IsNullOrEmpty(result.Message) Then
					InformationText.Links.Add(ErrorMessagePart1.Length + ErrorMessageLink.Length + ErrorMessagePart2.Length, CollapseText.Length, "Details")
				End If

			End If

			InformationText.Links.Add(ErrorMessagePart1.Length, ErrorMessageLink.Length, HelpLink)

		Else
			System.Diagnostics.Process.Start(CType(e.Link.LinkData, String))
		End If
	End Sub

	Private Sub CopyErrorToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CopyErrorToolStripMenuItem.Click
		Dim result As TemplateManagerBase.ApplicationInstallationResult = results(currentResultIndex)
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

	Private Function findResultByID(ByVal workspaceID As Int32) As Int32
		For i = 0 To results.Count
			If results(i).WorkspaceID = workspaceID Then
				Return i
			End If
		Next
		Throw New System.Exception(String.Format("The following Workspace ID was not found in the results list: {0}", workspaceID))
	End Function

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

	Private Function WorkspaceResultToString(ByVal stat As Boolean) As String
		If stat Then
			Return WorkspaceSuccessString
		Else
			Return WorkspaceErrorString
		End If
	End Function

End Class