Imports System.Linq
Imports System.Globalization
Imports kCura.EDDS.WebAPI

Public Class RelativityApplicationStatusForm
	Inherits System.Windows.Forms.Form

	Public WithEvents observer As kCura.Windows.Process.Generic.ProcessObserver(Of TemplateManagerBase.ApplicationInstallationResult)

#Region " Constants "

	Private Const FieldNameMaximumLength As Int32 = 50

	Private Const workspaceIDColumnName As String = "Workspace ID"
	Private Const workspaceNameColumnName As String = "Workspace Name"
	Private Const workspaceStatusColumnName As String = "Install Status"

	Private Const WorkspaceSuccessString As String = "Completed"
	Private Const WorkspaceErrorString As String = "Error"

	Private Const ArtifactNameColumnName As String = "Name"
	Private Const ArtifactIDColumnName As String = "Artifact ID"
	Private Const ArtifactStatusColumnName As String = "Status"
	Private Const ArtifactHiddenErrorColumnName As String = "Hidden Error"
	Private Const ArtifactApplicationIdsColumnName As String = "Application IDs"
	Private Const ArtifactResolutionColumnName As String = "Resolution"
	Private Const ArtifactConflictIDColumnName As String = "Conflicting Artifact ID"
	Private Const ArtifactTypeIDColumnName As String = "Artifact Type ID"
	Private Const ArtifactConflictNameColumnName As String = "Conflicting Artifact Name"
	Private Const ArtifactIndexColumnName As String = "Index"
	Private Const ArtifcactSelectedResolutionColumnName = "Selected Resolution"
	Private Const DropdownUnlock As String = "Unlock"
	Private Const DropdownRenameInWorkspace As String = "Rename in Workspace"
	Private Const DropdownRenameFriendlyNameInWorkspace As String = "Rename in Workspace"
	Private Const DropdownRetryRename As String = "Choose another Name"
	Private Const ExpandText As String = "[+]"
	Private Const CollapseText As String = "[-]"
	Private Const HelpLink As String = "http://help.kcura.com/relativity/Relativity Applications/Using Relativity Applications for RDC - 7.0.pdf#Resolving-Errors"

#End Region

	Private Delegate Sub SelectCell(ByVal cell As DataGridViewCell)
	Private _selectCell As SelectCell

	Private artifactTable As New DataTable()
	Private results As Generic.List(Of TemplateManagerBase.ApplicationInstallationResult)
	Private globalSuccess As Boolean = True
	Private currentResultIndex As Int32
	Private _workspaceView As Boolean
	Private errorExpanded As Boolean

	Private ErrorMessagePart1 As String = "Installation failed.  For details on potential resolutions to the errors you may have encountered here, refer to the "
	Private ErrorMessageLink As String = "Relativity Applications documentation."
	Private ErrorMessagePart2 As String = Environment.NewLine & Environment.NewLine & "The following errors occurred while installing the application:" & Environment.NewLine & Environment.NewLine

	Private application As Xml.XmlDocument
	Private credential As Net.NetworkCredential
	Private cookieContainer As Net.CookieContainer
	Private caseInfos As Generic.IEnumerable(Of Relativity.CaseInfo)
	Private _processPool As kCura.Windows.Process.ProcessPool
	Private _retryEnabled As Boolean
	Private ar As IAsyncResult = Nothing

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

	Public Sub New(ByVal app As Xml.XmlDocument, ByVal cred As Net.NetworkCredential, ByVal cookies As Net.CookieContainer, ByVal cases As Generic.IEnumerable(Of Relativity.CaseInfo))
		InitializeComponent()

		application = app
		credential = cred
		cookieContainer = cookies
		caseInfos = cases
		_processPool = New kCura.Windows.Process.ProcessPool()
	End Sub

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
				InformationText.Text = String.Format("Installation complete. Select a workspace, then click the ""View Details"" button for more information.")
			Else
				InformationText.Text = String.Format(CultureInfo.CurrentCulture, "{0}{1}{2} Select a workspace, then click the ""View Details"" button for more information.", ErrorMessagePart1, ErrorMessageLink, ErrorMessagePart2)
				InformationText.Links.Clear()
				InformationText.Links.Add(ErrorMessagePart1.Length, ErrorMessageLink.Length, HelpLink)
			End If
		End If

		InformationText.Parent.Height = InformationText.Height
		InformationText.Parent.Width = InformationText.Width

		artifactTable = CreateWorkspaceTable()
		ArtifactStatusTable.DataSource = artifactTable

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

		ArtifactStatusTable.DataSource = artifactTable

		UpdateArtifactStatusTableProperties()

		ExportButton.Enabled = True

		_retryEnabled = False
		SetButtonVisibility()
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

		For Each art As TemplateManagerBase.ApplicationArtifact In result.StatusApplicationArtifacts
			successTable.Rows.Add(New Object() {art.Name, TypeToString(art.Type), art.ArtifactId, StatusToString(art.Status, "")})
		Next

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

		failedTable.Columns.Add(ArtifactStatusColumnName, GetType(String))
		failedTable.Columns.Add(ArtifactHiddenErrorColumnName, GetType(TemplateManagerBase.StatusCode))
		failedTable.Columns.Add(ArtifactNameColumnName, GetType(String))
		failedTable.Columns.Add(ArtifactIDColumnName, GetType(Int32))
		failedTable.Columns.Add("Object Type Name", GetType(String))
		failedTable.Columns.Add("Artifact Type", GetType(String))
		failedTable.Columns.Add(ArtifactTypeIDColumnName, GetType(TemplateManagerBase.ApplicationArtifactType))
		failedTable.Columns.Add("Locked Applications", GetType(String))
		failedTable.Columns.Add(ArtifactApplicationIdsColumnName, GetType(Int32()))
		failedTable.Columns.Add(ArtifactConflictNameColumnName, GetType(String))
		failedTable.Columns.Add(ArtifactConflictIDColumnName, GetType(Integer))
		failedTable.Columns.Add("Details", GetType(String))
		failedTable.Columns.Add(ArtifactIndexColumnName, GetType(Integer))
		failedTable.Columns.Add(ArtifcactSelectedResolutionColumnName, GetType(String))

		Dim index As Int32 = 0
		For Each art As TemplateManagerBase.ApplicationArtifact In result.StatusApplicationArtifacts
			Dim parentName As String = ""
			Dim conflictName As String = ""
			Dim conflictID As Integer = Nothing
			Dim conflictApps As New System.Text.StringBuilder()
			Dim conflictAppIDs As New System.Collections.Generic.List(Of Int32)

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

						conflictAppIDs.Add(app.ID)
					Next
				End If
			End If

			failedTable.Rows.Add(New Object() { _
			 StatusToString(art.Status, conflictApps.ToString), _
			 art.Status, _
			 art.Name, _
			 art.ArtifactId, _
			 parentName, _
			 TypeToString(art.Type), _
			 art.Type, _
			 conflictApps, _
			 conflictAppIDs.ToArray(), _
			 conflictName, _
			 conflictID, _
			 art.StatusMessage, _
			 index})
			index += 1
		Next

		Return failedTable
	End Function

	Private Sub UpdateArtifactStatusTableProperties()
		If Not results(currentResultIndex).Success Then
			ArtifactStatusTable.Columns("Locked Applications").Visible = False
			ArtifactStatusTable.Columns(ArtifactHiddenErrorColumnName).Visible = False
			ArtifactStatusTable.Columns(ArtifactIndexColumnName).Visible = False
			ArtifactStatusTable.Columns(ArtifactTypeIDColumnName).Visible = False
			ArtifactStatusTable.Columns(ArtifcactSelectedResolutionColumnName).Visible = False

			If Not ArtifactStatusTable.Columns.Contains(ArtifactResolutionColumnName) Then
				Dim dropDownColumn As New DataGridViewComboBoxColumn()
				dropDownColumn.Name = ArtifactResolutionColumnName
				ArtifactStatusTable.Columns.Insert(0, dropDownColumn)
			End If

			For Each col As DataGridViewColumn In ArtifactStatusTable.Columns
				If Not col.Name.Equals(ArtifactConflictNameColumnName, StringComparison.CurrentCulture) AndAlso Not col.Name.Equals(ArtifactResolutionColumnName, StringComparison.CurrentCulture) Then
					col.ReadOnly = True
				End If
			Next

			For Each row As DataGridViewRow In ArtifactStatusTable.Rows
				Dim comboBoxCell As DataGridViewComboBoxCell = CType(row.Cells(ArtifactResolutionColumnName), DataGridViewComboBoxCell)

				Select Case CType(row.Cells(ArtifactHiddenErrorColumnName).Value, TemplateManagerBase.StatusCode)
					Case TemplateManagerBase.StatusCode.FriendlyNameConflict
						comboBoxCell.Items.Add(DropdownRenameFriendlyNameInWorkspace)
					Case TemplateManagerBase.StatusCode.NameConflict
						comboBoxCell.Items.Add(DropdownRenameInWorkspace)
					Case TemplateManagerBase.StatusCode.SharedByLockedApp
						comboBoxCell.Items.Add(DropdownUnlock)
					Case TemplateManagerBase.StatusCode.RenameConflict
						comboBoxCell.Items.Add(DropdownRetryRename)
					Case TemplateManagerBase.StatusCode.MultipleFileField
					Case TemplateManagerBase.StatusCode.UnknownError
					Case TemplateManagerBase.StatusCode.Updated
						comboBoxCell.ReadOnly = True
						comboBoxCell.DisplayStyle = DataGridViewComboBoxDisplayStyle.Nothing
					Case Else
				End Select

				Dim selectedResolution As String = If(row.Cells(ArtifcactSelectedResolutionColumnName).Value Is Nothing, String.Empty, row.Cells(ArtifcactSelectedResolutionColumnName).Value.ToString)
				If Not String.IsNullOrEmpty(selectedResolution) Then
					comboBoxCell.Value = selectedResolution
					row.Cells(ArtifactResolutionColumnName).ReadOnly = False
				End If

				comboBoxCell.ReadOnly = False
			Next
		End If

		ArtifactStatusTable.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells
		ArtifactStatusTable.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells
		If Not results(currentResultIndex).Success Then ArtifactStatusTable.Columns("Details").AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill
		ArtifactStatusTable.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.EnableResizing
		ArtifactStatusTable.AllowUserToResizeColumns = True
		ArtifactStatusTable.AllowUserToOrderColumns = True
		ArtifactStatusTable.MultiSelect = False
		ArtifactStatusTable.RowHeadersWidth = 15
		ArtifactStatusTable.EditMode = DataGridViewEditMode.EditOnEnter And DataGridViewEditMode.EditProgrammatically

		ColorTable()
	End Sub

	Private Sub ColorTable()
		If WorkspaceView Then
			For Each row As DataGridViewRow In ArtifactStatusTable.Rows
				If row.Cells(workspaceStatusColumnName).Value.ToString.Equals(WorkspaceSuccessString, StringComparison.CurrentCulture) Then
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
					row.Cells(ArtifactStatusColumnName).Style.BackColor = If(String.Equals(row.Cells(ArtifactStatusColumnName).Value.ToString(), "Updated", StringComparison.InvariantCulture), Color.PaleGreen, Color.LightPink)
				Next
			End If
		End If
	End Sub

	Private Sub SelectCellSub(ByVal cell As DataGridViewCell)
		cell.ReadOnly = False
		cell.Selected = True
		ArtifactStatusTable.BeginEdit(True)
	End Sub

	Private Sub GoToDetailsView()
		If ArtifactStatusTable.SelectedRows.Count > 0 Then
			currentResultIndex = FindResultByID(CInt(ArtifactStatusTable.SelectedRows(0).Cells(workspaceIDColumnName).Value))
		ElseIf ArtifactStatusTable.SelectedCells.Count > 0 Then
			currentResultIndex = FindResultByID(CInt(ArtifactStatusTable.Rows(ArtifactStatusTable.SelectedCells(0).RowIndex).Cells(workspaceIDColumnName).Value))
		Else
			Return 'The user selected a column. Do nothing.
		End If
		UpdateArtifactStatusView()
	End Sub

#Region " Event handlers "

	Private Sub artifactstatustable_sort(ByVal sender As Object, ByVal e As System.EventArgs) Handles ArtifactStatusTable.Sorted
		updateartifactstatustableproperties()
	End Sub

	Private Sub ArtifactStatusTable_EditingControlShowing(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewEditingControlShowingEventArgs) Handles ArtifactStatusTable.EditingControlShowing
		If String.Equals(ArtifactStatusTable.CurrentCell.OwningColumn.Name, ArtifactResolutionColumnName, StringComparison.CurrentCulture) Then
			Dim comboBox As ComboBox = DirectCast(e.Control, ComboBox)
			If Not comboBox Is Nothing Then
				RemoveHandler comboBox.SelectedValueChanged, AddressOf Me.renameCell_SelectedIndexChanged
				AddHandler comboBox.SelectedValueChanged, AddressOf Me.renameCell_SelectedIndexChanged
			End If
		ElseIf String.Equals(ArtifactStatusTable.CurrentCell.OwningColumn.Name, ArtifactConflictNameColumnName, StringComparison.CurrentCulture) Then
			Dim textBox As TextBox = DirectCast(e.Control, TextBox)
			RemoveHandler textBox.TextChanged, AddressOf Me.NameTextbox_TextChanged
			AddHandler textBox.TextChanged, AddressOf Me.NameTextbox_TextChanged
			textBox.MaxLength = FieldNameMaximumLength
		End If
	End Sub

	Private Sub NameTextbox_TextChanged(ByVal sender As Object, ByVal e As System.EventArgs)
		If ArtifactStatusTable.IsCurrentCellInEditMode Then
			CheckForRetryEnabled()
		End If
	End Sub

	Private Sub ArtifactStatusTable_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles ArtifactStatusTable.DoubleClick
		If WorkspaceView Then
			GoToDetailsView()
		End If
	End Sub

	Private Sub ArtifactStatusTable_CellValidating(ByVal sender As Object, ByVal e As DataGridViewCellValidatingEventArgs) Handles ArtifactStatusTable.CellValidating
		' Validate the rename entry by disallowing empty strings and ensuring size limits.
		If ArtifactStatusTable.Columns(e.ColumnIndex).Name = ArtifactConflictNameColumnName Then
			If e.FormattedValue Is Nothing OrElse _
			 e.FormattedValue.ToString().Length < 1 OrElse e.FormattedValue.ToString.Length > FieldNameMaximumLength Then
				e.Cancel = True
			End If
		End If
	End Sub

	Private Sub ArtifactStatusTable_CellEndEdit(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles ArtifactStatusTable.CellEndEdit
		' Clear the row error in case the user presses ESC.   
		ArtifactStatusTable.Rows(e.RowIndex).ErrorText = String.Empty

		If Not ar Is Nothing Then
			ArtifactStatusTable.EndInvoke(ar)
		End If
	End Sub

	Private Sub renameCell_SelectedIndexChanged(ByVal sender As Object, ByVal e As EventArgs)
		If ArtifactStatusTable.SelectedCells.Count > 0 Then
			Dim cell As DataGridViewComboBoxCell = DirectCast(ArtifactStatusTable.SelectedCells.Item(0), DataGridViewComboBoxCell)
			If Not cell Is Nothing AndAlso cell.Items.Count > 0 Then
				Dim choice As String = DirectCast(cell.Items.Item(0), String)
				If Not choice.Equals(cell.OwningRow.Cells(ArtifcactSelectedResolutionColumnName).Value.ToString, StringComparison.InvariantCulture) Then
					cell.OwningRow.Cells(ArtifcactSelectedResolutionColumnName).Value = choice
				End If

				If Not cell Is Nothing AndAlso cell.Items.Count > 0 AndAlso (String.Equals(DirectCast(cell.EditedFormattedValue, String), DropdownRenameInWorkspace) _
				OrElse String.Equals(DirectCast(cell.EditedFormattedValue, String), DropdownRenameFriendlyNameInWorkspace) _
				OrElse String.Equals(DirectCast(cell.EditedFormattedValue, String), DropdownRetryRename)) Then
					cell.Selected = False
					Dim conflictingNameCell As DataGridViewCell = cell.OwningRow.Cells(ArtifactConflictNameColumnName)

					_selectCell = New SelectCell(AddressOf SelectCellSub)
					ar = ArtifactStatusTable.BeginInvoke(_selectCell, conflictingNameCell)
				End If
			End If
		End If

		CheckForRetryEnabled()
	End Sub

	Private Sub CheckForRetryEnabled()
		_retryEnabled = True

		For Each row As DataGridViewRow In ArtifactStatusTable.Rows
			Dim status As String = row.Cells(ArtifactStatusColumnName).Value.ToString()
			If Not String.Equals(status, "Updated", StringComparison.InvariantCulture) Then
				Dim cb As DataGridViewComboBoxCell = DirectCast(row.Cells("Resolution"), DataGridViewComboBoxCell)
				Dim cbStr As String = DirectCast(cb.EditedFormattedValue, String)
				Dim renamedText As String = Nothing

				If String.IsNullOrEmpty(cbStr) Then _retryEnabled = False : Exit For

				If String.Equals(cbStr, DropdownRenameInWorkspace, StringComparison.InvariantCulture) OrElse String.Equals(cbStr, DropdownRenameFriendlyNameInWorkspace, StringComparison.InvariantCulture) OrElse String.Equals(cbStr, DropdownRetryRename, StringComparison.InvariantCulture) Then
					If TypeOf (row.Cells(ArtifactConflictNameColumnName)) Is DataGridViewTextBoxCell Then
						renamedText = DirectCast(row.Cells(ArtifactConflictNameColumnName), DataGridViewTextBoxCell).EditedFormattedValue.ToString()
					Else
						renamedText = row.Cells(ArtifactConflictNameColumnName).Value.ToString()
					End If
					If String.IsNullOrEmpty(renamedText) OrElse String.Equals(renamedText, row.Cells(ArtifactNameColumnName).Value.ToString(), StringComparison.CurrentCultureIgnoreCase) Then
						_retryEnabled = False
						Exit For
					End If
				ElseIf Not String.Equals(cbStr, DropdownUnlock, StringComparison.CurrentCulture) Then
					_retryEnabled = False
					Exit For
				End If
			End If

		Next

		SetButtonVisibility()
	End Sub

	Private Sub DetailsButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles DetailsButton.Click
		If WorkspaceView Then
			GoToDetailsView()
		Else
			UpdateWorkspaceStatusView()
		End If
	End Sub

	Private Sub InformationLabel_LinkClicked(ByVal sender As Object, ByVal e As System.Windows.Forms.LinkLabelLinkClickedEventArgs) Handles InformationText.LinkClicked
		Dim result As TemplateManagerBase.ApplicationInstallationResult = results(currentResultIndex)
		Dim linkData As String = CType(e.Link.LinkData, String)

		If String.Equals(linkData, "Details", StringComparison.CurrentCulture) Then
			InformationText.Links.Clear()
			If errorExpanded Then
				InformationText.Text = String.Format(CultureInfo.CurrentCulture, "{0}{1}{2}{3} {4}", ErrorMessagePart1, ErrorMessageLink, ErrorMessagePart2, ExpandText, result.Message)
				errorExpanded = False

				If Not String.IsNullOrEmpty(result.Message) Then
					InformationText.Links.Add(ErrorMessagePart1.Length + ErrorMessageLink.Length + ErrorMessagePart2.Length, ExpandText.Length, "Details")
				End If
			Else
				InformationText.Text = String.Format(CultureInfo.CurrentCulture, "{0}{1}{2}{3} {4}{5}{6}", ErrorMessagePart1, ErrorMessageLink, ErrorMessagePart2, CollapseText, result.Message, Environment.NewLine, result.Details)
				errorExpanded = True

				If Not String.IsNullOrEmpty(result.Message) Then
					InformationText.Links.Add(ErrorMessagePart1.Length + ErrorMessageLink.Length + ErrorMessagePart2.Length, CollapseText.Length, "Details")
				End If
			End If

			InformationText.Parent.Height = InformationText.Height
			InformationText.Parent.Width = InformationText.Width
			InformationText.Links.Add(ErrorMessagePart1.Length, ErrorMessageLink.Length, HelpLink)
		Else
			System.Diagnostics.Process.Start(linkData)
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

	Private Sub RetryImportButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles RetryImportButton.Click
		Dim appsToOverride() As Int32 = GetAppsToOverride()
		Dim resArts() As TemplateManagerBase.ResolveArtifact = GetResolveArtifacts()

		Dim applicationDeploymentProcess As New kCura.WinEDDS.ApplicationDeploymentProcess(appsToOverride, resArts, application, Me.credential, Me.cookieContainer, caseInfos)
		Me.observer = applicationDeploymentProcess.ProcessObserver

		InformationText.Text = "Importing..."
		ArtifactStatusTable.DataSource = Nothing
		ArtifactStatusTable.Columns.Clear()
		results = Nothing

		_processPool.StartProcess(applicationDeploymentProcess)
	End Sub

	Private Sub CloseButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CloseButton.Click
		Me.Close()
	End Sub

#End Region

	Private Function GetAppsToOverride() As Int32()
		Dim apps As New System.Collections.Generic.List(Of Int32)

		For Each row As DataGridViewRow In ArtifactStatusTable.Rows
			If row.Cells(ArtifactResolutionColumnName).Value IsNot Nothing AndAlso String.Equals(row.Cells(ArtifactResolutionColumnName).Value.ToString, DropdownUnlock) Then
				Dim index As Int32 = CInt(row.Cells(ArtifactIndexColumnName).Value)
				Dim appIDs() As Int32 = CType(artifactTable.Rows(index).Item(ArtifactApplicationIdsColumnName), Int32())
				Dim ids() As Int32 = (From i As Int32 In (appIDs).AsEnumerable() Select i Where Not apps.Contains(i)).ToArray()
				apps.AddRange(ids)

			End If
		Next

		Return apps.ToArray
	End Function

	Private Function GetResolveArtifacts() As TemplateManagerBase.ResolveArtifact()
		Dim resArts As New System.Collections.Generic.List(Of TemplateManagerBase.ResolveArtifact)
		Dim kvp As TemplateManagerBase.FieldKVP

		For Each row As DataGridViewRow In ArtifactStatusTable.Rows
			If row.Cells(ArtifactResolutionColumnName).Value IsNot Nothing Then
				If (String.Equals(row.Cells(ArtifactResolutionColumnName).Value.ToString, DropdownRenameInWorkspace) OrElse String.Equals(row.Cells(ArtifactResolutionColumnName).Value.ToString, DropdownRenameFriendlyNameInWorkspace)) Then
					kvp = New TemplateManagerBase.FieldKVP()
					If CType(row.Cells(ArtifactHiddenErrorColumnName).Value, TemplateManagerBase.StatusCode) = TemplateManagerBase.StatusCode.FriendlyNameConflict Then
						kvp.Key = "Friendly Name"
					Else
						kvp.Key = "Name"
					End If
					kvp.Value = row.Cells(ArtifactConflictNameColumnName).Value
					resArts.Add(New TemplateManagerBase.ResolveArtifact() With { _
						.ArtifactID = CInt(row.Cells(ArtifactConflictIDColumnName).Value), _
						.ArtifactTypeID = CType(row.Cells(ArtifactTypeIDColumnName).Value,  _
						TemplateManagerBase.ApplicationArtifactType), .Fields = New TemplateManagerBase.FieldKVP() {kvp}, _
						.Action = TemplateManagerBase.ResolveAction.Update})
				ElseIf String.Equals(row.Cells(ArtifactResolutionColumnName).Value.ToString, DropdownRetryRename) Then
					kvp = New TemplateManagerBase.FieldKVP()
					kvp.Key = "Name"
					kvp.Value = row.Cells(ArtifactConflictNameColumnName).Value
					resArts.Add(New TemplateManagerBase.ResolveArtifact() With { _
						.ArtifactID = CInt(row.Cells(ArtifactIDColumnName).Value), _
						.ArtifactTypeID = CType(row.Cells(ArtifactTypeIDColumnName).Value, TemplateManagerBase.ApplicationArtifactType), _
						.Fields = New TemplateManagerBase.FieldKVP() {kvp}, _
						.Action = TemplateManagerBase.ResolveAction.Update})
				End If
			End If
		Next

		Return resArts.ToArray()
	End Function

	Private Function FindResultByID(ByVal workspaceID As Int32) As Int32
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

	Private Function StatusToString(ByVal stat As TemplateManagerBase.StatusCode, ByVal lockedApps As String) As String
		Dim resolutionDropDown As New ComboBox
		Select Case stat
			Case TemplateManagerBase.StatusCode.FriendlyNameConflict
				Return "Friendly Name Conflict"
			Case TemplateManagerBase.StatusCode.MultipleFileField
				Return "Multiple File Fields"
			Case TemplateManagerBase.StatusCode.NameConflict
				Return "Name Conflict"
			Case TemplateManagerBase.StatusCode.SharedByLockedApp
				Return String.Format("Shared By A Locked App: {0}", lockedApps)
			Case TemplateManagerBase.StatusCode.UnknownError
				Return "Unknown Error"
			Case TemplateManagerBase.StatusCode.RenameConflict
				Return "Rename Conflict"
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

	Private Sub SetButtonVisibility()
		If Not _retryEnabled Then
			RetryImportButton.Enabled = False
		Else
			RetryImportButton.Enabled = True
		End If
	End Sub

End Class