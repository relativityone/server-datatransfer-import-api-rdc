// -----------------------------------------------------------------------------------------------------
// <copyright file="CachedLabelManagerForArtifactTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Directories;

	[TestFixture]
	public class CachedLabelManagerForArtifactTests
	{
		private static readonly Dictionary<string, string> MethodNameToReturnedPrefix =
			new Dictionary<string, string>()
				{
					{ nameof(CachedLabelManagerForArtifact.GetVolumeLabel), "VOL" },
					{ nameof(CachedLabelManagerForArtifact.GetImageSubdirectoryLabel), "IMG" },
					{ nameof(CachedLabelManagerForArtifact.GetNativeSubdirectoryLabel), "NAT" },
					{ nameof(CachedLabelManagerForArtifact.GetTextSubdirectoryLabel), "TXT" }
				};

		private CachedLabelManagerForArtifact _instance;
		private Mock<ILabelManager> _labelManager;
		private Mock<IDirectoryManager> _directoryManager;
		private VolumePredictions[] _volumePredictions;
		private ObjectExportInfo[] _artifacts;

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
			this._artifacts = new[]
				             {
					             new ObjectExportInfo() { ArtifactID = 0 }, new ObjectExportInfo() { ArtifactID = 1 },
					             new ObjectExportInfo() { ArtifactID = 2 }
				             };
			this._volumePredictions = new[] { new VolumePredictions(), new VolumePredictions(), new VolumePredictions() };

			this._labelManager = new Mock<ILabelManager>();
			this._directoryManager = new Mock<IDirectoryManager>();

			this._labelManager.SetupSequence(lm => lm.GetCurrentVolumeLabel()).Returns("VOL0").Returns("VOL1").Returns("VOL2");
			this._labelManager.SetupSequence(lm => lm.GetCurrentImageSubdirectoryLabel()).Returns("IMG0").Returns("IMG1").Returns("IMG2");
			this._labelManager.SetupSequence(lm => lm.GetCurrentNativeSubdirectoryLabel()).Returns("NAT0").Returns("NAT1").Returns("NAT2");
			this._labelManager.SetupSequence(lm => lm.GetCurrentTextSubdirectoryLabel()).Returns("TXT0").Returns("TXT1").Returns("TXT2");

			this._instance = new CachedLabelManagerForArtifact(this._labelManager.Object, this._directoryManager.Object);
		}

		[Test]
		public void MoveNextShouldBeCalledForEachArtifact()
		{
			this._instance.InitializeFor(this._artifacts, this._volumePredictions, CancellationToken.None);

			this._volumePredictions.ToList().ForEach(x => this._directoryManager.Verify(dm => dm.MoveNext(x), Times.Once));
		}

		[Test]
		public void DirectoryManagerShouldNotBeCalledAfterInitialization()
		{
			this._instance.InitializeFor(this._artifacts, this._volumePredictions, CancellationToken.None);

            this._directoryManager.ResetCalls();

			foreach (Func<CachedLabelManagerForArtifact, Func<int, string>> methodGenerator in MethodsToTest())
			{
				methodGenerator(this._instance)(1);
			}

			this._directoryManager.Verify(dm => dm.MoveNext(It.IsAny<VolumePredictions>()), Times.Never);
		}

		[Test]
		[TestCaseSource(nameof(MethodsToTest))]
		public void ItShouldReturnSameResultsForSameArtifact(Func<CachedLabelManagerForArtifact, Func<int, string>> testMethodGenerator)
		{
			this._instance.InitializeFor(this._artifacts, this._volumePredictions, CancellationToken.None);

			Func<int, string> testMethod = testMethodGenerator(this._instance);

			string firstResult = testMethod(1);
			string secondResult = testMethod(1);

			Assert.That(firstResult, Is.EqualTo(secondResult));
		}

		[Test]
		[TestCaseSource(nameof(MethodsToTest))]
		public void ItShouldReturnDifferentResultsForDifferentArtifacts(Func<CachedLabelManagerForArtifact, Func<int, string>> testMethodGenerator)
		{
			this._instance.InitializeFor(this._artifacts, this._volumePredictions, CancellationToken.None);

			Func<int, string> testMethod = testMethodGenerator(this._instance);

			string firstResult = testMethod(1);
			string secondResult = testMethod(2);

			Assert.That(firstResult, Is.Not.EqualTo(secondResult));
		}

		[Test]
		[TestCaseSource(nameof(MethodsToTest))]
		public void ItShouldReturnCorrectResultsForDifferentArtifacts(Func<CachedLabelManagerForArtifact, Func<int, string>> testMethodGenerator)
		{
			this._instance.InitializeFor(this._artifacts, this._volumePredictions, CancellationToken.None);

			Func<int, string> testMethod = testMethodGenerator(this._instance);

			for (int i = this._artifacts.Length - 1; i >= 0; i--)
			{
				int artifactId = this._artifacts[i].ArtifactID;
				string testMethodName = testMethod.Method.Name;

				string expectedResult = $"{MethodNameToReturnedPrefix[testMethodName]}{artifactId}";

				string result = testMethod(artifactId);

				Assert.That(result, Is.EqualTo(expectedResult));
			}
		}
	}
}