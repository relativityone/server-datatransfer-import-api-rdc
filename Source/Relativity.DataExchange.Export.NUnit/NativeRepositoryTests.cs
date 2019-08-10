// -----------------------------------------------------------------------------------------------------
// <copyright file="NativeRepositoryTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Collections.Generic;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Exporters;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;

	[TestFixture]
	public class NativeRepositoryTests
	{
		private NativeRepository _instance;

		private IList<Native> _natives;

		[SetUp]
		public void SetUp()
		{
			this._natives = this.CreateDataSet();

			this._instance = new NativeRepository();
			foreach (Native native in this._natives)
			{
				this._instance.Add(native);
			}
		}

		[Test]
		public void ItShouldGetNativeByArtifactId()
		{
			// ACT
			Native native1 = this._instance.GetNative(1);
			Native native2 = this._instance.GetNative(2);
			Native native3 = this._instance.GetNative(3);
			Native native4 = this._instance.GetNative(4);

			// ASSERT
			Assert.That(native1, Is.EqualTo(this._natives[0]));
			Assert.That(native2, Is.EqualTo(this._natives[1]));
			Assert.That(native3, Is.EqualTo(this._natives[2]));
			Assert.That(native4, Is.Null);
		}

		[Test]
		public void ItShouldGetAllNatives()
		{
			// ACT
			IList<Native> natives = this._instance.GetNatives();

			// ASSERT
			CollectionAssert.AreEquivalent(this._natives, natives);
		}

		[Test]
		public void ItShouldGetExportRequests()
		{
			var expectedExportRequests = new List<ExportRequest>
			{
				this._natives[0].ExportRequest,
				this._natives[2].ExportRequest
			};

			// ACT
			IEnumerable<ExportRequest> exportRequests = this._instance.GetExportRequests();

			// ASSERT
			CollectionAssert.AreEquivalent(expectedExportRequests, exportRequests);
		}

		[Test]
		public void ItShouldGetNativeByUniqueId()
		{
			// ACT
			Native native1 = this._instance.GetByLineNumber(1);
			Native native2 = this._instance.GetByLineNumber(2);
			Native native3 = this._instance.GetByLineNumber(3);

			// ASSERT
			Assert.That(native1, Is.EqualTo(this._natives[0]));
			Assert.That(native2, Is.Null);
			Assert.That(native3, Is.EqualTo(this._natives[2]));
		}

		[Test]
		public void ItShouldClearRepository()
		{
			// ACT
			this._instance.Clear();
			IList<Native> natives = this._instance.GetNatives();

			// ASSERT
			CollectionAssert.IsEmpty(natives);
		}

		[Test]
		public void ItShouldGetAnyRequestForLocation()
		{
			// ACT
			bool result1 = this._instance.AnyRequestForLocation(@"C:\temp\native1.docx");
			bool result1DiffCase = this._instance.AnyRequestForLocation(@"c:\Temp\NATIVE1.Docx");
			bool result2 = this._instance.AnyRequestForLocation(@"C:\temp\does-not-exist.docx");
			bool result3 = this._instance.AnyRequestForLocation(null);
			bool result4 = this._instance.AnyRequestForLocation(string.Empty);
			bool result5 = this._instance.AnyRequestForLocation(@"C:\temp\native3.docx");
			bool result5DiffCase = this._instance.AnyRequestForLocation(@"c:\Temp\NATIVE3.Docx");

			// ASSERT
			Assert.That(result1, Is.True);
			Assert.That(result1DiffCase, Is.True);
			Assert.That(result2, Is.False);
			Assert.That(result3, Is.False);
			Assert.That(result4, Is.False);
			Assert.That(result5, Is.True);
			Assert.That(result5DiffCase, Is.True);
		}

		private List<Native> CreateDataSet()
		{
			ObjectExportInfo artifact1 = new ObjectExportInfo
			{
				ArtifactID = 1
			};
			Native native1 = new Native(artifact1)
			{
				HasBeenDownloaded = false,
				ExportRequest = new PhysicalFileExportRequest(artifact1, @"C:\temp\native1.docx")
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
				ExportRequest = new PhysicalFileExportRequest(artifact3, @"C:\temp\native3.docx")
				{
					FileName = "filename_3",
					Order = 3
				}
			};

			return new List<Native> { native1, native2, native3 };
		}
	}
}