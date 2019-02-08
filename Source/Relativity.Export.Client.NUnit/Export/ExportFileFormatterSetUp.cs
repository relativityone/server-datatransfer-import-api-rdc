// ----------------------------------------------------------------------------
// <copyright file="ExportFileFormatterSetUp.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------

namespace Relativity.Export.Client.NUnit.Export
{
    using System;
    using System.Collections.Generic;

    using kCura.WinEDDS;
    using kCura.WinEDDS.Core.Export;
    using kCura.WinEDDS.Exporters;

    using Relativity.Export.Client.NUnit.Helpers;
    
    using Moq;

    using global::NUnit.Framework;

    using Relativity;

    public class ExportFileFormatterSetUp<T> where T : ExportFileFormatterBase
	{
		protected T SubjectUnderTest;

		protected Mock<IFieldNameProvider> FieldNameProviderMock;
		protected ExportFile ExpFile;
		protected kCura.WinEDDS.ViewFieldInfo[] Fields;

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
            
			FieldNameProviderMock.Setup(mock => mock.GetDisplayName(It.IsAny<kCura.WinEDDS.ViewFieldInfo>()))
				.Returns((kCura.WinEDDS.ViewFieldInfo field) => field.DisplayName);

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
