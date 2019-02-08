using System;
using System.Collections.Generic;
using System.Threading;
using Castle.Core.Internal;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Directories;
using kCura.WinEDDS.Exporters;
using Moq;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Directories
{
	[TestFixture]
	public class CachedLabelManagerForArtifactTests
	{
		private CachedLabelManagerForArtifact _instance;
		private Mock<ILabelManager> _labelManager;
		private Mock<IDirectoryManager> _directoryManager;

		private VolumePredictions[] _volumePredictions;
		private ObjectExportInfo[] _artifacts;

		private static readonly Dictionary<string, string> MethodNameToReturnedPrefix =
			new Dictionary<string, string>()
			{
				{nameof(CachedLabelManagerForArtifact.GetVolumeLabel), "VOL"},
				{nameof(CachedLabelManagerForArtifact.GetImageSubdirectoryLabel), "IMG"},
				{nameof(CachedLabelManagerForArtifact.GetNativeSubdirectoryLabel), "NAT"},
				{nameof(CachedLabelManagerForArtifact.GetTextSubdirectoryLabel), "TXT"},
			};

		private static List<Func<CachedLabelManagerForArtifact, Func<int, string>>> MethodsToTest()
		{
			return new List<Func<CachedLabelManagerForArtifact, Func<int, string>>>
			{
				lm => lm.GetVolumeLabel,
				lm => lm.GetImageSubdirectoryLabel,
				lm => lm.GetNativeSubdirectoryLabel,
				lm => lm.GetTextSubdirectoryLabel
			};
		}

		[SetUp]
		public void SetUp()
		{
			_artifacts = new[] { new ObjectExportInfo() {ArtifactID = 0}, new ObjectExportInfo() { ArtifactID = 1 }, new ObjectExportInfo() { ArtifactID = 2 } };
			_volumePredictions = new[] { new VolumePredictions(), new VolumePredictions(), new VolumePredictions() };

			_labelManager = new Mock<ILabelManager>();
			_directoryManager = new Mock<IDirectoryManager>();

			_labelManager.SetupSequence(lm => lm.GetCurrentVolumeLabel()).Returns("VOL0").Returns("VOL1").Returns("VOL2");
			_labelManager.SetupSequence(lm => lm.GetCurrentImageSubdirectoryLabel()).Returns("IMG0").Returns("IMG1").Returns("IMG2");
			_labelManager.SetupSequence(lm => lm.GetCurrentNativeSubdirectoryLabel()).Returns("NAT0").Returns("NAT1").Returns("NAT2");
			_labelManager.SetupSequence(lm => lm.GetCurrentTextSubdirectoryLabel()).Returns("TXT0").Returns("TXT1").Returns("TXT2");

			_instance = new CachedLabelManagerForArtifact(_labelManager.Object, _directoryManager.Object);
		}

		[Test]
		public void MoveNextShouldBeCalledForEachArtifact()
		{
			_instance.InitializeFor(_artifacts, _volumePredictions, CancellationToken.None);

			_volumePredictions.ForEach(x => _directoryManager.Verify(dm => dm.MoveNext(x), Times.Once));
		}

		[Test]
		public void DirectoryManagerShouldNotBeCalledAfterInitialization()
		{
			_instance.InitializeFor(_artifacts, _volumePredictions, CancellationToken.None);

			_directoryManager.ResetCalls();

			foreach (Func<CachedLabelManagerForArtifact, Func<int, string>> methodGenerator in MethodsToTest())
			{
				methodGenerator(_instance)(1);
			}

			_directoryManager.Verify(dm => dm.MoveNext(It.IsAny<VolumePredictions>()), Times.Never);
		}

		[Test]
		[TestCaseSource(nameof(MethodsToTest))]
		public void ItShouldReturnSameResultsForSameArtifact(Func<CachedLabelManagerForArtifact, Func<int, string>> testMethodGenerator)
		{
			_instance.InitializeFor(_artifacts, _volumePredictions, CancellationToken.None);

			Func<int, string> testMethod = testMethodGenerator(_instance);

			string firstResult = testMethod(1);
			string secondResult = testMethod(1);

			Assert.That(firstResult, Is.EqualTo(secondResult));
		}

		[Test]
		[TestCaseSource(nameof(MethodsToTest))]
		public void ItShouldReturnDifferentResultsForDifferentArtifacts(Func<CachedLabelManagerForArtifact, Func<int, string>> testMethodGenerator)
		{
			_instance.InitializeFor(_artifacts, _volumePredictions, CancellationToken.None);

			Func<int, string> testMethod = testMethodGenerator(_instance);

			string firstResult = testMethod(1);
			string secondResult = testMethod(2);

			Assert.That(firstResult, Is.Not.EqualTo(secondResult));
		}

		[Test]
		[TestCaseSource(nameof(MethodsToTest))]
		public void ItShouldReturnCorrectResultsForDifferentArtifacts(Func<CachedLabelManagerForArtifact, Func<int, string>> testMethodGenerator)
		{
			_instance.InitializeFor(_artifacts, _volumePredictions, CancellationToken.None);

			Func<int, string> testMethod = testMethodGenerator(_instance);

			for (int i = _artifacts.Length - 1; i >= 0; i--)
			{
				int artifactId = _artifacts[i].ArtifactID;
				string testMethodName = testMethod.Method.Name;

				string expectedResult = $"{MethodNameToReturnedPrefix[testMethodName]}{artifactId}";

				string result = testMethod(artifactId);

				Assert.That(result, Is.EqualTo(expectedResult));				
			}
		}
	}
}