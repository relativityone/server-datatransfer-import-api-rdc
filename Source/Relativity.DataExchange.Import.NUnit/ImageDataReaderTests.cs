// -----------------------------------------------------------------------------------------------------
// <copyright file="ImageDataReaderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Import.NUnit
{
	using System;
	using System.Data;

	using global::NUnit.Framework;

	using kCura.Relativity.DataReaderClient;
	using kCura.WinEDDS.Api;
	using kCura.WinEDDS.ImportExtension;

	using Moq;

	[TestFixture]
	public class ImageDataReaderTests
	{
		private const string TestBatesNumber = "TEST_BATES_NUMBER";
		private const string TestDocumentIdentifier = "TEST_DOCUMENT_IDENTIFIER";

		private const string TestFileGuid = "207d79b9-f3d8-47cd-bdd3-6d355725c27f";
		private const string TestFileLocation = "C:/test_location/" + TestFileGuid;
		private const string TestFileName = "test_image.jpg";

		private ImageDataReader reader;
		private Mock<IDataReader> dataReaderMock;
		private ImageSettings imageSettings;

		[SetUp]
		public void SetUp()
		{
			this.dataReaderMock = new Mock<IDataReader>();
			this.dataReaderMock.Setup(dataReader => dataReader.IsClosed).Returns(false);
			this.dataReaderMock.Setup(dataReader => dataReader.Read()).Returns(true);
			this.dataReaderMock.Setup(dataReader => dataReader.GetName(0)).Returns("Bates_number");
			this.dataReaderMock.Setup(dataReader => dataReader.GetName(1)).Returns("Document_identifier");
			this.dataReaderMock.Setup(dataReader => dataReader.GetName(2)).Returns("File_location");
			this.dataReaderMock.Setup(dataReader => dataReader.GetName(3)).Returns("File_name");
			this.dataReaderMock.Setup(dataReader => dataReader.FieldCount).Returns(4);

			this.imageSettings = new ImageSettings();
			this.imageSettings.BatesNumberField = "Bates_number";
			this.imageSettings.DocumentIdentifierField = "Document_identifier";
			this.imageSettings.FileLocationField = "File_location";
			this.imageSettings.FileNameField = "File_name";
		}

		[Test]
		public void GetImageRecordWithFileNamesTest()
		{
			// arrange
			this.reader = new ImageDataReader(this.dataReaderMock.Object, this.imageSettings);

			this.dataReaderMock.SetupSequence(dataReader => dataReader.GetString(0))
				.Returns($"{TestBatesNumber}_0001")
				.Returns($"{TestBatesNumber}_0002");
			this.dataReaderMock.Setup(dataReader => dataReader.GetString(1))
				.Returns(TestDocumentIdentifier);
			this.dataReaderMock.SetupSequence(dataReader => dataReader.GetString(2))
				.Returns($"{TestFileLocation}_0001")
				.Returns($"{TestFileLocation}_0002");
			this.dataReaderMock.SetupSequence(dataReader => dataReader.GetString(3))
				.Returns($"0001_{TestFileName}")
				.Returns($"0002_{TestFileName}");

			// act
			ImageRecord firstImageRecord = this.reader.GetImageRecord();
			ImageRecord secondImageRecord = this.reader.GetImageRecord();

			// asert
			Assert.AreEqual($"{TestBatesNumber}_0001", firstImageRecord.BatesNumber);
			Assert.AreEqual($"{TestFileLocation}_0001", firstImageRecord.FileLocation);
			Assert.AreEqual($"0001_{TestFileName}", firstImageRecord.FileName);
			Assert.AreEqual(true, firstImageRecord.IsNewDoc);

			Assert.AreEqual($"{TestBatesNumber}_0002", secondImageRecord.BatesNumber);
			Assert.AreEqual($"{TestFileLocation}_0002", secondImageRecord.FileLocation);
			Assert.AreEqual($"0002_{TestFileName}", secondImageRecord.FileName);
			Assert.AreEqual(false, secondImageRecord.IsNewDoc);
		}

		[Test]
		public void GetImageRecordWithoutFileNamesTest()
		{
			// arrange
			this.reader = new ImageDataReader(this.dataReaderMock.Object, this.imageSettings);

			this.dataReaderMock.SetupSequence(dataReader => dataReader.GetString(0))
				.Returns($"{TestBatesNumber}_0001")
				.Returns($"{TestBatesNumber}_0002");
			this.dataReaderMock.Setup(dataReader => dataReader.GetString(1))
				.Returns(TestDocumentIdentifier);
			this.dataReaderMock.SetupSequence(dataReader => dataReader.GetString(2))
				.Returns($"{TestFileLocation}_0001")
				.Returns($"{TestFileLocation}_0002");

			// act
			ImageRecord firstImageRecord = this.reader.GetImageRecord();
			ImageRecord secondImageRecord = this.reader.GetImageRecord();

			// asert
			Assert.AreEqual($"{TestBatesNumber}_0001", firstImageRecord.BatesNumber);
			Assert.AreEqual($"{TestFileLocation}_0001", firstImageRecord.FileLocation);
			Assert.AreEqual(null, firstImageRecord.FileName);
			Assert.AreEqual(true, firstImageRecord.IsNewDoc);

			Assert.AreEqual($"{TestBatesNumber}_0002", secondImageRecord.BatesNumber);
			Assert.AreEqual($"{TestFileLocation}_0002", secondImageRecord.FileLocation);
			Assert.AreEqual(null, secondImageRecord.FileName);
			Assert.AreEqual(false, secondImageRecord.IsNewDoc);
		}

		[Test]
		public void CurrentRecordNumberTest()
		{
			// arrange
			this.reader = new ImageDataReader(this.dataReaderMock.Object, this.imageSettings);

			// act
			int recordNumberBeforeFirstAdvance = this.reader.CurrentRecordNumber;
			this.reader.AdvanceRecord();
			int recordNumberAfterFirstAdvance = this.reader.CurrentRecordNumber;
			this.reader.GetImageRecord();
			int recordNumberAfterSecondAdvance = this.reader.CurrentRecordNumber;

			// assert
			Assert.AreEqual(0, recordNumberBeforeFirstAdvance);
			Assert.AreEqual(1, recordNumberAfterFirstAdvance);
			Assert.AreEqual(2, recordNumberAfterSecondAdvance);
		}

		[Test]
		public void HasMoreRecordsTest()
		{
			// arrange
			this.reader = new ImageDataReader(this.dataReaderMock.Object, this.imageSettings);

			// act
			bool hasMoreRecords = this.reader.HasMoreRecords;

			// assert
			Assert.AreEqual(true, hasMoreRecords);
		}

		[Test]
		public void HasMoreRecordsWhenReaderClosedTest()
		{
			// arrange
			this.reader = new ImageDataReader(this.dataReaderMock.Object, this.imageSettings);
			this.dataReaderMock.Setup(dataReader => dataReader.IsClosed).Returns(true);

			// act
			bool hasMoreRecords = this.reader.HasMoreRecords;

			// assert
			Assert.AreEqual(false, hasMoreRecords);
		}

		[Test]
		public void CountRecordsOpenedReaderTest()
		{
			// arrange
			this.reader = new ImageDataReader(this.dataReaderMock.Object, this.imageSettings);

			// act
			long? recordCountBeforeAdvance = this.reader.CountRecords();
			this.reader.AdvanceRecord();
			long? recordCountAfterAdvance = this.reader.CountRecords();

			// assert
			Assert.IsNull(recordCountBeforeAdvance);
			Assert.IsNull(recordCountAfterAdvance);
		}

		[Test]
		public void CountRecordsClosedReaderTest()
		{
			// arrange
			this.reader = new ImageDataReader(this.dataReaderMock.Object, this.imageSettings);
			this.reader.AdvanceRecord();
			this.reader.AdvanceRecord();

			// act
			this.dataReaderMock.Setup(dataReader => dataReader.IsClosed).Returns(true);
			this.dataReaderMock.Setup(dataReader => dataReader.Read()).Returns(false);
			long? recordCountBeforeAdvance = this.reader.CountRecords();
			this.reader.AdvanceRecord();
			long? recordCountAfterAdvance = this.reader.CountRecords();

			// assert
			Assert.AreEqual(2, recordCountBeforeAdvance.GetValueOrDefault());
			Assert.AreEqual(3, recordCountAfterAdvance.GetValueOrDefault());
		}

		[Test]
		public void InvalidBatesNumberFieldNameTest()
		{
			// arrange
			this.imageSettings.BatesNumberField = "BatesNumber!";

			// act && assert
			Assert.Throws<ArgumentException>(() => new ImageDataReader(this.dataReaderMock.Object, this.imageSettings));
		}

		[Test]
		public void InvalidDocumentIdentifierFieldNameTest()
		{
			// arrange
			this.imageSettings.DocumentIdentifierField = "DocumentIdentifier!";

			// act && assert
			Assert.Throws<ArgumentException>(() => new ImageDataReader(this.dataReaderMock.Object, this.imageSettings));
		}

		[Test]
		public void InvalidFileLocationFieldNameTest()
		{
			// arrange
			this.imageSettings.FileLocationField = "FileLocation!";

			// act && assert
			Assert.Throws<ArgumentException>(() => new ImageDataReader(this.dataReaderMock.Object, this.imageSettings));
		}

		[Test]
		public void DataReaderIsClosedTest()
		{
			// arrange
			this.dataReaderMock.Setup(dataReader => dataReader.IsClosed).Returns(true);

			// act && assert
			Assert.Throws<ArgumentException>(() => new ImageDataReader(this.dataReaderMock.Object, this.imageSettings));
		}

		[Test]
		public void DataReaderIsNullTest()
		{
			// act && assert
			Assert.Throws<ArgumentNullException>(() => new ImageDataReader(null, this.imageSettings));
		}
	}
}