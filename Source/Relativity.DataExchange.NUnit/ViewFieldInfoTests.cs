// -----------------------------------------------------------------------------------------------------
// <copyright file="ViewFieldInfoTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System.Data;
	using System.Globalization;

	using global::NUnit.Framework;

	using Relativity.DataExchange.Service;

	[TestFixture]
	public class ViewFieldInfoTests : SerializationTestsBase
	{
		[Test]
		public static void ShouldSerializeAndDeserializeTheObject()
		{
			ViewFieldInfo expected = new ViewFieldInfo
				                         {
					                         AllowFieldName = "Control",
					                         AllowHtml = true,
					                         ArtifactTypeID = 9,
					                         ArtifactTypeTableName = "Document",
					                         AssociativeArtifactTypeID = 2,
					                         AvfColumnName = "Path",
					                         AvfHeaderName = "Header",
					                         AvfId = 99,
					                         Category = FieldCategory.Identifier,
					                         ColumnSource = ColumnSourceType.MainTable,
					                         ConnectorFieldArtifactID = 999,
					                         ConnectorFieldCategory = FieldCategory.FileInfo,
					                         ConnectorFieldName = "Size",
					                         DataSource = "MySource",
					                         DisplayName = "My Display",
					                         EnableDataGrid = true,
					                         FieldArtifactId = 9999,
					                         FieldIsArtifactBaseField = true,
					                         FieldCodeTypeID = 500,
					                         FieldType = FieldType.Integer,
					                         FormatString = "format",
					                         IsLinked = true,
					                         IsUnicodeEnabled = true,
					                         IsVirtualAssociativeArtifactType = true,
					                         ParentFileFieldArtifactID = 600,
					                         ParentFileFieldDisplayName = "Parent",
					                         ParentReflectionType = ParentReflectionType.Parent,
					                         ReflectedConnectorIdentifierColumnName = "FilePath",
					                         ReflectedFieldArtifactTypeTableName = "Transfer",
					                         ReflectedFieldConnectorFieldName = "IsProcessed",
					                         ReflectedFieldIdentifierColumnName = "IsImaged",
					                         RelationalTableColumnName = "IsDate",
					                         RelationalTableColumnName2 = "IsDocument",
					                         RelationalTableName = "TransferSummary",
					                         SourceFieldArtifactID = 700,
					                         SourceFieldArtifactTypeID = 705,
					                         SourceFieldArtifactTypeTableName = "TransferDetail",
					                         SourceFieldName = "Date"
				                         };
			ViewFieldInfo actual = BinarySerialize(expected) as ViewFieldInfo;
			Assert.That(actual, Is.Not.Null);
			ValidatePropertyValues(actual);
			actual = SoapSerialize(expected) as ViewFieldInfo;
			Assert.That(actual, Is.Not.Null);
			ValidatePropertyValues(actual);
		}

		[Test]
		public static void ShouldMapTheDataRow()
		{
			using (DataTable table = new DataTable())
			{
				table.Locale = CultureInfo.CurrentCulture;
				table.Columns.Add("AllowFieldName", typeof(string));
				table.Columns.Add("AllowHtml", typeof(bool));
				table.Columns.Add("ArtifactTypeID", typeof(int));
				table.Columns.Add("ArtifactTypeTableName", typeof(string));
				table.Columns.Add("AssociativeArtifactTypeID", typeof(int));
				table.Columns.Add("AvfColumnName", typeof(string));
				table.Columns.Add("AvfHeaderName", typeof(string));
				table.Columns.Add("AvfID", typeof(int));
				table.Columns.Add("ColumnSource", typeof(ColumnSourceType));
				table.Columns.Add("ConnectorFieldArtifactID", typeof(int));
				table.Columns.Add("ConnectorFieldCategoryID", typeof(FieldCategory));
				table.Columns.Add("ConnectorFieldName", typeof(string));
				table.Columns.Add("DataSource", typeof(string));
				table.Columns.Add("DisplayName", typeof(string));
				table.Columns.Add("EnableDataGrid", typeof(bool));
				table.Columns.Add("FieldArtifactID", typeof(int));
				table.Columns.Add("FieldCategoryID", typeof(FieldCategory));
				table.Columns.Add("FieldCodeTypeID", typeof(int));
				table.Columns.Add("FieldIsArtifactBaseField", typeof(bool));
				table.Columns.Add("FieldTypeID", typeof(FieldType));
				table.Columns.Add("FormatString", typeof(string));
				table.Columns.Add("IsLinked", typeof(bool));
				table.Columns.Add("IsUnicodeEnabled", typeof(bool));
				table.Columns.Add("IsVirtualAssociativeArtifactType", typeof(bool));
				table.Columns.Add("ParentFileFieldArtifactID", typeof(int));
				table.Columns.Add("ParentFileFieldDisplayName", typeof(string));
				table.Columns.Add("ParentReflectionType", typeof(ParentReflectionType));
				table.Columns.Add("ReflectedConnectorArtifactTypeIdentifierColumnName", typeof(string));
				table.Columns.Add("ReflectedFieldArtifactTypeIdentifierColumnName", typeof(string));
				table.Columns.Add("ReflectedFieldArtifactTypeTableName", typeof(string));
				table.Columns.Add("ReflectedFieldArtifactTypeConnectorFieldName", typeof(string));
				table.Columns.Add("RelationalTableName", typeof(string));
				table.Columns.Add("RelationalTableColumnName", typeof(string));
				table.Columns.Add("RelationalTableColumnName2", typeof(string));
				table.Columns.Add("SourceFieldDisplayName", typeof(string));
				table.Columns.Add("SourceFieldArtifactID", typeof(int));
				table.Columns.Add("SourceFieldArtifactTypeID", typeof(int));
				table.Columns.Add("SourceFieldArtifactTypeTableName", typeof(string));
				DataRow row = table.NewRow();
				row["AllowFieldName"] = "Control";
				row["AllowHtml"] = true;
				row["ArtifactTypeID"] = 9;
				row["ArtifactTypeTableName"] = "Document";
				row["AssociativeArtifactTypeID"] = 2;
				row["AvfColumnName"] = "Path";
				row["AvfHeaderName"] = "Header";
				row["AvfID"] = 99;
				row["ColumnSource"] = ColumnSourceType.MainTable;
				row["ConnectorFieldArtifactID"] = 999;
				row["ConnectorFieldCategoryID"] = FieldCategory.FileInfo;
				row["ConnectorFieldName"] = "Size";
				row["DataSource"] = "MySource";
				row["DisplayName"] = "My Display";
				row["EnableDataGrid"] = true;
				row["FieldArtifactID"] = 9999;
				row["FieldCategoryID"] = FieldCategory.Identifier;
				row["FieldCodeTypeID"] = 500;
				row["FieldIsArtifactBaseField"] = true;
				row["FieldTypeID"] = FieldType.Integer;
				row["FormatString"] = "format";
				row["IsLinked"] = true;
				row["IsUnicodeEnabled"] = true;
				row["IsVirtualAssociativeArtifactType"] = true;
				row["ParentFileFieldArtifactID"] = 600;
				row["ParentFileFieldDisplayName"] = "Parent";
				row["ParentReflectionType"] = ParentReflectionType.Parent;
				row["ReflectedConnectorArtifactTypeIdentifierColumnName"] = "FilePath";
				row["ReflectedFieldArtifactTypeTableName"] = "Transfer";
				row["ReflectedFieldArtifactTypeConnectorFieldName"] = "IsProcessed";
				row["ReflectedFieldArtifactTypeIdentifierColumnName"] = "IsImaged";
				row["RelationalTableName"] = "TransferSummary";
				row["RelationalTableColumnName"] = "IsDate";
				row["RelationalTableColumnName2"] = "IsDocument";
				row["SourceFieldDisplayName"] = "Date";
				row["SourceFieldArtifactID"] = 700;
				row["SourceFieldArtifactTypeID"] = 705;
				row["SourceFieldArtifactTypeTableName"] = "TransferDetail";
				table.Rows.Add(row);
				ViewFieldInfo actual = new ViewFieldInfo(row);
				ValidatePropertyValues(actual);
			}
		}

		private static void ValidatePropertyValues(ViewFieldInfo actual)
		{
			Assert.That(actual.AllowFieldName, Is.EqualTo("Control"));
			Assert.That(actual.AllowHtml, Is.True);
			Assert.That(actual.ArtifactTypeID, Is.EqualTo(9));
			Assert.That(actual.ArtifactTypeTableName, Is.EqualTo("Document"));
			Assert.That(actual.AssociativeArtifactTypeID, Is.EqualTo(2));
			Assert.That(actual.AvfColumnName, Is.EqualTo("Path"));
			Assert.That(actual.AvfHeaderName, Is.EqualTo("Header"));
			Assert.That(actual.AvfId, Is.EqualTo(99));
			Assert.That(actual.Category, Is.EqualTo(FieldCategory.Identifier));
			Assert.That(actual.ColumnSource, Is.EqualTo(ColumnSourceType.MainTable));
			Assert.That(actual.ConnectorFieldArtifactID, Is.EqualTo(999));
			Assert.That(actual.ConnectorFieldCategory, Is.EqualTo(FieldCategory.FileInfo));
			Assert.That(actual.ConnectorFieldName, Is.EqualTo("Size"));
			Assert.That(actual.DataSource, Is.EqualTo("MySource"));
			Assert.That(actual.DisplayName, Is.EqualTo("My Display"));
			Assert.That(actual.EnableDataGrid, Is.True);
			Assert.That(actual.FieldArtifactId, Is.EqualTo(9999));
			Assert.That(actual.FieldCodeTypeID, Is.EqualTo(500));
			Assert.That(actual.FieldType, Is.EqualTo(FieldType.Integer));
			Assert.That(actual.FieldIsArtifactBaseField, Is.True);
			Assert.That(actual.FormatString, Is.EqualTo("format"));
			Assert.That(actual.IsLinked, Is.True);
			Assert.That(actual.IsUnicodeEnabled, Is.True);
			Assert.That(actual.IsVirtualAssociativeArtifactType, Is.True);
			Assert.That(actual.ParentFileFieldArtifactID, Is.EqualTo(600));
			Assert.That(actual.ParentFileFieldDisplayName, Is.EqualTo("Parent"));
			Assert.That(actual.ParentReflectionType, Is.EqualTo(ParentReflectionType.Parent));
			Assert.That(actual.ReflectedConnectorIdentifierColumnName, Is.EqualTo("FilePath"));
			Assert.That(actual.ReflectedFieldArtifactTypeTableName, Is.EqualTo("Transfer"));
			Assert.That(actual.ReflectedFieldConnectorFieldName, Is.EqualTo("IsProcessed"));
			Assert.That(actual.ReflectedFieldIdentifierColumnName, Is.EqualTo("IsImaged"));
			Assert.That(actual.RelationalTableColumnName, Is.EqualTo("IsDate"));
			Assert.That(actual.RelationalTableColumnName2, Is.EqualTo("IsDocument"));
			Assert.That(actual.RelationalTableName, Is.EqualTo("TransferSummary"));
			Assert.That(actual.SourceFieldArtifactID, Is.EqualTo(700));
			Assert.That(actual.SourceFieldArtifactTypeID, Is.EqualTo(705));
			Assert.That(actual.SourceFieldArtifactTypeTableName, Is.EqualTo("TransferDetail"));
			Assert.That(actual.SourceFieldName, Is.EqualTo("Date"));
		}
	}
}