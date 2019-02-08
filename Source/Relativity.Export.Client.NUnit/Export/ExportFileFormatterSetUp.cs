using System;
using System.Collections.Generic;
using kCura.WinEDDS.Core.Export;
using kCura.WinEDDS.Core.NUnit.Helpers;
using kCura.WinEDDS.Exporters;
using Moq;
using NUnit.Framework;
using Relativity;

namespace kCura.WinEDDS.Core.NUnit.Export
{
	public class ExportFileFormatterSetUp<T> where T : ExportFileFormatterBase
	{
		protected T SubjectUnderTest;

		protected Mock<IFieldNameProvider> FieldNameProviderMock;
		protected ExportFile ExpFile;
		protected ViewFieldInfo[] Fields;

		protected const string FIELD_NAME_1 = "Name1";
		protected const string FIELD_NAME_2 = "Name2";

		protected const char RECORD_DELIMITER = ',';
		protected const char QUOTE_DELIMITER = '"';

		[SetUp]
		public void Init()
		{
			InitTestCase();
		}

		protected virtual void InitTestCase()
		{
			FieldNameProviderMock = new Mock<IFieldNameProvider>();

			FieldNameProviderMock.Setup(mock => mock.GetDisplayName(It.IsAny<ViewFieldInfo>()))
				.Returns((ViewFieldInfo field) => field.DisplayName);

			int index = 1;
			Fields = ViewFieldInfoMockFactory.CreateMockedViewFieldInfoArray(new List<Tuple<int, string>>
			{
				Tuple.Create(index++, FIELD_NAME_1),
				Tuple.Create(index, FIELD_NAME_2)
			});

			ExpFile = new ExportFile((int)ArtifactType.Document);

			ExpFile.QuoteDelimiter = QUOTE_DELIMITER;
			ExpFile.RecordDelimiter = RECORD_DELIMITER;
		}
	}
}
