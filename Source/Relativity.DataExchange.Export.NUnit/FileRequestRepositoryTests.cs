// -----------------------------------------------------------------------------------------------------
// <copyright file="FileRequestRepositoryTests.cs" company="Relativity ODA LLC">
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
	public class FileRequestRepositoryTests
	{
		private FileRequestRepository _instance;

		private IList<FileRequest<ObjectExportInfo>> _fileRequests;

		[SetUp]
		public void SetUp()
		{
			this._fileRequests = this.CreateDataSet();

			this._instance = new FileRequestRepository();
			foreach (FileRequest<ObjectExportInfo> fileRequest in this._fileRequests)
			{
				this._instance.Add(fileRequest);
			}
		}

		[Test]
		public void ItShouldGetFIleRequestByArtifactId()
		{
			// ACT
			FileRequest<ObjectExportInfo> fileRequest1 = this._instance.GetFileRequest(1);
			FileRequest<ObjectExportInfo> fileRequest2 = this._instance.GetFileRequest(2);
			FileRequest<ObjectExportInfo> fileRequest3 = this._instance.GetFileRequest(3);
			FileRequest<ObjectExportInfo> fileRequest4 = this._instance.GetFileRequest(4);

			// ASSERT
			Assert.That(fileRequest1, Is.EqualTo(this._fileRequests[0]));
			Assert.That(fileRequest2, Is.EqualTo(this._fileRequests[1]));
			Assert.That(fileRequest3, Is.EqualTo(this._fileRequests[2]));
			Assert.That(fileRequest4, Is.Null);
		}

		[Test]
		public void ItShouldGetAllFileRequests()
		{
			// ACT
			IList<FileRequest<ObjectExportInfo>> fileRequests = this._instance.GetFileRequests();

			// ASSERT
			CollectionAssert.AreEquivalent(this._fileRequests, fileRequests);
		}

		[Test]
		public void ItShouldGetExportRequests()
		{
			var expectedExportRequests = new List<ExportRequest>
			{
				this._fileRequests[0].ExportRequest,
				this._fileRequests[2].ExportRequest
			};

			// ACT
			IEnumerable<ExportRequest> exportRequests = this._instance.GetExportRequests();

			// ASSERT
			CollectionAssert.AreEquivalent(expectedExportRequests, exportRequests);
		}

		[Test]
		public void ItShouldGetFileRequestByTargetFile()
		{
			// ACT
			IList<FileRequest<ObjectExportInfo>> fileRequest1 = this._instance.GetFileRequestByDestinationLocation(@"C:\temp\native1.docx");
			IList<FileRequest<ObjectExportInfo>> fileRequest2 = this._instance.GetFileRequestByDestinationLocation(@"C:\temp\native2.docx");
			IList<FileRequest<ObjectExportInfo>> fileRequest3 = this._instance.GetFileRequestByDestinationLocation(@"C:\temp\searchablePdf1.pdf");
			IList<FileRequest<ObjectExportInfo>> fileRequest4 = this._instance.GetFileRequestByDestinationLocation(null);
			IList<FileRequest<ObjectExportInfo>> fileRequest5 = this._instance.GetFileRequestByDestinationLocation(string.Empty);

			// ASSERT
			Assert.That(fileRequest1.Count, Is.EqualTo(1));
			Assert.That(fileRequest1[0], Is.EqualTo(this._fileRequests[0]));
			Assert.That(fileRequest2.Count, Is.Zero);
			Assert.That(fileRequest3.Count, Is.EqualTo(1));
			Assert.That(fileRequest3[0], Is.EqualTo(this._fileRequests[2]));
			Assert.That(fileRequest4.Count, Is.Zero);
			Assert.That(fileRequest5.Count, Is.Zero);
		}

		[Test]
		public void ItShouldClearRepository()
		{
			// ACT
			this._instance.Clear();
			IList<FileRequest<ObjectExportInfo>> fileRequests = this._instance.GetFileRequests();

			// ASSERT
			CollectionAssert.IsEmpty(fileRequests);
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
			bool result5 = this._instance.AnyRequestForLocation(@"C:\temp\searchablePdf1.pdf");
			bool result5DiffCase = this._instance.AnyRequestForLocation(@"c:\Temp\SEARCHABLEpdf1.PdF");

			// ASSERT
			Assert.That(result1, Is.True);
			Assert.That(result1DiffCase, Is.True);
			Assert.That(result2, Is.False);
			Assert.That(result3, Is.False);
			Assert.That(result4, Is.False);
			Assert.That(result5, Is.True);
			Assert.That(result5DiffCase, Is.True);
		}

		private List<FileRequest<ObjectExportInfo>> CreateDataSet()
		{
			ObjectExportInfo artifact1 = new ObjectExportInfo
			{
				ArtifactID = 1
			};
			ExportRequest exportRequest1 =
				PhysicalFileExportRequest.CreateRequestForNative(artifact1, @"C:\temp\native1.docx");
			exportRequest1.FileName = "filename_1";
			exportRequest1.Order = 1;
			FileRequest<ObjectExportInfo> fileRequest1 = new FileRequest<ObjectExportInfo>(artifact1)
			{
				TransferCompleted = false,
				ExportRequest = exportRequest1,
			};

			ObjectExportInfo artifact2 = new ObjectExportInfo
			{
				ArtifactID = 2
			};
			FileRequest<ObjectExportInfo> fileRequest2 = new FileRequest<ObjectExportInfo>(artifact2)
			{
				TransferCompleted = true
			};

			ObjectExportInfo artifact3 = new ObjectExportInfo
			{
				ArtifactID = 3
			};
			ExportRequest exportRequest3 =
				PhysicalFileExportRequest.CreateRequestForPdf(artifact3, @"C:\temp\searchablePdf1.pdf");
			exportRequest3.FileName = "filename_3";
			exportRequest3.Order = 3;
			FileRequest<ObjectExportInfo> fileRequest3 = new FileRequest<ObjectExportInfo>(artifact3)
			{
				TransferCompleted = false,
				ExportRequest = exportRequest3,
			};

			return new List<FileRequest<ObjectExportInfo>> { fileRequest1, fileRequest2, fileRequest3 };
		}
	}
}