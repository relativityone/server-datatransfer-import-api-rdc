using System.Collections;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Images;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Metadata.Writers;
using kCura.WinEDDS.Exporters;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Metadata.Images
{
	[TestFixture]
	public class ImageLoadFileMetadataBuilderTests
	{
		private ImageLoadFileMetadataBuilder _instance;

		private Mock<IImageLoadFileMetadataForArtifactBuilder> _forArtifactBuilder;
		private Mock<IImageLoadFileMetadataForArtifactBuilder> _unsuccessfulRollupForArtifactBuilder;
		private Mock<IRetryableStreamWriter> _writer;

		[SetUp]
		public void SetUp()
		{
			_forArtifactBuilder = new Mock<IImageLoadFileMetadataForArtifactBuilder>();
			_unsuccessfulRollupForArtifactBuilder = new Mock<IImageLoadFileMetadataForArtifactBuilder>();
			_writer = new Mock<IRetryableStreamWriter>();

			_instance = new ImageLoadFileMetadataBuilder(_forArtifactBuilder.Object, _unsuccessfulRollupForArtifactBuilder.Object, _writer.Object, new NullLogger());
		}

		[Test]
		[TestCase(true, false, true)]
		[TestCase(true, true, true)]
		[TestCase(false, false, false)]
		[TestCase(false, true, false)]
		public void ItShouldWriteEntryBasedOnRollupResultInFirstImage(bool firstImageRollupResult, bool secondImageRollupResult, bool expectingCallToSuccessfulRollupBuilder)
		{
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Images = new ArrayList
				{
					new ImageExportInfo
					{
						SuccessfulRollup = firstImageRollupResult
					},
					new ImageExportInfo
					{
						SuccessfulRollup = secondImageRollupResult
					}
				}
			};

			//ACT
			_instance.CreateLoadFileEntries(new[] {artifact}, CancellationToken.None);

			//ASSERT
			_forArtifactBuilder.Verify(x => x.WriteLoadFileEntry(artifact, _writer.Object, CancellationToken.None), expectingCallToSuccessfulRollupBuilder ? Times.Once() : Times.Never());
			_unsuccessfulRollupForArtifactBuilder.Verify(x => x.WriteLoadFileEntry(artifact, _writer.Object, CancellationToken.None),
				expectingCallToSuccessfulRollupBuilder ? Times.Never() : Times.Once());
		}

		[Test]
		public void ItShouldHandleEachArtifactIndependently()
		{
			ObjectExportInfo artifact1 = new ObjectExportInfo
			{
				Images = new ArrayList
				{
					new ImageExportInfo
					{
						SuccessfulRollup = true
					}
				}
			};
			ObjectExportInfo artifact2 = new ObjectExportInfo
			{
				Images = new ArrayList
				{
					new ImageExportInfo
					{
						SuccessfulRollup = false
					}
				}
			};

			//ACT
			_instance.CreateLoadFileEntries(new[] {artifact1, artifact2}, CancellationToken.None);

			//ASSERT
			_forArtifactBuilder.Verify(x => x.WriteLoadFileEntry(artifact1, _writer.Object, CancellationToken.None), Times.Once);
			_unsuccessfulRollupForArtifactBuilder.Verify(x => x.WriteLoadFileEntry(artifact2, _writer.Object, CancellationToken.None), Times.Once);
		}

		[Test]
		public void ItShouldHandleEmptyImageList()
		{
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Images = new ArrayList()
			};

			//ACT & ASSERT
			Assert.DoesNotThrow(() => _instance.CreateLoadFileEntries(new[] {artifact}, CancellationToken.None));

			_forArtifactBuilder.Verify(x => x.WriteLoadFileEntry(artifact, _writer.Object, CancellationToken.None), Times.Never);
			_unsuccessfulRollupForArtifactBuilder.Verify(x => x.WriteLoadFileEntry(artifact, _writer.Object, CancellationToken.None), Times.Never);
		}
	}
}