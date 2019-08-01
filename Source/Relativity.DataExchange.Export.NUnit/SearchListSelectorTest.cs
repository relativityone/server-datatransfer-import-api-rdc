// -----------------------------------------------------------------------------------------------------
// <copyright file="SearchListSelectorTest.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Data;
	using System.Globalization;

	using global::NUnit.Framework;

	internal class SearchListSelectorTest : Relativity.Desktop.Client.SearchListSelector
	{
		private DataTable _dt;

		private void AddRowsToDataTable(int numberOfRows, string prefix)
		{
			for (var i = 0; i < numberOfRows; i++)
			{
				this._dt.Rows.Add(prefix + i, i);
			}
		}

		[SetUp]
		public void SetUp()
		{
			this._dt = new DataTable { Locale = CultureInfo.InvariantCulture };
			this._dt.Columns.Add("Name", typeof(string));
			this._dt.Columns.Add("ArtifactID", typeof(int));
		}

		[Test]
		public void ItShouldReturnNothingIfListIsEmpty()
		{
			System.Data.DataTable result = this.FilterRowsFromDataTable(this._dt, "substr");
			Assert.AreEqual(0, result.Rows.Count);
		}

		[Test]
		public void ItShouldReturnNothingIfPatternDoesNotMatch()
		{
			const int amount = 4;
			this.AddRowsToDataTable(amount, "test");
			System.Data.DataTable result = this.FilterRowsFromDataTable(this._dt, "substr");
			Assert.AreEqual(0, result.Rows.Count);
		}

		[Test]
		public void ItShouldReturnOneIfPatternMatchesOne()
		{
			const int amount = 4;
			this.AddRowsToDataTable(amount, "test");
			this.AddRowsToDataTable(1, "substr");
			System.Data.DataTable result = this.FilterRowsFromDataTable(this._dt, "substr");
			Assert.AreEqual(1, result.Rows.Count);
		}

		[Test]
		public void ItShouldReturnMoreIfPatternMatchesMoreThanOne()
		{
			const int amount = 4;
			this.AddRowsToDataTable(amount, "test");
			this.AddRowsToDataTable(amount, "substr");
			System.Data.DataTable result = this.FilterRowsFromDataTable(this._dt, "substr");
			Assert.AreEqual(amount, result.Rows.Count);
		}

		[Test]
		public void ItShouldReturnAllIfPatternMatchesAll()
		{
			const int amount = 4;
			this.AddRowsToDataTable(amount, "substr");
			System.Data.DataTable result = this.FilterRowsFromDataTable(this._dt, "substr");
			Assert.AreEqual(amount, result.Rows.Count);
		}

		[Test]
		public void ItShouldReturnAllIfPatternIsEmpty()
		{
			const int amount = 4;
			this.AddRowsToDataTable(amount, "substr");
			System.Data.DataTable result = this.FilterRowsFromDataTable(this._dt, string.Empty);
			Assert.AreEqual(this._dt, result);
		}

		[Test]
		public void ItShouldReturnIfPatternIsAnywhereInTheString()
		{
			this.AddRowsToDataTable(1, "Begining");
			this.AddRowsToDataTable(1, "Middle");
			this.AddRowsToDataTable(1, "End");

			DataTable[] dtArr =
			{
				this.FilterRowsFromDataTable(this._dt, "Beg"),
				this.FilterRowsFromDataTable(this._dt, "iddle"),
				this.FilterRowsFromDataTable(this._dt, "nd")
			};
			foreach (var dataTable in dtArr)
			{
				Assert.AreEqual(1, dataTable.Rows.Count);
			}
		}

		[Test]
		public void ItShouldReturnAllNoMatterTheLettersAreCaps()
		{
			this.AddRowsToDataTable(1, "TEST");
			this.AddRowsToDataTable(1, "test");
			this.AddRowsToDataTable(1, "TeSt");
			const int count = 3;

			System.Data.DataTable result = this.FilterRowsFromDataTable(this._dt, "test");
			Assert.AreEqual(count, result.Rows.Count);
		}
	}
}