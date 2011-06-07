<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class RelativityApplicationStatusForm
	Inherits System.Windows.Forms.Form

	'Form overrides dispose to clean up the component list.
	<System.Diagnostics.DebuggerNonUserCode()> _
	Protected Overrides Sub Dispose(ByVal disposing As Boolean)
		Try
			If disposing AndAlso components IsNot Nothing Then
				components.Dispose()
			End If
		Finally
			MyBase.Dispose(disposing)
		End Try
	End Sub

	'Required by the Windows Form Designer
	Private components As System.ComponentModel.IContainer

	'NOTE: The following procedure is required by the Windows Form Designer
	'It can be modified using the Windows Form Designer.  
	'Do not modify it using the code editor.
	<System.Diagnostics.DebuggerStepThrough()> _
	Private Sub InitializeComponent()
		Me.components = New System.ComponentModel.Container()
		Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(RelativityApplicationStatusForm))
		Me.StatusHeader = New System.Windows.Forms.Label()
		Me.ArtifactStatusTable = New System.Windows.Forms.DataGridView()
		Me.InformationText = New System.Windows.Forms.LinkLabel()
		Me.CopyErrorMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
		Me.CopyErrorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
		Me.MainSplitContainer = New System.Windows.Forms.SplitContainer()
		Me.Panel1 = New System.Windows.Forms.Panel()
		Me.RetryImportButton = New System.Windows.Forms.Button()
		Me.DetailsButton = New System.Windows.Forms.Button()
		Me.CloseButton = New System.Windows.Forms.Button()
		Me.ExportButton = New System.Windows.Forms.Button()
		CType(Me.ArtifactStatusTable, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.CopyErrorMenu.SuspendLayout()
		Me.MainSplitContainer.Panel1.SuspendLayout()
		Me.MainSplitContainer.Panel2.SuspendLayout()
		Me.MainSplitContainer.SuspendLayout()
		Me.Panel1.SuspendLayout()
		Me.SuspendLayout()
		'
		'StatusHeader
		'
		Me.StatusHeader.AutoSize = True
		Me.StatusHeader.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.StatusHeader.Location = New System.Drawing.Point(0, 2)
		Me.StatusHeader.Name = "StatusHeader"
		Me.StatusHeader.Size = New System.Drawing.Size(178, 24)
		Me.StatusHeader.TabIndex = 0
		Me.StatusHeader.Text = "Import Status Report"
		'
		'ArtifactStatusTable
		'
		Me.ArtifactStatusTable.AllowUserToAddRows = False
		Me.ArtifactStatusTable.AllowUserToDeleteRows = False
		DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
		Me.ArtifactStatusTable.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
		Me.ArtifactStatusTable.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
						Or System.Windows.Forms.AnchorStyles.Left) _
						Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.ArtifactStatusTable.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells
		Me.ArtifactStatusTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
		Me.ArtifactStatusTable.Location = New System.Drawing.Point(3, 3)
		Me.ArtifactStatusTable.Name = "ArtifactStatusTable"
		Me.ArtifactStatusTable.RowHeadersWidth = 75
		DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
		Me.ArtifactStatusTable.RowsDefaultCellStyle = DataGridViewCellStyle2
		Me.ArtifactStatusTable.RowTemplate.Height = 30
		Me.ArtifactStatusTable.RowTemplate.ReadOnly = True
		Me.ArtifactStatusTable.Size = New System.Drawing.Size(874, 378)
		Me.ArtifactStatusTable.TabIndex = 1
		'
		'InformationText
		'
		Me.InformationText.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
						Or System.Windows.Forms.AnchorStyles.Left) _
						Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.InformationText.AutoSize = True
		Me.InformationText.ContextMenuStrip = Me.CopyErrorMenu
		Me.InformationText.LinkArea = New System.Windows.Forms.LinkArea(0, 0)
		Me.InformationText.Location = New System.Drawing.Point(0, 0)
		Me.InformationText.Name = "InformationText"
		Me.InformationText.Size = New System.Drawing.Size(59, 13)
		Me.InformationText.TabIndex = 2
		Me.InformationText.Text = "Importing..."
		'
		'CopyErrorMenu
		'
		Me.CopyErrorMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CopyErrorToolStripMenuItem})
		Me.CopyErrorMenu.Name = "CopyErrorMenu"
		Me.CopyErrorMenu.Size = New System.Drawing.Size(127, 26)
		'
		'CopyErrorToolStripMenuItem
		'
		Me.CopyErrorToolStripMenuItem.Name = "CopyErrorToolStripMenuItem"
		Me.CopyErrorToolStripMenuItem.Size = New System.Drawing.Size(126, 22)
		Me.CopyErrorToolStripMenuItem.Text = "Copy Error"
		'
		'MainSplitContainer
		'
		Me.MainSplitContainer.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
						Or System.Windows.Forms.AnchorStyles.Left) _
						Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.MainSplitContainer.Location = New System.Drawing.Point(0, 36)
		Me.MainSplitContainer.Name = "MainSplitContainer"
		Me.MainSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal
		'
		'MainSplitContainer.Panel1
		'
		Me.MainSplitContainer.Panel1.AutoScroll = True
		Me.MainSplitContainer.Panel1.AutoScrollMargin = New System.Drawing.Size(5, 5)
		Me.MainSplitContainer.Panel1.Controls.Add(Me.Panel1)
		'
		'MainSplitContainer.Panel2
		'
		Me.MainSplitContainer.Panel2.Controls.Add(Me.RetryImportButton)
		Me.MainSplitContainer.Panel2.Controls.Add(Me.DetailsButton)
		Me.MainSplitContainer.Panel2.Controls.Add(Me.CloseButton)
		Me.MainSplitContainer.Panel2.Controls.Add(Me.ExportButton)
		Me.MainSplitContainer.Panel2.Controls.Add(Me.ArtifactStatusTable)
		Me.MainSplitContainer.Size = New System.Drawing.Size(886, 534)
		Me.MainSplitContainer.SplitterDistance = 122
		Me.MainSplitContainer.TabIndex = 3
		'
		'Panel1
		'
		Me.Panel1.AutoScroll = True
		Me.Panel1.Controls.Add(Me.InformationText)
		Me.Panel1.Location = New System.Drawing.Point(6, 3)
		Me.Panel1.Name = "Panel1"
		Me.Panel1.Size = New System.Drawing.Size(161, 55)
		Me.Panel1.TabIndex = 3
		'
		'RetryImportButton
		'
		Me.RetryImportButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.RetryImportButton.Enabled = False
		Me.RetryImportButton.Location = New System.Drawing.Point(556, 383)
		Me.RetryImportButton.Name = "RetryImportButton"
		Me.RetryImportButton.Size = New System.Drawing.Size(103, 23)
		Me.RetryImportButton.TabIndex = 6
		Me.RetryImportButton.Text = "Retry Import"
		Me.RetryImportButton.UseVisualStyleBackColor = True
		'
		'DetailsButton
		'
		Me.DetailsButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.DetailsButton.Enabled = False
		Me.DetailsButton.Location = New System.Drawing.Point(416, 383)
		Me.DetailsButton.Name = "DetailsButton"
		Me.DetailsButton.Size = New System.Drawing.Size(134, 23)
		Me.DetailsButton.TabIndex = 5
		Me.DetailsButton.Text = "View Details"
		Me.DetailsButton.UseVisualStyleBackColor = True
		'
		'CloseButton
		'
		Me.CloseButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.CloseButton.Location = New System.Drawing.Point(774, 383)
		Me.CloseButton.Name = "CloseButton"
		Me.CloseButton.Size = New System.Drawing.Size(103, 23)
		Me.CloseButton.TabIndex = 4
		Me.CloseButton.Text = "Close"
		Me.CloseButton.UseVisualStyleBackColor = True
		'
		'ExportButton
		'
		Me.ExportButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.ExportButton.Enabled = False
		Me.ExportButton.Location = New System.Drawing.Point(665, 383)
		Me.ExportButton.Name = "ExportButton"
		Me.ExportButton.Size = New System.Drawing.Size(103, 23)
		Me.ExportButton.TabIndex = 2
		Me.ExportButton.Text = "Export To CSV"
		Me.ExportButton.UseVisualStyleBackColor = True
		'
		'RelativityApplicationStatusForm
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(883, 582)
		Me.Controls.Add(Me.MainSplitContainer)
		Me.Controls.Add(Me.StatusHeader)
		Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
		Me.MinimumSize = New System.Drawing.Size(380, 395)
		Me.Name = "RelativityApplicationStatusForm"
		Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
		Me.Text = "Relativity Desktop Client | Import Status"
		CType(Me.ArtifactStatusTable, System.ComponentModel.ISupportInitialize).EndInit()
		Me.CopyErrorMenu.ResumeLayout(False)
		Me.MainSplitContainer.Panel1.ResumeLayout(False)
		Me.MainSplitContainer.Panel2.ResumeLayout(False)
		Me.MainSplitContainer.ResumeLayout(False)
		Me.Panel1.ResumeLayout(False)
		Me.Panel1.PerformLayout()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents StatusHeader As System.Windows.Forms.Label
	Friend WithEvents ArtifactStatusTable As System.Windows.Forms.DataGridView
	Friend WithEvents InformationText As System.Windows.Forms.LinkLabel
	Friend WithEvents MainSplitContainer As System.Windows.Forms.SplitContainer
	Friend WithEvents Panel1 As System.Windows.Forms.Panel
	Friend WithEvents CopyErrorMenu As System.Windows.Forms.ContextMenuStrip
	Friend WithEvents CopyErrorToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
	Friend WithEvents ExportButton As System.Windows.Forms.Button
	Friend WithEvents CloseButton As System.Windows.Forms.Button
	Friend WithEvents DetailsButton As System.Windows.Forms.Button
	Friend WithEvents RetryImportButton As System.Windows.Forms.Button
End Class
