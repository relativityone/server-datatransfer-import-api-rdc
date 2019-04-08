﻿Imports Relativity.Desktop.Client.Legacy.Controls

Namespace Relativity.Desktop.Client
	Public Class TextPrecedenceForm
		Friend WithEvents _longTextFieldsTwoListBox As TwoListBox
		Friend WithEvents _availableLongTextFieldsLabel As System.Windows.Forms.Label
		Friend WithEvents _selectedLongTextFieldsLabel As System.Windows.Forms.Label

		Private _listOfAllLongTextFields As New List(Of kCura.WinEDDS.ViewFieldInfo)
		Private _selectedTextFields As New List(Of kCura.WinEDDS.ViewFieldInfo)

		Public Sub New(ByVal listOfLongTextFields As List(Of kCura.WinEDDS.ViewFieldInfo), ByVal selectedTextFields As List(Of kCura.WinEDDS.ViewFieldInfo))

			' This call is required by the designer.
			InitializeComponent()

			' Add any initialization after the InitializeComponent() call.
			_listOfAllLongTextFields.Clear()
			_listOfAllLongTextFields.AddRange(listOfLongTextFields)

			_selectedTextFields.AddRange(selectedTextFields)

		End Sub

		Private Sub TextPrecedenceForm_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

			For Each field As kCura.WinEDDS.ViewFieldInfo In _listOfAllLongTextFields
				_longTextFieldsTwoListBox.LeftSearchableList.AddField(field)
			Next

			For Each item As kCura.WinEDDS.ViewFieldInfo In _selectedTextFields
				Dim itemExistsInAllAvailable As Boolean = GetItemExistsInAllAvailable(item)
				If itemExistsInAllAvailable Then
					_longTextFieldsTwoListBox.RightSearchableList.AddField(item)
					RemoveItemByName(_longTextFieldsTwoListBox.LeftSearchableList, item)
				End If
			Next
		End Sub

		Private Sub RemoveItemByName(searchableList As SearchableList, item As kCura.WinEDDS.ViewFieldInfo)
			Dim fieldToRemove As kCura.WinEDDS.ViewFieldInfo = Nothing
			For Each field As kCura.WinEDDS.ViewFieldInfo In searchableList.DataSource
				If field.DisplayName.Equals(item.DisplayName, StringComparison.InvariantCulture) Then
					fieldToRemove = field
					Exit For
				End If
			Next
			If fieldToRemove IsNot Nothing Then searchableList.RemoveField(fieldToRemove)
		End Sub

		Private Function GetItemExistsInAllAvailable(ByVal item As kCura.WinEDDS.ViewFieldInfo) As Boolean
			Dim itemExistsInSelected As Boolean = False
			For Each selectedItem As kCura.WinEDDS.ViewFieldInfo In _listOfAllLongTextFields
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

		Public ReadOnly Property SelectedTextFields() As List(Of Object)
			Get
				Return _longTextFieldsTwoListBox.RightSearchableListItems
			End Get
		End Property

#Region "Resizing"
		'These member variables are populated with data needed to resize the controls

		'Avoid adjusting the layout if the size hasn't changed
		Private _layoutLastFormSize As Size

		' Used to keep track of whether we need to calculate the layout values.  In addition to
		' initial population, they may need to be populated later due to autoscaling.  Autoscaling
		' will change the distance between concrols which we would not expect to change.  If this
		' happens, the _layout info which contains the relative location of controls needs to be 
		' updated.
		Private _layoutReferenceDistance As Int32 = 0

		Private _layoutDifferenceList As List(Of RelativeLayoutData)

		Private Function CalcReferenceDistance() As Int32
			Return _availableLongTextFieldsLabel.Width
		End Function

		Private Sub OnForm_Layout(ByVal sender As Object, ByVal e As System.Windows.Forms.LayoutEventArgs) Handles MyBase.Layout
			'The reference distance should remain constant even if the dialog box is resized
			If _layoutReferenceDistance <> CalcReferenceDistance() Then
				InitializeLayout()
			Else
				AdjustLayout()
			End If
		End Sub

		Private Sub InitializeLayout()
			_layoutLastFormSize = Me.Size

			'Layout properties which are directly based on another layout property.  These are all properties with a 1-1 ration
			If _layoutDifferenceList Is Nothing Then
				_layoutDifferenceList = New List(Of RelativeLayoutData)

				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Height, _longTextFieldsTwoListBox, LayoutRelativePropertyTypeForDifference.Height))
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Width, _longTextFieldsTwoListBox, LayoutRelativePropertyTypeForDifference.Width))

				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Width, _okButton, LayoutRelativePropertyTypeForDifference.Left))
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Width, _cancelButton, LayoutRelativePropertyTypeForDifference.Left))
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Height, _okButton, LayoutRelativePropertyTypeForDifference.Top))
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutBasePropertyTypeForDifference.Height, _cancelButton, LayoutRelativePropertyTypeForDifference.Top))
			End If

			_layoutDifferenceList.ForEach(Sub(x)
											  x.InitializeDifference()
										  End Sub)

			_layoutReferenceDistance = CalcReferenceDistance()

			AdjustColumnLabel()
		End Sub

		Public Sub AdjustLayout()
			If Not _layoutLastFormSize.Equals(Me.Size) Then
				For Each x As RelativeLayoutData In _layoutDifferenceList
					x.AdjustRelativeControlBasedOnDifference()
				Next

				_layoutLastFormSize = Me.Size

				AdjustColumnLabel()
			End If
		End Sub

		Private Sub AdjustColumnLabel()
			'Adjust the location of the label to be aligned with the left side of the Right ListBox

			'Get the absolute position of the Right ListBox of the TwoListBox in screen coordinates
			Dim absoluteListBoxLoc As Point = _longTextFieldsTwoListBox.RightSearchableList.PointToScreen(New Point(0, 0))
			'Convert to a location relative to its parent (the form)
			Dim relativeListBoxLoc As Point = Me._selectedLongTextFieldsLabel.Parent.PointToClient(absoluteListBoxLoc)
			'Adjust the location of the label
			Me._selectedLongTextFieldsLabel.Left = relativeListBoxLoc.X
		End Sub
#End Region
	End Class
End Namespace