// -----------------------------------------------------------------------------------------------------
// <copyright file="SqlDataViewTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="SqlDataView"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit
{
	using System;
	using System.Data;
	using System.Globalization;
	using System.IO;
	using System.Runtime.Serialization;
	using System.Runtime.Serialization.Formatters.Binary;

	using global::NUnit.Framework;

	using Relativity.Import.Export.Data;
	using Relativity.Import.Export.TestFramework;

	/// <summary>
	/// Represents <see cref="SqlDataView"/> tests.
	/// </summary>
	[TestFixture]
	public static class SqlDataViewTests
	{
		[Test]
		public static void ShouldThrowWhenTheConstructorArgsAreInvalid()
		{
			Assert.Throws<ArgumentNullException>(
				() =>
					{
						DataSet dataset = null;
						SqlDataView view = new SqlDataView(dataset);
						Assert.That(view, Is.Null);
					});
			Assert.Throws<ArgumentException>(
				() =>
					{
						using (DataSet dataset = new DataSet())
						{
							dataset.Locale = CultureInfo.CurrentCulture;
							SqlDataView view = new SqlDataView(dataset);
							Assert.That(view, Is.Not.Null);
						}
					});
			Assert.Throws<ArgumentNullException>(
				() =>
					{
						DataTable table = null;
						SqlDataView view = new SqlDataView(table);
						Assert.That(view, Is.Null);
					});
		}

		[Test]
		[Category(TestCategories.Framework)]
		public static void ShouldVerifyTheEnumerableInterface()
		{
			using (DataTable table = new DataTable())
			{
				table.Locale = CultureInfo.CurrentCulture;
				table.Columns.Add("Name", typeof(string));
				table.Rows.Add("abc");
				SqlDataView view = new SqlDataView(table);
				Assert.That(view.Table, Is.EqualTo(table));
				Assert.That(view.Count, Is.EqualTo(1));
				SqlDataRow sqlDataRow = view[0];
				Assert.That(sqlDataRow, Is.Not.Null);
				Assert.That(sqlDataRow[0], Is.EqualTo("abc"));
				foreach (DataRowView dataRow in view)
				{
					Assert.That(dataRow, Is.Not.Null);
				}
			}
		}

		[Test]
		public static void ShouldSerializeAndDeserializeTheSqlDataView()
		{
			IFormatter formatter = new BinaryFormatter();
			using (MemoryStream stream = new MemoryStream())
			{
				using (DataTable table = new DataTable())
				{
					table.Locale = CultureInfo.CurrentCulture;
					table.Columns.Add("Name", typeof(string));
					table.Rows.Add("abc");
					SqlDataView dataView = new SqlDataView(table);
					formatter.Serialize(stream, dataView);
					stream.Seek(0, SeekOrigin.Begin);
					SqlDataView deserializedDataView = (SqlDataView)formatter.Deserialize(stream);
					Assert.IsNotNull(deserializedDataView);
					Assert.That(dataView.Table.Rows.Count, Is.EqualTo(1));
				}
			}
		}
	}
}