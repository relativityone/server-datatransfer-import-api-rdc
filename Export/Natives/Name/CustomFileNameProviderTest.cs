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
			_exportObjectInfo.OriginalFileName = "OriginalFileName.html";
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
				_fileNamePartProviderContainerMock.Object, false);

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
				_fileNamePartProviderContainerMock.Object, false);

			// Act
			string retFileName = _subjectUnderTest.GetTextName(_exportObjectInfo);
			string extension = TextExtension;



			// Assert
			Assert.That(retFileName, Is.EqualTo($"{firstPartName}.{extension}"));
		}


		[Test]
		public void ItShouldReturnOnlyControlNumberWhenGivenOneField()
		{
			// Arrange
			var firstDescriptor = new FieldDescriptorPart(1);

			string firstPartName = "ControlNumber";

			var firstfieldProviderMock = new Mock<IFileNamePartProvider>();


			firstfieldProviderMock.Setup(mock => mock.GetPartName(firstDescriptor, _exportObjectInfo))
				.Returns(firstPartName);

			_fileNamePartProviderContainerMock.Setup(mock => mock.GetProvider(firstDescriptor))
				.Returns(firstfieldProviderMock.Object);

			_subjectUnderTest = new CustomFileNameProvider(new List<DescriptorPart>
				{
					firstDescriptor
				},
				_fileNamePartProviderContainerMock.Object, false);

			// Act
			string retFileName = _subjectUnderTest.GetTextName(_exportObjectInfo);
			string extension = TextExtension;


			// Assert
			Assert.That(retFileName, Is.EqualTo($"{firstPartName}.{extension}"));
		}


		[Test]
		public void ItShouldCreateCorrectFileNameForFiveParts()
		{
			const int numOfParts = 5;
			var descriptors = new DescriptorPart[]
			{
				new FieldDescriptorPart(1),
				new SeparatorDescriptorPart("-"),
				new FieldDescriptorPart(1),
				new SeparatorDescriptorPart("."),
				new FieldDescriptorPart(1)
			};
			string[] partNames = new string[]
			{
				"Control Number",
				"_",
				"MD5 Hash",
				".",
				"Folder Name"
			};

			var mocks = new Mock<IFileNamePartProvider>[]
			{
				new Mock<IFileNamePartProvider>(),
				new Mock<IFileNamePartProvider>(),
				new Mock<IFileNamePartProvider>(),
				new Mock<IFileNamePartProvider>(),
				new Mock<IFileNamePartProvider>()
			};

			for (int i = 0; i < numOfParts; i++)
			{
				mocks[i].Setup(mock => mock.GetPartName(descriptors[i], _exportObjectInfo)).Returns(partNames[i]);
				_fileNamePartProviderContainerMock.Setup(mock => mock.GetProvider(descriptors[i]))
					.Returns(mocks[i].Object);
			}

			_subjectUnderTest = new CustomFileNameProvider(new List<DescriptorPart>(descriptors),
				_fileNamePartProviderContainerMock.Object, false);

			// Act
			string retFileName = _subjectUnderTest.GetTextName(_exportObjectInfo);
			string extension = TextExtension;

			string expectedVal = string.Join("", partNames) + "." + extension;

			// Assert
			Assert.AreEqual(expectedVal, retFileName);
		}

		[Test]
		public void ItShouldAppendOriginalFileName()
		{
			// Arrange
			var firstDescriptor = new FieldDescriptorPart(1);

			string firstPartName = "ControlNumber";

			var firstfieldProviderMock = new Mock<IFileNamePartProvider>();


			firstfieldProviderMock.Setup(mock => mock.GetPartName(firstDescriptor, _exportObjectInfo))
				.Returns(firstPartName);

			_fileNamePartProviderContainerMock.Setup(mock => mock.GetProvider(firstDescriptor))
				.Returns(firstfieldProviderMock.Object);

			_subjectUnderTest = new CustomFileNameProvider(new List<DescriptorPart>
				{
					firstDescriptor
				},
				_fileNamePartProviderContainerMock.Object, true);

			// Act
			string retFileName = _subjectUnderTest.GetTextName(_exportObjectInfo);
			string extension = TextExtension;


			// Assert
			Assert.That(retFileName, Is.EqualTo($"{firstPartName}_{_exportObjectInfo.OriginalFileName}"));
		}
	}
}
