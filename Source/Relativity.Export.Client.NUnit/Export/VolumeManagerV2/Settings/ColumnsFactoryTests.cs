using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Settings;
using kCura.WinEDDS.NUnit.TestObjectFactories;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Settings
{
	[TestFixture]
	public class ColumnsFactoryTests
	{
		private ColumnsFactory _instance;

		[SetUp]
		public void SetUp()
		{
			_instance = new ColumnsFactory(new NullLogger());
		}

		[Test]
		public void ItShouldNotModifyColumnsWhenTextPrecedenceIsNotSet()
		{
			ViewFieldInfo[] fields = new QueryFieldFactory().GetAllDocumentFields();

			ExportFile exportSettings = new ExportFile(1)
			{
				SelectedTextFields = new ViewFieldInfo[0],
				SelectedViewFields = fields
			};

			//ACT
			ViewFieldInfo[] actualFields = _instance.CreateColumns(exportSettings);

			//ASSERT
			CollectionAssert.AreEquivalent(fields, actualFields);
		}

		[Test]
		[TestCase(2)]
		[TestCase(5)]
		[TestCase(99)]
		public void ItShouldAddTextPrecedenceFieldIfMultipleTextFieldsSelected(int numberOfTextFields)
		{
			QueryFieldFactory fieldFactory = new QueryFieldFactory();
			ViewFieldInfo[] fields = fieldFactory.GetAllDocumentFields();

			List<ViewFieldInfo> textFields = new List<ViewFieldInfo>();
			for (int i = 0; i < numberOfTextFields; i++)
			{
				textFields.Add(fieldFactory.GetExtractedTextField());
			}

			ExportFile exportSettings = new ExportFile(1)
			{
				SelectedTextFields = textFields.ToArray(),
				SelectedViewFields = fields
			};

			//ACT
			ViewFieldInfo[] actualFields = _instance.CreateColumns(exportSettings);

			//ASSERT
			CollectionAssert.IsSubsetOf(fields, actualFields);
			Assert.That(actualFields.Any(x => x is CoalescedTextViewField));
			Assert.That(actualFields.Length, Is.EqualTo(fields.Length + 1));
		}

		[Test]
		public void ItShouldReplaceOnlyTextFieldWithTextPrecedenceUsingOldFieldName()
		{
			QueryFieldFactory fieldFactory = new QueryFieldFactory();
			ViewFieldInfo[] fields = fieldFactory.GetAllDocumentFields();

			ViewFieldInfo[] textFields = {fieldFactory.GetExtractedTextField()};

			ViewFieldInfo fieldToReplace = fields.First(x => x.Equals(textFields[0]));
			int indexOfFieldToReplace = fields.ToList().IndexOf(fieldToReplace);

			ExportFile exportSettings = new ExportFile(1)
			{
				SelectedTextFields = textFields,
				SelectedViewFields = fields
			};

			//ACT
			ViewFieldInfo[] actualFields = _instance.CreateColumns(exportSettings);

			//ASSERT
			Assert.That(actualFields.Length, Is.EqualTo(fields.Length));
			for (int i = 0; i < fields.Length; i++)
			{
				if (i == indexOfFieldToReplace)
				{
					Assert.That(actualFields[i], Is.InstanceOf<CoalescedTextViewField>());
					Assert.That(actualFields[i].DisplayName, Is.EqualTo(fields[i].DisplayName));
					Assert.That(actualFields[i].AvfColumnName, Is.EqualTo("Text Precedence"));
				}
				else
				{
					Assert.That(actualFields[i], Is.EqualTo(fields[i]));
				}
			}
		}

		[Test]
		public void ItShouldAddTextPrecedenceFieldIfSourceFieldNotMapped()
		{
			QueryFieldFactory fieldFactory = new QueryFieldFactory();
			ViewFieldInfo[] fields = fieldFactory.GetAllDocumentFields().Where(x => x.AvfColumnName != "ExtractedText").ToArray();

			ViewFieldInfo[] textFields = {fieldFactory.GetExtractedTextField()};

			ExportFile exportSettings = new ExportFile(1)
			{
				SelectedTextFields = textFields,
				SelectedViewFields = fields
			};

			//ACT
			ViewFieldInfo[] actualFields = _instance.CreateColumns(exportSettings);

			//ASSERT
			Assert.That(actualFields.Length, Is.EqualTo(fields.Length + 1));
			CollectionAssert.IsSubsetOf(fields, actualFields);
			Assert.That(actualFields.Last().AvfColumnName, Is.EqualTo("Text Precedence"));
			Assert.That(actualFields.Last().DisplayName, Is.EqualTo("Text Precedence"));
		}
	}
}