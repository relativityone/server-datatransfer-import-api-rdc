// -----------------------------------------------------------------------------------------------------
// <copyright file="LoadFileDeserializationTests.cs" company="Relativity ODA LLC">
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
	using Relativity.DataExchange.TestFramework;

	[TestFixture]
	public class LoadFileDeserializationTests
	{
		private static IEnumerable<string> FileNames => Directory.GetFiles(ResourceFileHelper.GetResourceFolderDirectory("LoadFile"), "*.kwe");

		[Test]
		[TestCaseSource(nameof(FileNames))]
		[Category(TestCategories.SeparateDomain)]
		public void DeserializeLoadFile(string filename)
		{
			var loadFile = SerializationHelper.DeserializeFromSoapFile<LoadFile>(filename);

			Assert.AreEqual(@"S:\100M\loadFile.csv", loadFile.FilePath);
			Assert.IsTrue(loadFile.FirstLineContainsHeaders);
			Assert.IsFalse(loadFile.LoadNativeFiles);
			Assert.IsNull(loadFile.OverlayBehavior);
			Assert.AreEqual(44, loadFile.RecordDelimiter);
			Assert.AreEqual(34, loadFile.QuoteDelimiter);
			Assert.AreEqual(174, loadFile.NewlineDelimiter);
			Assert.AreEqual(59, loadFile.MultiRecordDelimiter);
			Assert.AreEqual("None", loadFile.OverwriteDestination);
			Assert.IsNull(loadFile.NativeFilePathColumn);
			Assert.IsNull(loadFile.FolderStructureContainedInColumn);
			Assert.IsFalse(loadFile.CreateFolderStructure);
			Assert.IsFalse(loadFile.FullTextColumnContainsFileLocation);
			Assert.IsNull(loadFile.LongTextColumnThatContainsPathToFullText);
			Assert.IsNull(loadFile.GroupIdentifierColumn);
			Assert.IsNull(loadFile.DataGridIDColumn);
			Assert.AreEqual(92, loadFile.HierarchicalValueDelimiter);
			Assert.AreEqual(65001, loadFile.SourceFileEncoding.CodePage);
			Assert.AreEqual(10, loadFile.ArtifactTypeID);
			Assert.AreEqual(0, loadFile.StartLineNumber);
			Assert.AreEqual(1003667, loadFile.IdentityFieldId);
			Assert.IsFalse(loadFile.SendEmailOnLoadCompletion);
			Assert.IsTrue(loadFile.ForceFolderPreview);
			Assert.AreEqual(1003667, loadFile.FieldMap.DocumentFields[0].FieldID);
			Assert.AreEqual(0, loadFile.FieldMap.DocumentFields[0].FieldTypeID);
			Assert.IsNull(loadFile.FieldMap.DocumentFields[0].Value);
			Assert.AreEqual(2, loadFile.FieldMap.DocumentFields[0].FieldCategoryID);
			Assert.AreEqual(0, loadFile.FieldMap.DocumentFields[0].FileColumnIndex);
			Assert.AreEqual(1, loadFile.FieldMap[0].NativeFileColumnIndex);
			Assert.AreEqual(2, loadFile.FieldMap[1].NativeFileColumnIndex);
			Assert.AreEqual(1003668, loadFile.FieldMap.DocumentFields[1].FieldID);
			Assert.AreEqual(4, loadFile.FieldMap.DocumentFields[1].FieldTypeID);
			Assert.IsNull(loadFile.FieldMap.DocumentFields[1].Value);
			Assert.AreEqual(1, loadFile.FieldMap.DocumentFields[1].FieldCategoryID);
			Assert.AreEqual(0, loadFile.FieldMap.DocumentFields[1].FileColumnIndex);
		}
	}
}
