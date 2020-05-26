// -----------------------------------------------------------------------------------------------------
// <copyright file="LabelManagerForArtifactTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;

	[TestFixture]
	public class LabelManagerForArtifactTests
	{
		private LabelManagerForArtifact _instance;
		private Mock<ILabelManager> _labelManager;

		[SetUp]
		public void SetUp()
		{
			this._labelManager = new Mock<ILabelManager>();

			this._instance = new LabelManagerForArtifact(this._labelManager.Object);
		}

		[Test]
		public void ItShouldDelegateGetVolumeLabelCallToLabelManager()
		{
			this._instance.GetVolumeLabel(1);

			this._labelManager.Verify(manager => manager.GetCurrentVolumeLabel(), Times.Once);
		}

		[Test]
		public void ItShouldDelegateGetImageSubdirectoryLabelCallToLabelManager()
		{
			this._instance.GetImageSubdirectoryLabel(1);

			this._labelManager.Verify(manager => manager.GetCurrentImageSubdirectoryLabel(), Times.Once);
		}

		[Test]
		public void ItShouldDelegateGetNativeSubdirectoryLabelCallToLabelManager()
		{
			this._instance.GetNativeSubdirectoryLabel(1);

			this._labelManager.Verify(manager => manager.GetCurrentNativeSubdirectoryLabel(), Times.Once);
		}

		[Test]
		public void ItShouldDelegateGetTextSubdirectoryLabelCallToLabelManager()
		{
			this._instance.GetTextSubdirectoryLabel(1);

			this._labelManager.Verify(manager => manager.GetCurrentTextSubdirectoryLabel(), Times.Once);
		}

		[Test]
		public void ItShouldDelegateGetPdfSubdirectoryLabelCallToLabelManager()
		{
			this._instance.GetPdfSubdirectoryLabel(1);

			this._labelManager.Verify(manager => manager.GetCurrentPdfSubdirectoryLabel(), Times.Once);
		}
	}
}