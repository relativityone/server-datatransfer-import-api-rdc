Namespace kCura.EDDS.WinForm
	Public Class CaseSelectForm
		Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

		Public Sub New()
			MyBase.New()

			'This call is required by the Windows Form Designer.
			InitializeComponent()

			'Add any initialization after the InitializeComponent() call
			_application = kCura.EDDS.WinForm.Application.Instance
		End Sub

		'Form overrides dispose to clean up the component list.
		Protected Overloads Overrides Sub Dispose(ByVal disposing As Boolean)
			If disposing Then
				If Not (components Is Nothing) Then
					components.Dispose()
				End If
			End If
			MyBase.Dispose(disposing)
		End Sub

		'Required by the Windows Form Designer
		Private components As System.ComponentModel.IContainer

		'NOTE: The following procedure is required by the Windows Form Designer
		'It can be modified using the Windows Form Designer.  
		'Do not modify it using the code editor.
		Friend WithEvents RepositoryName As System.Windows.Forms.TextBox
		Friend WithEvents CaseListView As System.Windows.Forms.ListView
		Friend WithEvents NameColumnHeader As System.Windows.Forms.ColumnHeader
		Friend WithEvents OKButton As System.Windows.Forms.Button
		Friend WithEvents CancelBtn As System.Windows.Forms.Button
		Friend WithEvents SearchQuery As System.Windows.Forms.TextBox
		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Dim resources As System.Resources.ResourceManager = New System.Resources.ResourceManager(GetType(CaseSelectForm))
			Me.OKButton = New System.Windows.Forms.Button
			Me.CancelBtn = New System.Windows.Forms.Button
			Me.CaseListView = New System.Windows.Forms.ListView
			Me.NameColumnHeader = New System.Windows.Forms.ColumnHeader
			Me.SearchQuery = New System.Windows.Forms.TextBox
			Me.SuspendLayout()
			'
			'OKButton
			'
			Me.OKButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.OKButton.Enabled = False
			Me.OKButton.Location = New System.Drawing.Point(128, 256)
			Me.OKButton.Name = "OKButton"
			Me.OKButton.TabIndex = 3
			Me.OKButton.Text = "OK"
			'
			'CancelBtn
			'
			Me.CancelBtn.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.CancelBtn.Location = New System.Drawing.Point(208, 256)
			Me.CancelBtn.Name = "CancelBtn"
			Me.CancelBtn.TabIndex = 4
			Me.CancelBtn.Text = "Cancel"
			'
			'CaseListView
			'
			Me.CaseListView.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
					Or System.Windows.Forms.AnchorStyles.Left) _
					Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.CaseListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
			Me.CaseListView.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.NameColumnHeader})
			Me.CaseListView.Location = New System.Drawing.Point(4, 28)
			Me.CaseListView.MultiSelect = False
			Me.CaseListView.Name = "CaseListView"
			Me.CaseListView.Size = New System.Drawing.Size(280, 224)
			Me.CaseListView.Sorting = System.Windows.Forms.SortOrder.Ascending
			Me.CaseListView.TabIndex = 6
			Me.CaseListView.View = System.Windows.Forms.View.Details
			'
			'NameColumnHeader
			'
			Me.NameColumnHeader.Text = "Name"
			'
			'SearchQuery
			'
			Me.SearchQuery.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
					Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.SearchQuery.Location = New System.Drawing.Point(4, 4)
			Me.SearchQuery.Name = "SearchQuery"
			Me.SearchQuery.Size = New System.Drawing.Size(280, 20)
			Me.SearchQuery.TabIndex = 7
			Me.SearchQuery.Text = ""
			'
			'CaseSelectForm
			'
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.ClientSize = New System.Drawing.Size(288, 285)
			Me.Controls.Add(Me.SearchQuery)
			Me.Controls.Add(Me.CaseListView)
			Me.Controls.Add(Me.CancelBtn)
			Me.Controls.Add(Me.OKButton)
			Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
			Me.MaximizeBox = False
			Me.MinimizeBox = False
			Me.Name = "CaseSelectForm"
			Me.ShowInTaskbar = False
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
			Me.Text = "Relativity Desktop Client | Select Workspace"
			Me.TopMost = True
			Me.ResumeLayout(False)

		End Sub

#End Region

#Region " Declarations & Properties "
		Private WithEvents _application As kCura.EDDS.WinForm.Application
		Private _selectedCaseInfo As Relativity.CaseInfo

		Public ReadOnly Property SelectedCaseInfo() As Relativity.CaseInfo
			Get
				Return _selectedCaseInfo
			End Get
		End Property
#End Region
		Private _cases As System.Data.DataSet
		Private Sub CaseSelectForm_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
			_cases = _application.GetCases
			Me.LoadCases("")
			Me.Focus()
			SearchQuery.Focus()
		End Sub

		Private Sub CaseListView_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CaseListView.SelectedIndexChanged
			Dim caseManager As New kCura.EDDS.WebAPI.CaseManagerBase.CaseManager
			If CaseListView.SelectedItems.Count <> 0 Then
				_selectedCaseInfo = DirectCast(CaseListView.SelectedItems.Item(0).Tag, Relativity.CaseInfo)
				_OKButton.Enabled = True
			Else
				_selectedCaseInfo = Nothing
			End If
		End Sub

		Private Sub CaseListView_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles CaseListView.Resize
			NameColumnHeader.Width = CaseListView.Width - 6
		End Sub

		Private Sub CancelBtn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CancelBtn.Click
			_selectedCaseInfo = Nothing
			Me.Close()
		End Sub

		Private Sub OKButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OKButton.Click
			Me.Close()
		End Sub

		Private Sub CaseListView_DoubleClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles CaseListView.DoubleClick
			Me.Close()
		End Sub

		Private Sub LoadCases(ByVal searchText As String)
			Me.Cursor = Cursors.WaitCursor
			CaseListView.Items.Clear()
			If _cases Is Nothing Then
				Me.Cursor = Cursors.Default
				Me.Close()
				Exit Sub
			End If
			Dim dt As DataTable = _cases.Tables(0)
			Dim row As DataRow
			For Each row In dt.Rows
				If searchText.Trim = "" OrElse CType(row.Item("Name"), String).ToLower.IndexOf(searchText.Trim.ToLower) <> -1 Then
					Dim listItem As New System.Windows.Forms.ListViewItem
					listItem.Text = CType(row.Item("Name"), String)
					listItem.Tag = New Relativity.CaseInfo(row)
					CaseListView.Items.Add(listItem)
				End If
			Next
			NameColumnHeader.Width = CaseListView.Width - 6
			Me.Cursor = Cursors.Default
		End Sub

		Private Sub SearchQuery_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SearchQuery.TextChanged
			Me.LoadCases(SearchQuery.Text)
		End Sub

	End Class
End Namespace