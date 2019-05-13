// -----------------------------------------------------------------------------------------------------
// <copyright file="LongTextToLoadFileTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS.LoadFileEntry;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Metadata.Text;
	using Relativity.DataExchange.Service;
	using Relativity.DataExchange.TestFramework;
	using Relativity.Logging;

	using ViewFieldInfo = kCura.WinEDDS.ViewFieldInfo;

	[TestFixture]
	public class LongTextToLoadFileTests
	{
		private LongTextToLoadFile _instance;

		private Mock<IFieldService> _fieldService;
		private Mock<ILongTextHandler> _tooLongTextToLoadFile;
		private Mock<ILongTextHandler> _notTooLongTextToLoadFile;

		[SetUp]
		public void SetUp()
		{
			ExportFile exportSettings = new ExportFile(1);

			this._fieldService = new Mock<IFieldService>();

			LongTextHelper longTextHelper = new LongTextHelper(exportSettings, this._fieldService.Object, null);

			this._tooLongTextToLoadFile = new Mock<ILongTextHandler>();
			this._notTooLongTextToLoadFile = new Mock<ILongTextHandler>();

			this._instance = new LongTextToLoadFile(longTextHelper, this._tooLongTextToLoadFile.Object, this._notTooLongTextToLoadFile.Object, new NullLogger());
		}

		[Test]
		public void ItShouldHandleLongText()
		{
			ObjectExportInfo artifact = new ObjectExportInfo { Metadata = new object[] { "not too long text" } };

			ViewFieldInfo field = new QueryFieldFactory().GetArtifactIdField();

			DeferredEntry lineEntry = new DeferredEntry();

			this._fieldService.Setup(x => x.GetOrdinalIndex(It.IsAny<string>())).Returns(0);

			// ACT
			this._instance.HandleLongText(artifact, field, lineEntry);

			// ASSERT
			this._notTooLongTextToLoadFile.Verify(x => x.HandleLongText(artifact, field, lineEntry));
		}

		[Test]
		public void ItShouldHandleTooLongText()
		{
			ObjectExportInfo artifact = new ObjectExportInfo
			{
				Metadata = new object[] { ServiceConstants.LONG_TEXT_EXCEEDS_MAX_LENGTH_FOR_LIST_TOKEN }
			};

			ViewFieldInfo field = new QueryFieldFactory().GetArtifactIdField();

			DeferredEntry lineEntry = new DeferredEntry();

			this._fieldService.Setup(x => x.GetOrdinalIndex(It.IsAny<string>())).Returns(0);

			// ACT
			this._instance.HandleLongText(artifact, field, lineEntry);

			// ASSERT
			this._tooLongTextToLoadFile.Verify(x => x.HandleLongText(artifact, field, lineEntry));
		}
	}
}