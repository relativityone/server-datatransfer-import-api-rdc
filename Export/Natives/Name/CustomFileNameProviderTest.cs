using System.Collections.Generic;
using FileNaming.CustomFileNaming;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.FileNaming.CustomFileNaming;
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
			var thirdDescriptor = new FieldDescriptorPart(1);

			string firstPartName = "First";
			string secondPartName = "_";
			string thirdPartName = "Second";

			Mock<IFileNamePartProvider> firstfieldProviderMock = new Mock<IFileNamePartProvider>();
			Mock<IFileNamePartProvider> separatorProviderMock = new Mock<IFileNamePartProvider>();
			Mock<IFileNamePartProvider> secondfieldProviderMock = new Mock<IFileNamePartProvider>();


			firstfieldProviderMock.Setup(mock => mock.GetPartName(firstDescriptor, _exportObjectInfo)).Returns(firstPartName);
			separatorProviderMock.Setup(mock => mock.GetPartName(secondDescriptor, _exportObjectInfo)).Returns(secondPartName);
			secondfieldProviderMock.Setup(mock => mock.GetPartName(thirdDescriptor, _exportObjectInfo)).Returns(thirdPartName);

			_fileNamePartProviderContainerMock.Setup(mock => mock.GetProvider(firstDescriptor)).Returns(firstfieldProviderMock.Object);
			_fileNamePartProviderContainerMock.Setup(mock => mock.GetProvider(secondDescriptor)).Returns(separatorProviderMock.Object);
			_fileNamePartProviderContainerMock.Setup(mock => mock.GetProvider(thirdDescriptor)).Returns(secondfieldProviderMock.Object);

			_subjectUnderTest = new CustomFileNameProvider(new List<DescriptorPart>
				{
					firstDescriptor,
					secondDescriptor,
					thirdDescriptor
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
			Assert.That(retFileName, Is.EqualTo($"{firstPartName}{secondPartName}{thirdPartName}.{extension}"));
		}


		[Test]
		public void ItShouldRemoveDoubleSeparators()
		{
			// Arrange
			var firstDescriptor = new FieldDescriptorPart(1);
			var secondDescriptor = new SeparatorDescriptorPart("");
			var thirdDescriptor = new FieldDescriptorPart(1);

			string firstPartName = "First";
			string secondPartName = "_";
			string thirdPartName = "";

			Mock<IFileNamePartProvider> firstfieldProviderMock = new Mock<IFileNamePartProvider>();
			Mock<IFileNamePartProvider> separatorProviderMock = new Mock<IFileNamePartProvider>();
			Mock<IFileNamePartProvider> secondfieldProviderMock = new Mock<IFileNamePartProvider>();


			firstfieldProviderMock.Setup(mock => mock.GetPartName(firstDescriptor, _exportObjectInfo)).Returns(firstPartName);
			separatorProviderMock.Setup(mock => mock.GetPartName(secondDescriptor, _exportObjectInfo)).Returns(secondPartName);
			secondfieldProviderMock.Setup(mock => mock.GetPartName(thirdDescriptor, _exportObjectInfo)).Returns(thirdPartName);

			_fileNamePartProviderContainerMock.Setup(mock => mock.GetProvider(firstDescriptor)).Returns(firstfieldProviderMock.Object);
			_fileNamePartProviderContainerMock.Setup(mock => mock.GetProvider(secondDescriptor)).Returns(separatorProviderMock.Object);
			_fileNamePartProviderContainerMock.Setup(mock => mock.GetProvider(thirdDescriptor)).Returns(secondfieldProviderMock.Object);

			_subjectUnderTest = new CustomFileNameProvider(new List<DescriptorPart>
				{
					firstDescriptor,
					secondDescriptor,
					thirdDescriptor
				},
				_fileNamePartProviderContainerMock.Object);

			// Act
			string retFileName = _subjectUnderTest.GetTextName(_exportObjectInfo);
			string extension = TextExtension;



			// Assert
			Assert.That(retFileName, Is.EqualTo($"{firstPartName}{thirdPartName}.{extension}"));
		}

	}
}
