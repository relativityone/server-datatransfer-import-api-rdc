Imports NUnit.Framework

Namespace kCura.EDDS.WinForm.Tests
	<TestFixture()>
	Public Class ExportFilterSelectForm
		Private _filtersDataTable As New DataTable()

		<SetUp()> Public Sub SetUp()
			_filtersDataTable = New DataTable
			_filtersDataTable.Columns.Add("Name", GetType(String))
			_filtersDataTable.Columns.Add("ArtifactID", GetType(Int32))
			_filtersDataTable.Rows.Add({"123", 123})
			_filtersDataTable.Rows.Add({"245", 245})
			_filtersDataTable.Rows.Add({"245", 666})
			_filtersDataTable.Rows.Add({"345", 345})
		End Sub

		<Test()>
		Public Sub NewFormAllItemsAreDisplayedNoneSelected()
			Dim form As New MockExportFilterSelectForm("IDONOTEXIST", "DOESNOTMATTER", _filtersDataTable)
			form.FakeLoad()
			Assert.AreEqual(4, form.ListOfItems.Items.Count)
			Assert.AreEqual(Nothing, form.SelectedItemArtifactIDs)
		End Sub


		<Test()>
		Public Sub NewFormAllItemsAreDisplayedOneSelected()
			Dim form As New MockExportFilterSelectForm("245", "DOESNOTMATTER", _filtersDataTable)
			form.Show()
			Assert.AreEqual(4, form.ListOfItems.Items.Count)
			Assert.AreEqual(1, form.SelectedItemArtifactIDs.Count)
			Assert.AreEqual(245, form.SelectedItemArtifactIDs.Item(0))
			form.Close()
		End Sub

		<Test()>
		Public Sub NewFormAllItemsAreDisplayedNoneSelectedOkButtonDisabled()
			Dim form As New MockExportFilterSelectForm("DOESNOTCOMPUTE", "DOESNOTMATTER", _filtersDataTable)
			form.Show()
			Assert.AreEqual(4, form.ListOfItems.Items.Count)
			Assert.AreEqual(Nothing, form.SelectedItemArtifactIDs)
			Assert.IsFalse(form.OkButtonEnabled)
			form.Close()
		End Sub

		<Test()>
		Public Sub NewFormAllItemsAreDisplayedOneSelectedOkButtonEnabled()
			Dim form As New MockExportFilterSelectForm("245", "DOESNOTMATTER", _filtersDataTable)
			form.Show()
			Assert.AreEqual(4, form.ListOfItems.Items.Count)
			Assert.AreEqual(1, form.SelectedItemArtifactIDs.Count)
			Assert.AreEqual(245, form.SelectedItemArtifactIDs.Item(0))
			Assert.IsTrue(form.OkButtonEnabled)
			form.Close()
		End Sub


		Private Class MockExportFilterSelectForm
			Inherits kCura.EDDS.WinForm.ExportFilterSelectForm

			Public ReadOnly Property OkButtonEnabled As Boolean
				Get
					Return ConfirmButton.Enabled
				End Get
			End Property


			Public Sub FakeLoad()
				ItemSelectForm_Load(Nothing, Nothing)
			End Sub

			Public ReadOnly Property ListOfItems As System.Windows.Forms.ListView
				Get
					Return ItemListView
				End Get

			End Property

			Public Sub New(ByVal savedItemNameToSelect As String, ByVal objectTypeName As String, ByVal listViewDataSource As DataTable)
				MyBase.New(savedItemNameToSelect, objectTypeName, listViewDataSource)
			End Sub
		End Class

	End Class
End Namespace

