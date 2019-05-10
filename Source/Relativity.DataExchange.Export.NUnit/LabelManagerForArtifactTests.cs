﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="LabelManagerForArtifactTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
{
	using global::NUnit.Framework;

    using Moq;

	using Relativity.Export.VolumeManagerV2.Directories;

	[TestFixture]
	public class LabelManagerForArtifactTests
	{
		private LabelManagerForArtifact _instance;
		private Mock<ILabelManager> _labelManager;

		[SetUp]
		public void SetUp()
		{
			_labelManager = new Mock<ILabelManager>();

			_instance = new LabelManagerForArtifact(_labelManager.Object);
		}

		[Test]
		public void ItShouldDelegateGetVolumeLabelCallToLabelManager()
		{
			_instance.GetVolumeLabel(1);

			_labelManager.Verify(manager => manager.GetCurrentVolumeLabel(), Times.Once);
		}

		[Test]
		public void ItShouldDelegateGetImageSubdirectoryLabelCallToLabelManager()
		{
			_instance.GetImageSubdirectoryLabel(1);

			_labelManager.Verify(manager => manager.GetCurrentImageSubdirectoryLabel(), Times.Once);
		}

		[Test]
		public void ItShouldDelegateGetNativeSubdirectoryLabelCallToLabelManager()
		{
			_instance.GetNativeSubdirectoryLabel(1);

			_labelManager.Verify(manager => manager.GetCurrentNativeSubdirectoryLabel(), Times.Once);
		}

		[Test]
		public void ItShouldDelegateGetTextSubdirectoryLabelCallToLabelManager()
		{
			_instance.GetTextSubdirectoryLabel(1);

			_labelManager.Verify(manager => manager.GetCurrentTextSubdirectoryLabel(), Times.Once);
		}
	}
}