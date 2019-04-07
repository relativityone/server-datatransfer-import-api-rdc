﻿Imports Relativity.Desktop.Client.Legacy.Controls

Namespace Relativity.Desktop.Client
	<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
	Partial Class TextPrecedenceForm
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
			Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(TextPrecedenceForm))
			Me._okButton = New System.Windows.Forms.Button()
			Me._cancelButton = New System.Windows.Forms.Button()
			Me.SuspendLayout()
			'
			'_okButton
			'
			Me._okButton.Location = New System.Drawing.Point(369, 247)
			Me._okButton.Name = "_okButton"
			Me._okButton.Size = New System.Drawing.Size(75, 23)
			Me._okButton.TabIndex = 0
			Me._okButton.Text = "OK"
			Me._okButton.UseVisualStyleBackColor = True
			'
			'_cancelButton
			'
			Me._cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel
			Me._cancelButton.Location = New System.Drawing.Point(369, 278)
			Me._cancelButton.Name = "_cancelButton"
			Me._cancelButton.Size = New System.Drawing.Size(75, 23)
			Me._cancelButton.TabIndex = 1
			Me._cancelButton.Text = "Cancel"
			Me._cancelButton.UseVisualStyleBackColor = True
			'
			'_longTextFields - must be defined in InitializeComponent, otherwise medium & high DPI don't work
			'
			Me._longTextFieldsTwoListBox = New TwoListBox()
			Me._longTextFieldsTwoListBox.Name = "_longTextFields"
			Me._longTextFieldsTwoListBox.Location = New System.Drawing.Point(8, 24)
			Me._longTextFieldsTwoListBox.Size = New System.Drawing.Size(356, 276)
			Me._longTextFieldsTwoListBox.AlternateRowColors = False
			Me._longTextFieldsTwoListBox.KeepButtonsCentered = True
			Me._longTextFieldsTwoListBox.LeftOrderControlsVisible = False

			Me._longTextFieldsTwoListBox.RightOrderControlVisible = True

			Me._longTextFieldsTwoListBox.TabIndex = 2
			'
			'_availableTextFieldsLabel - must be defined in IntializeComponent, otherwise medium & high DPI don't work
			'
			Me._availableLongTextFieldsLabel = New System.Windows.Forms.Label
			Me._availableLongTextFieldsLabel.Name = "_availableLongTextFieldsLabel"
			Me._availableLongTextFieldsLabel.Location = New System.Drawing.Point(8, 8)
			Me._availableLongTextFieldsLabel.Size = New System.Drawing.Size(144, 16)
			Me._availableLongTextFieldsLabel.TabIndex = 3
			Me._availableLongTextFieldsLabel.Text = "Available Long Text Fields"
			'
			'_selectedLongTextFieldsLabel - must be defined in IntializeComponent, otherwise medium & high DPI don't work
			'
			Me._selectedLongTextFieldsLabel = New System.Windows.Forms.Label
			Me._selectedLongTextFieldsLabel.Name = "_selectedLongTextFieldsLabel"
			Me._selectedLongTextFieldsLabel.Location = New System.Drawing.Point(184, 8)
			Me._selectedLongTextFieldsLabel.Size = New System.Drawing.Size(144, 16)
			Me._selectedLongTextFieldsLabel.TabIndex = 4
			Me._selectedLongTextFieldsLabel.Text = "Selected Long Text Fields"

			Me.Controls.Add(Me._longTextFieldsTwoListBox)
			Me.Controls.Add(Me._availableLongTextFieldsLabel)
			Me.Controls.Add(Me._selectedLongTextFieldsLabel)

			'TextPrecedenceForm
			'
			Me.AutoScaleBaseSize = New System.Drawing.Size(5, 13)
			Me.ClientSize = New System.Drawing.Size(456, 313)
			Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable
			Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
			Me.AcceptButton = Me._okButton
			Me.CancelButton = Me._cancelButton
			Me.Controls.Add(Me._cancelButton)
			Me.Controls.Add(Me._okButton)
			Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
			Me.MinimizeBox = False
			Me.MinimumSize = New System.Drawing.Size(464, 340)
			Me.Name = "TextPrecedenceForm"
			Me.Text = "Pick Text Precedence"

			Me.ResumeLayout(False)

		End Sub

		Friend WithEvents _okButton As System.Windows.Forms.Button
		Friend WithEvents _cancelButton As System.Windows.Forms.Button


	End Class
End Namespace
