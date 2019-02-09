﻿// ----------------------------------------------------------------------------
// <copyright file="ImageLoadFileMetadataForArtifactBuilderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit
{
    using kCura.WinEDDS;
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images;
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images.Lines;
    using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
    using kCura.WinEDDS.Exporters;

    using Moq;

    using global::NUnit.Framework;

    [TestFixture]
	public abstract class ImageLoadFileMetadataForArtifactBuilderTests
	{
		protected ImageLoadFileMetadataForArtifactBuilder Instance { get; private set; }

		protected ExportFile ExportSettings { get; private set; }

		protected Mock<IFilePathTransformer> FilePathTransformer { get; private set; }
		protected Mock<IImageLoadFileEntry> ImageLoadFileEntry { get; private set; }
		protected Mock<IFullTextLoadFileEntry> FullTextLoadFileEntry { get; private set; }
		protected Mock<IRetryableStreamWriter> Writer { get; private set; }

		[SetUp]
		public void SetUp()
		{
			ExportSettings = new ExportFile(1)
			{
				VolumeInfo = new VolumeInfo()
			};
			FilePathTransformer = new Mock<IFilePathTransformer>();
			ImageLoadFileEntry = new Mock<IImageLoadFileEntry>();
			FullTextLoadFileEntry = new Mock<IFullTextLoadFileEntry>();
			Writer = new Mock<IRetryableStreamWriter>();

			Instance = CreateInstance(ExportSettings, FilePathTransformer.Object, ImageLoadFileEntry.Object, FullTextLoadFileEntry.Object);
		}

		protected abstract ImageLoadFileMetadataForArtifactBuilder CreateInstance(ExportFile exportSettings, IFilePathTransformer filePathTransformer,
			IImageLoadFileEntry imageLoadFileEntry,
			IFullTextLoadFileEntry fullTextLoadFileEntry);
	}
}