Imports System.Collections.Generic
Imports System.Linq

Namespace kCura.EDDS.WinForm
	Public Class TextPrecedenceForm
		Friend WithEvents _longTextFieldsTwoListBox As kCura.Windows.Forms.TwoListBox
		Friend WithEvents _availableLongTextFieldsLabel As System.Windows.Forms.Label
		Friend WithEvents _selectedLongTextFieldsLabel As System.Windows.Forms.Label

		Private _listOfAllLongTextFields As New List(Of ViewFieldInfo)
		Private _selectedTextFields As New List(Of ViewFieldInfo)

		Public Sub New(ByVal listOfLongTextFields As List(Of ViewFieldInfo), ByVal selectedTextFields As List(Of ViewFieldInfo))

			' This call is required by the designer.
			InitializeComponent()

			' Add any initialization after the InitializeComponent() call.
			_listOfAllLongTextFields.Clear()
			_listOfAllLongTextFields.AddRange(listOfLongTextFields)

			_selectedTextFields.AddRange(selectedTextFields)

		End Sub



		Private Sub TextPrecedenceForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
			'Doing this here so designer works at least for some elements
			Me._longTextFieldsTwoListBox = New kCura.Windows.Forms.TwoListBox()
			Me._longTextFieldsTwoListBox.AlternateRowColors = False
			Me._longTextFieldsTwoListBox.KeepButtonsCentered = True
			Me._longTextFieldsTwoListBox.LeftOrderControlsVisible = False
			Me._longTextFieldsTwoListBox.Location = New System.Drawing.Point(8, 24)
			Me._longTextFieldsTwoListBox.Name = "_longTextFields"
			Me._longTextFieldsTwoListBox.RightOrderControlVisible = True
			Me._longTextFieldsTwoListBox.Size = New System.Drawing.Size(360, 280)
			Me._longTextFieldsTwoListBox.TabIndex = 2

			Me._availableLongTextFieldsLabel = New System.Windows.Forms.Label
			Me._availableLongTextFieldsLabel.Location = New System.Drawing.Point(8, 8)
			Me._availableLongTextFieldsLabel.Name = "_availableLongTextFieldsLabel"
			Me._availableLongTextFieldsLabel.Size = New System.Drawing.Size(144, 16)
			Me._availableLongTextFieldsLabel.TabIndex = 3
			Me._availableLongTextFieldsLabel.Text = "Available Long Text Fields"

			Me._selectedLongTextFieldsLabel = New System.Windows.Forms.Label
			Me._selectedLongTextFieldsLabel.Location = New System.Drawing.Point(196, 8)
			Me._selectedLongTextFieldsLabel.Name = "_selectedLongTextFieldsLabel"
			Me._selectedLongTextFieldsLabel.Size = New System.Drawing.Size(144, 16)
			Me._selectedLongTextFieldsLabel.TabIndex = 4
			Me._selectedLongTextFieldsLabel.Text = "Selected Long Text Fields"

			Me.Controls.Add(Me._longTextFieldsTwoListBox)
			Me.Controls.Add(Me._availableLongTextFieldsLabel)
			Me.Controls.Add(Me._selectedLongTextFieldsLabel)


			For Each field As ViewFieldInfo In _listOfAllLongTextFields
				_longTextFieldsTwoListBox.LeftListBoxItems.Add(field)
			Next

			For Each item As ViewFieldInfo In _selectedTextFields
				Dim itemExistsInAllAvailable As Boolean = GetItemExistsInAllAvailable(item)
				If itemExistsInAllAvailable Then
					_longTextFieldsTwoListBox.RightListBoxItems.Add(item)
					RemoveItemByName(_longTextFieldsTwoListBox.LeftListBoxItems, item)
				End If
			Next
		End Sub

		Private Sub RemoveItemByName(objectCollection As System.Windows.Forms.ListBox.ObjectCollection, item As ViewFieldInfo)
			Dim fieldToRemove As ViewFieldInfo = Nothing
			For Each field As ViewFieldInfo In objectCollection
				If field.DisplayName.Equals(item.DisplayName, StringComparison.InvariantCulture) Then
					fieldToRemove = field
					Exit For
				End If
			Next
			If fieldToRemove IsNot Nothing Then objectCollection.Remove(fieldToRemove)
		End Sub

		Private Function GetItemExistsInAllAvailable(ByVal item As ViewFieldInfo) As Boolean
			Dim itemExistsInSelected As Boolean = False
			For Each selectedItem As ViewFieldInfo In _listOfAllLongTextFields
				If selectedItem.DisplayName.Equals(item.DisplayName, StringComparison.InvariantCulture) Then
					itemExistsInSelected = True
					Exit For
				End If
			Next
			Return itemExistsInSelected
		End Function

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