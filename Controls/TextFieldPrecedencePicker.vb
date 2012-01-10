Imports System.Collections.Generic
Imports System.Text

Namespace kCura.EDDS.WinForm
	Public Class TextFieldPrecedencePicker
		Private WithEvents _textFieldPrecedenceForm As kCura.EDDS.WinForm.TextPrecedenceForm

		Public Property AllAvailableLongTextFields() As List(Of ViewFieldInfo)
		Private _toolTip As New ToolTip()

		Public ReadOnly Property PotentialTextFieldsList() As List(Of ViewFieldInfo)
			Get
				Dim retVal As New List(Of ViewFieldInfo)()
				For i As Int32 = 0 To PotentialTextFieldsDropDown.Items.Count - 1
					retVal.Add(DirectCast(PotentialTextFieldsDropDown.Items(i), ViewFieldInfo))
				Next
				Return retVal
			End Get
		End Property

		Private Sub _pickTextFieldPrecedenceButton_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles _pickTextFieldPrecedenceButton.Click
			_textFieldPrecedenceForm = New kCura.EDDS.WinForm.TextPrecedenceForm(AllAvailableLongTextFields, PotentialTextFieldsDropDown)
			_textFieldPrecedenceForm.ShowDialog()
		End Sub

		Private Sub _textFieldPrecedenceForm_OkClicked() Handles _textFieldPrecedenceForm.OkClicked
			PotentialTextFieldsDropDown.Items.Clear()
			Dim toolTipStringBuilder As New StringBuilder()
			For Each item As Object In _textFieldPrecedenceForm.SelectedTextFields
				PotentialTextFieldsDropDown.Items.Add(item)
				toolTipStringBuilder.AppendLine(CType(item, ViewFieldInfo).DisplayName)
			Next
			If PotentialTextFieldsDropDown.Items.Count > 0 Then
				PotentialTextFieldsDropDown.SelectedIndex = 0
			End If

			SetToolTip(toolTipStringBuilder.ToString())
		End Sub

		Private Sub TextFieldPrecedencePicker_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
			SetToolTip("Comfing from load: has to be first text field")
		End Sub

		Private Sub SetToolTip(ByVal text As String)
			_toolTip.AutoPopDelay = 5000
			_toolTip.InitialDelay = 1000
			_toolTip.ReshowDelay = 500
			_toolTip.ShowAlways = True
			_toolTip.SetToolTip(PotentialTextFieldsDropDown, text)
			End Sub
	End Class
End Namespace
