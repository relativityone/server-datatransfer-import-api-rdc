// -----------------------------------------------------------------------------------------------------
// <copyright file="ChoiceInfoTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit
{
	using System.Data;
	using System.Globalization;

	using global::NUnit.Framework;

	using Relativity.Import.Export.Service;

	[TestFixture]
	public static class ChoiceInfoTests
	{
		[Test]
		public static void ShouldMapTheDataRow()
		{
			using (DataTable table = new DataTable())
			{
				table.Locale = CultureInfo.CurrentCulture;
				table.Columns.Add("ArtifactID", typeof(int));
				table.Columns.Add("CodeTypeID", typeof(int));
				table.Columns.Add("Name", typeof(string));
				table.Columns.Add("Order", typeof(int));
				table.Columns.Add("ParentArtifactID", typeof(int));
				DataRow row = table.NewRow();
				row["ArtifactID"] = 999;
				row["CodeTypeID"] = 1999;
				row["Name"] = "Test";
				row["Order"] = 5;
				row["ParentArtifactID"] = 2;
				table.Rows.Add(row);
				ChoiceInfo expected = new ChoiceInfo(row);
				ValidatePropertyValues(expected);
			}
		}

		private static void ValidatePropertyValues(ChoiceInfo actual)
		{
			Assert.That(actual.ArtifactID, Is.EqualTo(999));
			Assert.That(actual.CodeTypeID, Is.EqualTo(1999));
			Assert.That(actual.Name, Is.EqualTo("Test"));
			Assert.That(actual.Order, Is.EqualTo(5));
			Assert.That(actual.ParentArtifactID, Is.EqualTo(2));
		}
	}
}