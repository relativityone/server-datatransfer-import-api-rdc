Imports System.Threading.Tasks
Imports System.Windows.Forms
Imports NUnit.Framework
Imports kCura.WinEDDS
Imports kCura.EDDS.WinForm

Namespace kCura.EDDS.WinForm.Tests
	<TestFixture()>
	Public Class ExportFormTest
		Private _form As ExportForm
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

			_form = New ExportForm()
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

			_form = New ExportForm
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

			_form = New ExportForm
			_form.ExportFile = New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			.DataTable = _filtersDataTable,
			.ArtifactAvfLookup = avfLookUp}
			_form.HandleLoad(Nothing, Nothing, 100, 100)

		End Sub

#Region "Mock Classes"

		Public Class MockApplication
			Inherits kCura.EDDS.WinForm.Application

			Public Overrides Async Function GetProductionPrecendenceList(ByVal caseInfo As Relativity.CaseInfo) As Task(Of System.Data.DataTable)
				Dim retval As New System.Data.DataTable
				retval.Columns.Add("Display")
				retval.Columns.Add("Value")

				retval.Rows.Add(New String() {"Name1", "1012345"})
				retval.Rows.Add(New String() {"Name2", "1234567"})
				retval.Rows.Add(New String() {"Name3", "1000001"})
				Return Await Task.FromResult(retval)
			End Function
		End Class

#End Region


		'***********Data Source Tab**********************
#Region "Productions Section"

		<Test()> Public Async Function LoadExportFile_Production_NotSelected() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.Production}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Function

		<Test()> Public Async Function LoadExportFile_SavedSearch_Into_ProductionExport_DefaultProduction() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch,
			 .ArtifactID = 45646}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(123, _form._filters.SelectedValue)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Function

		<Test()> Public Async Function LoadExportFile_Folder_Into_ProductionExport_DefaultProduction() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ParentSearch,
			 .ViewID = 4654897}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(123, _form._filters.SelectedValue)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Function

		<Test()> Public Async Function LoadExportFile_FolderAndSubfolder_Into_ProductionExport_DefaultProduction() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.AncestorSearch,
			 .ViewID = 4654897}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(123, _form._filters.SelectedValue)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Function

		<Test()> Public Async Function LoadExportFile_ProductionSelected_SavedProductionNoLongerExists() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.Production,
			 .ArtifactID = 1234567}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Function

		<Test()> Public Async Function LoadExportFile_Production_AllExportableFieldsShowUp() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.Production}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields.Count, _form._columnSelector.LeftSearchableListItems.Count)
		End Function

		<Test()> Public Async Function LoadExportFile_Production_AllExportableFields_OneFieldSelected() As Task
			Dim selectedField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(3)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {selectedField},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.Production}
			Await _form.LoadExportFile(ef)
			For Each avf As kCura.WinEDDS.ViewFieldInfo In _form._columnSelector.RightSearchableListItems
				If selectedField.DisplayName.Equals(avf.DisplayName, StringComparison.InvariantCulture) Then

				End If
			Next
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count - 1, _form._columnSelector.LeftSearchableListItems.Count)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField, _form._columnSelector.RightSearchableListItems))
		End Function

		<Test()> Public Async Function LoadExportFile_Production_AllExportableFields_TwoFieldSelected() As Task
			Dim selectedField1 As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(3)
			Dim selectedField2 As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(2)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {selectedField1, selectedField2},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.Production}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count - 2, _form._columnSelector.LeftSearchableListItems.Count)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField1, _form._columnSelector.RightSearchableListItems))
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField2, _form._columnSelector.RightSearchableListItems))
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count - 2, _form._columnSelector.LeftSearchableListItems.Count)
		End Function

		<Test()> Public Async Function LoadExportFile_Production_AllExportableFields_OneFieldSelected_OneFieldPrePopulated() As Task
			SetupOneFieldSelected(kCura.WinEDDS.ExportFile.ExportType.Production)
			Dim selectedField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(0)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {selectedField},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.Production}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField, _form._columnSelector.RightSearchableListItems))
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count - 1, _form._columnSelector.LeftSearchableListItems.Count)
			Assert.AreEqual(1, _form._columnSelector.RightSearchableListItems.Count)
		End Function

		<Test()> Public Async Function LoadExportFile_Production_AllExportableFields_ZeroFieldsSelected_OneFieldPrePopulated() As Task
			SetupOneFieldSelected(kCura.WinEDDS.ExportFile.ExportType.Production)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.Production}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count, _form._columnSelector.LeftSearchableListItems.Count)
			Assert.AreEqual(0, _form._columnSelector.RightSearchableListItems.Count)
		End Function

		<Test()> Public Async Function LoadExportFile_Production_AllExportableFields_TwoFieldSelected_OneFieldIsNoLongerAvaialble() As Task
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
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField1, _form._columnSelector.RightSearchableListItems))
			Assert.IsFalse(_form._columnSelector.RightSearchableListItems.Contains(selectedField2))
			Assert.IsFalse(_form._columnSelector.LeftSearchableListItems.Contains(selectedField2))
			Assert.AreEqual(0, _form._columnSelector.LeftSearchableListItems.Count)
		End Function

		<Test()> Public Async Function LoadExportFile_Production_StartExportAtDocumentNumber_Is15() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .StartAtDocumentNumber = 15,
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.Production}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(16, _form._startExportAtDocumentNumber.Value)
		End Function

		<Test()> Public Async Function LoadExportFile_Production_StartExportAtDocumentNumber_Is1_When_PreLoadedAs5() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .StartAtDocumentNumber = 0,
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.Production}
			Dim numericUpDown As NumericUpDown = New System.Windows.Forms.NumericUpDown()
			numericUpDown.Value = 5D
			_form._startExportAtDocumentNumber = numericUpDown
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(1, _form._startExportAtDocumentNumber.Value)
		End Function







#End Region

#Region "Saved Search Section"

		<Test()> Public Async Function LoadExportFile_SavedSearch_NotSelected() As Task
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Function

		<Test()> Public Async Function LoadExportFile_Folder_Into_SavedSearchExport_DefaultSavedSearch() As Task
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ParentSearch,
			.ViewID = 45646}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(123, _form._filters.SelectedValue)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Function

		<Test()> Public Async Function LoadExportFile_Production_Into_SavedSearchExport_DefaultSavedSearch() As Task
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.Production,
			.ArtifactID = 4654897}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(123, _form._filters.SelectedValue)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Function

		<Test()> Public Async Function LoadExportFile_FolderAndSubfolder_Into_SavedSearchExport_DefaultSavedSearch() As Task
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.AncestorSearch,
			.ViewID = 4654897}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(123, _form._filters.SelectedValue)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Function

		<Test()> Public Async Function LoadExportFile_SavedSearchSelected_SavedSavedSearchNoLongerExists() As Task
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch,
			.ArtifactID = 1234567}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Function

		<Test()> Public Async Function LoadExportFile_SavedSearch_AllExportableFieldsShowUp() As Task
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields.Count, _form._columnSelector.LeftSearchableListItems.Count)
		End Function

		<Test()> Public Async Function LoadExportFile_SavedSearch_AllExportableFields_OneFieldSelected() As Task
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch)
			Dim selectedField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(3)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {selectedField},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField, _form._columnSelector.RightSearchableListItems))
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count - 1, _form._columnSelector.LeftSearchableListItems.Count)
		End Function

		<Test()> Public Async Function LoadExportFile_SavedSearch_AllExportableFields_TwoFieldSelected() As Task
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch)
			Dim selectedField1 As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(3)
			Dim selectedField2 As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(2)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {selectedField1, selectedField2},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField1, _form._columnSelector.RightSearchableListItems))
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField2, _form._columnSelector.RightSearchableListItems))
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count - 2, _form._columnSelector.LeftSearchableListItems.Count)
		End Function

		<Test()> Public Async Function LoadExportFile_SavedSearch_AllExportableFields_OneFieldSelected_OneFieldPrePopulated() As Task
			SetupOneFieldSelected(kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch)
			Dim selectedField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(0)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {selectedField},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField, _form._columnSelector.RightSearchableListItems))
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count - 1, _form._columnSelector.LeftSearchableListItems.Count)
			Assert.AreEqual(1, _form._columnSelector.RightSearchableListItems.Count)
		End Function

		<Test()> Public Async Function LoadExportFile_SavedSearch_AllExportableFields_ZeroFieldsSelected_OneFieldPrePopulated() As Task
			SetupOneFieldSelected(kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count, _form._columnSelector.LeftSearchableListItems.Count)
			Assert.AreEqual(0, _form._columnSelector.RightSearchableListItems.Count)
		End Function

		<Test()> Public Async Function LoadExportFile_SavedSearch_AllExportableFields_TwoFieldSelected_OneFieldIsNoLongerAvaialble() As Task
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
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField1, _form._columnSelector.RightSearchableListItems))
			Assert.IsFalse(_form._columnSelector.RightSearchableListItems.Contains(selectedField2))
			Assert.IsFalse(_form._columnSelector.LeftSearchableListItems.Contains(selectedField2))
			Assert.AreEqual(0, _form._columnSelector.LeftSearchableListItems.Count)
		End Function

		<Test()> Public Async Function LoadExportFile_SavedSearch_StartExportAtDocumentNumber_Is15() As Task
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.StartAtDocumentNumber = 15,
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(16, _form._startExportAtDocumentNumber.Value)
		End Function

#End Region

#Region "Folder Section"

		<Test()> Public Async Function LoadExportFile_Folder_NotSelected() As Task
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ParentSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ParentSearch}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Function

		<Test()> Public Async Function LoadExportFile_SavedSearch_Into_FolderExport_DefaultFolder() As Task
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ParentSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch,
			.ArtifactID = 45646}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(123, _form._filters.SelectedValue)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Function

		<Test()> Public Async Function LoadExportFile_Production_Into_FolderExport_DefaultFolder() As Task
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ParentSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.Production,
			.ArtifactID = 4654897}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(123, _form._filters.SelectedValue)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Function

		<Test()> Public Async Function LoadExportFile_FolderAndSubfolder_Into_FolderExport_DefaultFolder() As Task
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ParentSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.AncestorSearch,
			.ViewID = 4654897}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(123, _form._filters.SelectedValue)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Function

		<Test()> Public Async Function LoadExportFile_FolderSelected_SavedFolderNoLongerExists() As Task
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ParentSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ParentSearch,
			.ViewID = 1234567}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Function

		<Test()> Public Async Function LoadExportFile_Folder_AllExportableFieldsShowUp() As Task
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ParentSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ParentSearch}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields.Count, _form._columnSelector.LeftSearchableListItems.Count)
		End Function

		<Test()> Public Async Function LoadExportFile_Folder_AllExportableFields_OneFieldSelected() As Task
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ParentSearch)
			Dim selectedField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(3)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {selectedField},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ParentSearch}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField, _form._columnSelector.RightSearchableListItems))
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count - 1, _form._columnSelector.LeftSearchableListItems.Count)
		End Function

		<Test()> Public Async Function LoadExportFile_Folder_AllExportableFields_TwoFieldSelected() As Task
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ParentSearch)
			Dim selectedField1 As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(3)
			Dim selectedField2 As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(2)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {selectedField1, selectedField2},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ParentSearch}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField1, _form._columnSelector.RightSearchableListItems))
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField2, _form._columnSelector.RightSearchableListItems))
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count - 2, _form._columnSelector.LeftSearchableListItems.Count)
		End Function

		<Test()> Public Async Function LoadExportFile_Folder_AllExportableFields_OneFieldSelected_OneFieldPrePopulated() As Task
			SetupOneFieldSelected(kCura.WinEDDS.ExportFile.ExportType.ParentSearch)
			Dim selectedField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(0)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {selectedField},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ParentSearch}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField, _form._columnSelector.RightSearchableListItems))
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count - 1, _form._columnSelector.LeftSearchableListItems.Count)
			Assert.AreEqual(1, _form._columnSelector.RightSearchableListItems.Count)
		End Function

		<Test()> Public Async Function LoadExportFile_Folder_AllExportableFields_ZeroFieldsSelected_OneFieldPrePopulated() As Task
			SetupOneFieldSelected(kCura.WinEDDS.ExportFile.ExportType.ParentSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ParentSearch}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count, _form._columnSelector.LeftSearchableListItems.Count)
			Assert.AreEqual(0, _form._columnSelector.RightSearchableListItems.Count)
		End Function

		<Test()> Public Async Function LoadExportFile_Folder_AllExportableFields_TwoFieldSelected_OneFieldIsNoLongerAvaialble() As Task
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
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField1, _form._columnSelector.RightSearchableListItems))
			Assert.IsFalse(_form._columnSelector.RightSearchableListItems.Contains(selectedField2))
			Assert.IsFalse(_form._columnSelector.LeftSearchableListItems.Contains(selectedField2))
			Assert.AreEqual(0, _form._columnSelector.LeftSearchableListItems.Count)
		End Function

		<Test()> Public Async Function LoadExportFile_Folder_StartExportAtDocumentNumber_Is15() As Task
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.ParentSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.StartAtDocumentNumber = 15,
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ParentSearch}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(16, _form._startExportAtDocumentNumber.Value)
		End Function

#End Region

#Region "FolderAndSubFolder Section"

		<Test()> Public Async Function LoadExportFile_FolderAndSubFolder_NotSelected() As Task
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.AncestorSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.AncestorSearch}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Function

		<Test()> Public Async Function LoadExportFile_SavedSearch_Into_FolderAndSubFolderExport_DefaultFolderAndSubFolder() As Task
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.AncestorSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ArtifactSearch,
			.ArtifactID = 45646}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(123, _form._filters.SelectedValue)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Function

		<Test()> Public Async Function LoadExportFile_Production_Into_FolderAndSubFolderExport_DefaultFolderAndSubFolder() As Task
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.AncestorSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.Production,
			.ArtifactID = 4654897}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(123, _form._filters.SelectedValue)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Function

		<Test()> Public Async Function LoadExportFile_Folder_Into_FolderAndSubFolderExport_DefaultFolderAndSubFolder() As Task
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.AncestorSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.ParentSearch,
			.ViewID = 4654897}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(123, _form._filters.SelectedValue)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Function

		<Test()> Public Async Function LoadExportFile_FolderAndSubFolderSelected_SavedFolderAndSubFolderNoLongerExists() As Task
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.AncestorSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.AncestorSearch,
			.ViewID = 1234567}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(_filtersDataTable.Rows(0), DirectCast(_form._filters.SelectedItem, System.Data.DataRowView).Row)
		End Function

		<Test()> Public Async Function LoadExportFile_FolderAndSubFolder_AllExportableFieldsShowUp() As Task
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.AncestorSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.AncestorSearch}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields.Count, _form._columnSelector.LeftSearchableListItems.Count)
		End Function

		<Test()> Public Async Function LoadExportFile_FolderAndSubFolder_AllExportableFields_OneFieldSelected() As Task
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.AncestorSearch)
			Dim selectedField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(3)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {selectedField},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.AncestorSearch}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField, _form._columnSelector.RightSearchableListItems))
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count - 1, _form._columnSelector.LeftSearchableListItems.Count)
		End Function

		<Test()> Public Async Function LoadExportFile_FolderAndSubFolder_AllExportableFields_TwoFieldSelected() As Task
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.AncestorSearch)
			Dim selectedField1 As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(3)
			Dim selectedField2 As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(2)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {selectedField1, selectedField2},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.AncestorSearch}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField1, _form._columnSelector.RightSearchableListItems))
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField2, _form._columnSelector.RightSearchableListItems))
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count - 2, _form._columnSelector.LeftSearchableListItems.Count)
		End Function

		<Test()> Public Async Function LoadExportFile_FolderAndSubFolder_AllExportableFields_OneFieldSelected_OneFieldPrePopulated() As Task
			SetupOneFieldSelected(kCura.WinEDDS.ExportFile.ExportType.AncestorSearch)
			Dim selectedField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(0)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {selectedField},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.AncestorSearch}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField, _form._columnSelector.RightSearchableListItems))
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count - 1, _form._columnSelector.LeftSearchableListItems.Count)
			Assert.AreEqual(1, _form._columnSelector.RightSearchableListItems.Count)
		End Function

		<Test()> Public Async Function LoadExportFile_FolderAndSubFolder_AllExportableFields_ZeroFieldsSelected_OneFieldPrePopulated() As Task
			SetupOneFieldSelected(kCura.WinEDDS.ExportFile.ExportType.AncestorSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			 .SelectedViewFields = New WinEDDS.ViewFieldInfo() {},
			 .TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.AncestorSearch}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(_queryFieldFactory.GetAllDocumentFields().Count, _form._columnSelector.LeftSearchableListItems.Count)
			Assert.AreEqual(0, _form._columnSelector.RightSearchableListItems.Count)
		End Function

		<Test()> Public Async Function LoadExportFile_FolderAndSubFolder_AllExportableFields_TwoFieldSelected_OneFieldIsNoLongerAvaialble() As Task
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
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(DoesFieldExistsInListBox(selectedField1, _form._columnSelector.RightSearchableListItems))
			Assert.IsFalse(_form._columnSelector.RightSearchableListItems.Contains(selectedField2))
			Assert.IsFalse(_form._columnSelector.LeftSearchableListItems.Contains(selectedField2))
			Assert.AreEqual(0, _form._columnSelector.LeftSearchableListItems.Count)
		End Function

		<Test()> Public Async Function LoadExportFile_FolderAndSubFolder_StartExportAtDocumentNumber_Is15() As Task
			SetUpForTypeOfExport(kCura.WinEDDS.ExportFile.ExportType.AncestorSearch)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.StartAtDocumentNumber = 15,
			.TypeOfExport = kCura.WinEDDS.ExportFile.ExportType.AncestorSearch}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(16, _form._startExportAtDocumentNumber.Value)
		End Function

#End Region

#Region "Production Precedence Section"
		<Test()> Public Async Function LoadExportFile_ProductionPrecedence_OriginalIsDefault() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document)
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(1, _form._productionPrecedenceList.Items.Count)
			Assert.AreEqual("-1", DirectCast(_form._productionPrecedenceList.Items.Item(0), kCura.WinEDDS.Pair).Value)
			Assert.AreEqual("Original", DirectCast(_form._productionPrecedenceList.Items.Item(0), kCura.WinEDDS.Pair).Display)
		End Function

		<Test()> Public Async Function LoadExportFile_ProductionPrecedence_SetWithOne() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .ImagePrecedence = New kCura.WinEDDS.Pair() {New kCura.WinEDDS.Pair("1012345", "Name1")},
			 .CaseInfo = New Relativity.CaseInfo()}
			_form.Application = New MockApplication
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(1, _form._productionPrecedenceList.Items.Count)
			Assert.AreEqual("1012345", DirectCast(_form._productionPrecedenceList.Items.Item(0), kCura.WinEDDS.Pair).Value)
			Assert.AreEqual("Name1", DirectCast(_form._productionPrecedenceList.Items.Item(0), kCura.WinEDDS.Pair).Display)
		End Function

		<Test()> Public Async Function LoadExportFile_ProductionPrecedence_SetWithTwo() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .ImagePrecedence = New kCura.WinEDDS.Pair() {New kCura.WinEDDS.Pair("1012345", "Name1"), New kCura.WinEDDS.Pair("1234567", "Name2")},
			 .CaseInfo = New Relativity.CaseInfo()}
			_form.Application = New MockApplication
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(2, _form._productionPrecedenceList.Items.Count)
			Assert.AreEqual("1012345", DirectCast(_form._productionPrecedenceList.Items.Item(0), kCura.WinEDDS.Pair).Value)
			Assert.AreEqual("Name1", DirectCast(_form._productionPrecedenceList.Items.Item(0), kCura.WinEDDS.Pair).Display)
			Assert.AreEqual("1234567", DirectCast(_form._productionPrecedenceList.Items.Item(1), kCura.WinEDDS.Pair).Value)
			Assert.AreEqual("Name2", DirectCast(_form._productionPrecedenceList.Items.Item(1), kCura.WinEDDS.Pair).Display)
		End Function

		<Test()> Public Async Function LoadExportFile_ProductionPrecedence_SetWithOne_IDDoesNotMatch_NameMatches() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .ImagePrecedence = New kCura.WinEDDS.Pair() {New kCura.WinEDDS.Pair("14", "Name1")},
			 .CaseInfo = New Relativity.CaseInfo()}
			_form.Application = New MockApplication
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(1, _form._productionPrecedenceList.Items.Count)
			Assert.AreEqual("1012345", DirectCast(_form._productionPrecedenceList.Items.Item(0), kCura.WinEDDS.Pair).Value)
			Assert.AreEqual("Name1", DirectCast(_form._productionPrecedenceList.Items.Item(0), kCura.WinEDDS.Pair).Display)
		End Function

		<Test()> Public Async Function LoadExportFile_ProductionPrecedence_SetWithTwo_IDDoesNotMatch_NameMatches() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .ImagePrecedence = New kCura.WinEDDS.Pair() {New kCura.WinEDDS.Pair("14", "Name1"), New kCura.WinEDDS.Pair("16", "Name2")},
			 .CaseInfo = New Relativity.CaseInfo()}
			_form.Application = New MockApplication
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(2, _form._productionPrecedenceList.Items.Count)
			Assert.AreEqual("1012345", DirectCast(_form._productionPrecedenceList.Items.Item(0), kCura.WinEDDS.Pair).Value)
			Assert.AreEqual("Name1", DirectCast(_form._productionPrecedenceList.Items.Item(0), kCura.WinEDDS.Pair).Display)
			Assert.AreEqual("1234567", DirectCast(_form._productionPrecedenceList.Items.Item(1), kCura.WinEDDS.Pair).Value)
			Assert.AreEqual("Name2", DirectCast(_form._productionPrecedenceList.Items.Item(1), kCura.WinEDDS.Pair).Display)
		End Function

		<Test()> Public Async Function LoadExportFile_ProductionPrecedence_SetWithOneInvalid_OnlyOriginal() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .ImagePrecedence = New kCura.WinEDDS.Pair() {New kCura.WinEDDS.Pair("1", "Bogus")},
			 .CaseInfo = New Relativity.CaseInfo()}
			_form.Application = New MockApplication
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(1, _form._productionPrecedenceList.Items.Count)
			Assert.AreEqual("-1", DirectCast(_form._productionPrecedenceList.Items.Item(0), kCura.WinEDDS.Pair).Value)
			Assert.AreEqual("Original", DirectCast(_form._productionPrecedenceList.Items.Item(0), kCura.WinEDDS.Pair).Display)
		End Function


#End Region

		'***********Destination Files Tab**********************

#Region "Export Location Section"

		<Test()> Public Async Function LoadExportFile_ExportLocation_Set() As Task
			Dim somePath As String = "C:\SOMEPATH"
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.FolderPath = somePath}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._folderPath.Text.Equals(somePath, StringComparison.InvariantCultureIgnoreCase))

		End Function

		<Test()> Public Async Function LoadExportFile_ExportLocation_NotSet() As Task
			Dim somePath As String = ""
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.FolderPath = somePath}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._folderPath.Text.Equals(somePath, StringComparison.InvariantCultureIgnoreCase))
		End Function

		<Test()> Public Async Function LoadExportFile_OverwriteFiles_False() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.Overwrite = False}
			Await _form.LoadExportFile(ef)
			Assert.IsFalse(_form._overwriteCheckBox.Checked)
		End Function

		<Test()> Public Async Function LoadExportFile_OverwriteFiles_True() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.Overwrite = True}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._overwriteCheckBox.Checked)
		End Function

#End Region

#Region "Physical File Export Section"

		<Test()> Public Async Function LoadExportFile_CopyFilesFromRepository_True() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With {
			 .CopyNativeFilesFromRepository = True, .CopyImageFilesFromRepository = True}
			}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._copyFilesFromRepository.Checked)
		End Function

		<Test()> Public Async Function LoadExportFile_CopyFilesFromRepository_False() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With {
			 .CopyImageFilesFromRepository = False, .CopyNativeFilesFromRepository = False}
			}
			Await _form.LoadExportFile(ef)
			Assert.IsFalse(_form._copyFilesFromRepository.Checked)
		End Function

#End Region

#Region "Volume Information Section"
		<Test()> Public Async Function LoadExportFile_Prefix_IsNotEmpty() As Task
			Dim prefixText As String = "TestPrefix"
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With {
			.VolumePrefix = prefixText}}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._volumePrefix.Text.Equals(prefixText, StringComparison.InvariantCultureIgnoreCase))
		End Function

		<Test()> Public Async Function LoadExportFile_Prefix_IsEmpty() As Task
			Dim prefixText As String = ""
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With {
			.VolumePrefix = prefixText}}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._volumePrefix.Text.Equals(prefixText, StringComparison.InvariantCultureIgnoreCase))
		End Function


		<Test()> Public Async Function LoadExportFile_StartNumber_IsNotEmpty() As Task
			Dim startNumber As Decimal = CDec(10.0)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With {
			.VolumeStartNumber = CInt(startNumber)}}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._volumeStartNumber.Value = startNumber)
		End Function

		<Test()> Public Async Function LoadExportFile_NumberOfDigits_IsNotEmpty() As Task
			Dim numberOfDigits As Int32 = 350
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.VolumeDigitPadding = numberOfDigits}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._volumeDigitPadding.Value = numberOfDigits)
		End Function

		<Test()> Public Async Function LoadExportFile_MaxSize_IsNotEmpty() As Task
			Dim maxSize As Int64 = 1000
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With {
			.VolumeMaxSize = maxSize}}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._volumeMaxSize.Value = maxSize)
		End Function




#End Region

#Region "Subdirectory Information Section"
		<Test()> Public Async Function LoadExportFile_ImagePrefix_IsNotEmpty() As Task
			Dim prefixText As String = "TestPrefix"
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With {
			.SubdirectoryImagePrefix = prefixText}}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(prefixText, _form._subdirectoryImagePrefix.Text)
		End Function

		<Test()> Public Async Function LoadExportFile_ImagePrefix_IsEmpty() As Task
			Dim prefixText As String = ""
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With {
			.SubdirectoryImagePrefix = prefixText}}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(prefixText, _form._subdirectoryImagePrefix.Text)
		End Function

		<Test()> Public Async Function LoadExportFile_NativePrefix_IsNotEmpty() As Task
			Dim prefixText As String = "TestPrefix"
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With {
			.SubdirectoryNativePrefix = prefixText}}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(prefixText, _form._subDirectoryNativePrefix.Text)
		End Function

		<Test()> Public Async Function LoadExportFile_NativePrefix_IsEmpty() As Task
			Dim prefixText As String = ""
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With {
			.SubdirectoryNativePrefix = prefixText}}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(prefixText, _form._subDirectoryNativePrefix.Text)
		End Function

		<Test()> Public Async Function LoadExportFile_TextPrefix_IsNotEmpty() As Task
			Dim prefixText As String = "TestPrefix"
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With {
			.SubdirectoryFullTextPrefix = prefixText}}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(prefixText, _form._subdirectoryTextPrefix.Text)
		End Function

		<Test()> Public Async Function LoadExportFile_TextPrefix_IsEmpty() As Task
			Dim prefixText As String = ""
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With {
			.SubdirectoryFullTextPrefix = prefixText}}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(prefixText, _form._subdirectoryTextPrefix.Text)
		End Function


		<Test()> Public Async Function LoadExportFile_SubdirectoryStartNumber_IsNotEmpty() As Task
			Dim startNumber As Decimal = CDec(10.0)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With {
			.SubdirectoryStartNumber = CInt(startNumber)}}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(startNumber, _form._subdirectoryStartNumber.Value)
		End Function

		<Test()> Public Async Function LoadExportFile_SubdirectoryNumberOfDigits_IsNotEmpty() As Task
			Dim numberOfDigits As Int32 = 350
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.SubdirectoryDigitPadding = numberOfDigits}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(numberOfDigits, _form._subdirectoryDigitPadding.Value)
		End Function

		<Test()> Public Async Function LoadExportFile_SubdirectoryMaxSize_IsNotEmpty() As Task
			Dim maxSize As Int64 = 1000
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With {
			.SubdirectoryMaxSize = maxSize}}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(maxSize, _form._subDirectoryMaxSize.Value)
		End Function



#End Region

#Region "File Path Section"
		<Test()> Public Async Function LoadExportFile_UseRelativePath_IsChecked() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.TypeOfExportedFilePath = WinEDDS.ExportFile.ExportedFilePathType.Relative}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._useRelativePaths.Checked)
		End Function

		<Test()> Public Async Function LoadExportFile_UseRelativePath_IsNotChecked() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.TypeOfExportedFilePath = WinEDDS.ExportFile.ExportedFilePathType.Absolute}
			Await _form.LoadExportFile(ef)
			Assert.IsFalse(_form._useRelativePaths.Checked)
		End Function

		<Test()> Public Async Function LoadExportFile_UseAbsolutePath_IsChecked() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.TypeOfExportedFilePath = WinEDDS.ExportFile.ExportedFilePathType.Absolute}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._useAbsolutePaths.Checked)
		End Function

		<Test()> Public Async Function LoadExportFile_UseAbsolutePath_IsNotChecked() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.TypeOfExportedFilePath = WinEDDS.ExportFile.ExportedFilePathType.Prefix}
			Await _form.LoadExportFile(ef)
			Assert.IsFalse(_form._useAbsolutePaths.Checked)
		End Function

		<Test()> Public Async Function LoadExportFile_UsePrefix_IsChecked() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.TypeOfExportedFilePath = WinEDDS.ExportFile.ExportedFilePathType.Prefix}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._usePrefix.Checked)
		End Function

		<Test()> Public Async Function LoadExportFile_UsePrefix_IsNotChecked() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.TypeOfExportedFilePath = WinEDDS.ExportFile.ExportedFilePathType.Absolute}
			Await _form.LoadExportFile(ef)
			Assert.IsFalse(_form._usePrefix.Checked)
		End Function

		<Test()> Public Async Function LoadExportFile_UsePrefix_IsChecked_PrefixTextEnabled() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.TypeOfExportedFilePath = WinEDDS.ExportFile.ExportedFilePathType.Prefix}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._usePrefix.Checked)
			Assert.IsTrue(_form._prefixText.Enabled)
		End Function

		<Test()> Public Async Function LoadExportFile_UsePrefix_IsChecked_TextIsNotEmpty_PrefixTextEnabled() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .TypeOfExportedFilePath = WinEDDS.ExportFile.ExportedFilePathType.Prefix,
			 .FilePrefix = "TEST"}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._usePrefix.Checked)
			Assert.IsTrue(_form._prefixText.Enabled)
			Assert.AreEqual("TEST", _form._prefixText.Text)
		End Function

		<Test()> Public Async Function LoadExportFile_UsePrefix_IsChecked_TextIsEmpty_PrefixTextEnabled() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.TypeOfExportedFilePath = WinEDDS.ExportFile.ExportedFilePathType.Prefix}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._usePrefix.Checked)
			Assert.IsTrue(_form._prefixText.Enabled)
		End Function

		<Test()> Public Async Function LoadExportFile_UsePrefix_IsNotChecked_PrefixTextDisabled() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.TypeOfExportedFilePath = WinEDDS.ExportFile.ExportedFilePathType.Absolute,
				 .FilePrefix = ""}
			Await _form.LoadExportFile(ef)
			Assert.IsFalse(_form._usePrefix.Checked)
			Assert.IsFalse(_form._prefixText.Enabled)
			Assert.AreEqual("", _form._prefixText.Text)
		End Function

#End Region

#Region "Load File Characters Section"

		<Test()> Public Async Function LoadExportFile_NativeFileFormatSetToCustom_FileCharacterInformation_Enabled() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.LoadFileExtension = "txt"}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._recordDelimiter.Enabled)
			Assert.IsTrue(_form._quoteDelimiter.Enabled)
			Assert.IsTrue(_form._newLineDelimiter.Enabled)
			Assert.IsTrue(_form._multiRecordDelimiter.Enabled)
			Assert.IsTrue(_form._nestedValueDelimiter.Enabled)
		End Function

		<Test()> Public Async Function LoadExportFile_NativeFileFormatSetToConcordance_FileCharacterInformation_Disabled() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.LoadFileExtension = "dat"}
			Await _form.LoadExportFile(ef)
			Assert.IsFalse(_form._recordDelimiter.Enabled)
			Assert.IsFalse(_form._quoteDelimiter.Enabled)
			Assert.IsFalse(_form._newLineDelimiter.Enabled)
			Assert.IsFalse(_form._multiRecordDelimiter.Enabled)
			Assert.IsFalse(_form._nestedValueDelimiter.Enabled)
		End Function

		<Test()> Public Async Function LoadExportFile_NativeFileFormatSetToCustom_LoadFileCharactersEnabled_ColumnDilimiterSetToPipe() As Task
			Dim expectedChar As Char = "|"c
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.LoadFileExtension = "txt", .RecordDelimiter = "|"c}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(expectedChar, ChrW(CType(_form._recordDelimiter.SelectedValue, Int32)))
			Assert.IsTrue(_form._recordDelimiter.Enabled)
		End Function

		<Test()> Public Async Function LoadExportFile_NativeFileFormatSetToCustom_LoadFileCharactersEnabled_QuoteDilimiterSetToPipe() As Task
			Dim expectedChar As Char = "|"c
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.LoadFileExtension = "txt", .QuoteDelimiter = "|"c}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(expectedChar, ChrW(CType(_form._quoteDelimiter.SelectedValue, Int32)))
			Assert.IsTrue(_form._quoteDelimiter.Enabled)
		End Function

		<Test()> Public Async Function LoadExportFile_NativeFileFormatSetToCustom_LoadFileCharactersEnabled_NewLineDilimiterSetToPipe() As Task
			Dim expectedChar As Char = "|"c
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.LoadFileExtension = "txt", .NewlineDelimiter = "|"c}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(expectedChar, ChrW(CType(_form._newLineDelimiter.SelectedValue, Int32)))
			Assert.IsTrue(_form._newLineDelimiter.Enabled)
		End Function

		<Test()> Public Async Function LoadExportFile_NativeFileFormatSetToCustom_LoadFileCharactersEnabled_MultiValueDilimiterSetToPipe() As Task
			Dim expectedChar As Char = "|"c
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.LoadFileExtension = "txt", .MultiRecordDelimiter = "|"c}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(expectedChar, ChrW(CType(_form._multiRecordDelimiter.SelectedValue, Int32)))
			Assert.IsTrue(_form._multiRecordDelimiter.Enabled)
		End Function

		<Test()> Public Async Function LoadExportFile_NativeFileFormatSetToCustom_LoadFileCharactersEnabled_NestedValueDilimiterSetToPipe() As Task
			Dim expectedChar As Char = "|"c
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.LoadFileExtension = "txt", .NestedValueDelimiter = "|"c}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(expectedChar, ChrW(CType(_form._nestedValueDelimiter.SelectedValue, Int32)))
			Assert.IsTrue(_form._nestedValueDelimiter.Enabled)
		End Function

#End Region

#Region "Text And Native File Names Section"

		<Test()> Public Async Function LoadExportFile_AppendOriginalFileName_Checked_True() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.AppendOriginalFileName = True}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._appendOriginalFilenameCheckbox.Checked)
		End Function

		<Test()> <Ignore("Running longer than 5 seconds")> Public Async Function LoadExportFile_AppendOriginalFileName_Checked_False() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.AppendOriginalFileName = False}
			Await _form.LoadExportFile(ef)
			Assert.IsFalse(_form._appendOriginalFilenameCheckbox.Checked)
		End Function

		<Test()> Public Async Function LoadExportFile_NamedAfter_NotSelected() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportNativesToFileNamedFrom = kCura.WinEDDS.ExportNativeWithFilenameFrom.Select}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual("Select...", _form._nativeFileNameSourceCombo.SelectedItem.ToString)
		End Function

		<Test()> Public Async Function LoadExportFile_NamedAfter_Identifier() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.AppendOriginalFileName = False, .ExportNativesToFileNamedFrom = kCura.WinEDDS.ExportNativeWithFilenameFrom.Identifier}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual("Identifier", _form._nativeFileNameSourceCombo.SelectedItem.ToString)
		End Function

		<Test()> Public Async Function LoadExportFile_NamedAfter_Production() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.AppendOriginalFileName = False, .ExportNativesToFileNamedFrom = kCura.WinEDDS.ExportNativeWithFilenameFrom.Production}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual("Begin production number", _form._nativeFileNameSourceCombo.SelectedItem.ToString)
		End Function

#End Region

#Region "Export Images Section"

		<Test()> Public Async Function LoadExportFile_ExportImages_False() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportImages = False}
			Await _form.LoadExportFile(ef)
			Assert.IsFalse(_form._exportImages.Checked)
		End Function

		<Test()> Public Async Function LoadExportFile_ExportImages_True() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportImages = True}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._exportImages.Checked)
			Assert.IsTrue(_form._imageFileFormat.Enabled)
		End Function

		<Test()> Public Async Function LoadExportFile_ExportImages_False_CopyFilesFromRepository_True_All_InTheGroup_Disabled() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportImages = False, .VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With
			{.CopyImageFilesFromRepository = True, .CopyNativeFilesFromRepository = True}}
			Await _form.LoadExportFile(ef)
			Assert.IsFalse(_form._exportImages.Checked)
			Assert.IsFalse(_form._imageFileFormat.Enabled)
			Assert.IsFalse(_form._imageTypeDropdown.Enabled)
			Assert.IsTrue(_form._useAbsolutePaths.Enabled)
		End Function

		<Test()> Public Async Function LoadExportFile_ExportImages_False_CopyFilesFromRepository_False_All_InTheGroup_Disabled() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportImages = False, .VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With
			{.CopyImageFilesFromRepository = False, .CopyNativeFilesFromRepository = False}}
			Await _form.LoadExportFile(ef)
			Assert.IsFalse(_form._exportImages.Checked)
			Assert.IsFalse(_form._imageFileFormat.Enabled)
			Assert.IsFalse(_form._imageTypeDropdown.Enabled)
			Assert.IsTrue(_form._useAbsolutePaths.Enabled)
		End Function

		<Test()> Public Async Function LoadExportFile_ExportImages_True_CopyFilesFromRepository_False_All_InTheGroup_Disabled() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportImages = True, .VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With
			{.CopyImageFilesFromRepository = False, .CopyNativeFilesFromRepository = False}}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._exportImages.Checked)
			Assert.IsTrue(_form._imageFileFormat.Enabled)
			Assert.IsFalse(_form._imageTypeDropdown.Enabled)
			Assert.IsTrue(_form._useAbsolutePaths.Enabled)
		End Function

		<Test()> Public Async Function LoadExportFile_ExportImages_True_CopyFilesFromRepository_True_All_InTheGroup_Disabled() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportImages = True, .VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With
			{.CopyImageFilesFromRepository = True, .CopyNativeFilesFromRepository = True}}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._exportImages.Checked)
			Assert.IsTrue(_form._imageFileFormat.Enabled)
			Assert.IsTrue(_form._imageTypeDropdown.Enabled)
			Assert.IsTrue(_form._useAbsolutePaths.Enabled)
		End Function

		<Test()> Public Async Function LoadExportFile_ExportImages_True_ImageLoadFileType_Unselected() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportImages = True, .LogFileFormat = Nothing, .VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With
			{.CopyImageFilesFromRepository = True, .CopyNativeFilesFromRepository = True}}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._exportImages.Checked)
			Assert.IsTrue(_form._imageFileFormat.Enabled)
			Assert.IsTrue(_form._imageTypeDropdown.Enabled)
			Assert.IsTrue(_form._useAbsolutePaths.Enabled)
			Assert.AreEqual(-1, _form._imageFileFormat.SelectedValue)
		End Function

		<Test()> Public Async Function LoadExportFile_ExportImages_True_ImageLoadFileType_IPRO() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportImages = True, .LogFileFormat = kCura.WinEDDS.LoadFileType.FileFormat.IPRO, .VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With
			{.CopyImageFilesFromRepository = True, .CopyNativeFilesFromRepository = True}}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._exportImages.Checked)
			Assert.IsTrue(_form._imageFileFormat.Enabled)
			Assert.IsTrue(_form._imageTypeDropdown.Enabled)
			Assert.IsTrue(_form._useAbsolutePaths.Enabled)
			Assert.AreEqual(CInt(kCura.WinEDDS.LoadFileType.FileFormat.IPRO), _form._imageFileFormat.SelectedValue)
		End Function

		<Test()> Public Async Function LoadExportFile_ExportImages_True_ImageLoadFileType_IPRO_FullText() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportImages = True, .LogFileFormat = kCura.WinEDDS.LoadFileType.FileFormat.IPRO_FullText, .VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With
			{.CopyImageFilesFromRepository = True, .CopyNativeFilesFromRepository = True}}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._exportImages.Checked)
			Assert.IsTrue(_form._imageFileFormat.Enabled)
			Assert.IsTrue(_form._imageTypeDropdown.Enabled)
			Assert.IsTrue(_form._useAbsolutePaths.Enabled)
			Assert.AreEqual(CInt(kCura.WinEDDS.LoadFileType.FileFormat.IPRO_FullText), _form._imageFileFormat.SelectedValue)
		End Function

		<Test()> Public Async Function LoadExportFile_ExportImages_True_ImageLoadFileType_Opticon() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportImages = True, .LogFileFormat = kCura.WinEDDS.LoadFileType.FileFormat.Opticon, .VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With
			{.CopyImageFilesFromRepository = True, .CopyNativeFilesFromRepository = True}}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._exportImages.Checked)
			Assert.IsTrue(_form._imageFileFormat.Enabled)
			Assert.IsTrue(_form._imageTypeDropdown.Enabled)
			Assert.IsTrue(_form._useAbsolutePaths.Enabled)
			Assert.AreEqual(CInt(kCura.WinEDDS.LoadFileType.FileFormat.Opticon), _form._imageFileFormat.SelectedValue)
		End Function
        'kCura.WinEDDS.ExportFile.ImageType.SinglePage
        <Test()> Public Async Function LoadExportFile_ExportImages_True_TypeOfImage_Unselected() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportImages = True, .TypeOfImage = Nothing, .VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With
			{.CopyImageFilesFromRepository = True, .CopyNativeFilesFromRepository = True}}
	        Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._exportImages.Checked)
			Assert.IsTrue(_form._imageFileFormat.Enabled)
			Assert.IsTrue(_form._imageTypeDropdown.Enabled)
			Assert.IsTrue(_form._useAbsolutePaths.Enabled)
			Assert.AreEqual(0, _form._imageTypeDropdown.SelectedIndex)
		End Function
		<Test()> Public Async Function LoadExportFile_ExportImages_True_TypeOfImage_SinglePage() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportImages = True, .TypeOfImage = kCura.WinEDDS.ExportFile.ImageType.SinglePage, .VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With
			{.CopyImageFilesFromRepository = True, .CopyNativeFilesFromRepository = True}}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._exportImages.Checked)
			Assert.IsTrue(_form._imageFileFormat.Enabled)
			Assert.IsTrue(_form._imageTypeDropdown.Enabled)
			Assert.IsTrue(_form._useAbsolutePaths.Enabled)
			Assert.AreEqual(1, _form._imageTypeDropdown.SelectedIndex)
		End Function
		<Test()> Public Async Function LoadExportFile_ExportImages_True_TypeOfImage_MultiPageTiff() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportImages = True, .TypeOfImage = WinEDDS.ExportFile.ImageType.MultiPageTiff, .VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With
			{.CopyImageFilesFromRepository = True, .CopyNativeFilesFromRepository = True}}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._exportImages.Checked)
			Assert.IsTrue(_form._imageFileFormat.Enabled)
			Assert.IsTrue(_form._imageTypeDropdown.Enabled)
			Assert.IsTrue(_form._useAbsolutePaths.Enabled)
			Assert.AreEqual(2, _form._imageTypeDropdown.SelectedIndex)
		End Function
		<Test()> Public Async Function LoadExportFile_ExportImages_True_TypeOfImage_Pdf() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportImages = True, .TypeOfImage = WinEDDS.ExportFile.ImageType.Pdf, .VolumeInfo = New kCura.WinEDDS.Exporters.VolumeInfo() With
			{.CopyImageFilesFromRepository = True, .CopyNativeFilesFromRepository = True}}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._exportImages.Checked)
			Assert.IsTrue(_form._imageFileFormat.Enabled)
			Assert.IsTrue(_form._imageTypeDropdown.Enabled)
			Assert.IsTrue(_form._useAbsolutePaths.Enabled)
			Assert.AreEqual(3, _form._imageTypeDropdown.SelectedIndex)
		End Function
#End Region

#Region "Native Section"

		<Test()> Public Async Function LoadExportFile_ExportNatives_False() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportNative = False}
			Await _form.LoadExportFile(ef)
			Assert.IsFalse(_form._exportNativeFiles.Checked)
			Assert.IsTrue(_form._useAbsolutePaths.Enabled)
			Assert.IsFalse(_form._nativeFileFormat.Enabled)
			Assert.IsFalse(_form._metadataGroupBox.Enabled)
		End Function


		<Test()> Public Async Function LoadExportFile_ExportNatives_False_RightListBoxItemsIsZero() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportNative = False, .SelectedViewFields = New kCura.WinEDDS.ViewFieldInfo() {}}
			Await _form.LoadExportFile(ef)
			Assert.IsFalse(_form._exportNativeFiles.Checked)
			Assert.IsTrue(_form._useAbsolutePaths.Enabled)
			Assert.IsFalse(_form._nativeFileFormat.Enabled)
			Assert.IsFalse(_form._metadataGroupBox.Enabled)
		End Function

		<Test()> Public Async Function LoadExportFile_ExportNatives_False_RightListBoxItemsIs1() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document)
			ef.ExportNative = False
			ef.SelectedViewFields = {_queryFieldFactory.GetExtractedTextField()}
			Await _form.LoadExportFile(ef)
			Assert.IsFalse(_form._exportNativeFiles.Checked)
			Assert.IsTrue(_form._useAbsolutePaths.Enabled)
			Assert.IsFalse(_form._nativeFileFormat.Enabled)
			Assert.IsFalse(_form._metadataGroupBox.Enabled)
		End Function

		<Test()> Public Async Function LoadExportFile_RightListBoxItemsIsZero_ExportNativesFalse_MetaDataDisabled() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document)
			ef.ExportNative = False
			ef.SelectedViewFields = New kCura.WinEDDS.ViewFieldInfo() {}
			Await _form.LoadExportFile(ef)
			Assert.IsFalse(_form._metadataGroupBox.Enabled)
		End Function

		<Test()> Public Async Function LoadExportFile_RightListBoxItemsIs1_ExportNativesTrue_MetaDataEnabled() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			 .AllExportableFields = _queryFieldFactory.GetAllDocumentFields(),
			 .ExportNative = True,
			 .SelectedViewFields = {_queryFieldFactory.GetExtractedTextField()}}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._metadataGroupBox.Enabled)
		End Function

		<Test()> Public Async Function LoadExportFile_RightListBoxItemsIs1_ExportNativesFalse_MetaDataDisabled() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.AllExportableFields = _queryFieldFactory.GetAllDocumentFields(),
				.ExportNative = False,
			 .SelectedViewFields = {_queryFieldFactory.GetExtractedTextField()}}
			Await _form.LoadExportFile(ef)
			Assert.IsFalse(_form._metadataGroupBox.Enabled)
		End Function

		<Test()> Public Async Function LoadExportFile_ExportNatives_True() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportNative = True}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._exportNativeFiles.Checked)
		End Function

#End Region

#Region "Metadata Section"

		<Test()> Public Async Function LoadExportFile_DataFileFormat_CustomTxt() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.LoadFileExtension = "txt"}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual("Custom (.txt)", _form._nativeFileFormat.SelectedItem.ToString)
		End Function

		<Test()> Public Async Function LoadExportFile_DataFileFormat_ConcordanceDat() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.LoadFileExtension = "dat"}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual("Concordance (.dat)", _form._nativeFileFormat.SelectedItem.ToString)
		End Function

		<Test()> Public Async Function LoadExportFile_DataFileFormat_CommaSeparatedCsv() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.LoadFileExtension = "csv"}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual("Comma-separated (.csv)", _form._nativeFileFormat.SelectedItem.ToString)
		End Function

		<Test()> Public Async Function LoadExportFile_DataFileFormat_IsHtml() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.LoadFileIsHtml = True}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual("HTML (.html)", _form._nativeFileFormat.SelectedItem.ToString)
		End Function

		<Test()> Public Async Function LoadExportFile_DataFileFormat_IsHtml_AlsoHasLoadFileExtensionField() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.LoadFileIsHtml = True, .LoadFileExtension = "txt"}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual("HTML (.html)", _form._nativeFileFormat.SelectedItem.ToString)
		End Function

		<Test()> Public Async Function LoadExportFile_DataFileEncoding_NotSelected() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document)
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(Nothing, _form._dataFileEncoding.SelectedEncoding)
		End Function

		<Test()> Public Async Function LoadExportFile_DataFileEncoding_UTF8() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.LoadFileEncoding = System.Text.Encoding.UTF8}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(System.Text.Encoding.UTF8, _form._dataFileEncoding.SelectedEncoding)
		End Function

		<Test()> Public Async Function LoadExportFile_ExportTextAsFiles_Checked_True() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportFullTextAsFile = True}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._exportFullTextAsFile.Checked)
		End Function

		<Test()> Public Async Function LoadExportFile_ExportTextAsFiles_Checked_False() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.ExportFullTextAsFile = False}
			Await _form.LoadExportFile(ef)
			Assert.IsFalse(_form._exportFullTextAsFile.Checked)
		End Function

		<Test()> Public Async Function LoadExportFile_TextFileEncoding_NotSelected() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document)
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(Nothing, _form._textFileEncoding.SelectedEncoding)
		End Function

		<Test()> Public Async Function LoadExportFile_TextFileEncoding_ASCII() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.TextFileEncoding = System.Text.Encoding.ASCII}
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(System.Text.Encoding.ASCII, _form._textFileEncoding.SelectedEncoding)
		End Function

		<Test()> Public Async Function LoadExportFile_TextField_NotSelected() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document)
			Await _form.LoadExportFile(ef)
			Assert.AreEqual(0, _form._textFieldPrecedencePicker.SelectedFields.Count)
		End Function

		<Test()> Public Async Function LoadExportFile_TextField_FieldSelected() As Task
			Me.ValidTextFieldSetup()
			Dim textField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(1)
			Dim boolField As kCura.WinEDDS.ViewFieldInfo = _queryFieldFactory.GetAllDocumentFields(2)
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {
			.AllExportableFields = _queryFieldFactory.GetAllDocumentFields,
			.SelectedViewFields = New WinEDDS.ViewFieldInfo() {textField, boolField},
			.SelectedTextFields = {textField}}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(textField.Equals(_form._textFieldPrecedencePicker.SelectedFields(0)))
		End Function

		<Test()> Public Async Function LoadExportFile_TextField_BeforeLoad_TwoPotential_AfterLoad_RenamedOnePotential_FirstSelected() As Task
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

			Await _form.LoadExportFile(ef)
			Assert.AreEqual(1, _form._textFieldPrecedencePicker.SelectedFields.Count)
			Assert.IsTrue(_form._textFieldPrecedencePicker.SelectedFields(0).Equals(extractedTextField))
		End Function

		<Test()> Public Async Function LoadExportFile_ExportMultiChoiceFieldsAsNested_Checked() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.MulticodesAsNested = True}
			Await _form.LoadExportFile(ef)
			Assert.IsTrue(_form._exportMulticodeFieldsAsNested.Checked)
		End Function

		<Test()> Public Async Function LoadExportFile_ExportMultiChoiceFieldsAsNested_NotChecked() As Task
			Dim ef As New kCura.WinEDDS.ExportFile(Relativity.ArtifactType.Document) With {.MulticodesAsNested = False}
			Await _form.LoadExportFile(ef)
			Assert.IsFalse(_form._exportMulticodeFieldsAsNested.Checked)
		End Function

#End Region


#Region "Helpers"
		Private Function DoesFieldExistsInListBox(ByVal field As kCura.WinEDDS.ViewFieldInfo, ByVal list As List(Of Object)) As Boolean
			Dim retVal As Boolean = False
			For Each avf As kCura.WinEDDS.ViewFieldInfo In list
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
