using System.Collections.Generic;
using kCura.WinEDDS.Core.Export.Natives.Name;
using kCura.WinEDDS.Core.Export.Natives.Name.Factories;
using kCura.WinEDDS.Core.Model;
using kCura.WinEDDS.Exporters;
using Moq;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Export.Natives.Name
{
	public class CustomFileNameProviderTest
	{
		private CustomFileNameProvider _subjectUnderTest;

		private ObjectExportInfo _exportObjectInfo;

		private const string TextExtension = "txt";
		private const string NativeExtension = "xls";

		private Mock<IFileNamePartProviderContainer> _fileNamePartProviderContainerMock;

		[SetUp]
		public void Init()
		{
			_exportObjectInfo = new ObjectExportInfo();

			_exportObjectInfo.NativeExtension = NativeExtension;

			_fileNamePartProviderContainerMock = new Mock<IFileNamePartProviderContainer>();
		}

		[Test]
		[TestCase(true)]
		[TestCase(false)]
		public void ItShouldReturnFileNameTest(bool nativeTypeExport)
		{
			// Arrange
			var firstDescriptor = new FieldDescriptorPart(1);
			var secondDescriptor = new SeparatorDescriptorPart("");

			string firstPartName = "First";
			string secondPartName = "Second";

			Mock<IFileNamePartProvider> separatorProviderMock = new Mock<IFileNamePartProvider>();
			Mock<IFileNamePartProvider> fieldProviderMock = new Mock<IFileNamePartProvider>();

			separatorProviderMock.Setup(mock => mock.GetPartName(firstDescriptor, _exportObjectInfo)).Returns(firstPartName);
			fieldProviderMock.Setup(mock => mock.GetPartName(secondDescriptor, _exportObjectInfo)).Returns(secondPartName);

			_fileNamePartProviderContainerMock.Setup(mock => mock.GetProvider(firstDescriptor)).Returns(separatorProviderMock.Object);
			_fileNamePartProviderContainerMock.Setup(mock => mock.GetProvider(secondDescriptor)).Returns(fieldProviderMock.Object);

			_subjectUnderTest = new CustomFileNameProvider(new List<DescriptorPart>
				{
					firstDescriptor,
					secondDescriptor
				}, 
				_fileNamePartProviderContainerMock.Object);

			// Act
			string retFileName;
			string extension;
			if (nativeTypeExport)
			{
				retFileName = _subjectUnderTest.GetName(_exportObjectInfo);
				extension = NativeExtension;
			}
			else
			{
				retFileName = _subjectUnderTest.GetTextName(_exportObjectInfo);
				extension = TextExtension;
			}
			

			// Assert
			Assert.That(retFileName, Is.EqualTo($"{firstPartName}{secondPartName}.{extension}"));
		}
	}
}
