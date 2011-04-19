﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ApplicationOutputForm
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
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ApplicationOutputForm))
		Me.Label1 = New System.Windows.Forms.Label()
		Me.ArtifactStatusTable = New System.Windows.Forms.DataGridView()
		Me.InformationText = New System.Windows.Forms.LinkLabel()
		Me.CopyErrorMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
		Me.CopyErrorToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
		Me.MainSplitContainer = New System.Windows.Forms.SplitContainer()
		Me.Panel1 = New System.Windows.Forms.Panel()
		CType(Me.ArtifactStatusTable, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.CopyErrorMenu.SuspendLayout()
		Me.MainSplitContainer.Panel1.SuspendLayout()
		Me.MainSplitContainer.Panel2.SuspendLayout()
		Me.MainSplitContainer.SuspendLayout()
		Me.Panel1.SuspendLayout()
		Me.SuspendLayout()
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label1.Location = New System.Drawing.Point(0, 2)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(178, 24)
		Me.Label1.TabIndex = 0
		Me.Label1.Text = "Import Status Report"
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
		Me.ArtifactStatusTable.Size = New System.Drawing.Size(805, 285)
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
		Me.MainSplitContainer.Panel2.Controls.Add(Me.ArtifactStatusTable)
		Me.MainSplitContainer.Size = New System.Drawing.Size(811, 538)
		Me.MainSplitContainer.SplitterDistance = 218
		Me.MainSplitContainer.TabIndex = 3
		'
		'Panel1
		'
		Me.Panel1.AutoScroll = True
		Me.Panel1.Controls.Add(Me.InformationText)
		Me.Panel1.Location = New System.Drawing.Point(12, 3)
		Me.Panel1.Name = "Panel1"
		Me.Panel1.Size = New System.Drawing.Size(789, 204)
		Me.Panel1.TabIndex = 3
		'
		'ApplicationOutputForm
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(813, 571)
		Me.Controls.Add(Me.MainSplitContainer)
		Me.Controls.Add(Me.Label1)
		Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
		Me.Name = "ApplicationOutputForm"
		Me.Text = "Relativity Desktop Client | Application Deployment System"
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
	Friend WithEvents Label1 As System.Windows.Forms.Label
	Friend WithEvents ArtifactStatusTable As System.Windows.Forms.DataGridView
	Friend WithEvents InformationText As System.Windows.Forms.LinkLabel
	Friend WithEvents MainSplitContainer As System.Windows.Forms.SplitContainer
	Friend WithEvents Panel1 As System.Windows.Forms.Panel
	Friend WithEvents CopyErrorMenu As System.Windows.Forms.ContextMenuStrip
	Friend WithEvents CopyErrorToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
End Class
