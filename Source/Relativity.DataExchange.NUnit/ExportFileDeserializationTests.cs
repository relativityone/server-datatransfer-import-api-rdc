// -----------------------------------------------------------------------------------------------------
// <copyright file="ExportFileDeserializationTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="ProcessErrorWriter"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System.Collections.Generic;
	using System.IO;

	using global::NUnit.Framework;

	using kCura.WinEDDS;

	using Relativity.DataExchange;
	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.TestFramework;

	[TestFixture]
	public class ExportFileDeserializationTests
	{
		private static IEnumerable<string> FileNames => Directory.GetFiles(ResourceFileHelper.GetResourceFolderDirectory("ExportFile"), "*.kwx");

		[Test]
		[TestCaseSource(nameof(FileNames))]
		[Category(TestCategories.SeparateDomain)]
		public void DeserializeExportFile(string filename)
		{
			var exportFile = SerializationHelper.DeserializeFromSoapFile<ExportFile>(filename);

			Assert.AreEqual(1038052, exportFile.ArtifactID);
			Assert.AreEqual(92, exportFile.NestedValueDelimiter);
			Assert.AreEqual(ExportFile.ExportType.ArtifactSearch, exportFile.TypeOfExport);
			Assert.AreEqual(0, exportFile.ViewID);
			Assert.IsFalse(exportFile.Overwrite);
			Assert.AreEqual(44, exportFile.RecordDelimiter);
			Assert.AreEqual(34, exportFile.QuoteDelimiter);
			Assert.AreEqual(10, exportFile.NewlineDelimiter);
			Assert.AreEqual(59, exportFile.MultiRecordDelimiter);
			Assert.IsFalse(exportFile.ExportFullText);
			Assert.IsTrue(exportFile.ExportFullTextAsFile);
			Assert.IsTrue(exportFile.ExportNative);
			Assert.IsTrue(exportFile.RenameFilesToIdentifier);
			Assert.IsTrue(exportFile.ExportImages);
			Assert.AreEqual(ExportNativeWithFilenameFrom.Identifier, exportFile.ExportNativesToFileNamedFrom);
			Assert.AreEqual(ExportFile.ExportedFilePathType.Relative, exportFile.TypeOfExportedFilePath);
			Assert.IsTrue(exportFile.AppendOriginalFileName);
			Assert.IsFalse(exportFile.LoadFileIsHtml);
			Assert.IsTrue(exportFile.MulticodesAsNested);
			Assert.AreEqual(65001, exportFile.LoadFileEncoding.CodePage);
			Assert.AreEqual(65001, exportFile.TextFileEncoding.CodePage);
			Assert.AreEqual(2, exportFile.VolumeDigitPadding);
			Assert.AreEqual(3, exportFile.SubdirectoryDigitPadding);
			Assert.AreEqual(0, exportFile.StartAtDocumentNumber);
			Assert.IsNull(exportFile.SelectedTextFields);
			Assert.IsFalse(exportFile.UseCustomFileNaming);
			Assert.IsNull(exportFile.CustomFileNaming);
			Assert.AreEqual(1003667, exportFile.SelectedViewFields[0].FieldArtifactId);
			Assert.AreEqual(1000186, exportFile.SelectedViewFields[0].AvfId);
			Assert.AreEqual(FieldCategory.Identifier, exportFile.SelectedViewFields[0].Category);
			Assert.AreEqual((FieldCategory)(-1), exportFile.SelectedViewFields[0].ConnectorFieldCategory);
			Assert.AreEqual(ColumnSourceType.MainTable, exportFile.SelectedViewFields[0].ColumnSource);
			Assert.AreEqual(-1, exportFile.SelectedViewFields[0].SourceFieldArtifactTypeID);
			Assert.AreEqual(-1, exportFile.SelectedViewFields[0].SourceFieldArtifactID);
			Assert.AreEqual(-1, exportFile.SelectedViewFields[0].ConnectorFieldArtifactID);
			Assert.AreEqual(FieldType.Varchar, exportFile.SelectedViewFields[0].FieldType);
			Assert.IsTrue(exportFile.SelectedViewFields[0].IsLinked);
			Assert.AreEqual(-1, exportFile.SelectedViewFields[0].FieldCodeTypeID);
			Assert.AreEqual(10, exportFile.SelectedViewFields[0].ArtifactTypeID);
			Assert.IsFalse(exportFile.SelectedViewFields[0].FieldIsArtifactBaseField);
			Assert.IsTrue(exportFile.SelectedViewFields[0].IsUnicodeEnabled);
			Assert.IsFalse(exportFile.SelectedViewFields[0].AllowHtml);
			Assert.AreEqual(-1, exportFile.SelectedViewFields[0].ParentFileFieldArtifactID);
			Assert.AreEqual(-1, exportFile.SelectedViewFields[0].AssociativeArtifactTypeID);
			Assert.AreEqual(ParentReflectionType.Empty, exportFile.SelectedViewFields[0].ParentReflectionType);
			Assert.IsFalse(exportFile.SelectedViewFields[0].EnableDataGrid);
			Assert.IsFalse(exportFile.SelectedViewFields[0].IsVirtualAssociativeArtifactType);
		}
	}
}
