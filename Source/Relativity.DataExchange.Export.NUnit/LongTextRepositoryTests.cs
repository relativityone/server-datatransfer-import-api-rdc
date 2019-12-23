// -----------------------------------------------------------------------------------------------------
// <copyright file="LongTextRepositoryTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Collections.Generic;
	using System.Text;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange;
	using Relativity.DataExchange.Export.VolumeManagerV2;
	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.DataExchange.Io;
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
			this._longTexts = this.CreateDataSet();

			this._fileHelper = new Mock<IFile>();

			this._instance = new LongTextRepository(this._fileHelper.Object, new NullLogger());
			this._instance.Add(this._longTexts);
		}

		[Test]
		public void ItShouldSearchLongTextByArtifactIdAndFieldArtifactId()
		{
			// ACT
			LongText image1 = this._instance.GetLongText(1, 10);
			LongText image2 = this._instance.GetLongText(1, 20);
			LongText image3 = this._instance.GetLongText(2, 30);

			// ASSERT
			Assert.That(image1, Is.EqualTo(this._longTexts[0]));
			Assert.That(image2, Is.EqualTo(this._longTexts[1]));
			Assert.That(image3, Is.EqualTo(this._longTexts[2]));
		}

		[Test]
		public void ItShouldGetLongTextsForArtifact()
		{
			// ACT
			IEnumerable<LongText> images1 = this._instance.GetArtifactLongTexts(1);
			IEnumerable<LongText> images2 = this._instance.GetArtifactLongTexts(2);

			// ASSERT
			CollectionAssert.AreEquivalent(this._longTexts.GetRange(0, 2), images1);
			CollectionAssert.AreEquivalent(this._longTexts[2].InList(), images2);
		}

		[Test]
		public void ItShouldGetAllImages()
		{
			// ACT
			IList<LongText> images = this._instance.GetLongTexts();

			// ASSERT
			CollectionAssert.AreEquivalent(this._longTexts, images);
		}

		[Test]
		public void ItShouldGetExportRequests()
		{
			var expectedExportRequests = new List<ExportRequest>
			{
				this._longTexts[0].ExportRequest,
				this._longTexts[2].ExportRequest
			};

			// ACT
			IEnumerable<LongTextExportRequest> exportRequests = this._instance.GetExportRequests();

			// ASSERT
			CollectionAssert.AreEquivalent(expectedExportRequests, exportRequests);
		}

		[Test]
		public void ItShouldGetLongTextByUniqueId()
		{
			// ACT
			LongText image1 = this._instance.GetByLineNumber(1);
			LongText image2 = this._instance.GetByLineNumber(2);
			LongText image3 = this._instance.GetByLineNumber(3);

			// ASSERT
			Assert.That(image1, Is.EqualTo(this._longTexts[0]));
			Assert.That(image2, Is.Null);
			Assert.That(image3, Is.EqualTo(this._longTexts[2]));
		}

		[Test]
		public void ItShouldClearRepository()
		{
			// ACT
			this._instance.Clear();
			IList<LongText> images = this._instance.GetLongTexts();

			// ASSERT
			CollectionAssert.IsEmpty(images);
			this._fileHelper.Verify(x => x.Delete("require_deletion"), Times.Once);
			this._fileHelper.Verify(x => x.Delete("do_not_require_deletion"), Times.Never);
		}

		[Test]
		public void ItShouldGetAnyRequestForLocation()
		{
			// ACT
			bool result1 = this._instance.AnyRequestForLocation("require_deletion");
			bool result1DiffCase = this._instance.AnyRequestForLocation("Require_Deletion");
			bool result2 = this._instance.AnyRequestForLocation("does-not-exist");
			bool result3 = this._instance.AnyRequestForLocation(null);
			bool result4 = this._instance.AnyRequestForLocation(string.Empty);
			bool result5 = this._instance.AnyRequestForLocation("do_not_require_deletion");
			bool result5DiffCase = this._instance.AnyRequestForLocation("Do_Not_Require_Deletion");

			// ASSERT
			Assert.That(result1, Is.True);
			Assert.That(result1DiffCase, Is.True);
			Assert.That(result2, Is.False);
			Assert.That(result3, Is.False);
			Assert.That(result4, Is.False);
			Assert.That(result5, Is.True);
			Assert.That(result5DiffCase, Is.True);
		}

		private List<LongText> CreateDataSet()
		{
			ObjectExportInfo artifact1 = new ObjectExportInfo
			{
				ArtifactID = 1
			};

			LongText longText1 =
				LongText.CreateFromMissingValue(artifact1.ArtifactID, 10, LongTextExportRequest.CreateRequestForLongText(artifact1, 10, "require_deletion"), Encoding.Default, 1);
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
				Encoding.Default,
				artifact2.LongTextLength);
			longText3.ExportRequest.Order = 3;

			return new List<LongText> { longText1, longText2, longText3 };
		}
	}
}