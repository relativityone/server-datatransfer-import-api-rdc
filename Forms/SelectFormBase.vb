Namespace kCura.EDDS.WinForm
	Public MustInherit Class SelectFormBase
		Inherits System.Windows.Forms.Form

#Region " Windows Form Designer generated code "

		Public Sub New()
			MyBase.New()

			'This call is required by the Windows Form Designer.
			InitializeComponent()

			'Add any initialization after the InitializeComponent() call
			Application = kCura.EDDS.WinForm.Application.Instance
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
		Protected Friend WithEvents ItemListView As System.Windows.Forms.ListView
		Friend WithEvents NameColumnHeader As System.Windows.Forms.ColumnHeader
		Friend WithEvents OKButton As System.Windows.Forms.Button
		Friend WithEvents CancelBtn As System.Windows.Forms.Button
		Friend WithEvents SearchQuery As System.Windows.Forms.TextBox
		<System.Diagnostics.DebuggerStepThrough()> Private Sub InitializeComponent()
			Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(SelectFormBase))
			Me.OKButton = New System.Windows.Forms.Button()
			Me.CancelBtn = New System.Windows.Forms.Button()
			Me.ItemListView = New System.Windows.Forms.ListView()
			Me.NameColumnHeader = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
			Me.SearchQuery = New System.Windows.Forms.TextBox()
			Me.SuspendLayout()
			'
			'OKButton
			'
			Me.OKButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.OKButton.Enabled = False
			Me.OKButton.Location = New System.Drawing.Point(128, 256)
			Me.OKButton.Name = "OKButton"
			Me.OKButton.Size = New System.Drawing.Size(75, 23)
			Me.OKButton.TabIndex = 2
			Me.OKButton.Text = "OK"
			'
			'CancelBtn
			'
			Me.CancelBtn.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.CancelBtn.Location = New System.Drawing.Point(208, 256)
			Me.CancelBtn.Name = "CancelBtn"
			Me.CancelBtn.Size = New System.Drawing.Size(75, 23)
			Me.CancelBtn.TabIndex = 4
			Me.CancelBtn.Text = "Cancel"
			'
			'ItemListView
			'
			Me.ItemListView.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
							Or System.Windows.Forms.AnchorStyles.Left) _
							Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
			Me.ItemListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
			Me.ItemListView.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.NameColumnHeader})
			Me.ItemListView.Location = New System.Drawing.Point(4, 28)
			Me.ItemListView.MultiSelect = False
			Me.ItemListView.Name = "ItemListView"
			Me.ItemListView.Size = New System.Drawing.Size(280, 224)
			Me.ItemListView.Sorting = System.Windows.Forms.SortOrder.Ascending
			Me.ItemListView.TabIndex = 1
			Me.ItemListView.UseCompatibleStateImageBehavior = False
			Me.ItemListView.View = System.Windows.Forms.View.Details
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
			Me.SearchQuery.TabIndex = 0
			'
			'SelectFormBase
			'
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.ClientSize = New System.Drawing.Size(288, 285)
			Me.Controls.Add(Me.SearchQuery)
			Me.Controls.Add(Me.ItemListView)
			Me.Controls.Add(Me.CancelBtn)
			Me.Controls.Add(Me.OKButton)
			Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
			Me.MaximizeBox = False
			Me.MinimizeBox = False
			Me.Name = "SelectFormBase"
			Me.ShowInTaskbar = False
			Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
			Me.TopMost = True
			Me.ResumeLayout(False)
			Me.PerformLayout()

		End Sub

#End Region

#Region " Declarations & Properties "
		Protected WithEvents Application As kCura.EDDS.WinForm.Application

		Protected ReadOnly Property ConfirmButton As Button
			Get
				Return _OKButton
			End Get
		End Property

		Public WriteOnly Property MultiSelect As Boolean
			Set(ByVal value As Boolean)
				Me.ItemListView.MultiSelect = value
			End Set
		End Property

#End Region

		Protected MustOverride Sub LoadItems(ByVal searchText As String)
		Protected MustOverride Sub ItemListView_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ItemListView.SelectedIndexChanged

		Private Sub OKButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OKButton.Click, ItemListView.DoubleClick
			Me.DialogResult = DialogResult.OK
			Me.Close()
		End Sub

		Private Sub SearchQuery_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles SearchQuery.TextChanged
			Me.LoadItems(SearchQuery.Text)
		End Sub

		Private Sub ItemListView_Resize(ByVal sender As Object, ByVal e As System.EventArgs) Handles ItemListView.Resize
			NameColumnHeader.Width = ItemListView.Width - 6
		End Sub

	End Class
End Namespace