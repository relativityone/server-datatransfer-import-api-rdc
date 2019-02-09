// -----------------------------------------------------------------------------------------------------
// <copyright file="MultiPageTiffImagesRollupTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit
{
    using System.Linq;

    using kCura.WinEDDS;
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.ImagesRollup;
    using kCura.WinEDDS.Exporters;

    using Moq;

    using Relativity.Logging;

    public class MultiPageTiffImagesRollupTests : MultiPageImagesRollupTests
	{
		protected override MultiPageImagesRollup CreateInstance(ExportFile exportSettings, IFileHelper fileHelper, IStatus status, IImage imageConverter)
		{
			return new MultiPageTiffImagesRollup(exportSettings, fileHelper, status, new NullLogger(), imageConverter);
		}

		protected override void AssertImageConverterCall(Mock<IImage> imageConverter, ObjectExportInfo artifact)
		{
			string[] imagesList = artifact.Images.Cast<ImageExportInfo>().Select(x => x.TempLocation).ToArray();
			imageConverter.Verify(x => x.ConvertTIFFsToMultiPage(It.Is<string[]>(y => y.SequenceEqual(imagesList)), It.Is<string>(y => y.EndsWith(".tmp"))));
		}

		protected override string Extension()
		{
			return ".tif";
		}
	}
}