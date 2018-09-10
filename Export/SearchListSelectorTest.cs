using System.Data;
using NUnit.Framework;
using Specialized;

namespace kCura.WinEDDS.Core.NUnit.Export
{
	internal class SearchListSelectorTest : SearchListSelector
	{
		private DataTable _dt;

		private void AddRowsToDataTable(int numberOfRows, string prefix)
		{
			for (var i = 0; i < numberOfRows; i++)
			{
				_dt.Rows.Add(prefix + i, i);
			}
		}

		[SetUp]
		public void SetUp()
		{
			_dt = new DataTable();
			_dt.Columns.Add("Name", typeof(string));
			_dt.Columns.Add("ArtifactID", typeof(int));
		}

		[Test]
		public void ItShouldReturnNothingIfListIsEmpty()
		{
			System.Data.DataTable result = FilterRowsFromDataTable(_dt, "substr");
			Assert.AreEqual(0, result.Rows.Count);
		}

		[Test]
		public void ItShouldReturnNothingIfPatternDoesNotMatch()
		{
			const int amount = 4;
			AddRowsToDataTable(amount, "test");
			System.Data.DataTable result = FilterRowsFromDataTable(_dt, "substr");
			Assert.AreEqual(0, result.Rows.Count);
		}

		[Test]
		public void ItShouldReturnOneIfPatternMatchesOne()
		{
			const int amount = 4;
			AddRowsToDataTable(amount, "test");
			AddRowsToDataTable(1, "substr");
			System.Data.DataTable result = FilterRowsFromDataTable(_dt, "substr");
			Assert.AreEqual(1, result.Rows.Count);
		}

		[Test]
		public void ItShouldReturnMoreIfPatternMachesMoreThanOne()
		{
			const int amount = 4;
			AddRowsToDataTable(amount, "test");
			AddRowsToDataTable(amount, "substr");
			System.Data.DataTable result = FilterRowsFromDataTable(_dt, "substr");
			Assert.AreEqual(amount, result.Rows.Count);
		}

		[Test]
		public void ItShouldReturnAllIfPatternMatchesAll()
		{
			const int amount = 4;
			AddRowsToDataTable(amount, "substr");
			System.Data.DataTable result = FilterRowsFromDataTable(_dt, "substr");
			Assert.AreEqual(amount, result.Rows.Count);
		}

		[Test]
		public void ItShouldReturnAllIfPatternIsEmpty()
		{
			const int amount = 4;
			AddRowsToDataTable(amount, "substr");
			System.Data.DataTable result = FilterRowsFromDataTable(_dt, "");
			Assert.AreEqual(_dt, result);
		}

		[Test]
		public void ItShouldReturnIfPatternIsAnywhereInTheString()
		{
			AddRowsToDataTable(1, "Begining");
			AddRowsToDataTable(1, "Middle");
			AddRowsToDataTable(1, "End");

			DataTable[] dtArr =
			{
				FilterRowsFromDataTable(_dt, "Beg"),
				FilterRowsFromDataTable(_dt, "iddle"),
				FilterRowsFromDataTable(_dt, "nd")
			};
			foreach (var dataTable in dtArr)
			{
				Assert.AreEqual(1, dataTable.Rows.Count);
			}
		}

		[Test]
		public void ItShouldReturnAllNoMatterTheLettersAreCaps()
		{
			AddRowsToDataTable(1, "TEST");
			AddRowsToDataTable(1, "test");
			AddRowsToDataTable(1, "TeSt");
			const int count = 3;

			System.Data.DataTable result = FilterRowsFromDataTable(_dt, "test");
			Assert.AreEqual(count, result.Rows.Count);
		}
	}
}