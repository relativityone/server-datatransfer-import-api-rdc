<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class FieldMapForm
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
		Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(FieldMapForm))
		Me.FieldMapGrid = New System.Windows.Forms.DataGridView()
		Me._exportButton = New System.Windows.Forms.Button()
		Me.Panel1 = New System.Windows.Forms.Panel()
		Me.CloseButton = New System.Windows.Forms.Button()
		Me.ExportToFileDialog = New System.Windows.Forms.SaveFileDialog()
		CType(Me.FieldMapGrid, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.Panel1.SuspendLayout()
		Me.SuspendLayout()
		'
		'FieldMapGrid
		'
		Me.FieldMapGrid.AllowUserToAddRows = False
		Me.FieldMapGrid.AllowUserToDeleteRows = False
		DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft
		DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control
		DataGridViewCellStyle1.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText
		DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
		DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
		DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
		Me.FieldMapGrid.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle1
		Me.FieldMapGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
		Me.FieldMapGrid.Dock = System.Windows.Forms.DockStyle.Fill
		Me.FieldMapGrid.Location = New System.Drawing.Point(0, 0)
		Me.FieldMapGrid.Name = "FieldMapGrid"
		Me.FieldMapGrid.ReadOnly = True
		Me.FieldMapGrid.Size = New System.Drawing.Size(434, 241)
		Me.FieldMapGrid.TabIndex = 0
		'
		'_exportButton
		'
		Me._exportButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me._exportButton.Location = New System.Drawing.Point(259, 250)
		Me._exportButton.Name = "_exportButton"
		Me._exportButton.Size = New System.Drawing.Size(85, 23)
		Me._exportButton.TabIndex = 1
		Me._exportButton.Text = "Export to CSV"
		Me._exportButton.UseVisualStyleBackColor = True
		'
		'Panel1
		'
		Me.Panel1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
						Or System.Windows.Forms.AnchorStyles.Left) _
						Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.Panel1.Controls.Add(Me.FieldMapGrid)
		Me.Panel1.Location = New System.Drawing.Point(1, 3)
		Me.Panel1.Name = "Panel1"
		Me.Panel1.Size = New System.Drawing.Size(434, 241)
		Me.Panel1.TabIndex = 2
		'
		'CloseButton
		'
		Me.CloseButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.CloseButton.Location = New System.Drawing.Point(350, 250)
		Me.CloseButton.Name = "CloseButton"
		Me.CloseButton.Size = New System.Drawing.Size(85, 23)
		Me.CloseButton.TabIndex = 3
		Me.CloseButton.Text = "Close"
		Me.CloseButton.UseVisualStyleBackColor = True
		'
		'ExportToFileDialog
		'
		Me.ExportToFileDialog.DefaultExt = "csv"
		Me.ExportToFileDialog.Filter = "CSV files|*.csv|All files|*.*"
		'
		'FieldMapForm
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(435, 273)
		Me.Controls.Add(Me.CloseButton)
		Me.Controls.Add(Me.Panel1)
		Me.Controls.Add(Me._exportButton)
		Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
		Me.Name = "FieldMapForm"
		Me.Text = "Relativity Desktop Client | Selected Field Map"
		CType(Me.FieldMapGrid, System.ComponentModel.ISupportInitialize).EndInit()
		Me.Panel1.ResumeLayout(False)
		Me.ResumeLayout(False)

	End Sub
	Friend WithEvents FieldMapGrid As System.Windows.Forms.DataGridView
	Friend WithEvents _exportButton As System.Windows.Forms.Button
	Friend WithEvents Panel1 As System.Windows.Forms.Panel
	Friend WithEvents CloseButton As System.Windows.Forms.Button
	Friend WithEvents ExportToFileDialog As System.Windows.Forms.SaveFileDialog
End Class
