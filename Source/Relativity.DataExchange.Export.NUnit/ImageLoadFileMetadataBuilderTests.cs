// -----------------------------------------------------------------------------------------------------
// <copyright file="ImageLoadFileMetadataBuilderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Collections;
	using System.Threading;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Images;
	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Writers;
	using Relativity.Logging;

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
			this._forArtifactBuilder = new Mock<IImageLoadFileMetadataForArtifactBuilder>();
			this._unsuccessfulRollupForArtifactBuilder = new Mock<IImageLoadFileMetadataForArtifactBuilder>();
			this._writer = new Mock<IRetryableStreamWriter>();

			this._instance = new ImageLoadFileMetadataBuilder(this._forArtifactBuilder.Object, this._unsuccessfulRollupForArtifactBuilder.Object, this._writer.Object, new NullLogger());
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

			// ACT
			this._instance.CreateLoadFileEntries(new[] { artifact }, CancellationToken.None);

			// ASSERT
			this._forArtifactBuilder.Verify(x => x.WriteLoadFileEntry(artifact, this._writer.Object, CancellationToken.None), expectingCallToSuccessfulRollupBuilder ? Times.Once() : Times.Never());
			this._unsuccessfulRollupForArtifactBuilder.Verify(
				x => x.WriteLoadFileEntry(artifact, this._writer.Object, CancellationToken.None),
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

			// ACT
			this._instance.CreateLoadFileEntries(new[] { artifact1, artifact2 }, CancellationToken.None);

			// ASSERT
			this._forArtifactBuilder.Verify(
				x => x.WriteLoadFileEntry(artifact1, this._writer.Object, CancellationToken.None),
				Times.Once);
			this._unsuccessfulRollupForArtifactBuilder.Verify(
				x => x.WriteLoadFileEntry(artifact2, this._writer.Object, CancellationToken.None),
				Times.Once);
		}

		[Test]
		public void ItShouldHandleEmptyImageList()
		{
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Images = new ArrayList()
			};

			// ACT & ASSERT
			Assert.DoesNotThrow(() => this._instance.CreateLoadFileEntries(new[] { artifact }, CancellationToken.None));

			this._forArtifactBuilder.Verify(x => x.WriteLoadFileEntry(artifact, this._writer.Object, CancellationToken.None), Times.Never);
			this._unsuccessfulRollupForArtifactBuilder.Verify(x => x.WriteLoadFileEntry(artifact, this._writer.Object, CancellationToken.None), Times.Never);
		}
	}
}