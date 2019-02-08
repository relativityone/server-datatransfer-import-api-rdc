using System;
using kCura.WinEDDS.Core.Export;
using kCura.WinEDDS.Exporters;
using Moq;
using NUnit.Framework;
using Relativity;

namespace kCura.WinEDDS.Core.NUnit.Export
{
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

		[TestCase(typeof (ExportFileFormatter), false)]
		[TestCase(typeof (HtmlExportFileFormatter), true)]
		public void ItShouldReturnCorrectFormatterType(Type formatterType, bool isHtml)
		{
			// Arrange
			ExportFile exportFile = new ExportFile((int) ArtifactType.Document)
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