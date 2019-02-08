using System.Collections.Generic;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Settings;
using NUnit.Framework;
using Relativity.Logging;
using Constants = Relativity.Export.Constants;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Settings
{
	[TestFixture]
	public class ColumnsOrdinalLookupFactoryTests
	{
		private ColumnsOrdinalLookupFactory _instance;

		[SetUp]
		public void SetUp()
		{
			_instance = new ColumnsOrdinalLookupFactory(new NullLogger());
		}

		[Test]
		[TestCaseSource(nameof(ColumnsInOrderSets))]
		public void ItShouldCreateOrdinalLookupWithoutTextPrecedence(string[] columnsInOrder)
		{
			ExportFile exportSettings = new ExportFile(1);

			//ACT
			Dictionary<string, int> ordinalLookup = _instance.CreateOrdinalLookup(exportSettings, columnsInOrder);

			//ASSERT
			for (int i = 0; i < columnsInOrder.Length; i++)
			{
				Assert.That(i, Is.EqualTo(ordinalLookup[columnsInOrder[i]]));
			}

			Assert.That(ordinalLookup.Count, Is.EqualTo(columnsInOrder.Length));
		}

		[Test]
		[TestCaseSource(nameof(ColumnsInOrderSets))]
		public void ItShouldCreateOrdinalLookupWithTextPrecedence(string[] columnsInOrder)
		{
			ExportFile exportSettings = new ExportFile(1)
			{
				SelectedTextFields = new ViewFieldInfo[1]
			};

			//ACT
			Dictionary<string, int> ordinalLookup = _instance.CreateOrdinalLookup(exportSettings, columnsInOrder);

			//ASSERT
			for (int i = 0; i < columnsInOrder.Length; i++)
			{
				Assert.That(i, Is.EqualTo(ordinalLookup[columnsInOrder[i]]));
			}

			Assert.That(columnsInOrder.Length, Is.EqualTo(ordinalLookup[Constants.TEXT_PRECEDENCE_AWARE_ORIGINALSOURCE_AVF_COLUMN_NAME]));
			Assert.That(columnsInOrder.Length + 1, Is.EqualTo(ordinalLookup[Constants.TEXT_PRECEDENCE_AWARE_AVF_COLUMN_NAME]));

			Assert.That(ordinalLookup.Count, Is.EqualTo(columnsInOrder.Length + 2));
		}

		private static IEnumerable<string[]> ColumnsInOrderSets()
		{
			yield return new[] {"Column"};
			yield return new[] {"Column1", "Column2", "Column3"};
			yield return new[] {"Column3", "Column2", "Column1"};
			yield return new string[0];
		}
	}
}