using System.Collections.Generic;
using System.Text;
using kCura.Utility.Extensions;
using kCura.WinEDDS.Core.Export.VolumeManagerV2;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Text.Repository;
using kCura.WinEDDS.Exporters;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Metadata.Text.Repository
{
	[TestFixture]
	public class LongTextRepositoryTests
	{
		private LongTextRepository _instance;

		private IList<LongText> _longTexts;

		private Mock<IFileHelper> _fileHelper;

		[SetUp]
		public void SetUp()
		{
			_longTexts = CreateDataSet();

			_fileHelper = new Mock<IFileHelper>();

			_instance = new LongTextRepository(_fileHelper.Object, new NullLogger());
			_instance.Add(_longTexts);
		}

		[Test]
		public void ItShouldSearchLongTextByArtifactIdAndFieldArtifactId()
		{
			//ACT
			LongText image1 = _instance.GetLongText(1, 10);
			LongText image2 = _instance.GetLongText(1, 20);
			LongText image3 = _instance.GetLongText(2, 30);

			//ASSERT
			Assert.That(image1, Is.EqualTo(_longTexts[0]));
			Assert.That(image2, Is.EqualTo(_longTexts[1]));
			Assert.That(image3, Is.EqualTo(_longTexts[2]));
		}

		[Test]
		public void ItShouldGetLongTextsForArtifact()
		{
			//ACT
			IList<LongText> images1 = _instance.GetArtifactLongTexts(1);
			IList<LongText> images2 = _instance.GetArtifactLongTexts(2);

			//ASSERT
			CollectionAssert.AreEquivalent(_longTexts.GetRange(0, 2), images1);
			CollectionAssert.AreEquivalent(_longTexts[2].InList(), images2);
		}

		[Test]
		public void ItShouldGetAllImages()
		{
			//ACT
			IList<LongText> images = _instance.GetLongTexts();

			//ASSERT
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

			//ACT
			IList<LongTextExportRequest> exportRequests = _instance.GetExportRequests();

			//ASSERT
			CollectionAssert.AreEquivalent(expectedExportRequests, exportRequests);
		}

		[Test]
		public void ItShouldGetLongTextByUniqueId()
		{
			//ACT
			LongText image1 = _instance.GetByUniqueId("unique_1");
			LongText image2 = _instance.GetByUniqueId("unique_2");
			LongText image3 = _instance.GetByUniqueId("unique_3");

			//ASSERT
			Assert.That(image1, Is.EqualTo(_longTexts[0]));
			Assert.That(image2, Is.Null);
			Assert.That(image3, Is.EqualTo(_longTexts[2]));
		}

		[Test]
		public void ItShouldClearRepository()
		{
			//ACT
			_instance.Clear();
			IList<LongText> images = _instance.GetLongTexts();

			//ASSERT
			CollectionAssert.IsEmpty(images);
			_fileHelper.Verify(x => x.Delete("require_deletion"), Times.Once);
			_fileHelper.Verify(x => x.Delete("do_not_require_deletion"), Times.Never);
		}

		#region DataSet

		private List<LongText> CreateDataSet()
		{
			ObjectExportInfo artifact1 = new ObjectExportInfo
			{
				ArtifactID = 1
			};

			LongText longText1 = LongText.CreateFromMissingValue(artifact1.ArtifactID, 10, LongTextExportRequest.CreateRequestForLongText(artifact1, 10, "require_deletion"), Encoding.Default);
			longText1.ExportRequest.UniqueId = "unique_1";

			LongText longText2 = LongText.CreateFromExistingValue(artifact1.ArtifactID, 20, "text");

			ObjectExportInfo artifact2 = new ObjectExportInfo
			{
				ArtifactID = 2
			};

			LongText longText3 =
				LongText.CreateFromMissingFile(artifact2.ArtifactID, 30, LongTextExportRequest.CreateRequestForFullText(artifact2, 30, "do_not_require_deletion"), Encoding.Default, Encoding.Default);
			longText3.ExportRequest.UniqueId = "unique_3";

			return new List<LongText> {longText1, longText2, longText3};
		}

		#endregion
	}
}