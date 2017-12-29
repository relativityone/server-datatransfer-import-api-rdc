using System;
using System.Collections.Generic;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images.Lines;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Metadata.Images.Lines
{
	[TestFixture]
	public class IproLoadFileEntryTests
	{
		private IproLoadFileEntry _instance;

		private ExportFile _exportSettings;

		[SetUp]
		public void SetUp()
		{
			_exportSettings = new ExportFile(1);

			_instance = new IproLoadFileEntry(_exportSettings, new NullLogger());
		}

		[Test]
		[TestCaseSource(nameof(DataSetsSinglePage))]
		[TestCaseSource(nameof(DataSetsMultiPage))]
		public void ItShouldCreateIproEntry(ImageLoadFileEntryDataSet dataSet)
		{
			_exportSettings.TypeOfImage = dataSet.ImageType;

			//ACT
			string actualResult = _instance.Create(dataSet.BatesNumber, dataSet.FilePath, dataSet.Volume, dataSet.PageNumber, dataSet.NumberOfImages);

			//ASSERT
			Assert.That(actualResult, Is.EqualTo(dataSet.ExpectedResult));
		}

		private static IEnumerable<ImageLoadFileEntryDataSet> DataSetsSinglePage()
		{
			ExportFile.ImageType imageType = ExportFile.ImageType.SinglePage;
			yield return new ImageLoadFileEntryDataSet
			{
				BatesNumber = "BatesNumber",
				FilePath = @"Folder\FilePath",
				Volume = "Volume",
				PageNumber = 1,
				NumberOfImages = 1,
				ImageType = imageType,
				ExpectedResult = $"IM,BatesNumber,D,0,@Volume;Folder;FilePath;{Environment.NewLine}"
			};
			yield return new ImageLoadFileEntryDataSet
			{
				BatesNumber = "B",
				FilePath = @"Dir\F.pdf",
				Volume = "V",
				PageNumber = 1,
				NumberOfImages = 1,
				ImageType = imageType,
				ExpectedResult = $"IM,B,D,0,@V;Dir;F.pdf;7{Environment.NewLine}"
			};
			yield return new ImageLoadFileEntryDataSet
			{
				BatesNumber = "B",
				FilePath = @"F.jpg",
				Volume = "V",
				PageNumber = 5,
				NumberOfImages = 1,
				ImageType = imageType,
				ExpectedResult = $"IM,B,,0,@V;;F.jpg;4{Environment.NewLine}"
			};
			yield return new ImageLoadFileEntryDataSet
			{
				BatesNumber = "B",
				FilePath = @"Dir\F.jpeg",
				Volume = "V",
				PageNumber = 5,
				NumberOfImages = 3,
				ImageType = imageType,
				ExpectedResult = $"IM,B,,0,@V;Dir;F.jpeg;4{Environment.NewLine}"
			};
			yield return new ImageLoadFileEntryDataSet
			{
				BatesNumber = "B",
				FilePath = @"Dir\F.tif",
				Volume = "V",
				PageNumber = 1,
				NumberOfImages = 3,
				ImageType = imageType,
				ExpectedResult = $"IM,B,D,0,@V;Dir;F.tif;2{Environment.NewLine}"
			};
			yield return new ImageLoadFileEntryDataSet
			{
				BatesNumber = "B",
				FilePath = @"Dir\F.tiff",
				Volume = "V",
				PageNumber = 1,
				NumberOfImages = 3,
				ImageType = imageType,
				ExpectedResult = $"IM,B,D,0,@V;Dir;F.tiff;2{Environment.NewLine}"
			};
		}

		private static IEnumerable<ImageLoadFileEntryDataSet> DataSetsMultiPage()
		{
			foreach (var imageLoadFileEntryDataSet in MultiPageDataSets(ExportFile.ImageType.MultiPageTiff))
			{
				yield return imageLoadFileEntryDataSet;
			}
			foreach (var imageLoadFileEntryDataSet in MultiPageDataSets(ExportFile.ImageType.Pdf))
			{
				yield return imageLoadFileEntryDataSet;
			}
		}

		private static IEnumerable<ImageLoadFileEntryDataSet> MultiPageDataSets(ExportFile.ImageType imageType)
		{
			yield return new ImageLoadFileEntryDataSet
			{
				BatesNumber = "BatesNumber",
				FilePath = @"Folder\FilePath",
				Volume = "Volume",
				PageNumber = 1,
				NumberOfImages = 1,
				ImageType = imageType,
				ExpectedResult = $"IM,BatesNumber,D,1,@Volume;Folder;FilePath;{Environment.NewLine}"
			};
			yield return new ImageLoadFileEntryDataSet
			{
				BatesNumber = "B",
				FilePath = @"Dir\F.pdf",
				Volume = "V",
				PageNumber = 1,
				NumberOfImages = 1,
				ImageType = imageType,
				ExpectedResult = $"IM,B,D,1,@V;Dir;F.pdf;7{Environment.NewLine}"
			};
			yield return new ImageLoadFileEntryDataSet
			{
				BatesNumber = "B",
				FilePath = @"F.jpg",
				Volume = "V",
				PageNumber = 5,
				NumberOfImages = 1,
				ImageType = imageType,
				ExpectedResult = $"IM,B,,5,@V;;F.jpg;4{Environment.NewLine}"
			};
			yield return new ImageLoadFileEntryDataSet
			{
				BatesNumber = "B",
				FilePath = @"Dir\F.jpeg",
				Volume = "V",
				PageNumber = 5,
				NumberOfImages = 3,
				ImageType = imageType,
				ExpectedResult = $"IM,B,,5,@V;Dir;F.jpeg;4{Environment.NewLine}"
			};
			yield return new ImageLoadFileEntryDataSet
			{
				BatesNumber = "B",
				FilePath = @"Dir\F.tif",
				Volume = "V",
				PageNumber = 1,
				NumberOfImages = 3,
				ImageType = imageType,
				ExpectedResult = $"IM,B,D,1,@V;Dir;F.tif;2{Environment.NewLine}"
			};
			yield return new ImageLoadFileEntryDataSet
			{
				BatesNumber = "B",
				FilePath = @"Dir\F.tiff",
				Volume = "V",
				PageNumber = 1,
				NumberOfImages = 3,
				ImageType = imageType,
				ExpectedResult = $"IM,B,D,1,@V;Dir;F.tiff;2{Environment.NewLine}"
			};
		}
	}
}