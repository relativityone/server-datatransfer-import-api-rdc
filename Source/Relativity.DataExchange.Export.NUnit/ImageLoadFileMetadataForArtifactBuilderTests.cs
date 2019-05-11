// -----------------------------------------------------------------------------------------------------
// <copyright file="ImageLoadFileMetadataForArtifactBuilderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Images;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Images.Lines;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers;

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
			this.ExportSettings = new ExportFile(1)
			{
				VolumeInfo = new VolumeInfo()
			};
			this.FilePathTransformer = new Mock<IFilePathTransformer>();
			this.ImageLoadFileEntry = new Mock<IImageLoadFileEntry>();
			this.FullTextLoadFileEntry = new Mock<IFullTextLoadFileEntry>();
			this.Writer = new Mock<IRetryableStreamWriter>();

			this.Instance = this.CreateInstance(this.ExportSettings, this.FilePathTransformer.Object, this.ImageLoadFileEntry.Object, this.FullTextLoadFileEntry.Object);
		}

		protected abstract ImageLoadFileMetadataForArtifactBuilder CreateInstance(
			ExportFile exportSettings,
			IFilePathTransformer filePathTransformer,
			IImageLoadFileEntry imageLoadFileEntry,
			IFullTextLoadFileEntry fullTextLoadFileEntry);
	}
}