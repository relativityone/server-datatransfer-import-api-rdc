Imports System.Windows.Forms
Imports NUnit.Framework
Imports kCura.WinEDDS
'Imports kCura.EDDS.WinForm.Forms

Namespace kCura.EDDS.WinForm.Tests
	<TestFixture()>
	Public Class ExportForm
        Private _form As Forms.ExportForm
		Private _queryFieldFactory As New kCura.WinEDDS.NUnit.TestObjectFactories.QueryFieldFactory()
		Private _filtersDataTable As New DataTable()

		<SetUp()> Public Sub SetUp()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.Production)
		End Sub

		Public Sub SetUpForTypeOfExport(ByVal type As kCura.WinEDDS.ExportFile.ExportType)
			Dim avfLookUp As New System.Collections.Specialized.HybridDictionary()
			avfLookUp.Add(123, New System.Collections.ArrayList(New Int32() {123}))
			avfLookUp.Add(245, New System.Collections.ArrayList(New Int32() {245}))
			_filtersDataTable = New DataTable
			_filtersDataTable.Columns.Add("Name", GetType(String))
			_filtersDataTable.Columns.Add("ArtifactID", GetType(Int32))
			_filtersDataTable.Rows.Add({"123", 123})
			_filtersDataTable.Rows.Add({"245", 245})

            _form = New Forms.ExportForm
			_form.ExportFile = New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.AllExportableFields = New kCura.WinEDDS.ViewFieldInfo() {},
			.DataTable = _filtersDataTable,
			.ArtifactAvfLookup = avfLookUp,
			.TypeOfExport = type}
			_form.HandleLoad(Nothing, Nothing, 100, 100)
		End Sub


		Public Sub SetupOneFieldSelected(ByVal type As kCura.WinEDDS.ExportFile.ExportType)

			Dim avfLookUp As New System.Collections.Specialized.HybridDictionary()
			avfLookUp.Add(123, New System.Collections.ArrayList(New Int32() {123}))
			_filtersDataTable = New DataTable
			_filtersDataTable.Columns.Add("Name", GetType(String))
			_filtersDataTable.Columns.Add("ArtifactID", GetType(Int32))
			_filtersDataTable.Rows.Add({"123", 123})
			_filtersDataTable.Rows.Add({"245", 245})

            _form = New Forms.ExportForm
			_form.ExportFile = New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			.DataTable = _filtersDataTable,
			.ArtifactAvfLookup = avfLookUp,
			.SelectedViewFields = {_queryFieldFactory.GetAllDocumentFields(0)},
			.TypeOfExport = type}
			_form.HandleLoad(Nothing, Nothing, 100, 100)

		End Sub

		Public Sub ValidTextFieldSetup()

			Dim avfLookUp As New System.Collections.Specialized.HybridDictionary()
			Dim textField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(1)
			avfLookUp.Add(123, New System.Collections.ArrayList(New Int32() {123, textField.AvfId}))
			_filtersDataTable = New DataTable
			_filtersDataTable.Columns.Add("Name", GetType(Int32))
			_filtersDataTable.Columns.Add("ArtifactID", GetType(Int32))
			_filtersDataTable.Rows.Add({"123", 123})
			_filtersDataTable.Rows.Add({"245", 245})

            _form = New Forms.ExportForm
			_form.ExportFile = New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			.DataTable = _filtersDataTable,
			.ArtifactAvfLookup = avfLookUp}
			_form.HandleLoad(Nothing, Nothing, 100, 100)

		End Sub

#Region "Mock Classes"

		Public Class MockApplication
			Inherits kCura.EDDS.WinForm.Application

			Public Overrides Function GetProductionPrecendenceList(ByVal caseInfo As Relativity.CaseInfo) As System.Data.DataTable
				Dim retval As New System.Data.DataTable
				retval.Columns.Add("Display")
				retval.Columns.Add("Value")

				retval.Rows.Add(New String() {"Name1", "1012345"})
				retval.Rows.Add(New String() {"Name2", "1234567"})
				retval.Rows.Add(New String() {"Name3", "1000001"})
				Return retval
			End Function
		End Class

#End Region


		'***********Data Source Tab**********************
#Region "Productions Section"

		<Test()> Public Sub LoadExportFile_Production_NotSelected()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.Production}
			_form.LoadExportFile(ef)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Sub

		<Test()> Public Sub LoadExportFile_SavedSearch_Into_ProductionExport_DefaultProduction()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch,
			 .ArtifactID = 45646}
			_form.LoadExportFile(ef)
			Assert.AreEqual(123, _form._filters.SelectedValue)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Sub

		<Test()> Public Sub LoadExportFile_Folder_Into_ProductionExport_DefaultProduction()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ParentSearch,
			 .ViewID = 4654897}
			_form.LoadExportFile(ef)
			Assert.AreEqual(123, _form._filters.SelectedValue)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Sub

		<Test()> Public Sub LoadExportFile_FolderAndSubfolder_Into_ProductionExport_DefaultProduction()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.AncestorSearch,
			 .ViewID = 4654897}
			_form.LoadExportFile(ef)
			Assert.AreEqual(123, _form._filters.SelectedValue)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Sub

		<Test()> Public Sub LoadExportFile_ProductionSelected_SavedProductionNoLongerExists()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.Production,
			 .ArtifactID = 1234567}
			_form.LoadExportFile(ef)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Sub

		<Test()> Public Sub LoadExportFile_Production_AllExportableFieldsShowUp()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.Production}
			_form.LoadExportFile(ef)
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields.Count, _form._columnSelecter.LeftListBoxItems.Count)
		End Sub

		<Test()> Public Sub LoadExportFile_Production_AllExportableFields_OneFieldSelected()
			Dim selectedField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(3)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {selectedField},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.Production}
			_form.LoadExportFile(ef)
			For Each avf As kCura.WinEDDS.ViewFieldInfo In _form._columnSelecter.RightListBoxItems
				If selectedField.DisplayName.Equals(avf.DisplayName, StringComparison.InvariantCulture) Then

				End If
			Next
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count - 1, _form._columnSelecter.LeftListBoxItems.Count)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField, _form._columnSelecter.RightListBoxItems))
		End Sub

		<Test()> Public Sub LoadExportFile_Production_AllExportableFields_TwoFieldSelected()
			Dim selectedField1 As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(3)
			Dim selectedField2 As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(2)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {selectedField1, selectedField2},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.Production}
			_form.LoadExportFile(ef)
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count - 2, _form._columnSelecter.LeftListBoxItems.Count)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField1, _form._columnSelecter.RightListBoxItems))
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField2, _form._columnSelecter.RightListBoxItems))
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count - 2, _form._columnSelecter.LeftListBoxItems.Count)
		End Sub

		<Test()> Public Sub LoadExportFile_Production_AllExportableFields_OneFieldSelected_OneFieldPrePopulated()
			SetupOneFieldSelected(kCura.WinEDDS.ExportFile.ExportType.Production)
			Dim selectedField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(0)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {selectedField},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.Production}
			_form.LoadExportFile(ef)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField, _form._columnSelecter.RightListBoxItems))
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count - 1, _form._columnSelecter.LeftListBoxItems.Count)
			Assert.AreEqual(1, _form._columnSelecter.RightListBoxItems.Count)
		End Sub

		<Test()> Public Sub LoadExportFile_Production_AllExportableFields_ZeroFieldsSelected_OneFieldPrePopulated()
			SetupOneFieldSelected(kCura.WinEDDS.ExportFile.ExportType.Production)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.Production}
			_form.LoadExportFile(ef)
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count, _form._columnSelecter.LeftListBoxItems.Count)
			Assert.AreEqual(0, _form._columnSelecter.RightListBoxItems.Count)
		End Sub

		<Test()> Public Sub LoadExportFile_Production_AllExportableFields_TwoFieldSelected_OneFieldIsNoLongerAvaialble()
			Dim selectedField1 As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(3)
			Dim selectedField2 As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(2)
			Dim selectedField3 As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(4)
			Dim fields As New List(Of kCura.WinEDDS.ViewFieldInfo)()
			fields.Add(selectedField1)
			fields.Add(selectedField3)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = fields.ToArray(),
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {selectedField1, selectedField2, selectedField3},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.Production}
			_form.LoadExportFile(ef)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField1, _form._columnSelecter.RightListBoxItems))
			Assert.IsFalse(_form._columnSelecter.RightListBoxItems.Contains(selectedField2))
			Assert.IsFalse(_form._columnSelecter.LeftListBoxItems.Contains(selectedField2))
			Assert.AreEqual(0, _form._columnSelecter.LeftListBoxItems.Count)
		End Sub

		<Test()> Public Sub LoadExportFile_Production_StartExportAtDocumentNumber_Is15()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .StartAtDocumentNumber = 15,
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.Production}
			_form.LoadExportFile(ef)
			Assert.AreEqual(16, _form._startExportAtDocumentNumber.Value)
		End Sub

		<Test()> Public Sub LoadExportFile_Production_StartExportAtDocumentNumber_Is1_When_PreLoadedAs5()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .StartAtDocumentNumber = 0,
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.Production}
			Dim numericUpDown As NumericUpDown = New System.Windows.Forms.NumericUpDown()
			numericUpDown.Value = 5D
			_form._startExportAtDocumentNumber = numericUpDown
			_form.LoadExportFile(ef)
			Assert.AreEqual(1, _form._startExportAtDocumentNumber.Value)
		End Sub







#End Region

#Region "Saved Search Section"

		<Test()> Public Sub LoadExportFile_SavedSearch_NotSelected()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch}
			_form.LoadExportFile(ef)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Sub

		<Test()> Public Sub LoadExportFile_Folder_Into_SavedSearchExport_DefaultSavedSearch()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ParentSearch,
			.ViewID = 45646}
			_form.LoadExportFile(ef)
			Assert.AreEqual(123, _form._filters.SelectedValue)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Sub

		<Test()> Public Sub LoadExportFile_Production_Into_SavedSearchExport_DefaultSavedSearch()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.Production,
			.ArtifactID = 4654897}
			_form.LoadExportFile(ef)
			Assert.AreEqual(123, _form._filters.SelectedValue)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Sub

		<Test()> Public Sub LoadExportFile_FolderAndSubfolder_Into_SavedSearchExport_DefaultSavedSearch()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.AncestorSearch,
			.ViewID = 4654897}
			_form.LoadExportFile(ef)
			Assert.AreEqual(123, _form._filters.SelectedValue)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Sub

		<Test()> Public Sub LoadExportFile_SavedSearchSelected_SavedSavedSearchNoLongerExists()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch,
			.ArtifactID = 1234567}
			_form.LoadExportFile(ef)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Sub

		<Test()> Public Sub LoadExportFile_SavedSearch_AllExportableFieldsShowUp()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch}
			_form.LoadExportFile(ef)
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields.Count, _form._columnSelecter.LeftListBoxItems.Count)
		End Sub

		<Test()> Public Sub LoadExportFile_SavedSearch_AllExportableFields_OneFieldSelected()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch)
			Dim selectedField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(3)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {selectedField},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch}
			_form.LoadExportFile(ef)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField, _form._columnSelecter.RightListBoxItems))
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count - 1, _form._columnSelecter.LeftListBoxItems.Count)
		End Sub

		<Test()> Public Sub LoadExportFile_SavedSearch_AllExportableFields_TwoFieldSelected()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch)
			Dim selectedField1 As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(3)
			Dim selectedField2 As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(2)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {selectedField1, selectedField2},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch}
			_form.LoadExportFile(ef)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField1, _form._columnSelecter.RightListBoxItems))
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField2, _form._columnSelecter.RightListBoxItems))
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count - 2, _form._columnSelecter.LeftListBoxItems.Count)
		End Sub

		<Test()> Public Sub LoadExportFile_SavedSearch_AllExportableFields_OneFieldSelected_OneFieldPrePopulated()
			SetupOneFieldSelected(kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch)
			Dim selectedField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(0)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {selectedField},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch}
			_form.LoadExportFile(ef)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField, _form._columnSelecter.RightListBoxItems))
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count - 1, _form._columnSelecter.LeftListBoxItems.Count)
			Assert.AreEqual(1, _form._columnSelecter.RightListBoxItems.Count)
		End Sub

		<Test()> Public Sub LoadExportFile_SavedSearch_AllExportableFields_ZeroFieldsSelected_OneFieldPrePopulated()
			SetupOneFieldSelected(kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch}
			_form.LoadExportFile(ef)
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count, _form._columnSelecter.LeftListBoxItems.Count)
			Assert.AreEqual(0, _form._columnSelecter.RightListBoxItems.Count)
		End Sub

		<Test()> Public Sub LoadExportFile_SavedSearch_AllExportableFields_TwoFieldSelected_OneFieldIsNoLongerAvaialble()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch)
			Dim selectedField1 As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(3)
			Dim selectedField2 As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(2)
			Dim selectedField3 As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(4)
			Dim fields As New List(Of kCura.WinEDDS.ViewFieldInfo)()
			fields.Add(selectedField1)
			fields.Add(selectedField3)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = fields.ToArray(),
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {selectedField1, selectedField2, selectedField3},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch}
			_form.LoadExportFile(ef)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField1, _form._columnSelecter.RightListBoxItems))
			Assert.IsFalse(_form._columnSelecter.RightListBoxItems.Contains(selectedField2))
			Assert.IsFalse(_form._columnSelecter.LeftListBoxItems.Contains(selectedField2))
			Assert.AreEqual(0, _form._columnSelecter.LeftListBoxItems.Count)
		End Sub

		<Test()> Public Sub LoadExportFile_SavedSearch_StartExportAtDocumentNumber_Is15()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.StartAtDocumentNumber = 15,
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch}
			_form.LoadExportFile(ef)
			Assert.AreEqual(16, _form._startExportAtDocumentNumber.Value)
		End Sub

#End Region

#Region "Folder Section"

		<Test()> Public Sub LoadExportFile_Folder_NotSelected()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ParentSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ParentSearch}
			_form.LoadExportFile(ef)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Sub

		<Test()> Public Sub LoadExportFile_SavedSearch_Into_FolderExport_DefaultFolder()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ParentSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch,
			.ArtifactID = 45646}
			_form.LoadExportFile(ef)
			Assert.AreEqual(123, _form._filters.SelectedValue)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Sub

		<Test()> Public Sub LoadExportFile_Production_Into_FolderExport_DefaultFolder()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ParentSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.Production,
			.ArtifactID = 4654897}
			_form.LoadExportFile(ef)
			Assert.AreEqual(123, _form._filters.SelectedValue)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Sub

		<Test()> Public Sub LoadExportFile_FolderAndSubfolder_Into_FolderExport_DefaultFolder()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ParentSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.AncestorSearch,
			.ViewID = 4654897}
			_form.LoadExportFile(ef)
			Assert.AreEqual(123, _form._filters.SelectedValue)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Sub

		<Test()> Public Sub LoadExportFile_FolderSelected_SavedFolderNoLongerExists()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ParentSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ParentSearch,
			.ViewID = 1234567}
			_form.LoadExportFile(ef)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Sub

		<Test()> Public Sub LoadExportFile_Folder_AllExportableFieldsShowUp()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ParentSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ParentSearch}
			_form.LoadExportFile(ef)
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields.Count, _form._columnSelecter.LeftListBoxItems.Count)
		End Sub

		<Test()> Public Sub LoadExportFile_Folder_AllExportableFields_OneFieldSelected()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ParentSearch)
			Dim selectedField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(3)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {selectedField},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ParentSearch}
			_form.LoadExportFile(ef)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField, _form._columnSelecter.RightListBoxItems))
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count - 1, _form._columnSelecter.LeftListBoxItems.Count)
		End Sub

		<Test()> Public Sub LoadExportFile_Folder_AllExportableFields_TwoFieldSelected()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ParentSearch)
			Dim selectedField1 As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(3)
			Dim selectedField2 As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(2)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {selectedField1, selectedField2},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ParentSearch}
			_form.LoadExportFile(ef)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField1, _form._columnSelecter.RightListBoxItems))
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField2, _form._columnSelecter.RightListBoxItems))
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count - 2, _form._columnSelecter.LeftListBoxItems.Count)
		End Sub

		<Test()> Public Sub LoadExportFile_Folder_AllExportableFields_OneFieldSelected_OneFieldPrePopulated()
			SetupOneFieldSelected(kCura.WinEDDS.ExportFile.ExportType.ParentSearch)
			Dim selectedField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(0)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {selectedField},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ParentSearch}
			_form.LoadExportFile(ef)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField, _form._columnSelecter.RightListBoxItems))
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count - 1, _form._columnSelecter.LeftListBoxItems.Count)
			Assert.AreEqual(1, _form._columnSelecter.RightListBoxItems.Count)
		End Sub

		<Test()> Public Sub LoadExportFile_Folder_AllExportableFields_ZeroFieldsSelected_OneFieldPrePopulated()
			SetupOneFieldSelected(kCura.WinEDDS.ExportFile.ExportType.ParentSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ParentSearch}
			_form.LoadExportFile(ef)
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count, _form._columnSelecter.LeftListBoxItems.Count)
			Assert.AreEqual(0, _form._columnSelecter.RightListBoxItems.Count)
		End Sub

		<Test()> Public Sub LoadExportFile_Folder_AllExportableFields_TwoFieldSelected_OneFieldIsNoLongerAvaialble()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ParentSearch)
			Dim selectedField1 As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(3)
			Dim selectedField2 As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(2)
			Dim selectedField3 As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(4)
			Dim fields As New List(Of kCura.WinEDDS.ViewFieldInfo)()
			fields.Add(selectedField1)
			fields.Add(selectedField3)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = fields.ToArray(),
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {selectedField1, selectedField2, selectedField3},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ParentSearch}
			_form.LoadExportFile(ef)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField1, _form._columnSelecter.RightListBoxItems))
			Assert.IsFalse(_form._columnSelecter.RightListBoxItems.Contains(selectedField2))
			Assert.IsFalse(_form._columnSelecter.LeftListBoxItems.Contains(selectedField2))
			Assert.AreEqual(0, _form._columnSelecter.LeftListBoxItems.Count)
		End Sub

		<Test()> Public Sub LoadExportFile_Folder_StartExportAtDocumentNumber_Is15()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ParentSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.StartAtDocumentNumber = 15,
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ParentSearch}
			_form.LoadExportFile(ef)
			Assert.AreEqual(16, _form._startExportAtDocumentNumber.Value)
		End Sub

#End Region

#Region "FolderAndSubFolder Section"

		<Test()> Public Sub LoadExportFile_FolderAndSubFolder_NotSelected()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.AncestorSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.AncestorSearch}
			_form.LoadExportFile(ef)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Sub

		<Test()> Public Sub LoadExportFile_SavedSearch_Into_FolderAndSubFolderExport_DefaultFolderAndSubFolder()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.AncestorSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch,
			.ArtifactID = 45646}
			_form.LoadExportFile(ef)
			Assert.AreEqual(123, _form._filters.SelectedValue)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Sub

		<Test()> Public Sub LoadExportFile_Production_Into_FolderAndSubFolderExport_DefaultFolderAndSubFolder()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.AncestorSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.Production,
			.ArtifactID = 4654897}
			_form.LoadExportFile(ef)
			Assert.AreEqual(123, _form._filters.SelectedValue)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Sub

		<Test()> Public Sub LoadExportFile_Folder_Into_FolderAndSubFolderExport_DefaultFolderAndSubFolder()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.AncestorSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ParentSearch,
			.ViewID = 4654897}
			_form.LoadExportFile(ef)
			Assert.AreEqual(123, _form._filters.SelectedValue)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Sub

		<Test()> Public Sub LoadExportFile_FolderAndSubFolderSelected_SavedFolderAndSubFolderNoLongerExists()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.AncestorSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.AncestorSearch,
			.ViewID = 1234567}
			_form.LoadExportFile(ef)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Sub

		<Test()> Public Sub LoadExportFile_FolderAndSubFolder_AllExportableFieldsShowUp()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.AncestorSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.AncestorSearch}
			_form.LoadExportFile(ef)
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields.Count, _form._columnSelecter.LeftListBoxItems.Count)
		End Sub

		<Test()> Public Sub LoadExportFile_FolderAndSubFolder_AllExportableFields_OneFieldSelected()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.AncestorSearch)
			Dim selectedField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(3)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {selectedField},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.AncestorSearch}
			_form.LoadExportFile(ef)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField, _form._columnSelecter.RightListBoxItems))
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count - 1, _form._columnSelecter.LeftListBoxItems.Count)
		End Sub

		<Test()> Public Sub LoadExportFile_FolderAndSubFolder_AllExportableFields_TwoFieldSelected()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.AncestorSearch)
			Dim selectedField1 As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(3)
			Dim selectedField2 As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(2)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {selectedField1, selectedField2},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.AncestorSearch}
			_form.LoadExportFile(ef)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField1, _form._columnSelecter.RightListBoxItems))
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField2, _form._columnSelecter.RightListBoxItems))
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count - 2, _form._columnSelecter.LeftListBoxItems.Count)
		End Sub

		<Test()> Public Sub LoadExportFile_FolderAndSubFolder_AllExportableFields_OneFieldSelected_OneFieldPrePopulated()
			SetupOneFieldSelected(kCura.WinEDDS.ExportFile.ExportType.AncestorSearch)
			Dim selectedField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(0)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {selectedField},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.AncestorSearch}
			_form.LoadExportFile(ef)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField, _form._columnSelecter.RightListBoxItems))
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count - 1, _form._columnSelecter.LeftListBoxItems.Count)
			Assert.AreEqual(1, _form._columnSelecter.RightListBoxItems.Count)
		End Sub

		<Test()> Public Sub LoadExportFile_FolderAndSubFolder_AllExportableFields_ZeroFieldsSelected_OneFieldPrePopulated()
			SetupOneFieldSelected(kCura.WinEDDS.ExportFile.ExportType.AncestorSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.AncestorSearch}
			_form.LoadExportFile(ef)
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count, _form._columnSelecter.LeftListBoxItems.Count)
			Assert.AreEqual(0, _form._columnSelecter.RightListBoxItems.Count)
		End Sub

		<Test()> Public Sub LoadExportFile_FolderAndSubFolder_AllExportableFields_TwoFieldSelected_OneFieldIsNoLongerAvaialble()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.AncestorSearch)
			Dim selectedField1 As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(3)
			Dim selectedField2 As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(2)
			Dim selectedField3 As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(4)
			Dim fields As New List(Of kCura.WinEDDS.ViewFieldInfo)()
			fields.Add(selectedField1)
			fields.Add(selectedField3)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = fields.ToArray(),
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {selectedField1, selectedField2, selectedField3},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.AncestorSearch}
			_form.LoadExportFile(ef)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField1, _form._columnSelecter.RightListBoxItems))
			Assert.IsFalse(_form._columnSelecter.RightListBoxItems.Contains(selectedField2))
			Assert.IsFalse(_form._columnSelecter.LeftListBoxItems.Contains(selectedField2))
			Assert.AreEqual(0, _form._columnSelecter.LeftListBoxItems.Count)
		End Sub

		<Test()> Public Sub LoadExportFile_FolderAndSubFolder_StartExportAtDocumentNumber_Is15()
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.AncestorSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.StartAtDocumentNumber = 15,
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.AncestorSearch}
			_form.LoadExportFile(ef)
			Assert.AreEqual(16, _form._startExportAtDocumentNumber.Value)
		End Sub

#End Region

#Region "Production Precedence Section"
		<Test()> Public Sub LoadExportFile_ProductionPrecedence_OriginalIsDefault()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document)
			_form.LoadExportFile(ef)
			Assert.AreEqual(1, _form._productionPrecedenceList.Items.Count)
			Assert.AreEqual("-1", DirectCast(_form._productionPrecedenceList.Items.Item(0), kCura.WinEDDS.Pair).Value)
			Assert.AreEqual("Original", DirectCast(_form._productionPrecedenceList.Items.Item(0), kCura.WinEDDS.Pair).Display)
		End Sub

		<Test()> Public Sub LoadExportFile_ProductionPrecedence_SetWithOne()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .ImagePrecedence = New kCura.WinEDDS.Pair() {New kCura.WinEDDS.Pair("1012345", "Name1")},
			 .CaseInfo = New Relativity.CaseInfo()}
			_form.Application = New MockApplication
			_form.LoadExportFile(ef)
			Assert.AreEqual(1, _form._productionPrecedenceList.Items.Count)
			Assert.AreEqual("1012345", DirectCast(_form._productionPrecedenceList.Items.Item(0), kCura.WinEDDS.Pair).Value)
			Assert.AreEqual("Name1", DirectCast(_form._productionPrecedenceList.Items.Item(0), kCura.WinEDDS.Pair).Display)
		End Sub

		<Test()> Public Sub LoadExportFile_ProductionPrecedence_SetWithTwo()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .ImagePrecedence = New kCura.WinEDDS.Pair() {New kCura.WinEDDS.Pair("1012345", "Name1"), New kCura.WinEDDS.Pair("1234567", "Name2")},
			 .CaseInfo = New Relativity.CaseInfo()}
			_form.Application = New MockApplication
			_form.LoadExportFile(ef)
			Assert.AreEqual(2, _form._productionPrecedenceList.Items.Count)
			Assert.AreEqual("1012345", DirectCast(_form._productionPrecedenceList.Items.Item(0), kCura.WinEDDS.Pair).Value)
			Assert.AreEqual("Name1", DirectCast(_form._productionPrecedenceList.Items.Item(0), kCura.WinEDDS.Pair).Display)
			Assert.AreEqual("1234567", DirectCast(_form._productionPrecedenceList.Items.Item(1), kCura.WinEDDS.Pair).Value)
			Assert.AreEqual("Name2", DirectCast(_form._productionPrecedenceList.Items.Item(1), kCura.WinEDDS.Pair).Display)
		End Sub

		<Test()> Public Sub LoadExportFile_ProductionPrecedence_SetWithOne_IDDoesNotMatch_NameMatches()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .ImagePrecedence = New kCura.WinEDDS.Pair() {New kCura.WinEDDS.Pair("14", "Name1")},
			 .CaseInfo = New Relativity.CaseInfo()}
			_form.Application = New MockApplication
			_form.LoadExportFile(ef)
			Assert.AreEqual(1, _form._productionPrecedenceList.Items.Count)
			Assert.AreEqual("1012345", DirectCast(_form._productionPrecedenceList.Items.Item(0), kCura.WinEDDS.Pair).Value)
			Assert.AreEqual("Name1", DirectCast(_form._productionPrecedenceList.Items.Item(0), kCura.WinEDDS.Pair).Display)
		End Sub

		<Test()> Public Sub LoadExportFile_ProductionPrecedence_SetWithTwo_IDDoesNotMatch_NameMatches()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .ImagePrecedence = New kCura.WinEDDS.Pair() {New kCura.WinEDDS.Pair("14", "Name1"), New kCura.WinEDDS.Pair("16", "Name2")},
			 .CaseInfo = New Relativity.CaseInfo()}
			_form.Application = New MockApplication
			_form.LoadExportFile(ef)
			Assert.AreEqual(2, _form._productionPrecedenceList.Items.Count)
			Assert.AreEqual("1012345", DirectCast(_form._productionPrecedenceList.Items.Item(0), kCura.WinEDDS.Pair).Value)
			Assert.AreEqual("Name1", DirectCast(_form._productionPrecedenceList.Items.Item(0), kCura.WinEDDS.Pair).Display)
			Assert.AreEqual("1234567", DirectCast(_form._productionPrecedenceList.Items.Item(1), kCura.WinEDDS.Pair).Value)
			Assert.AreEqual("Name2", DirectCast(_form._productionPrecedenceList.Items.Item(1), kCura.WinEDDS.Pair).Display)
		End Sub

		<Test()> Public Sub LoadExportFile_ProductionPrecedence_SetWithOneInvalid_OnlyOriginal()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .ImagePrecedence = New kCura.WinEDDS.Pair() {New kCura.WinEDDS.Pair("1", "Bogus")},
			 .CaseInfo = New Relativity.CaseInfo()}
			_form.Application = New MockApplication
			_form.LoadExportFile(ef)
			Assert.AreEqual(1, _form._productionPrecedenceList.Items.Count)
			Assert.AreEqual("-1", DirectCast(_form._productionPrecedenceList.Items.Item(0), kCura.WinEDDS.Pair).Value)
			Assert.AreEqual("Original", DirectCast(_form._productionPrecedenceList.Items.Item(0), kCura.WinEDDS.Pair).Display)
		End Sub


#End Region

		'***********Destination Files Tab**********************

#Region "Export Location Section"

		<Test()> Public Sub LoadExportFile_ExportLocation_Set()
			Dim somePath As String = "C:\SOMEPATH"
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.FolderPath = somePath}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._folderPath.Text.Equals(somePath, StringComparison.InvariantCultureIgnoreCase))

		End Sub

		<Test()> Public Sub LoadExportFile_ExportLocation_NotSet()
			Dim somePath As String = ""
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.FolderPath = somePath}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._folderPath.Text.Equals(somePath, StringComparison.InvariantCultureIgnoreCase))
		End Sub

		<Test()> Public Sub LoadExportFile_OverwriteFiles_False()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.Overwrite = False}
			_form.LoadExportFile(ef)
			Assert.IsFalse(_form._overwriteCheckBox.Checked)
		End Sub

		<Test()> Public Sub LoadExportFile_OverwriteFiles_True()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.Overwrite = True}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._overwriteCheckBox.Checked)
		End Sub

#End Region

#Region "Physical File Export Section"

		<Test()> Public Sub LoadExportFile_CopyFilesFromRepository_True()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With {
			 .CopyFilesFromRepository = True}
			}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._copyFilesFromRepository.Checked)
		End Sub

		<Test()> Public Sub LoadExportFile_CopyFilesFromRepository_False()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With {
			 .CopyFilesFromRepository = False}
			}
			_form.LoadExportFile(ef)
			Assert.IsFalse(_form._copyFilesFromRepository.Checked)
		End Sub

#End Region

#Region "Volume Information Section"
		<Test()> Public Sub LoadExportFile_Prefix_IsNotEmpty()
			Dim prefixText As String = "TestPrefix"
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With {
			.VolumePrefix = prefixText}}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._volumePrefix.Text.Equals(prefixText, StringComparison.InvariantCultureIgnoreCase))
		End Sub

		<Test()> Public Sub LoadExportFile_Prefix_IsEmpty()
			Dim prefixText As String = ""
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With {
			.VolumePrefix = prefixText}}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._volumePrefix.Text.Equals(prefixText, StringComparison.InvariantCultureIgnoreCase))
		End Sub


		<Test()> Public Sub LoadExportFile_StartNumber_IsNotEmpty()
			Dim startNumber As Decimal = CDec(10.0)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With {
			.VolumeStartNumber = CInt(startNumber)}}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._volumeStartNumber.Value = startNumber)
		End Sub

		<Test()> Public Sub LoadExportFile_NumberOfDigits_IsNotEmpty()
			Dim numberOfDigits As Int32 = 350
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.VolumeDigitPadding = numberOfDigits}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._volumeDigitPadding.Value = numberOfDigits)
		End Sub

		<Test()> Public Sub LoadExportFile_MaxSize_IsNotEmpty()
			Dim maxSize As Int64 = 1000
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With {
			.VolumeMaxSize = maxSize}}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._volumeMaxSize.Value = maxSize)
		End Sub




#End Region

#Region "Subdirectory Information Section"
		<Test()> Public Sub LoadExportFile_ImagePrefix_IsNotEmpty()
			Dim prefixText As String = "TestPrefix"
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With {
			.SubdirectoryImagePrefix = prefixText}}
			_form.LoadExportFile(ef)
			Assert.AreEqual(prefixText, _form._subdirectoryImagePrefix.Text)
		End Sub

		<Test()> Public Sub LoadExportFile_ImagePrefix_IsEmpty()
			Dim prefixText As String = ""
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With {
			.SubdirectoryImagePrefix = prefixText}}
			_form.LoadExportFile(ef)
			Assert.AreEqual(prefixText, _form._subdirectoryImagePrefix.Text)
		End Sub

		<Test()> Public Sub LoadExportFile_NativePrefix_IsNotEmpty()
			Dim prefixText As String = "TestPrefix"
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With {
			.SubdirectoryNativePrefix = prefixText}}
			_form.LoadExportFile(ef)
			Assert.AreEqual(prefixText, _form._subDirectoryNativePrefix.Text)
		End Sub

		<Test()> Public Sub LoadExportFile_NativePrefix_IsEmpty()
			Dim prefixText As String = ""
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With {
			.SubdirectoryNativePrefix = prefixText}}
			_form.LoadExportFile(ef)
			Assert.AreEqual(prefixText, _form._subDirectoryNativePrefix.Text)
		End Sub

		<Test()> Public Sub LoadExportFile_TextPrefix_IsNotEmpty()
			Dim prefixText As String = "TestPrefix"
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With {
			.SubdirectoryFullTextPrefix = prefixText}}
			_form.LoadExportFile(ef)
			Assert.AreEqual(prefixText, _form._subdirectoryTextPrefix.Text)
		End Sub

		<Test()> Public Sub LoadExportFile_TextPrefix_IsEmpty()
			Dim prefixText As String = ""
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With {
			.SubdirectoryFullTextPrefix = prefixText}}
			_form.LoadExportFile(ef)
			Assert.AreEqual(prefixText, _form._subdirectoryTextPrefix.Text)
		End Sub


		<Test()> Public Sub LoadExportFile_SubdirectoryStartNumber_IsNotEmpty()
			Dim startNumber As Decimal = CDec(10.0)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With {
			.SubdirectoryStartNumber = CInt(startNumber)}}
			_form.LoadExportFile(ef)
			Assert.AreEqual(startNumber, _form._subdirectoryStartNumber.Value)
		End Sub

		<Test()> Public Sub LoadExportFile_SubdirectoryNumberOfDigits_IsNotEmpty()
			Dim numberOfDigits As Int32 = 350
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.SubdirectoryDigitPadding = numberOfDigits}
			_form.LoadExportFile(ef)
			Assert.AreEqual(numberOfDigits, _form._subdirectoryDigitPadding.Value)
		End Sub

		<Test()> Public Sub LoadExportFile_SubdirectoryMaxSize_IsNotEmpty()
			Dim maxSize As Int64 = 1000
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With {
			.SubdirectoryMaxSize = maxSize}}
			_form.LoadExportFile(ef)
			Assert.AreEqual(maxSize, _form._subDirectoryMaxSize.Value)
		End Sub



#End Region

#Region "File Path Section"
		<Test()> Public Sub LoadExportFile_UseRelativePath_IsChecked()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.TypeOfExportedFilePath = WinEDDS.ExportFile.ExportedFilePathType.Relative}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._useRelativePaths.Checked)
		End Sub

		<Test()> Public Sub LoadExportFile_UseRelativePath_IsNotChecked()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.TypeOfExportedFilePath = WinEDDS.ExportFile.ExportedFilePathType.Absolute}
			_form.LoadExportFile(ef)
			Assert.IsFalse(_form._useRelativePaths.Checked)
		End Sub

		<Test()> Public Sub LoadExportFile_UseAbsolutePath_IsChecked()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.TypeOfExportedFilePath = WinEDDS.ExportFile.ExportedFilePathType.Absolute}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._useAbsolutePaths.Checked)
		End Sub

		<Test()> Public Sub LoadExportFile_UseAbsolutePath_IsNotChecked()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.TypeOfExportedFilePath = WinEDDS.ExportFile.ExportedFilePathType.Prefix}
			_form.LoadExportFile(ef)
			Assert.IsFalse(_form._useAbsolutePaths.Checked)
		End Sub

		<Test()> Public Sub LoadExportFile_UsePrefix_IsChecked()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.TypeOfExportedFilePath = WinEDDS.ExportFile.ExportedFilePathType.Prefix}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._usePrefix.Checked)
		End Sub

		<Test()> Public Sub LoadExportFile_UsePrefix_IsNotChecked()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.TypeOfExportedFilePath = WinEDDS.ExportFile.ExportedFilePathType.Absolute}
			_form.LoadExportFile(ef)
			Assert.IsFalse(_form._usePrefix.Checked)
		End Sub

		<Test()> Public Sub LoadExportFile_UsePrefix_IsChecked_PrefixTextEnabled()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.TypeOfExportedFilePath = WinEDDS.ExportFile.ExportedFilePathType.Prefix}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._usePrefix.Checked)
			Assert.IsTrue(_form._prefixText.Enabled)
		End Sub

		<Test()> Public Sub LoadExportFile_UsePrefix_IsChecked_TextIsNotEmpty_PrefixTextEnabled()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .TypeOfExportedFilePath = WinEDDS.ExportFile.ExportedFilePathType.Prefix,
			 .FilePrefix = "TEST"}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._usePrefix.Checked)
			Assert.IsTrue(_form._prefixText.Enabled)
			Assert.AreEqual("TEST", _form._prefixText.Text)
		End Sub

		<Test()> Public Sub LoadExportFile_UsePrefix_IsChecked_TextIsEmpty_PrefixTextEnabled()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.TypeOfExportedFilePath = WinEDDS.ExportFile.ExportedFilePathType.Prefix}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._usePrefix.Checked)
			Assert.IsTrue(_form._prefixText.Enabled)
		End Sub

		<Test()> Public Sub LoadExportFile_UsePrefix_IsNotChecked_PrefixTextDisabled()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.TypeOfExportedFilePath = WinEDDS.ExportFile.ExportedFilePathType.Absolute,
								.FilePrefix = ""}
			_form.LoadExportFile(ef)
			Assert.IsFalse(_form._usePrefix.Checked)
			Assert.IsFalse(_form._prefixText.Enabled)
			Assert.AreEqual("", _form._prefixText.Text)
		End Sub

#End Region

#Region "Load File Characters Section"

		<Test()> Public Sub LoadExportFile_NativeFileFormatSetToCustom_FileCharacterInformation_Enabled()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.LoadFileExtension = "txt"}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._recordDelimiter.Enabled)
			Assert.IsTrue(_form._quoteDelimiter.Enabled)
			Assert.IsTrue(_form._newLineDelimiter.Enabled)
			Assert.IsTrue(_form._multiRecordDelimiter.Enabled)
			Assert.IsTrue(_form._nestedValueDelimiter.Enabled)
		End Sub

		<Test()> Public Sub LoadExportFile_NativeFileFormatSetToConcordance_FileCharacterInformation_Disabled()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.LoadFileExtension = "dat"}
			_form.LoadExportFile(ef)
			Assert.IsFalse(_form._recordDelimiter.Enabled)
			Assert.IsFalse(_form._quoteDelimiter.Enabled)
			Assert.IsFalse(_form._newLineDelimiter.Enabled)
			Assert.IsFalse(_form._multiRecordDelimiter.Enabled)
			Assert.IsFalse(_form._nestedValueDelimiter.Enabled)
		End Sub

		<Test()> Public Sub LoadExportFile_NativeFileFormatSetToCustom_LoadFileCharactersEnabled_ColumnDilimiterSetToPipe()
			Dim expectedChar As Char = "|"c
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.LoadFileExtension = "txt", .RecordDelimiter = "|"c}
			_form.LoadExportFile(ef)
			Assert.AreEqual(expectedChar, ChrW(CType(_form._recordDelimiter.SelectedValue, Int32)))
			Assert.IsTrue(_form._recordDelimiter.Enabled)
		End Sub

		<Test()> Public Sub LoadExportFile_NativeFileFormatSetToCustom_LoadFileCharactersEnabled_QuoteDilimiterSetToPipe()
			Dim expectedChar As Char = "|"c
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.LoadFileExtension = "txt", .QuoteDelimiter = "|"c}
			_form.LoadExportFile(ef)
			Assert.AreEqual(expectedChar, ChrW(CType(_form._quoteDelimiter.SelectedValue, Int32)))
			Assert.IsTrue(_form._quoteDelimiter.Enabled)
		End Sub

		<Test()> Public Sub LoadExportFile_NativeFileFormatSetToCustom_LoadFileCharactersEnabled_NewLineDilimiterSetToPipe()
			Dim expectedChar As Char = "|"c
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.LoadFileExtension = "txt", .NewlineDelimiter = "|"c}
			_form.LoadExportFile(ef)
			Assert.AreEqual(expectedChar, ChrW(CType(_form._newLineDelimiter.SelectedValue, Int32)))
			Assert.IsTrue(_form._newLineDelimiter.Enabled)
		End Sub

		<Test()> Public Sub LoadExportFile_NativeFileFormatSetToCustom_LoadFileCharactersEnabled_MultiValueDilimiterSetToPipe()
			Dim expectedChar As Char = "|"c
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.LoadFileExtension = "txt", .MultiRecordDelimiter = "|"c}
			_form.LoadExportFile(ef)
			Assert.AreEqual(expectedChar, ChrW(CType(_form._multiRecordDelimiter.SelectedValue, Int32)))
			Assert.IsTrue(_form._multiRecordDelimiter.Enabled)
		End Sub

		<Test()> Public Sub LoadExportFile_NativeFileFormatSetToCustom_LoadFileCharactersEnabled_NestedValueDilimiterSetToPipe()
			Dim expectedChar As Char = "|"c
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.LoadFileExtension = "txt", .NestedValueDelimiter = "|"c}
			_form.LoadExportFile(ef)
			Assert.AreEqual(expectedChar, ChrW(CType(_form._nestedValueDelimiter.SelectedValue, Int32)))
			Assert.IsTrue(_form._nestedValueDelimiter.Enabled)
		End Sub

#End Region

#Region "Text And Native File Names Section"

		<Test()> Public Sub LoadExportFile_AppendOriginalFileName_Checked_True()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.AppendOriginalFileName = True}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._appendOriginalFilename.Checked)
		End Sub

		<Test()> Public Sub LoadExportFile_AppendOriginalFileName_Checked_False()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.AppendOriginalFileName = False}
			_form.LoadExportFile(ef)
			Assert.IsFalse(_form._appendOriginalFilename.Checked)
		End Sub

		<Test()> Public Sub LoadExportFile_NamedAfter_NotSelected()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportNativesToFileNamedFrom = kCura.WinEDDS.ExportNativeWithFilenameFrom.Select}
			_form.LoadExportFile(ef)
			Assert.AreEqual("Select...", _form._nativeFileNameSource.SelectedItem.ToString)
		End Sub

		<Test()> Public Sub LoadExportFile_NamedAfter_Identifier()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.AppendOriginalFileName = False, .ExportNativesToFileNamedFrom = kCura.WinEDDS.ExportNativeWithFilenameFrom.Identifier}
			_form.LoadExportFile(ef)
			Assert.AreEqual("Identifier", _form._nativeFileNameSource.SelectedItem.ToString)
		End Sub

		<Test()> Public Sub LoadExportFile_NamedAfter_Production()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.AppendOriginalFileName = False, .ExportNativesToFileNamedFrom = kCura.WinEDDS.ExportNativeWithFilenameFrom.Production}
			_form.LoadExportFile(ef)
			Assert.AreEqual("Begin production number", _form._nativeFileNameSource.SelectedItem.ToString)
		End Sub

#End Region

#Region "Export Images Section"

		<Test()> Public Sub LoadExportFile_ExportImages_False()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportImages = False}
			_form.LoadExportFile(ef)
			Assert.IsFalse(_form._exportImages.Checked)
		End Sub

		<Test()> Public Sub LoadExportFile_ExportImages_True()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportImages = True}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._exportImages.Checked)
			Assert.IsTrue(_form._imageFileFormat.Enabled)
		End Sub

		<Test()> Public Sub LoadExportFile_ExportImages_False_CopyFilesFromRepository_True_All_InTheGroup_Disabled()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportImages = False, .VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With
			 {.CopyFilesFromRepository = True}}
			_form.LoadExportFile(ef)
			Assert.IsFalse(_form._exportImages.Checked)
			Assert.IsFalse(_form._imageFileFormat.Enabled)
			Assert.IsFalse(_form._imageTypeDropdown.Enabled)
			Assert.IsTrue(_form._useAbsolutePaths.Enabled)
		End Sub

		<Test()> Public Sub LoadExportFile_ExportImages_False_CopyFilesFromRepository_False_All_InTheGroup_Disabled()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportImages = False, .VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With
			 {.CopyFilesFromRepository = False}}
			_form.LoadExportFile(ef)
			Assert.IsFalse(_form._exportImages.Checked)
			Assert.IsFalse(_form._imageFileFormat.Enabled)
			Assert.IsFalse(_form._imageTypeDropdown.Enabled)
			Assert.IsTrue(_form._useAbsolutePaths.Enabled)
		End Sub

		<Test()> Public Sub LoadExportFile_ExportImages_True_CopyFilesFromRepository_False_All_InTheGroup_Disabled()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportImages = True, .VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With
			{.CopyFilesFromRepository = False}}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._exportImages.Checked)
			Assert.IsTrue(_form._imageFileFormat.Enabled)
			Assert.IsFalse(_form._imageTypeDropdown.Enabled)
			Assert.IsTrue(_form._useAbsolutePaths.Enabled)
		End Sub

		<Test()> Public Sub LoadExportFile_ExportImages_True_CopyFilesFromRepository_True_All_InTheGroup_Disabled()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportImages = True, .VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With
			{.CopyFilesFromRepository = True}}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._exportImages.Checked)
			Assert.IsTrue(_form._imageFileFormat.Enabled)
			Assert.IsTrue(_form._imageTypeDropdown.Enabled)
			Assert.IsTrue(_form._useAbsolutePaths.Enabled)
		End Sub

		<Test()> Public Sub LoadExportFile_ExportImages_True_ImageLoadFileType_Unselected()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportImages = True, .LogFileFormat = Nothing, .VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With
			{.CopyFilesFromRepository = True}}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._exportImages.Checked)
			Assert.IsTrue(_form._imageFileFormat.Enabled)
			Assert.IsTrue(_form._imageTypeDropdown.Enabled)
			Assert.IsTrue(_form._useAbsolutePaths.Enabled)
			Assert.AreEqual(-1, _form._imageFileFormat.SelectedValue)
		End Sub

		<Test()> Public Sub LoadExportFile_ExportImages_True_ImageLoadFileType_IPRO()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportImages = True, .LogFileFormat = kCura.WinEDDS.LoadFileType.FileFormat.IPRO, .VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With
			{.CopyFilesFromRepository = True}}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._exportImages.Checked)
			Assert.IsTrue(_form._imageFileFormat.Enabled)
			Assert.IsTrue(_form._imageTypeDropdown.Enabled)
			Assert.IsTrue(_form._useAbsolutePaths.Enabled)
			Assert.AreEqual(CInt(kCura.WinEDDS.LoadFileType.FileFormat.IPRO), _form._imageFileFormat.SelectedValue)
		End Sub

		<Test()> Public Sub LoadExportFile_ExportImages_True_ImageLoadFileType_IPRO_FullText()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportImages = True, .LogFileFormat = kCura.WinEDDS.LoadFileType.FileFormat.IPRO_FullText, .VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With
			{.CopyFilesFromRepository = True}}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._exportImages.Checked)
			Assert.IsTrue(_form._imageFileFormat.Enabled)
			Assert.IsTrue(_form._imageTypeDropdown.Enabled)
			Assert.IsTrue(_form._useAbsolutePaths.Enabled)
			Assert.AreEqual(CInt(kCura.WinEDDS.LoadFileType.FileFormat.IPRO_FullText), _form._imageFileFormat.SelectedValue)
		End Sub

		<Test()> Public Sub LoadExportFile_ExportImages_True_ImageLoadFileType_Opticon()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportImages = True, .LogFileFormat = kCura.WinEDDS.LoadFileType.FileFormat.Opticon, .VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With
			{.CopyFilesFromRepository = True}}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._exportImages.Checked)
			Assert.IsTrue(_form._imageFileFormat.Enabled)
			Assert.IsTrue(_form._imageTypeDropdown.Enabled)
			Assert.IsTrue(_form._useAbsolutePaths.Enabled)
			Assert.AreEqual(CInt(kCura.WinEDDS.LoadFileType.FileFormat.Opticon), _form._imageFileFormat.SelectedValue)
		End Sub
		'kCura.WinEDDS.ExportFile.ImageType.SinglePage
		<Test()> Public Sub LoadExportFile_ExportImages_True_TypeOfImage_Unselected()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportImages = True, .TypeOfImage = Nothing, .VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With
			{.CopyFilesFromRepository = True}}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._exportImages.Checked)
			Assert.IsTrue(_form._imageFileFormat.Enabled)
			Assert.IsTrue(_form._imageTypeDropdown.Enabled)
			Assert.IsTrue(_form._useAbsolutePaths.Enabled)
			Assert.AreEqual(0, _form._imageTypeDropdown.SelectedIndex)
		End Sub
		<Test()> Public Sub LoadExportFile_ExportImages_True_TypeOfImage_SinglePage()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportImages = True, .TypeOfImage = kCura.WinEDDS.ExportFile.ImageType.SinglePage, .VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With
			{.CopyFilesFromRepository = True}}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._exportImages.Checked)
			Assert.IsTrue(_form._imageFileFormat.Enabled)
			Assert.IsTrue(_form._imageTypeDropdown.Enabled)
			Assert.IsTrue(_form._useAbsolutePaths.Enabled)
			Assert.AreEqual(1, _form._imageTypeDropdown.SelectedIndex)
		End Sub
		<Test()> Public Sub LoadExportFile_ExportImages_True_TypeOfImage_MultiPageTiff()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportImages = True, .TypeOfImage = WinEDDS.ExportFile.ImageType.MultiPageTiff, .VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With
			{.CopyFilesFromRepository = True}}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._exportImages.Checked)
			Assert.IsTrue(_form._imageFileFormat.Enabled)
			Assert.IsTrue(_form._imageTypeDropdown.Enabled)
			Assert.IsTrue(_form._useAbsolutePaths.Enabled)
			Assert.AreEqual(2, _form._imageTypeDropdown.SelectedIndex)
		End Sub
		<Test()> Public Sub LoadExportFile_ExportImages_True_TypeOfImage_Pdf()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportImages = True, .TypeOfImage = WinEDDS.ExportFile.ImageType.Pdf, .VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With
			{.CopyFilesFromRepository = True}}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._exportImages.Checked)
			Assert.IsTrue(_form._imageFileFormat.Enabled)
			Assert.IsTrue(_form._imageTypeDropdown.Enabled)
			Assert.IsTrue(_form._useAbsolutePaths.Enabled)
			Assert.AreEqual(3, _form._imageTypeDropdown.SelectedIndex)
		End Sub
#End Region

#Region "Native Section"

		<Test()> Public Sub LoadExportFile_ExportNatives_False()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportNative = False}
			_form.LoadExportFile(ef)
			Assert.IsFalse(_form._exportNativeFiles.Checked)
			Assert.IsTrue(_form._useAbsolutePaths.Enabled)
			Assert.IsFalse(_form._nativeFileFormat.Enabled)
			Assert.IsFalse(_form._metadataGroup.Enabled)
		End Sub


		<Test()> Public Sub LoadExportFile_ExportNatives_False_RightListBoxItemsIsZero()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportNative = False, .SelectedViewFields = New kCura.WinEDDS.ViewFieldInfo() {}}
			_form.LoadExportFile(ef)
			Assert.IsFalse(_form._exportNativeFiles.Checked)
			Assert.IsTrue(_form._useAbsolutePaths.Enabled)
			Assert.IsFalse(_form._nativeFileFormat.Enabled)
			Assert.IsFalse(_form._metadataGroup.Enabled)
		End Sub

		<Test()> Public Sub LoadExportFile_ExportNatives_False_RightListBoxItemsIs1()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document)
			ef.ExportNative = False
			ef.SelectedViewFields = {_queryFieldFactory.GetExtractedTextField()}
			_form.LoadExportFile(ef)
			Assert.IsFalse(_form._exportNativeFiles.Checked)
			Assert.IsTrue(_form._useAbsolutePaths.Enabled)
			Assert.IsFalse(_form._nativeFileFormat.Enabled)
			Assert.IsFalse(_form._metadataGroup.Enabled)
		End Sub

		<Test()> Public Sub LoadExportFile_RightListBoxItemsIsZero_ExportNativesTrue_MetaDataDisabled()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document)
			ef.ExportNative = True
			ef.SelectedViewFields = New kCura.WinEDDS.ViewFieldInfo() {}
			_form.LoadExportFile(ef)
			Assert.IsFalse(_form._metadataGroup.Enabled)
		End Sub

		<Test()> Public Sub LoadExportFile_RightListBoxItemsIsZero_ExportNativesFalse_MetaDataDisabled()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document)
			ef.ExportNative = False
			ef.SelectedViewFields = New kCura.WinEDDS.ViewFieldInfo() {}
			_form.LoadExportFile(ef)
			Assert.IsFalse(_form._metadataGroup.Enabled)
		End Sub

		<Test()> Public Sub LoadExportFile_RightListBoxItemsIs1_ExportNativesTrue_MetaDataEnabled()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields(),
			 .ExportNative = True,
			 .SelectedViewFields = {_queryFieldFactory.GetExtractedTextField()}}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._metadataGroup.Enabled)
		End Sub

		<Test()> Public Sub LoadExportFile_RightListBoxItemsIs1_ExportNativesFalse_MetaDataDisabled()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.AllExportableFields = _queryFieldFactory.GetAllDocumentFields(),
				 .ExportNative = False,
			 .SelectedViewFields = {_queryFieldFactory.GetExtractedTextField()}}
			_form.LoadExportFile(ef)
			Assert.IsFalse(_form._metadataGroup.Enabled)
		End Sub

		<Test()> Public Sub LoadExportFile_ExportNatives_True()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportNative = True}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._exportNativeFiles.Checked)
		End Sub

#End Region

#Region "Metadata Section"

		<Test()> Public Sub LoadExportFile_DataFileFormat_CustomTxt()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.LoadFileExtension = "txt"}
			_form.LoadExportFile(ef)
			Assert.AreEqual("Custom (.txt)", _form._nativeFileFormat.SelectedItem.ToString)
		End Sub

		<Test()> Public Sub LoadExportFile_DataFileFormat_ConcordanceDat()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.LoadFileExtension = "dat"}
			_form.LoadExportFile(ef)
			Assert.AreEqual("Concordance (.dat)", _form._nativeFileFormat.SelectedItem.ToString)
		End Sub

		<Test()> Public Sub LoadExportFile_DataFileFormat_CommaSeparatedCsv()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.LoadFileExtension = "csv"}
			_form.LoadExportFile(ef)
			Assert.AreEqual("Comma-separated (.csv)", _form._nativeFileFormat.SelectedItem.ToString)
		End Sub

		<Test()> Public Sub LoadExportFile_DataFileFormat_IsHtml()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.LoadFileIsHtml = True}
			_form.LoadExportFile(ef)
			Assert.AreEqual("HTML (.html)", _form._nativeFileFormat.SelectedItem.ToString)
		End Sub

		<Test()> Public Sub LoadExportFile_DataFileFormat_IsHtml_AlsoHasLoadFileExtensionField()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.LoadFileIsHtml = True, .LoadFileExtension = "txt"}
			_form.LoadExportFile(ef)
			Assert.AreEqual("HTML (.html)", _form._nativeFileFormat.SelectedItem.ToString)
		End Sub

		<Test()> Public Sub LoadExportFile_DataFileEncoding_NotSelected()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document)
			_form.LoadExportFile(ef)
			Assert.AreEqual(Nothing, _form._dataFileEncoding.SelectedEncoding)
		End Sub

		<Test()> Public Sub LoadExportFile_DataFileEncoding_UTF8()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.LoadFileEncoding = System.Text.Encoding.UTF8}
			_form.LoadExportFile(ef)
			Assert.AreEqual(System.Text.Encoding.UTF8, _form._dataFileEncoding.SelectedEncoding)
		End Sub

		<Test()> Public Sub LoadExportFile_ExportTextAsFiles_Checked_True()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportFullTextAsFile = True}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._exportFullTextAsFile.Checked)
		End Sub

		<Test()> Public Sub LoadExportFile_ExportTextAsFiles_Checked_False()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportFullTextAsFile = False}
			_form.LoadExportFile(ef)
			Assert.IsFalse(_form._exportFullTextAsFile.Checked)
		End Sub

		<Test()> Public Sub LoadExportFile_TextFileEncoding_NotSelected()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document)
			_form.LoadExportFile(ef)
			Assert.AreEqual(Nothing, _form._textFileEncoding.SelectedEncoding)
		End Sub

		<Test()> Public Sub LoadExportFile_TextFileEncoding_ASCII()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.TextFileEncoding = System.Text.Encoding.ASCII}
			_form.LoadExportFile(ef)
			Assert.AreEqual(System.Text.Encoding.ASCII, _form._textFileEncoding.SelectedEncoding)
		End Sub

		<Test()> Public Sub LoadExportFile_TextField_NotSelected()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document)
			_form.LoadExportFile(ef)
			Assert.AreEqual(0, _form._textFieldPrecedencePicker.SelectedFields.Count)
		End Sub

		<Test()> Public Sub LoadExportFile_TextField_FieldSelected()
			Me.ValidTextFieldSetup()
			Dim textField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(1)
			Dim boolField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(2)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			.SelectedViewFields = New WinEDDS.ViewFieldInfo() {textField, boolField},
			.SelectedTextFields = {textField}}
			_form.LoadExportFile(ef)
			Assert.IsTrue(textField.Equals(_form._textFieldPrecedencePicker.SelectedFields(0)))
		End Sub

		<Test()> Public Sub LoadExportFile_TextField_BeforeLoad_TwoPotential_AfterLoad_RenamedOnePotential_FirstSelected()
			Dim longTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetGenericLongTextField
			Dim longTextFieldRenamed As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetRenamedGenericLongTextField
			Dim extractedTextField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(1)
			Dim boolField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(2)
			Dim allAvailableFields As New Collections.Generic.List(Of WinEDDS.ViewFieldInfo)()
			allAvailableFields.AddRange(_queryFieldFactory.GetAllDocumentFields)
			allAvailableFields.Add(longTextFieldRenamed)

			Dim exportAvailable As New Collections.Generic.List(Of WinEDDS.ViewFieldInfo)()
			exportAvailable.AddRange(_queryFieldFactory.GetAllDocumentFields)
			exportAvailable.Add(longTextField)


			'current values before load
			_form._textFieldPrecedencePicker.SelectedFields = New List(Of ViewFieldInfo)()
			_form._textFieldPrecedencePicker.SelectedFields.Add(longTextField)

			'the file to be loaded
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.AllExportableFields = allAvailableFields.ToArray,
			.SelectedViewFields = exportAvailable.ToArray,
			.SelectedTextFields = {longTextField, extractedTextField}}

			_form.LoadExportFile(ef)
			Assert.AreEqual(1, _form._textFieldPrecedencePicker.SelectedFields.Count)
			Assert.IsTrue(_form._textFieldPrecedencePicker.SelectedFields(0).Equals(extractedTextField))
		End Sub

		<Test()> Public Sub LoadExportFile_ExportMultiChoiceFieldsAsNested_Checked()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.MulticodesAsNested = True}
			_form.LoadExportFile(ef)
			Assert.IsTrue(_form._exportMulticodeFieldsAsNested.Checked)
		End Sub

		<Test()> Public Sub LoadExportFile_ExportMultiChoiceFieldsAsNested_NotChecked()
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.MulticodesAsNested = False}
			_form.LoadExportFile(ef)
			Assert.IsFalse(_form._exportMulticodeFieldsAsNested.Checked)
		End Sub

#End Region


#Region "Helpers"
		Private Function DoesFieldExistsInListBox(ByVal field As kCura.WinEDDS.ViewFieldInfo, ByVal listBox As Windows.Forms.ListBox.ObjectCollection) As Boolean
			Dim retVal As Boolean = False
			For Each avf As kCura.WinEDDS.ViewFieldInfo In listBox
				If field.DisplayName.Equals(avf.DisplayName, StringComparison.InvariantCulture) Then
					retVal = True
					Exit For
				End If
			Next
			Return retVal
		End Function
#End Region

	End Class
End Namespace
