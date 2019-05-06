Imports System.Collections.Generic
Imports System.Linq
Imports NUnit.Framework
Imports Relativity.Import.Export.TestFramework

Namespace Relativity.Desktop.Client.Legacy.NUnit

	<TestFixture>
	Public Class TextFieldPrecedencePicker

		Private _queryFieldFactory As New QueryFieldFactory()

		<Test>
		Public Sub SetAvailableSelectedField_SetAllFieldsDoesNotRemoveIt()
			Dim extractedTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(1)

			Dim picker As New Relativity.Desktop.Client.TextFieldPrecedencePicker()
			picker.SelectedFields = New List(Of kCura.WinEDDS.ViewFieldInfo)({extractedTextField})

			picker.AllAvailableLongTextFields = _queryFieldFactory.GetAllDocumentFields.ToList
			Assert.AreEqual(1, picker.SelectedFields.Count)
			Assert.AreEqual(extractedTextField, picker.SelectedFields(0))
		End Sub

		<Test>
		Public Sub SetUnavailableSelectedField_SetAllFieldsRemovesIt()
			Dim longTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetGenericLongTextField

			Dim picker As New Relativity.Desktop.Client.TextFieldPrecedencePicker()
			picker.SelectedFields = New List(Of kCura.WinEDDS.ViewFieldInfo)({longTextField})

			picker.AllAvailableLongTextFields = _queryFieldFactory.GetAllDocumentFields.ToList
			Assert.AreEqual(0, picker.SelectedFields.Count)
		End Sub

		<Test>
		Public Sub SetOneUnavailableOneAvailableSelectedField_SetAllFieldsRemovesOneKeepsOne()
			Dim longTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetGenericLongTextField
			Dim extractedTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(1)

			Using picker As New Relativity.Desktop.Client.TextFieldPrecedencePicker()
				picker.SelectedFields = New List(Of kCura.WinEDDS.ViewFieldInfo)({longTextField, extractedTextField})
				picker.AllAvailableLongTextFields = _queryFieldFactory.GetAllDocumentFields.ToList
				Assert.AreEqual(1, picker.SelectedFields.Count)
				Assert.AreEqual(extractedTextField, picker.SelectedFields(0))
			End Using
		End Sub

		<Test>
		Public Sub SetDefaultField_ShouldHaveOneSelected()
			Dim extractedTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(1)
			Using picker As New Relativity.Desktop.Client.TextFieldPrecedencePicker()
				picker.AllAvailableLongTextFields = _queryFieldFactory.GetAllDocumentFields.ToList
				picker.SelectDefaultTextField(extractedTextField)
				Assert.AreEqual(1, picker.SelectedFields.Count)
				Assert.AreEqual(extractedTextField, picker.SelectedFields(0))
			End Using
		End Sub

		<Test>
		Public Sub SetDefaultField_FieldIsNull_ShouldHaveNoneSelected()
			Using picker As New Relativity.Desktop.Client.TextFieldPrecedencePicker()
				picker.AllAvailableLongTextFields = _queryFieldFactory.GetAllDocumentFields.ToList
				picker.SelectDefaultTextField(Nothing)
				Assert.AreEqual(0, picker.SelectedFields.Count)
			End Using
		End Sub

		<Test>
		Public Sub SetDefaultField_NoAvailableFields_ShouldHaveNoneSelected()
			Dim extractedTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(1)
			Using picker As New Relativity.Desktop.Client.TextFieldPrecedencePicker()
				picker.AllAvailableLongTextFields = New List(Of kCura.WinEDDS.ViewFieldInfo)()
				picker.SelectDefaultTextField(extractedTextField)
				Assert.AreEqual(0, picker.SelectedFields.Count)
			End Using
		End Sub

		<Test>
		Public Sub StartWithOneSelected_LoadTwoSelectedFromKwx()
			Dim longTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetGenericLongTextField
			Dim extractedTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(1)
			Dim longTextFieldRenamed As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetRenamedGenericLongTextField

			Using picker As New Relativity.Desktop.Client.TextFieldPrecedencePicker()
				picker.AllAvailableLongTextFields = New List(Of kCura.WinEDDS.ViewFieldInfo)({longTextField, longTextFieldRenamed, extractedTextField})
				picker.SelectedFields = New List(Of kCura.WinEDDS.ViewFieldInfo)({extractedTextField})
				picker.LoadNewSelectedFields(New List(Of kCura.WinEDDS.ViewFieldInfo)({longTextField, longTextFieldRenamed}))
				Assert.AreEqual(2, picker.SelectedFields.Count)
			End Using
		End Sub

		<Test>
		Public Sub StartWithNoneSelected_LoadTwoSelectedFromKwx()
			Dim longTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetGenericLongTextField
			Dim extractedTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(1)
			Dim longTextFieldRenamed As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetRenamedGenericLongTextField

			Using picker As New Relativity.Desktop.Client.TextFieldPrecedencePicker()
				picker.AllAvailableLongTextFields = New List(Of kCura.WinEDDS.ViewFieldInfo)({longTextField, longTextFieldRenamed, extractedTextField})
				picker.SelectedFields = New List(Of kCura.WinEDDS.ViewFieldInfo)()
				picker.LoadNewSelectedFields(New List(Of kCura.WinEDDS.ViewFieldInfo)({longTextField, longTextFieldRenamed}))
				Assert.AreEqual(2, picker.SelectedFields.Count)
			End Using
		End Sub

		<Test>
		Public Shared Sub StartWithNoneSelected_LoadZeroSelectedFromKwx()

			Using picker As New Relativity.Desktop.Client.TextFieldPrecedencePicker()
				picker.AllAvailableLongTextFields = New List(Of kCura.WinEDDS.ViewFieldInfo)()
				picker.SelectedFields = New List(Of kCura.WinEDDS.ViewFieldInfo)()
				picker.LoadNewSelectedFields(New List(Of kCura.WinEDDS.ViewFieldInfo)())
				Assert.AreEqual(0, picker.SelectedFields.Count)
			End Using
		End Sub

		<Test>
		Public Sub StartWithOneSelected_LoadZeroSelectedFromKwx()
			Dim longTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetGenericLongTextField
			Dim extractedTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(1)
			Dim longTextFieldRenamed As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetRenamedGenericLongTextField

			Using picker As New Relativity.Desktop.Client.TextFieldPrecedencePicker()
				picker.AllAvailableLongTextFields = New List(Of kCura.WinEDDS.ViewFieldInfo)({longTextField, longTextFieldRenamed, extractedTextField})
				picker.SelectedFields = New List(Of kCura.WinEDDS.ViewFieldInfo)({extractedTextField})
				picker.LoadNewSelectedFields(New List(Of kCura.WinEDDS.ViewFieldInfo)())
				Assert.AreEqual(0, picker.SelectedFields.Count)
			End Using
		End Sub

		<Test>
		Public Sub StartWithTwoSelected_LoadTwoSelectedFromKwx()
			Dim longTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetGenericLongTextField
			Dim extractedTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(1)
			Dim longTextFieldRenamed As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetRenamedGenericLongTextField

			Using picker As New Relativity.Desktop.Client.TextFieldPrecedencePicker()
				picker.AllAvailableLongTextFields = New List(Of kCura.WinEDDS.ViewFieldInfo)({longTextField, longTextFieldRenamed, extractedTextField})
				picker.SelectedFields = New List(Of kCura.WinEDDS.ViewFieldInfo)({longTextField, longTextFieldRenamed})
				picker.LoadNewSelectedFields(New List(Of kCura.WinEDDS.ViewFieldInfo)({longTextField, longTextFieldRenamed}))
				Assert.AreEqual(2, picker.SelectedFields.Count)
			End Using
		End Sub

		<Test>
		Public Sub StartWithNoneSelected_LoadTwoSelectedFromKwx_OneNotAvailable()
			Dim longTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetGenericLongTextField
			Dim extractedTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(1)
			Dim longTextFieldRenamed As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetRenamedGenericLongTextField
			Dim boolField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(2)

			Using picker As New Relativity.Desktop.Client.TextFieldPrecedencePicker()
				picker.AllAvailableLongTextFields = New List(Of kCura.WinEDDS.ViewFieldInfo)({longTextField, longTextFieldRenamed, extractedTextField})
				picker.SelectedFields = New List(Of kCura.WinEDDS.ViewFieldInfo)()
				picker.LoadNewSelectedFields(New List(Of kCura.WinEDDS.ViewFieldInfo)({longTextField, boolField}))
				Assert.AreEqual(1, picker.SelectedFields.Count)
			End Using
		End Sub

		<Test>
		Public Sub StartWithOneSelected_LoadOneFromOldFieldMap()
			Dim longTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetGenericLongTextField
			Dim extractedTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(1)
			Dim oldLongTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetSameNameDifferentIdGenericLongTextField

			Using picker As New Relativity.Desktop.Client.TextFieldPrecedencePicker()
				picker.AllAvailableLongTextFields = New List(Of kCura.WinEDDS.ViewFieldInfo)({longTextField, extractedTextField})
				picker.SelectedFields = New List(Of kCura.WinEDDS.ViewFieldInfo)({extractedTextField})
				picker.LoadNewSelectedFields(New List(Of kCura.WinEDDS.ViewFieldInfo)({oldLongTextField}))
				Assert.AreEqual(1, picker.SelectedFields.Count)
				Assert.AreEqual(longTextField, picker.SelectedFields(0))
			End Using
		End Sub

		<Test>
		Public Sub LoadTwoSelectedFromKwx_TwoLineToolTip()
			Dim longTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetGenericLongTextField
			Dim extractedTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(1)

			Using picker As New Relativity.Desktop.Client.TextFieldPrecedencePicker()
				picker.AllAvailableLongTextFields = New List(Of kCura.WinEDDS.ViewFieldInfo)({longTextField, extractedTextField})
				picker.SelectedFields = New List(Of kCura.WinEDDS.ViewFieldInfo)()
				picker.LoadNewSelectedFields(New List(Of kCura.WinEDDS.ViewFieldInfo)({longTextField, extractedTextField}))
				Assert.AreEqual("Long Text" & vbCrLf & "Extracted Text", picker.ToolTipText)
			End Using
		End Sub

		<Test>
		Public Sub LoadOneSelectedFromKwx_OneLineToolTip()
			Dim longTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetGenericLongTextField

			Using picker As New Relativity.Desktop.Client.TextFieldPrecedencePicker()
				picker.AllAvailableLongTextFields = New List(Of kCura.WinEDDS.ViewFieldInfo)({longTextField})
				picker.SelectedFields = New List(Of kCura.WinEDDS.ViewFieldInfo)()
				picker.LoadNewSelectedFields(New List(Of kCura.WinEDDS.ViewFieldInfo)({longTextField}))
				Assert.AreEqual("Long Text", picker.ToolTipText)
			End Using
		End Sub

		<Test>
		Public Sub LoadNoneSelectedFromKwx_BlankToolTip()
			Dim longTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetGenericLongTextField

			Using picker As New Relativity.Desktop.Client.TextFieldPrecedencePicker()
				picker.AllAvailableLongTextFields = New List(Of kCura.WinEDDS.ViewFieldInfo)({longTextField})
				picker.SelectedFields = New List(Of kCura.WinEDDS.ViewFieldInfo)()
				picker.LoadNewSelectedFields(New List(Of kCura.WinEDDS.ViewFieldInfo)({}))
				Assert.AreEqual(String.Empty, picker.ToolTipText)
			End Using
		End Sub

		<Test>
		Public Sub LoadTwoSelectedFromKwx_LabelWithEllipsis()
			Dim longTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetGenericLongTextField
			Dim extractedTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(1)

			Using picker As New Relativity.Desktop.Client.TextFieldPrecedencePicker()
				picker.AllAvailableLongTextFields = New List(Of kCura.WinEDDS.ViewFieldInfo)({longTextField, extractedTextField})
				picker.SelectedFields = New List(Of kCura.WinEDDS.ViewFieldInfo)()
				picker.LoadNewSelectedFields(New List(Of kCura.WinEDDS.ViewFieldInfo)({longTextField, extractedTextField}))
				Assert.AreEqual("Long Text (+)", picker.LabelText)
			End Using
		End Sub

		<Test>
		Public Sub LoadOneSelectedFromKwx_LabelWithNoEllipsis()
			Dim longTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetGenericLongTextField

			Using picker As New Relativity.Desktop.Client.TextFieldPrecedencePicker()
				picker.AllAvailableLongTextFields = New List(Of kCura.WinEDDS.ViewFieldInfo)({longTextField})
				picker.SelectedFields = New List(Of kCura.WinEDDS.ViewFieldInfo)()
				picker.LoadNewSelectedFields(New List(Of kCura.WinEDDS.ViewFieldInfo)({longTextField}))
				Assert.AreEqual("Long Text", picker.LabelText)
			End Using
		End Sub

		<Test>
		Public Sub LoadNoneSelectedFromKwx_BlankLabel()
			Dim longTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetGenericLongTextField

			Using picker As New Relativity.Desktop.Client.TextFieldPrecedencePicker()
				picker.AllAvailableLongTextFields = New List(Of kCura.WinEDDS.ViewFieldInfo)({longTextField})
				picker.SelectedFields = New List(Of kCura.WinEDDS.ViewFieldInfo)()
				picker.LoadNewSelectedFields(New List(Of kCura.WinEDDS.ViewFieldInfo)({}))
				Assert.AreEqual(String.Empty, picker.LabelText)
			End Using
		End Sub
	End Class
End Namespace