using kCura.WinEDDS.Core.Export.VolumeManagerV2.Settings;
using kCura.WinEDDS.NUnit.TestObjectFactories;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Settings
{
	[TestFixture]
	public class FieldServiceFactoryTests
	{
		private FieldServiceFactory _instance;
		private Mock<IColumnsOrdinalLookupFactory> _columnsOrdinalLookup;
		private Mock<IColumnsFactory> _columnsFactory;

		[SetUp]
		public void SetUp()
		{
			_columnsOrdinalLookup = new Mock<IColumnsOrdinalLookupFactory>();
			_columnsFactory = new Mock<IColumnsFactory>();

			_instance = new FieldServiceFactory(_columnsOrdinalLookup.Object, _columnsFactory.Object, new NullLogger());
		}

		[Test]
		[TestCase(true, false, ExpectedResult = true)]
		[TestCase(false, true, ExpectedResult = true)]
		[TestCase(false, false, ExpectedResult = false)]
		public bool ItShouldUpdateExportFullTextSettingAccordingly(bool exportFullText, bool containsFullText)
		{
			ViewFieldInfo[] fields = new ViewFieldInfo[1];

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

			//ACT
			_instance.Create(exportSettings, null, string.Empty);

			//ASSERT
			return exportSettings.ExportFullText;
		}

		[Test]
		public void GoldWorkflow()
		{
			var exportSettings = new ExportFile(1)
			{
				SelectedViewFields = new ViewFieldInfo[0]
			};
			var columnNamesInOrder = new string[0];

			ViewFieldInfo[] fields = new ViewFieldInfo[1];
			_columnsFactory.Setup(x => x.CreateColumns(exportSettings)).Returns(fields);

			//ACT
			_instance.Create(exportSettings, columnNamesInOrder, string.Empty);

			//ASSERT
			_columnsFactory.Verify(x => x.CreateColumns(exportSettings));
			_columnsOrdinalLookup.Verify(x => x.CreateOrdinalLookup(exportSettings, columnNamesInOrder));
		}
	}
}