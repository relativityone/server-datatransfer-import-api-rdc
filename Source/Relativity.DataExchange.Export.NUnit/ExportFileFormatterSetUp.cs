// -----------------------------------------------------------------------------------------------------
// <copyright file="ExportFileFormatterSetUp.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.Collections.Generic;

	using global::NUnit.Framework;

	using kCura.WinEDDS;
	using kCura.WinEDDS.Exporters;

	using Moq;

	using Relativity.DataExchange.Export;
	using Relativity.DataExchange.Service;

	public class ExportFileFormatterSetUp<T>
	    where T : ExportFileFormatterBase
	{
		protected const string FileName1 = "Name1";
		protected const string FieldName2 = "Name2";
		protected const char RecordDelimiter = ',';
		protected const char QuoteDelimiter = '"';

		protected T SubjectUnderTest { get; set; }

		protected Mock<IFieldNameProvider> FieldNameProviderMock { get; set; }

		protected ExportFile ExpFile { get; set; }

		protected kCura.WinEDDS.ViewFieldInfo[] Fields { get; set; }

		[SetUp]
		public void Init()
		{
			this.InitTestCase();
		}

		protected virtual void InitTestCase()
		{
			this.FieldNameProviderMock = new Mock<IFieldNameProvider>();

			this.FieldNameProviderMock.Setup(mock => mock.GetDisplayName(It.IsAny<kCura.WinEDDS.ViewFieldInfo>()))
				.Returns((kCura.WinEDDS.ViewFieldInfo field) => field.DisplayName);

			int index = 1;
			this.Fields = ViewFieldInfoMockFactory.CreateMockedViewFieldInfoArray(new List<Tuple<int, string>>
			{
				Tuple.Create(index++, FileName1),
				Tuple.Create(index, FieldName2)
			});

			this.ExpFile = new ExportFile((int)ArtifactType.Document)
				          {
					          QuoteDelimiter = QuoteDelimiter, RecordDelimiter = RecordDelimiter
				          };
		}
	}
}