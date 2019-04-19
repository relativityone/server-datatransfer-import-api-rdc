// -----------------------------------------------------------------------------------------------------
// <copyright file="ExportFileFormatterFactoryTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
{
    using System;

    using global::NUnit.Framework;

	using kCura.WinEDDS;
    using kCura.WinEDDS.Exporters;

    using Moq;

    using Relativity.Import.Export.Services;

	public class ExportFileFormatterFactoryTests
	{
		private Mock<IFieldNameProvider> _fieldNameProviderMock;
		private ExportFileFormatterFactory _subjectUnderTest;

		[SetUp]
		public void Init()
		{
			_fieldNameProviderMock = new Mock<IFieldNameProvider>();

			_subjectUnderTest = new ExportFileFormatterFactory(_fieldNameProviderMock.Object);
		}

		[TestCase(typeof(ExportFileFormatter), false)]
		[TestCase(typeof(HtmlExportFileFormatter), true)]
		public void ItShouldReturnCorrectFormatterType(Type formatterType, bool isHtml)
		{
			// Arrange
			ExportFile exportFile = new ExportFile((int)ArtifactType.Document)
			{
				LoadFileIsHtml = isHtml
			};

			// Act
			ILoadFileHeaderFormatter retFormatter = _subjectUnderTest.Create(exportFile);

			// Assert
			Assert.IsInstanceOf(formatterType, retFormatter);
		}
	}
}