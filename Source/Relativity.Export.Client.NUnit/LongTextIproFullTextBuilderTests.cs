﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="LongTextIproFullTextBuilderTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
{
    using System.Collections.Generic;
    using System.Threading;

    using global::NUnit.Framework;

	using kCura.WinEDDS;
    using kCura.WinEDDS.Exporters;

    using Moq;

	using Relativity.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.Export.VolumeManagerV2.Repository;
    using Relativity.Import.Export.Service;
    using Relativity.Logging;

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
			_fieldService = new Mock<IFieldService>();
			_instance = new LongTextIproFullTextBuilder(new LongTextHelper(exportSettings, _fieldService.Object, new LongTextRepository(null, new NullLogger())), new NullLogger());
		}

		[Test]
		public void ItShouldHandleShortLongText()
		{
			const string notTooLongText = "not too long text";

			ObjectExportInfo artifact = new ObjectExportInfo { Metadata = new object[] { notTooLongText } };

			_fieldService.Setup(x => x.GetOrdinalIndex(LongTextHelper.EXTRACTED_TEXT_COLUMN_NAME)).Returns(0);

			// ACT
			IList<LongText> actualResult = _instance.CreateLongText(artifact, CancellationToken.None);

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

			_fieldService.Setup(x => x.GetOrdinalIndex(LongTextHelper.EXTRACTED_TEXT_COLUMN_NAME)).Returns(0);

			// ACT
			IList<LongText> actualResult = _instance.CreateLongText(artifact, CancellationToken.None);

			// ASSERT
			Assert.That(actualResult.Count, Is.EqualTo(1));
			Assert.That(actualResult[0].ExportRequest, Is.Not.Null);
			Assert.That(actualResult[0].ExportRequest.FullText, Is.True);
			Assert.That(actualResult[0].FieldArtifactId, Is.EqualTo(-1));
		}
	}
}