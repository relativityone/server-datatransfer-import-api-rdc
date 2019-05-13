// -----------------------------------------------------------------------------------------------------
// <copyright file="ImageLoadFileDeserializationTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="ProcessErrorWriter"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Import.Export.NUnit
{
	using System.Collections.Generic;
	using System.IO;

	using global::NUnit.Framework;

	using kCura.WinEDDS;

	using Relativity.Import.Export;
	using Relativity.Import.Export.TestFramework;

	[TestFixture]
	public class ImageLoadFileDeserializationTests
	{
		private static IEnumerable<string> FileNames => Directory.GetFiles(ResourceFileHelper.GetResourceFolderDirectory("ImageLoadFile"), "*.kwi");

		[Test]
		[TestCaseSource(nameof(FileNames))]
		[Category(TestCategories.SeparateDomain)]
		public void DeserializeImageLoadFile(string filename)
		{
			var imageLoadFile = SerializationHelper.DeserializeFromSoapFile<ImageLoadFile>(filename);

			Assert.AreEqual(1003697, imageLoadFile.DestinationFolderID);
			Assert.AreEqual(@"S:\100M\qqqq\www_export.opt", imageLoadFile.FileName);
			Assert.AreEqual("Append", imageLoadFile.Overwrite);
			Assert.AreEqual("Control Number", imageLoadFile.ControlKeyField);
			Assert.IsFalse(imageLoadFile.ReplaceFullText);
			Assert.IsFalse(imageLoadFile.ForProduction);
			Assert.IsFalse(imageLoadFile.AutoNumberImages);
			Assert.IsNull(imageLoadFile.ProductionTable);
			Assert.AreEqual(0, imageLoadFile.ProductionArtifactID);
			Assert.AreEqual(0, imageLoadFile.BeginBatesFieldArtifactID);
			Assert.IsNull(imageLoadFile.FullTextEncoding);
			Assert.AreEqual(0, imageLoadFile.StartLineNumber);
			Assert.AreEqual(1003667, imageLoadFile.IdentityFieldId);
		}
	}
}
