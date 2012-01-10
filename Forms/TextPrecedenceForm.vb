Imports System.Collections.Generic

Namespace kCura.EDDS.WinForm
	Public Class TextPrecedenceForm
		Friend WithEvents _longTextFieldsTwoListBox As kCura.Windows.Forms.TwoListBox
		Friend WithEvents _availableLongTextFieldsLabel As System.Windows.Forms.Label
		Friend WithEvents _selectedLongTextFieldsLabel As System.Windows.Forms.Label

		Private _listOfAllLongTextFields As New List(Of ViewFieldInfo)
		Private _selectedTextFields As New List(Of ViewFieldInfo)

		Public Sub New(ByVal listOfLongTextFields As List(Of ViewFieldInfo), ByVal selectedTextFieldsDropDown As ListBox)

			' This call is required by the designer.
			InitializeComponent()

			' Add any initialization after the InitializeComponent() call.
			_listOfAllLongTextFields.Clear()
			_listOfAllLongTextFields.AddRange(listOfLongTextFields)

			For i As Int32 = 0 To selectedTextFieldsDropDown.Items.Count - 1
				_selectedTextFields.Add(DirectCast(selectedTextFieldsDropDown.Items(i), ViewFieldInfo))
			Next

		End Sub



		Private Sub TextPrecedenceForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
			'Doing this here so designer works at least for some elements
			Me._longTextFieldsTwoListBox = New kCura.Windows.Forms.TwoListBox()
			Me._longTextFieldsTwoListBox.AlternateRowColors = False
			Me._longTextFieldsTwoListBox.KeepButtonsCentered = True
			Me._longTextFieldsTwoListBox.LeftOrderControlsVisible = False
			Me._longTextFieldsTwoListBox.Location = New System.Drawing.Point(8, 104)
			Me._longTextFieldsTwoListBox.Name = "_longTextFields"
			Me._longTextFieldsTwoListBox.RightOrderControlVisible = True
			Me._longTextFieldsTwoListBox.Size = New System.Drawing.Size(360, 280)
			Me._longTextFieldsTwoListBox.TabIndex = 2

			Me._availableLongTextFieldsLabel = New System.Windows.Forms.Label
			Me._availableLongTextFieldsLabel.Location = New System.Drawing.Point(8, 88)
			Me._availableLongTextFieldsLabel.Name = "_availableLongTextFieldsLabel"
			Me._availableLongTextFieldsLabel.Size = New System.Drawing.Size(144, 16)
			Me._availableLongTextFieldsLabel.TabIndex = 3
			Me._availableLongTextFieldsLabel.Text = "Available Long Text Fields"

			Me._selectedLongTextFieldsLabel = New System.Windows.Forms.Label
			Me._selectedLongTextFieldsLabel.Location = New System.Drawing.Point(196, 88)
			Me._selectedLongTextFieldsLabel.Name = "_selectedLongTextFieldsLabel"
			Me._selectedLongTextFieldsLabel.Size = New System.Drawing.Size(144, 16)
			Me._selectedLongTextFieldsLabel.TabIndex = 4
			Me._selectedLongTextFieldsLabel.Text = "Selected Long Text Fields"

			Me.Controls.Add(Me._longTextFieldsTwoListBox)
			Me.Controls.Add(Me._availableLongTextFieldsLabel)
			Me.Controls.Add(Me._selectedLongTextFieldsLabel)

			'Me._longTextFields.LeftListBoxItems.AddRange(_listOfAllLongTextFields)
			For Each item As ViewFieldInfo In _listOfAllLongTextFields
				If _selectedTextFields IsNot Nothing AndAlso _selectedTextFields.Contains(item) Then
					_longTextFieldsTwoListBox.RightListBoxItems.Add(item)
				Else
					_longTextFieldsTwoListBox.LeftListBoxItems.Add(item)
				End If
			Next
		End Sub

		Public Event OkClicked()

		Private Sub _okButton_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles _okButton.Click
			RaiseEvent OkClicked()
			Me.Close()
		End Sub

		Public ReadOnly Property SelectedTextFields() As System.Windows.Forms.ListBox.ObjectCollection
			Get
				Return _longTextFieldsTwoListBox.RightListBoxItems
			End Get
		End Property

	End Class
End Namespace