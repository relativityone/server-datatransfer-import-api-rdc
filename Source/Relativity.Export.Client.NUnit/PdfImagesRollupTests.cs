// -----------------------------------------------------------------------------------------------------
// <copyright file="PdfImagesRollupTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
{
    using System.Linq;

    using kCura.WinEDDS;
    using kCura.WinEDDS.Exporters;

    using Moq;

	using Relativity.Export.VolumeManagerV2.ImagesRollup;
	using Relativity.Import.Export.Io;
    using Relativity.Logging;

    public class PdfImagesRollupTests : MultiPageImagesRollupTests
	{
		protected override MultiPageImagesRollup CreateInstance(ExportFile exportSettings, IFile fileHelper, IStatus status, IImage imageConverter)
		{
			return new PdfImagesRollup(exportSettings, fileHelper, status, new NullLogger(), imageConverter);
		}

		protected override void AssertImageConverterCall(Mock<IImage> imageConverter, ObjectExportInfo artifact)
		{
			string[] imagesList = artifact.Images.Cast<ImageExportInfo>().Select(x => x.TempLocation).ToArray();
			imageConverter.Verify(x => x.ConvertImagesToMultiPagePdf(It.Is<string[]>(y => y.SequenceEqual(imagesList)), It.Is<string>(y => y.EndsWith(".tmp"))));
		}

		protected override string Extension()
		{
			return ".pdf";
		}
	}
}