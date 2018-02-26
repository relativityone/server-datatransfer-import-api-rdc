using System.Collections.Generic;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Repository;
using kCura.WinEDDS.Exporters;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Repository
{
	[TestFixture]
	public class NativeRepositoryTests
	{
		private NativeRepository _instance;

		private IList<Native> _natives;

		[SetUp]
		public void SetUp()
		{
			_natives = CreateDataSet();

			_instance = new NativeRepository();
			_instance.Add(_natives);
		}

		[Test]
		public void ItShouldGetNativeByArtifactId()
		{
			//ACT
			Native native1 = _instance.GetNative(1);
			Native native2 = _instance.GetNative(2);
			Native native3 = _instance.GetNative(3);

			//ASSERT
			Assert.That(native1, Is.EqualTo(_natives[0]));
			Assert.That(native2, Is.EqualTo(_natives[1]));
			Assert.That(native3, Is.EqualTo(_natives[2]));
		}

		[Test]
		public void ItShouldGetAllNatives()
		{
			//ACT
			IList<Native> natives = _instance.GetNatives();

			//ASSERT
			CollectionAssert.AreEquivalent(_natives, natives);
		}

		[Test]
		public void ItShouldGetExportRequests()
		{
			var expectedExportRequests = new List<ExportRequest>
			{
				_natives[0].ExportRequest,
				_natives[2].ExportRequest
			};

			//ACT
			IList<ExportRequest> exportRequests = _instance.GetExportRequests();

			//ASSERT
			CollectionAssert.AreEquivalent(expectedExportRequests, exportRequests);
		}

		[Test]
		public void ItShouldGetNativeByUniqueId()
		{
			//ACT
			Native native1 = _instance.GetByLineNumber(1);
			Native native2 = _instance.GetByLineNumber(2);
			Native native3 = _instance.GetByLineNumber(3);

			//ASSERT
			Assert.That(native1, Is.EqualTo(_natives[0]));
			Assert.That(native2, Is.Null);
			Assert.That(native3, Is.EqualTo(_natives[2]));
		}

		[Test]
		public void ItShouldClearRepository()
		{
			//ACT
			_instance.Clear();
			IList<Native> natives = _instance.GetNatives();

			//ASSERT
			CollectionAssert.IsEmpty(natives);
		}

		#region DataSet

		private List<Native> CreateDataSet()
		{
			ObjectExportInfo artifact1 = new ObjectExportInfo
			{
				ArtifactID = 1
			};
			Native native1 = new Native(artifact1)
			{
				HasBeenDownloaded = false,
				ExportRequest = new NativeFileExportRequest(artifact1, "")
				{
					FileName = "filename_1",
					Order = 1
				}
			};

			ObjectExportInfo artifact2 = new ObjectExportInfo
			{
				ArtifactID = 2
			};
			Native native2 = new Native(artifact2)
			{
				HasBeenDownloaded = true
			};


			ObjectExportInfo artifact3 = new ObjectExportInfo
			{
				ArtifactID = 3
			};
			Native native3 = new Native(artifact3)
			{
				HasBeenDownloaded = false,
				ExportRequest = new NativeFileExportRequest(artifact3, "")
				{
					FileName = "filename_3",
					Order = 3
				}
			};


			return new List<Native> {native1, native2, native3};
		}

		#endregion
	}
}