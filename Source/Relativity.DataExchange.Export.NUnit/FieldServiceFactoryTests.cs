// -----------------------------------------------------------------------------------------------------
// <copyright file="FieldServiceFactoryTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System.Collections.Generic;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Settings;
	using Relativity.DataExchange.TestFramework;

	[TestFixture]
	public class FieldServiceFactoryTests
	{
		private FieldServiceFactory _instance;
		private Mock<ILoadFileHeaderFormatter> _formatter;
		private Mock<IColumnsOrdinalLookupFactory> _columnsOrdinalLookup;
		private Mock<IColumnsFactory> _columnsFactory;

		[SetUp]
		public void SetUp()
		{
			this._formatter = new Mock<ILoadFileHeaderFormatter>();
			this._columnsOrdinalLookup = new Mock<IColumnsOrdinalLookupFactory>();
			this._columnsFactory = new Mock<IColumnsFactory>();

			this._instance = new FieldServiceFactory(this._formatter.Object, this._columnsOrdinalLookup.Object, this._columnsFactory.Object, new TestNullLogger());
		}

		[Test]
		[TestCase(true, false, ExpectedResult = true)]
		[TestCase(false, true, ExpectedResult = true)]
		[TestCase(false, false, ExpectedResult = false)]
		public bool ItShouldUpdateExportFullTextSettingAccordingly(bool exportFullText, bool containsFullText)
		{
            kCura.WinEDDS.ViewFieldInfo[] fields = new kCura.WinEDDS.ViewFieldInfo[1];

			if (containsFullText)
			{
				fields[0] = new QueryFieldFactory().GetExtractedTextField();
			}
			else
			{
				fields[0] = new QueryFieldFactory().GetArtifactIdField();
			}

			ExportFile exportSettings = new ExportFile(1)
			{
				ExportFullText = exportFullText,
				SelectedViewFields = fields
			};

			// ACT
			this._instance.Create(exportSettings, null);

			// ASSERT
			return exportSettings.ExportFullText;
		}

		[Test]
		public void GoldWorkflow()
		{
			var exportSettings = new ExportFile(1)
			{
				SelectedViewFields = new kCura.WinEDDS.ViewFieldInfo[0]
			};
			var columnNamesInOrder = new string[0];

            kCura.WinEDDS.ViewFieldInfo[] fields = new kCura.WinEDDS.ViewFieldInfo[1];
			this._columnsFactory.Setup(x => x.CreateColumns(exportSettings)).Returns(fields);

			// ACT
			this._instance.Create(exportSettings, columnNamesInOrder);

			// ASSERT
			this._columnsFactory.Verify(x => x.CreateColumns(exportSettings));
			this._formatter.Verify(x => x.GetHeader(It.IsAny<List<kCura.WinEDDS.ViewFieldInfo>>()));
			this._columnsOrdinalLookup.Verify(x => x.CreateOrdinalLookup(exportSettings, columnNamesInOrder));
		}
	}
}