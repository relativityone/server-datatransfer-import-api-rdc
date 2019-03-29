﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="LongTextRepositoryTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
{
    using System.Collections.Generic;
    using System.Text;

    using global::NUnit.Framework;

    using kCura.WinEDDS.Exporters;

    using Moq;

	using Relativity.Export.VolumeManagerV2;
	using Relativity.Export.VolumeManagerV2.Download;
	using Relativity.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.Export.VolumeManagerV2.Repository;
	using Relativity.Import.Export;
    using Relativity.Import.Export.Io;
    using Relativity.Logging;

    [TestFixture]
	public class LongTextRepositoryTests
	{
		private LongTextRepository _instance;

		private IList<LongText> _longTexts;

		private Mock<IFile> _fileHelper;

		[SetUp]
		public void SetUp()
		{
			_longTexts = CreateDataSet();

			_fileHelper = new Mock<IFile>();

			_instance = new LongTextRepository(_fileHelper.Object, new NullLogger());
			_instance.Add(_longTexts);
		}

		[Test]
		public void ItShouldSearchLongTextByArtifactIdAndFieldArtifactId()
		{
			// ACT
			LongText image1 = _instance.GetLongText(1, 10);
			LongText image2 = _instance.GetLongText(1, 20);
			LongText image3 = _instance.GetLongText(2, 30);

			// ASSERT
			Assert.That(image1, Is.EqualTo(_longTexts[0]));
			Assert.That(image2, Is.EqualTo(_longTexts[1]));
			Assert.That(image3, Is.EqualTo(_longTexts[2]));
		}

		[Test]
		public void ItShouldGetLongTextsForArtifact()
		{
			// ACT
			IEnumerable<LongText> images1 = _instance.GetArtifactLongTexts(1);
			IEnumerable<LongText> images2 = _instance.GetArtifactLongTexts(2);

			// ASSERT
			CollectionAssert.AreEquivalent(_longTexts.GetRange(0, 2), images1);
			CollectionAssert.AreEquivalent(_longTexts[2].InList(), images2);
		}

		[Test]
		public void ItShouldGetAllImages()
		{
			// ACT
			IList<LongText> images = _instance.GetLongTexts();

			// ASSERT
			CollectionAssert.AreEquivalent(_longTexts, images);
		}

		[Test]
		public void ItShouldGetExportRequests()
		{
			var expectedExportRequests = new List<ExportRequest>
			{
				_longTexts[0].ExportRequest,
				_longTexts[2].ExportRequest
			};

			// ACT
			IEnumerable<LongTextExportRequest> exportRequests = _instance.GetExportRequests();

			// ASSERT
			CollectionAssert.AreEquivalent(expectedExportRequests, exportRequests);
		}

		[Test]
		public void ItShouldGetLongTextByUniqueId()
		{
			// ACT
			LongText image1 = _instance.GetByLineNumber(1);
			LongText image2 = _instance.GetByLineNumber(2);
			LongText image3 = _instance.GetByLineNumber(3);

			// ASSERT
			Assert.That(image1, Is.EqualTo(_longTexts[0]));
			Assert.That(image2, Is.Null);
			Assert.That(image3, Is.EqualTo(_longTexts[2]));
		}

		[Test]
		public void ItShouldClearRepository()
		{
			// ACT
			_instance.Clear();
			IList<LongText> images = _instance.GetLongTexts();

			// ASSERT
			CollectionAssert.IsEmpty(images);
			_fileHelper.Verify(x => x.Delete("require_deletion"), Times.Once);
			_fileHelper.Verify(x => x.Delete("do_not_require_deletion"), Times.Never);
		}

		private List<LongText> CreateDataSet()
		{
			ObjectExportInfo artifact1 = new ObjectExportInfo
			{
				ArtifactID = 1
			};

			LongText longText1 =
				LongText.CreateFromMissingValue(artifact1.ArtifactID, 10, LongTextExportRequest.CreateRequestForLongText(artifact1, 10, "require_deletion"), Encoding.Default);
			longText1.ExportRequest.Order = 1;

			LongText longText2 = LongText.CreateFromExistingValue(artifact1.ArtifactID, 20, "text");

			ObjectExportInfo artifact2 = new ObjectExportInfo
			{
				ArtifactID = 2
			};

			LongText longText3 = LongText.CreateFromMissingFile(
				artifact2.ArtifactID,
				30,
				LongTextExportRequest.CreateRequestForFullText(artifact2, 30, "do_not_require_deletion"),
				Encoding.Default,
				Encoding.Default);
			longText3.ExportRequest.Order = 3;

			return new List<LongText> { longText1, longText2, longText3 };
		}
	}
}