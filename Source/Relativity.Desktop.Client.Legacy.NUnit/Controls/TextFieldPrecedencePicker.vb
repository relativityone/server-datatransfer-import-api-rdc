Imports NUnit.Framework

Namespace kCura.EDDS.WinForm.Tests
	<TestFixture()> Public Class TextFieldPrecedencePicker

		Private _queryFieldFactory As New kCura.WinEDDS.NUnit.TestObjectFactories.QueryFieldFactory()

#Region " setting available fields "

		<Test()> Public Sub SetAvailableSelectedField_SetAllFieldsDoesNotRemoveIt()
			Dim extractedTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(1)

			Dim picker As New kCura.EDDS.WinForm.TextFieldPrecedencePicker()
			picker.SelectedFields = New List(Of WinEDDS.ViewFieldInfo)({extractedTextField})

			picker.AllAvailableLongTextFields = _queryFieldFactory.GetAllDocumentFields.ToList
			Assert.AreEqual(1, picker.SelectedFields.Count)
			Assert.AreEqual(extractedTextField, picker.SelectedFields(0))
		End Sub

		<Test()> Public Sub SetUnavailableSelectedField_SetAllFieldsRemovesIt()
			Dim longTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetGenericLongTextField

			Dim picker As New kCura.EDDS.WinForm.TextFieldPrecedencePicker()
			picker.SelectedFields = New List(Of WinEDDS.ViewFieldInfo)({longTextField})

			picker.AllAvailableLongTextFields = _queryFieldFactory.GetAllDocumentFields.ToList
			Assert.AreEqual(0, picker.SelectedFields.Count)
		End Sub

		<Test()> Public Sub SetOneUnavailableOneAvailableSelectedField_SetAllFieldsRemovesOneKeepsOne()
			Dim longTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetGenericLongTextField
			Dim extractedTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(1)

			Dim picker As New kCura.EDDS.WinForm.TextFieldPrecedencePicker()
			picker.SelectedFields = New List(Of WinEDDS.ViewFieldInfo)({longTextField, extractedTextField})

			picker.AllAvailableLongTextFields = _queryFieldFactory.GetAllDocumentFields.ToList
			Assert.AreEqual(1, picker.SelectedFields.Count)
			Assert.AreEqual(extractedTextField, picker.SelectedFields(0))
		End Sub

#End Region

#Region " setting default 'first' field "

		<Test()> Public Sub SetDefaultField_ShouldHaveOneSelected()
			Dim extractedTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(1)
			Dim picker As New kCura.EDDS.WinForm.TextFieldPrecedencePicker()
			picker.AllAvailableLongTextFields = _queryFieldFactory.GetAllDocumentFields.ToList

			picker.SelectDefaultTextField(extractedTextField)
			Assert.AreEqual(1, picker.SelectedFields.Count)
			Assert.AreEqual(extractedTextField, picker.SelectedFields(0))
		End Sub

		<Test()> Public Sub SetDefaultField_FieldIsNull_ShouldHaveNoneSelected()
			Dim picker As New kCura.EDDS.WinForm.TextFieldPrecedencePicker()
			picker.AllAvailableLongTextFields = _queryFieldFactory.GetAllDocumentFields.ToList

			picker.SelectDefaultTextField(Nothing)
			Assert.AreEqual(0, picker.SelectedFields.Count)
		End Sub

		<Test()> Public Sub SetDefaultField_NoAvailableFields_ShouldHaveNoneSelected()
			Dim extractedTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(1)
			Dim picker As New kCura.EDDS.WinForm.TextFieldPrecedencePicker()
			picker.AllAvailableLongTextFields = New List(Of WinEDDS.ViewFieldInfo)()

			picker.SelectDefaultTextField(extractedTextField)
			Assert.AreEqual(0, picker.SelectedFields.Count)
		End Sub

#End Region
		
#Region " loading fields from kwx "

		<Test()> Public Sub StartWithOneSelected_LoadTwoSelectedFromKwx()
			Dim longTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetGenericLongTextField
			Dim extractedTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(1)
			Dim longTextFieldRenamed As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetRenamedGenericLongTextField

			Dim picker As New kCura.EDDS.WinForm.TextFieldPrecedencePicker()
			picker.AllAvailableLongTextFields = New List(Of WinEDDS.ViewFieldInfo)({longTextField, longTextFieldRenamed, extractedTextField})
			picker.SelectedFields = New List(Of WinEDDS.ViewFieldInfo)({extractedTextField})

			picker.LoadNewSelectedFields(New List(Of WinEDDS.ViewFieldInfo)({longTextField, longTextFieldRenamed}))
			Assert.AreEqual(2, picker.SelectedFields.Count)
		End Sub

		<Test()> Public Sub StartWithNoneSelected_LoadTwoSelectedFromKwx()
			Dim longTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetGenericLongTextField
			Dim extractedTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(1)
			Dim longTextFieldRenamed As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetRenamedGenericLongTextField

			Dim picker As New kCura.EDDS.WinForm.TextFieldPrecedencePicker()
			picker.AllAvailableLongTextFields = New List(Of WinEDDS.ViewFieldInfo)({longTextField, longTextFieldRenamed, extractedTextField})
			picker.SelectedFields = New List(Of WinEDDS.ViewFieldInfo)()

			picker.LoadNewSelectedFields(New List(Of WinEDDS.ViewFieldInfo)({longTextField, longTextFieldRenamed}))
			Assert.AreEqual(2, picker.SelectedFields.Count)
		End Sub

		<Test()> Public Sub StartWithNoneSelected_LoadZeroSelectedFromKwx()

			Dim picker As New kCura.EDDS.WinForm.TextFieldPrecedencePicker()
			picker.AllAvailableLongTextFields = New List(Of WinEDDS.ViewFieldInfo)()
			picker.SelectedFields = New List(Of WinEDDS.ViewFieldInfo)()

			picker.LoadNewSelectedFields(New List(Of WinEDDS.ViewFieldInfo)())
			Assert.AreEqual(0, picker.SelectedFields.Count)
		End Sub

		<Test()> Public Sub StartWithOneSelected_LoadZeroSelectedFromKwx()
			Dim longTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetGenericLongTextField
			Dim extractedTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(1)
			Dim longTextFieldRenamed As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetRenamedGenericLongTextField

			Dim picker As New kCura.EDDS.WinForm.TextFieldPrecedencePicker()
			picker.AllAvailableLongTextFields = New List(Of WinEDDS.ViewFieldInfo)({longTextField, longTextFieldRenamed, extractedTextField})
			picker.SelectedFields = New List(Of WinEDDS.ViewFieldInfo)({extractedTextField})

			picker.LoadNewSelectedFields(New List(Of WinEDDS.ViewFieldInfo)())
			Assert.AreEqual(0, picker.SelectedFields.Count)
		End Sub

		<Test()> Public Sub StartWithTwoSelected_LoadTwoSelectedFromKwx()
			Dim longTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetGenericLongTextField
			Dim extractedTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(1)
			Dim longTextFieldRenamed As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetRenamedGenericLongTextField

			Dim picker As New kCura.EDDS.WinForm.TextFieldPrecedencePicker()
			picker.AllAvailableLongTextFields = New List(Of WinEDDS.ViewFieldInfo)({longTextField, longTextFieldRenamed, extractedTextField})
			picker.SelectedFields = New List(Of WinEDDS.ViewFieldInfo)({longTextField, longTextFieldRenamed})

			picker.LoadNewSelectedFields(New List(Of WinEDDS.ViewFieldInfo)({longTextField, longTextFieldRenamed}))
			Assert.AreEqual(2, picker.SelectedFields.Count)
		End Sub

		<Test()> Public Sub StartWithNoneSelected_LoadTwoSelectedFromKwx_OneNotAvailable()
			Dim longTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetGenericLongTextField
			Dim extractedTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(1)
			Dim longTextFieldRenamed As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetRenamedGenericLongTextField
			Dim boolField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(2)

			Dim picker As New kCura.EDDS.WinForm.TextFieldPrecedencePicker()
			picker.AllAvailableLongTextFields = New List(Of WinEDDS.ViewFieldInfo)({longTextField, longTextFieldRenamed, extractedTextField})
			picker.SelectedFields = New List(Of WinEDDS.ViewFieldInfo)()

			picker.LoadNewSelectedFields(New List(Of WinEDDS.ViewFieldInfo)({longTextField, boolField}))
			Assert.AreEqual(1, picker.SelectedFields.Count)
		End Sub

		<Test()> Public Sub StartWithOneSelected_LoadOneFromOldFieldMap()
			Dim longTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetGenericLongTextField
			Dim extractedTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(1)
			Dim oldLongTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetSameNameDifferentIdGenericLongTextField

			Dim picker As New kCura.EDDS.WinForm.TextFieldPrecedencePicker()
			picker.AllAvailableLongTextFields = New List(Of WinEDDS.ViewFieldInfo)({longTextField, extractedTextField})
			picker.SelectedFields = New List(Of WinEDDS.ViewFieldInfo)({extractedTextField})

			picker.LoadNewSelectedFields(New List(Of WinEDDS.ViewFieldInfo)({oldLongTextField}))
			Assert.AreEqual(1, picker.SelectedFields.Count)
			Assert.AreEqual(longTextField, picker.SelectedFields(0))
		End Sub

#End Region

#Region " tooltip "

		<Test()> Public Sub LoadTwoSelectedFromKwx_TwoLineToolTip()
			Dim longTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetGenericLongTextField
			Dim extractedTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(1)

			Dim picker As New kCura.EDDS.WinForm.TextFieldPrecedencePicker()
			picker.AllAvailableLongTextFields = New List(Of WinEDDS.ViewFieldInfo)({longTextField, extractedTextField})
			picker.SelectedFields = New List(Of WinEDDS.ViewFieldInfo)()
			picker.LoadNewSelectedFields(New List(Of WinEDDS.ViewFieldInfo)({longTextField, extractedTextField}))

			Assert.AreEqual("Long Text" & vbCrLf & "Extracted Text", picker.ToolTipText)
		End Sub

		<Test()> Public Sub LoadOneSelectedFromKwx_OneLineToolTip()
			Dim longTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetGenericLongTextField

			Dim picker As New kCura.EDDS.WinForm.TextFieldPrecedencePicker()
			picker.AllAvailableLongTextFields = New List(Of WinEDDS.ViewFieldInfo)({longTextField})
			picker.SelectedFields = New List(Of WinEDDS.ViewFieldInfo)()
			picker.LoadNewSelectedFields(New List(Of WinEDDS.ViewFieldInfo)({longTextField}))

			Assert.AreEqual("Long Text", picker.ToolTipText)
		End Sub

		<Test()> Public Sub LoadNoneSelectedFromKwx_BlankToolTip()
			Dim longTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetGenericLongTextField

			Dim picker As New kCura.EDDS.WinForm.TextFieldPrecedencePicker()
			picker.AllAvailableLongTextFields = New List(Of WinEDDS.ViewFieldInfo)({longTextField})
			picker.SelectedFields = New List(Of WinEDDS.ViewFieldInfo)()
			picker.LoadNewSelectedFields(New List(Of WinEDDS.ViewFieldInfo)({}))

			Assert.AreEqual(String.Empty, picker.ToolTipText)
		End Sub

#End Region

#Region " label text "

		<Test()> Public Sub LoadTwoSelectedFromKwx_LabelWithEllipsis()
			Dim longTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetGenericLongTextField
			Dim extractedTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(1)

			Dim picker As New kCura.EDDS.WinForm.TextFieldPrecedencePicker()
			picker.AllAvailableLongTextFields = New List(Of WinEDDS.ViewFieldInfo)({longTextField, extractedTextField})
			picker.SelectedFields = New List(Of WinEDDS.ViewFieldInfo)()
			picker.LoadNewSelectedFields(New List(Of WinEDDS.ViewFieldInfo)({longTextField, extractedTextField}))

			Assert.AreEqual("Long Text (+)", picker.LabelText)
		End Sub

		<Test()> Public Sub LoadOneSelectedFromKwx_LabelWithNoEllipsis()
			Dim longTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetGenericLongTextField

			Dim picker As New kCura.EDDS.WinForm.TextFieldPrecedencePicker()
			picker.AllAvailableLongTextFields = New List(Of WinEDDS.ViewFieldInfo)({longTextField})
			picker.SelectedFields = New List(Of WinEDDS.ViewFieldInfo)()
			picker.LoadNewSelectedFields(New List(Of WinEDDS.ViewFieldInfo)({longTextField}))

			Assert.AreEqual("Long Text", picker.LabelText)
		End Sub

		<Test()> Public Sub LoadNoneSelectedFromKwx_BlankLabel()
			Dim longTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetGenericLongTextField

			Dim picker As New kCura.EDDS.WinForm.TextFieldPrecedencePicker()
			picker.AllAvailableLongTextFields = New List(Of WinEDDS.ViewFieldInfo)({longTextField})
			picker.SelectedFields = New List(Of WinEDDS.ViewFieldInfo)()
			picker.LoadNewSelectedFields(New List(Of WinEDDS.ViewFieldInfo)({}))

			Assert.AreEqual(String.Empty, picker.LabelText)
		End Sub

#End Region


	End Class
End Namespace