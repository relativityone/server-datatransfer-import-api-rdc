using FileNaming.CustomFileNaming;
using kCura.WinEDDS.Exporters;
using kCura.WinEDDS.FileNaming.CustomFileNaming;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace kCura.WinEDDS.Core.NUnit.Export.Natives.Name
{
	public class CustomFileNameProviderTest
	{
		private Mock<IFileNamePartProviderContainer> _fileNamePartProviderContainerMock;
		
		private const string _TEXT_EXTENSION = ".txt";
		private const string _NATIVE_EXTENSION = ".xls";

		private readonly ObjectExportInfo _exportObjectInfoHtmlFile = new ObjectExportInfo
		{
			NativeExtension = "xls",
			OriginalFileName = "OriginalFileName.html"
		};

		private readonly ObjectExportInfo _exportObjectInfoTextFile = new ObjectExportInfo
		{
			NativeExtension = "txt",
			OriginalFileName = "OriginalFileName.TxT"
		};

		private readonly ObjectExportInfo _exportObjectInfoNoExtensionFile = new ObjectExportInfo
		{
			NativeExtension = "",
			OriginalFileName = "OriginalFileName"
		};
		
		[SetUp]
		public void Init()
		{
			_fileNamePartProviderContainerMock = new Mock<IFileNamePartProviderContainer>();
		}

		[Test]
		public void ItShouldReturnTextFileNameWhenThreeDescriptorsAreUsed()
		{
			ItShouldReturnFileNameWhenThreeDescriptorsAreUsed((sut, objectExportInfo) => sut.GetTextName(objectExportInfo), _TEXT_EXTENSION);
		}

		[Test]
		public void ItShouldReturnNativeFileNameWhenThreeDescriptorsAreUsed()
		{
			ItShouldReturnFileNameWhenThreeDescriptorsAreUsed((sut, objectExportInfo) => sut.GetTextName(objectExportInfo), _TEXT_EXTENSION);
		}

		[Test]
		public void ItShouldRemoveDoubleSeparatorsFromTextFileName()
		{
			ItShouldRemoveDoubleSeparatorsFromFileName((sut, objectExportInfo) => sut.GetTextName(objectExportInfo), _TEXT_EXTENSION);
		}

		[Test]
		public void ItShouldRemoveDoubleSeparatorsFromNativeFileName()
		{
			ItShouldRemoveDoubleSeparatorsFromFileName((sut, objectExportInfo) => sut.GetName(objectExportInfo), _NATIVE_EXTENSION);
		}
		
		[Test]
		public void ItShouldReturnOnlyControlAsTextFileNameNumberWhenOneDescriptorIsUsed()
		{
			ItShouldReturnOnlyControlAsFileNameNumberWhenOneDescriptorIsUsed((sut, objectExportInfo) => sut.GetTextName(objectExportInfo), _TEXT_EXTENSION);
		}

		[Test]
		public void ItShouldReturnOnlyControlAsNativeFileNameNumberWhenOneDescriptorIsUsed()
		{
			ItShouldReturnOnlyControlAsFileNameNumberWhenOneDescriptorIsUsed((sut, objectExportInfo) => sut.GetName(objectExportInfo), _NATIVE_EXTENSION);
		}

		[Test]
		public void ItShouldCreateCorrectTextFileNameForFiveParts()
		{
			ItShouldCreateCorrectFileNameForFiveParts((sut, objectExportInfo) => sut.GetTextName(objectExportInfo), _TEXT_EXTENSION);
		}

		[Test]
		public void ItShouldCreateCorrectNativeFileNameForFiveParts()
		{
			ItShouldCreateCorrectFileNameForFiveParts((sut, objectExportInfo) => sut.GetName(objectExportInfo), _NATIVE_EXTENSION);
		}

		[Test]
		public void ItShouldAppendOriginalFileNameToTextFileName()
		{
			ItShouldAppendOriginalFileNameToFileName((sut, objectExportInfo) => sut.GetTextName(objectExportInfo), _TEXT_EXTENSION);
		}

		[Test]
		public void ItShouldNotAppendExtensionWhenAppendOriginalFileNameToNativeFileName()
		{
			ItShouldAppendOriginalFileNameToFileName((sut, objectExportInfo) => sut.GetName(objectExportInfo), "");
		}
		
		[Test]
		public void ItShouldAppendTextExtenstionToTextFilesWhenOriginalFileNameWithoutExtensionIsAppended()
		{
			// Arrange
			var firstDescriptor = new FieldDescriptorPart(1);
			string firstPartName = "ControlNumber";
			InitializeFileNamePartProviderContainer(firstDescriptor, firstPartName, _exportObjectInfoNoExtensionFile);

			var subjectUnderTest = new CustomFileNameProvider(new List<DescriptorPart>
				{
					firstDescriptor
				},
				_fileNamePartProviderContainerMock.Object, true);

			// Act
			string retFileName = subjectUnderTest.GetTextName(_exportObjectInfoNoExtensionFile);

			// Assert
			Assert.That(retFileName, Is.EqualTo($"{firstPartName}_{_exportObjectInfoNoExtensionFile.OriginalFileName}{_TEXT_EXTENSION}"));
		}

		[Test]
		public void ItShouldNotAppendTextExtensionToOriginalFileNameForTextFilesIfNotNeeded()
		{
			// Arrange
			var firstDescriptor = new FieldDescriptorPart(1);
			string firstPartName = "ControlNumber";
			InitializeFileNamePartProviderContainer(firstDescriptor, firstPartName, _exportObjectInfoTextFile);

			var subjectUnderTest = new CustomFileNameProvider(new List<DescriptorPart>
				{
					firstDescriptor
				},
				_fileNamePartProviderContainerMock.Object, true);

			// Act
			string retFileName = subjectUnderTest.GetTextName(_exportObjectInfoTextFile);

			// Assert
			Assert.That(retFileName, Is.EqualTo($"{firstPartName}_{_exportObjectInfoTextFile.OriginalFileName}"));
		}

		private void ItShouldReturnFileNameWhenThreeDescriptorsAreUsed(Func<CustomFileNameProvider, ObjectExportInfo, string> fileNameProvider, string expectedExtension)
		{
			// Arrange
			var descriptorParts = new List<DescriptorPart>
			{
				new FieldDescriptorPart(1),
				new SeparatorDescriptorPart(""),
				new FieldDescriptorPart(1)
			};

			var partNames = new List<string>
			{
				"First",
				"_",
				"Second"
			};

			InitializeFileNamePartProviderContainer(descriptorParts, partNames, _exportObjectInfoHtmlFile);

			var subjectUnderTest = new CustomFileNameProvider(descriptorParts, _fileNamePartProviderContainerMock.Object, false);

			// Act
			string retFileName = fileNameProvider(subjectUnderTest, _exportObjectInfoHtmlFile);

			// Assert
			Assert.That(retFileName, Is.EqualTo($"{partNames[0]}{partNames[1]}{partNames[2]}{expectedExtension}"));
		}

		private void ItShouldRemoveDoubleSeparatorsFromFileName(Func<CustomFileNameProvider, ObjectExportInfo, string> fileNameProvider, string expectedExtension)
		{
			// Arrange
			var descriptorParts = new List<DescriptorPart>
			{
				new FieldDescriptorPart(1),
				new SeparatorDescriptorPart(""),
				new FieldDescriptorPart(1)
			};

			var partNames = new List<string>
			{
				"First",
				"_",
				""
			};

			InitializeFileNamePartProviderContainer(descriptorParts, partNames, _exportObjectInfoHtmlFile);

			var subjectUnderTest = new CustomFileNameProvider(descriptorParts, _fileNamePartProviderContainerMock.Object, false);

			// Act
			string retFileName = fileNameProvider(subjectUnderTest, _exportObjectInfoHtmlFile);

			// Assert
			Assert.That(retFileName, Is.EqualTo($"{partNames[0]}{expectedExtension}"));
		}

		private void ItShouldReturnOnlyControlAsFileNameNumberWhenOneDescriptorIsUsed(Func<CustomFileNameProvider, ObjectExportInfo, string> testedFunction, string expectedExtension)
		{
			// Arrange
			var firstDescriptor = new FieldDescriptorPart(1);
			string firstPartName = "ControlNumber";
			InitializeFileNamePartProviderContainer(firstDescriptor, firstPartName, _exportObjectInfoHtmlFile);

			var subjectUnderTest = new CustomFileNameProvider(new List<DescriptorPart>
				{
					firstDescriptor
				},
				_fileNamePartProviderContainerMock.Object, false);

			// Act
			string retFileName = testedFunction(subjectUnderTest, _exportObjectInfoHtmlFile);

			// Assert
			Assert.That(retFileName, Is.EqualTo($"{firstPartName}{expectedExtension}"));
		}

		private void ItShouldCreateCorrectFileNameForFiveParts(Func<CustomFileNameProvider, ObjectExportInfo, string> testedFunction, string expectedExtension)
		{
			var descriptors = new List<DescriptorPart>
			{
				new FieldDescriptorPart(1),
				new SeparatorDescriptorPart("-"),
				new FieldDescriptorPart(1),
				new SeparatorDescriptorPart("."),
				new FieldDescriptorPart(1)
			};
			var partNames = new List<string>
			{
				"Control Number",
				"_",
				"MD5 Hash",
				".",
				"Folder Name"
			};

			InitializeFileNamePartProviderContainer(descriptors, partNames, _exportObjectInfoHtmlFile);

			var subjectUnderTest = new CustomFileNameProvider(descriptors, _fileNamePartProviderContainerMock.Object, false);

			// Act
			string retFileName = testedFunction(subjectUnderTest, _exportObjectInfoHtmlFile);

			string expectedVal = string.Join("", partNames) + expectedExtension;

			// Assert
			Assert.AreEqual(expectedVal, retFileName);
		}

		private void ItShouldAppendOriginalFileNameToFileName(Func<CustomFileNameProvider, ObjectExportInfo, string> testedFunction, string expectedExtension)
		{
			// Arrange
			var firstDescriptor = new FieldDescriptorPart(1);
			string firstPartName = "ControlNumber";
			InitializeFileNamePartProviderContainer(firstDescriptor, firstPartName, _exportObjectInfoHtmlFile);

			var subjectUnderTest = new CustomFileNameProvider(new List<DescriptorPart>
				{
					firstDescriptor
				},
				_fileNamePartProviderContainerMock.Object, true);

			// Act
			string retFileName = testedFunction(subjectUnderTest, _exportObjectInfoHtmlFile);

			// Assert
			Assert.That(retFileName, Is.EqualTo($"{firstPartName}_{_exportObjectInfoHtmlFile.OriginalFileName}{expectedExtension}"));
		}

		private void InitializeFileNamePartProviderContainer(List<DescriptorPart> fieldDescriptors, List<string> partNames, ObjectExportInfo objectExportInfo)
		{
			if (fieldDescriptors.Count != partNames.Count)
			{
				throw new ArgumentException("List counts should match.");
			}

			for (int i = 0; i < fieldDescriptors.Count; i++)
			{
				InitializeFileNamePartProviderContainer(fieldDescriptors[i], partNames[i], objectExportInfo);
			}
		}

		private void InitializeFileNamePartProviderContainer(DescriptorPart fieldDescriptor, string partName, ObjectExportInfo objectExportInfo)
		{
			var firstfieldProviderMock = new Mock<IFileNamePartProvider>();

			firstfieldProviderMock
				.Setup(mock => mock.GetPartName(fieldDescriptor, objectExportInfo))
				.Returns(partName);

			_fileNamePartProviderContainerMock
				.Setup(mock => mock.GetProvider(fieldDescriptor))
				.Returns(firstfieldProviderMock.Object);
		}
	}
}
