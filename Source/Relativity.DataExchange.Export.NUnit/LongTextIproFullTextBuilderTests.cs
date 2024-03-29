﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="LongTextIproFullTextBuilderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Collections.Generic;
	using System.Threading;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.DataExchange.Export.VolumeManagerV2.Repository;
	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.TestFramework;

	[TestFixture]
	public class LongTextIproFullTextBuilderTests
	{
		private LongTextIproFullTextBuilder _instance;

		private Mock<IFieldService> _fieldService;

		[SetUp]
		public void SetUp()
		{
			ExportFile exportSettings = new ExportFile(1)
			{
				LogFileFormat = LoadFileType.FileFormat.IPRO_FullText
			};
			this._fieldService = new Mock<IFieldService>();
			this._instance = new LongTextIproFullTextBuilder(new LongTextHelper(exportSettings, this._fieldService.Object, new LongTextRepository(null, new TestNullLogger())), new TestNullLogger());
		}

		[Test]
		public void ItShouldHandleShortLongText()
		{
			const string notTooLongText = "not too long text";

			ObjectExportInfo artifact = new ObjectExportInfo { Metadata = new object[] { notTooLongText } };

			this._fieldService.Setup(x => x.GetOrdinalIndex(LongTextHelper.EXTRACTED_TEXT_COLUMN_NAME)).Returns(0);

			// ACT
			IList<LongText> actualResult = this._instance.CreateLongText(artifact, CancellationToken.None);

			// ASSERT
			Assert.That(actualResult.Count, Is.EqualTo(1));
			Assert.That(actualResult[0].ExportRequest, Is.Null);
			Assert.That(actualResult[0].FieldArtifactId, Is.EqualTo(-1));
		}

		[Test]
		public void ItShouldHandleTooLongText()
		{
			const string tooLongText = ServiceConstants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN;

			ObjectExportInfo artifact = new ObjectExportInfo { Metadata = new object[] { tooLongText } };

			this._fieldService.Setup(x => x.GetOrdinalIndex(LongTextHelper.EXTRACTED_TEXT_COLUMN_NAME)).Returns(0);

			// ACT
			IList<LongText> actualResult = this._instance.CreateLongText(artifact, CancellationToken.None);

			// ASSERT
			Assert.That(actualResult.Count, Is.EqualTo(1));
			Assert.That(actualResult[0].ExportRequest, Is.Not.Null);
			Assert.That(actualResult[0].ExportRequest.FullText, Is.True);
			Assert.That(actualResult[0].FieldArtifactId, Is.EqualTo(-1));
		}
	}
}