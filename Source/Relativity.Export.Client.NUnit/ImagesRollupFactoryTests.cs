﻿// ----------------------------------------------------------------------------
// <copyright file="ImagesRollupFactoryTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit
{
    using Castle.Windsor;

    using kCura.WinEDDS;
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.ImagesRollup;
    using kCura.WinEDDS.Exporters;

    using Moq;

    using global::NUnit.Framework;

    using Relativity;
    using Relativity.Logging;

    [TestFixture]
	public class ImagesRollupFactoryTests
	{
		[Test]
		[TestCase(false, false)]
		[TestCase(false, true)]
		[TestCase(true, false)]
		public void ItShouldReturnSinglePageRollupWhenNotExportingImages(bool exportImages, bool copyFiles)
		{
			ExportFile exportSettings = new ExportFile((int) ArtifactType.Document)
			{
				ExportImages = exportImages,
				VolumeInfo = new VolumeInfo
				{
					CopyImageFilesFromRepository = copyFiles
				},
				TypeOfImage = ExportFile.ImageType.MultiPageTiff
			};

			var windsorContainerMock = new Mock<IWindsorContainer>();

			var instance = new ImagesRollupFactory(new NullLogger());

			//ACT
			instance.Create(exportSettings, windsorContainerMock.Object);

			//ASSERT
			windsorContainerMock.Verify(x => x.Resolve<SinglePageImagesRollup>());
		}
	}
}