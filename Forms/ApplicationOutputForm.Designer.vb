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
		Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
		Me.Label1 = New System.Windows.Forms.Label()
		Me.ArtifactStatusTable = New System.Windows.Forms.DataGridView()
		CType(Me.ArtifactStatusTable, System.ComponentModel.ISupportInitialize).BeginInit()
		Me.SuspendLayout()
		'
		'Label1
		'
		Me.Label1.AutoSize = True
		Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 14.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
		Me.Label1.Location = New System.Drawing.Point(5, 5)
		Me.Label1.Name = "Label1"
		Me.Label1.Size = New System.Drawing.Size(178, 24)
		Me.Label1.TabIndex = 0
		Me.Label1.Text = "Import Status Report"
		'
		'ArtifactStatusTable
		'
		Me.ArtifactStatusTable.AllowUserToAddRows = False
		DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
		Me.ArtifactStatusTable.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
		Me.ArtifactStatusTable.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
						Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
		Me.ArtifactStatusTable.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells
		Me.ArtifactStatusTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
		Me.ArtifactStatusTable.Location = New System.Drawing.Point(12, 132)
		Me.ArtifactStatusTable.Name = "ArtifactStatusTable"
		Me.ArtifactStatusTable.RowHeadersWidth = 75
		DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
		Me.ArtifactStatusTable.RowsDefaultCellStyle = DataGridViewCellStyle2
		Me.ArtifactStatusTable.RowTemplate.Height = 30
		Me.ArtifactStatusTable.Size = New System.Drawing.Size(841, 325)
		Me.ArtifactStatusTable.TabIndex = 1
		'
		'ApplicationOutputForm
		'
		Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
		Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
		Me.ClientSize = New System.Drawing.Size(863, 505)
		Me.Controls.Add(Me.ArtifactStatusTable)
		Me.Controls.Add(Me.Label1)
		Me.Name = "ApplicationOutputForm"
		Me.Text = "ApplicationOutputForm"
		CType(Me.ArtifactStatusTable, System.ComponentModel.ISupportInitialize).EndInit()
		Me.ResumeLayout(False)
		Me.PerformLayout()

	End Sub
	Friend WithEvents Label1 As System.Windows.Forms.Label
	Friend WithEvents ArtifactStatusTable As System.Windows.Forms.DataGridView
End Class
