Imports System.Collections.Generic
Imports System.Linq
Imports kCura.Windows.Forms

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

				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutPropertyType.Height, _longTextFieldsTwoListBox, LayoutPropertyType.Height))
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutPropertyType.Width, _longTextFieldsTwoListBox, LayoutPropertyType.Width))

				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutPropertyType.Width, _okButton, LayoutPropertyType.Left))
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutPropertyType.Width, _cancelButton, LayoutPropertyType.Left))
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutPropertyType.Height, _okButton, LayoutPropertyType.Top))
				_layoutDifferenceList.Add(New RelativeLayoutData(Me, LayoutPropertyType.Height, _cancelButton, LayoutPropertyType.Top))
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
			Dim absoluteListBoxLoc As Point = _longTextFieldsTwoListBox.RightListBox.PointToScreen(New Point(0, 0))
			'Convert to a location relative to its parent (the form)
			Dim relativeListBoxLoc As Point = Me._selectedLongTextFieldsLabel.Parent.PointToClient(absoluteListBoxLoc)
			'Adjust the location of the label
			Me._selectedLongTextFieldsLabel.Left = relativeListBoxLoc.X
		End Sub
#End Region
	End Class
End Namespace