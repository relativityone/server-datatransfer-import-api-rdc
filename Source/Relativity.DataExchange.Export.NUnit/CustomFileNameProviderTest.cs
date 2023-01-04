﻿// -----------------------------------------------------------------------------------------------------
// <copyright file="CustomFileNameProviderTest.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.Collections.Generic;

	using FileNaming.CustomFileNaming;

	using global::NUnit.Framework;

	using kCura.WinEDDS.Exporters;
	using kCura.WinEDDS.FileNaming.CustomFileNaming;

	using Moq;

	public class CustomFileNameProviderTest
	{
		private const string _TEXT_EXTENSION = ".txt";
		private const string _NATIVE_EXTENSION = ".xls";
		private const string _PDF_EXTENSION = ".pdf";

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

		private readonly ObjectExportInfo _exportObjectInfoPdfFile = new ObjectExportInfo
		{
			NativeExtension = "pdf",
			OriginalFileName = "OriginalFileName.PdF"
		};

		private readonly ObjectExportInfo _exportObjectInfoNoExtensionFile = new ObjectExportInfo
		{
			NativeExtension = string.Empty,
			OriginalFileName = "OriginalFileName"
		};

		private readonly ObjectExportInfo _exportObjectInfoNoNatives = new ObjectExportInfo
		{
			NativeExtension = string.Empty,
			OriginalFileName = string.Empty
		};

		private Mock<IFileNamePartProviderContainer> _fileNamePartProviderContainerMock;

		[SetUp]
		public void Init()
		{
			this._fileNamePartProviderContainerMock = new Mock<IFileNamePartProviderContainer>();
		}

		[Test]
		public void ItShouldReturnTextFileNameWhenThreeDescriptorsAreUsed()
		{
			this.ItShouldReturnFileNameWhenThreeDescriptorsAreUsed((sut, objectExportInfo) => sut.GetTextName(objectExportInfo), _TEXT_EXTENSION);
		}

		[Test]
		public void ItShouldReturnNativeFileNameWhenThreeDescriptorsAreUsed()
		{
			this.ItShouldReturnFileNameWhenThreeDescriptorsAreUsed((sut, objectExportInfo) => sut.GetName(objectExportInfo), _NATIVE_EXTENSION);
		}

		[Test]
		public void ItShouldReturnPdfFileNameWhenThreeDescriptorsAreUsed()
		{
			this.ItShouldReturnFileNameWhenThreeDescriptorsAreUsed((sut, objectExportInfo) => sut.GetPdfName(objectExportInfo), _PDF_EXTENSION);
		}

		[Test]
		public void ItShouldRemoveDoubleSeparatorsFromTextFileName()
		{
			this.ItShouldRemoveDoubleSeparatorsFromFileName((sut, objectExportInfo) => sut.GetTextName(objectExportInfo), _TEXT_EXTENSION);
		}

		[Test]
		public void ItShouldRemoveDoubleSeparatorsFromNativeFileName()
		{
			this.ItShouldRemoveDoubleSeparatorsFromFileName((sut, objectExportInfo) => sut.GetName(objectExportInfo), _NATIVE_EXTENSION);
		}

		[Test]
		public void ItShouldRemoveDoubleSeparatorsFromPdfFileName()
		{
			this.ItShouldRemoveDoubleSeparatorsFromFileName((sut, objectExportInfo) => sut.GetPdfName(objectExportInfo), _PDF_EXTENSION);
		}

		[Test]
		public void ItShouldConvertIllegalCharactersInTextFileName()
		{
			this.ItShouldConvertIllegalCharactersInTextFileName((sut, objectExportInfo) => sut.GetTextName(objectExportInfo), _TEXT_EXTENSION);
		}

		[Test]
		public void ItShouldConvertIllegalCharactersInNativeFileName()
		{
			this.ItShouldConvertIllegalCharactersInTextFileName((sut, objectExportInfo) => sut.GetName(objectExportInfo), _NATIVE_EXTENSION);
		}

		[Test]
		public void ItShouldConvertIllegalCharactersInPdfFileName()
		{
			this.ItShouldConvertIllegalCharactersInTextFileName((sut, objectExportInfo) => sut.GetPdfName(objectExportInfo), _PDF_EXTENSION);
		}

		[Test]
		public void ItShouldReturnOnlyControlAsTextFileNameNumberWhenOneDescriptorIsUsed()
		{
			this.ItShouldReturnOnlyControlAsFileNameNumberWhenOneDescriptorIsUsed((sut, objectExportInfo) => sut.GetTextName(objectExportInfo), _TEXT_EXTENSION);
		}

		[Test]
		public void ItShouldReturnOnlyControlAsNativeFileNameNumberWhenOneDescriptorIsUsed()
		{
			this.ItShouldReturnOnlyControlAsFileNameNumberWhenOneDescriptorIsUsed((sut, objectExportInfo) => sut.GetName(objectExportInfo), _NATIVE_EXTENSION);
		}

		[Test]
		public void ItShouldReturnOnlyControlAsPdfFileNameNumberWhenOneDescriptorIsUsed()
		{
			this.ItShouldReturnOnlyControlAsFileNameNumberWhenOneDescriptorIsUsed((sut, objectExportInfo) => sut.GetPdfName(objectExportInfo), _PDF_EXTENSION);
		}

		[Test]
		public void ItShouldCreateCorrectTextFileNameForFiveParts()
		{
			this.ItShouldCreateCorrectFileNameForFiveParts((sut, objectExportInfo) => sut.GetTextName(objectExportInfo), _TEXT_EXTENSION);
		}

		[Test]
		public void ItShouldCreateCorrectNativeFileNameForFiveParts()
		{
			this.ItShouldCreateCorrectFileNameForFiveParts((sut, objectExportInfo) => sut.GetName(objectExportInfo), _NATIVE_EXTENSION);
		}

		[Test]
		public void ItShouldCreateCorrectPdfFileNameForFiveParts()
		{
			this.ItShouldCreateCorrectFileNameForFiveParts((sut, objectExportInfo) => sut.GetPdfName(objectExportInfo), _PDF_EXTENSION);
		}

		[Test]
		public void ItShouldAppendOriginalFileNameToTextFileName()
		{
			this.ItShouldAppendOriginalFileNameToFileName((sut, objectExportInfo) => sut.GetTextName(objectExportInfo), _TEXT_EXTENSION);
		}

		[Test]
		public void ItShouldNotAppendOriginalFileNameToTextFileNameIfOriginalFileNameIsEmpty()
		{
			this.ItShouldNotAppendOriginalFileNameToFileName((sut, objectExportInfo) => sut.GetTextName(objectExportInfo), _TEXT_EXTENSION);
		}

		[Test]
		public void ItShouldNotAppendExtensionWhenAppendOriginalFileNameToNativeFileName()
		{
			this.ItShouldAppendOriginalFileNameToFileName((sut, objectExportInfo) => sut.GetName(objectExportInfo), string.Empty);
		}

		[Test]
		public void ItShouldNotAppendExtensionWhenAppendEmptyOriginalFileNameToNativeFileName()
		{
			this.ItShouldNotAppendOriginalFileNameToFileName((sut, objectExportInfo) => sut.GetName(objectExportInfo), string.Empty);
		}

		[Test]
		public void ItShouldAppendOriginalFileNameToPdfFileName()
		{
			this.ItShouldAppendOriginalFileNameToFileName((sut, objectExportInfo) => sut.GetPdfName(objectExportInfo), _PDF_EXTENSION);
		}

		[Test]
		public void ItShouldNotAppendOriginalFileNameToPdfFileNameIfOriginalFileNameIsEmpty()
		{
			this.ItShouldNotAppendOriginalFileNameToFileName((sut, objectExportInfo) => sut.GetPdfName(objectExportInfo), _PDF_EXTENSION);
		}

		[Test]
		public void ItShouldAppendTextExtenstionToTextFilesWhenOriginalFileNameWithoutExtensionIsAppended()
		{
			this.ItShouldAppendExtenstionToFilesWhenOriginalFileNameWithoutExtensionIsAppended(
				(sut, objectExportInfo) => sut.GetTextName(objectExportInfo),
				_TEXT_EXTENSION);
		}

		[Test]
		public void ItShouldAppendPdfExtenstionToPdfFilesWhenOriginalFileNameWithoutExtensionIsAppended()
		{
			this.ItShouldAppendExtenstionToFilesWhenOriginalFileNameWithoutExtensionIsAppended(
				(sut, objectExportInfo) => sut.GetPdfName(objectExportInfo),
				_PDF_EXTENSION);
		}

		[Test]
		public void ItShouldNotAppendTextExtensionToOriginalFileNameForTextFilesIfNotNeeded()
		{
			this.ItShouldNotAppendExtensionToOriginalFileNameIfNotNeeded(
				(sut, objectExportInfo) => sut.GetTextName(objectExportInfo),
				this._exportObjectInfoTextFile);
		}

		[Test]
		public void ItShouldNotAppendPdfExtensionToOriginalFileNameForPdfFilesIfNotNeeded()
		{
			this.ItShouldNotAppendExtensionToOriginalFileNameIfNotNeeded(
				(sut, objectExportInfo) => sut.GetPdfName(objectExportInfo),
				this._exportObjectInfoPdfFile);
		}

		private void ItShouldAppendExtenstionToFilesWhenOriginalFileNameWithoutExtensionIsAppended(Func<CustomFileNameProvider, ObjectExportInfo, string> testedFunction, string expectedExtension)
		{
			// Arrange
			var firstDescriptor = new FieldDescriptorPart(1);
			string firstPartName = "ControlNumber";
			this.InitializeFileNamePartProviderContainer(firstDescriptor, firstPartName, this._exportObjectInfoNoExtensionFile);

			var subjectUnderTest = new CustomFileNameProvider(
				new List<DescriptorPart> { firstDescriptor },
				this._fileNamePartProviderContainerMock.Object,
				true);

			// Act
			string retFileName = testedFunction(subjectUnderTest, this._exportObjectInfoNoExtensionFile);

			// Assert
			Assert.That(retFileName, Is.EqualTo($"{firstPartName}_{this._exportObjectInfoNoExtensionFile.OriginalFileName}{expectedExtension}"));
		}

		private void ItShouldNotAppendExtensionToOriginalFileNameIfNotNeeded(Func<CustomFileNameProvider, ObjectExportInfo, string> testedFunction, ObjectExportInfo artifact)
		{
			// Arrange
			var firstDescriptor = new FieldDescriptorPart(1);
			string firstPartName = "ControlNumber";
			this.InitializeFileNamePartProviderContainer(firstDescriptor, firstPartName, artifact);

			var subjectUnderTest = new CustomFileNameProvider(
				new List<DescriptorPart> { firstDescriptor },
				this._fileNamePartProviderContainerMock.Object,
				true);

			// Act
			string retFileName = testedFunction(subjectUnderTest, artifact);

			// Assert
			Assert.That(retFileName, Is.EqualTo($"{firstPartName}_{artifact.OriginalFileName}"));
		}

		private void ItShouldReturnFileNameWhenThreeDescriptorsAreUsed(Func<CustomFileNameProvider, ObjectExportInfo, string> testedFunction, string expectedExtension)
		{
			// Arrange
			var descriptorParts = new List<DescriptorPart>
			{
				new FieldDescriptorPart(1),
				new SeparatorDescriptorPart(string.Empty),
				new FieldDescriptorPart(1)
			};

			var partNames = new List<string>
			{
				"First",
				"_",
				"Second"
			};

			this.InitializeFileNamePartProviderContainer(descriptorParts, partNames, this._exportObjectInfoHtmlFile);

			var subjectUnderTest = new CustomFileNameProvider(descriptorParts, this._fileNamePartProviderContainerMock.Object, false);

			// Act
			string retFileName = testedFunction(subjectUnderTest, this._exportObjectInfoHtmlFile);

			// Assert
			Assert.That(retFileName, Is.EqualTo($"{partNames[0]}{partNames[1]}{partNames[2]}{expectedExtension}"));
		}

		private void ItShouldRemoveDoubleSeparatorsFromFileName(Func<CustomFileNameProvider, ObjectExportInfo, string> testedFunction, string expectedExtension)
		{
			// Arrange
			var descriptorParts = new List<DescriptorPart>
			{
				new FieldDescriptorPart(1),
				new SeparatorDescriptorPart(string.Empty),
				new FieldDescriptorPart(1)
			};

			var partNames = new List<string>
			{
				"First",
				"_",
				string.Empty
			};

			this.InitializeFileNamePartProviderContainer(descriptorParts, partNames, this._exportObjectInfoHtmlFile);

			var subjectUnderTest = new CustomFileNameProvider(descriptorParts, this._fileNamePartProviderContainerMock.Object, false);

			// Act
			string retFileName = testedFunction(subjectUnderTest, this._exportObjectInfoHtmlFile);

			// Assert
			Assert.That(retFileName, Is.EqualTo($"{partNames[0]}{expectedExtension}"));
		}

		private void ItShouldConvertIllegalCharactersInTextFileName(Func<CustomFileNameProvider, ObjectExportInfo, string> testedFunction, string expectedExtension)
		{
			// Arrange
			var firstDescriptor = new FieldDescriptorPart(1);
			string firstPartName = "Control/Number";
			string validFirstPathName = "Control_Number";
			this.InitializeFileNamePartProviderContainer(firstDescriptor, firstPartName, this._exportObjectInfoHtmlFile);

			var subjectUnderTest = new CustomFileNameProvider(
				new List<DescriptorPart> { firstDescriptor },
				this._fileNamePartProviderContainerMock.Object,
				false);

			// Act
			string retFileName = testedFunction(subjectUnderTest, this._exportObjectInfoHtmlFile);

			// Assert
			Assert.That(retFileName, Is.EqualTo($"{validFirstPathName}{expectedExtension}"));
		}

		private void ItShouldReturnOnlyControlAsFileNameNumberWhenOneDescriptorIsUsed(Func<CustomFileNameProvider, ObjectExportInfo, string> testedFunction, string expectedExtension)
		{
			// Arrange
			var firstDescriptor = new FieldDescriptorPart(1);
			string firstPartName = "ControlNumber";
			this.InitializeFileNamePartProviderContainer(firstDescriptor, firstPartName, this._exportObjectInfoHtmlFile);

			var subjectUnderTest = new CustomFileNameProvider(
				new List<DescriptorPart> { firstDescriptor },
				this._fileNamePartProviderContainerMock.Object,
				false);

			// Act
			string retFileName = testedFunction(subjectUnderTest, this._exportObjectInfoHtmlFile);

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

			this.InitializeFileNamePartProviderContainer(descriptors, partNames, this._exportObjectInfoHtmlFile);

			var subjectUnderTest = new CustomFileNameProvider(descriptors, this._fileNamePartProviderContainerMock.Object, false);

			// Act
			string retFileName = testedFunction(subjectUnderTest, this._exportObjectInfoHtmlFile);

			string expectedVal = string.Join(string.Empty, partNames) + expectedExtension;

			// Assert
			Assert.AreEqual(expectedVal, retFileName);
		}

		private void ItShouldAppendOriginalFileNameToFileName(Func<CustomFileNameProvider, ObjectExportInfo, string> testedFunction, string expectedExtension)
		{
			// Arrange
			var firstDescriptor = new FieldDescriptorPart(1);
			string firstPartName = "ControlNumber";
			this.InitializeFileNamePartProviderContainer(firstDescriptor, firstPartName, this._exportObjectInfoHtmlFile);

			var subjectUnderTest = new CustomFileNameProvider(
				new List<DescriptorPart> { firstDescriptor },
				this._fileNamePartProviderContainerMock.Object,
				true);

			// Act
			string retFileName = testedFunction(subjectUnderTest, this._exportObjectInfoHtmlFile);

			// Assert
			Assert.That(retFileName, Is.EqualTo($"{firstPartName}_{this._exportObjectInfoHtmlFile.OriginalFileName}{expectedExtension}"));
		}

		private void ItShouldNotAppendOriginalFileNameToFileName(Func<CustomFileNameProvider, ObjectExportInfo, string> testedFunction, string expectedExtension)
		{
			// Arrange
			var firstDescriptor = new FieldDescriptorPart(1);
			string firstPartName = "ControlNumber";
			this.InitializeFileNamePartProviderContainer(firstDescriptor, firstPartName, this._exportObjectInfoNoNatives);

			var subjectUnderTest = new CustomFileNameProvider(
				new List<DescriptorPart> { firstDescriptor },
				this._fileNamePartProviderContainerMock.Object,
				true);

			// Act
			string retFileName = testedFunction(subjectUnderTest, this._exportObjectInfoNoNatives);

			// Assert
			Assert.That(retFileName, Is.EqualTo($"{firstPartName}{expectedExtension}"));
		}

		private void InitializeFileNamePartProviderContainer(List<DescriptorPart> fieldDescriptors, List<string> partNames, ObjectExportInfo objectExportInfo)
		{
			if (fieldDescriptors.Count != partNames.Count)
			{
				throw new ArgumentException("List counts should match.");
			}

			for (int i = 0; i < fieldDescriptors.Count; i++)
			{
				this.InitializeFileNamePartProviderContainer(fieldDescriptors[i], partNames[i], objectExportInfo);
			}
		}

		private void InitializeFileNamePartProviderContainer(DescriptorPart fieldDescriptor, string partName, ObjectExportInfo objectExportInfo)
		{
			var firstFieldProviderMock = new Mock<IFileNamePartProvider>();

			firstFieldProviderMock
				.Setup(mock => mock.GetPartName(fieldDescriptor, objectExportInfo))
				.Returns(partName);

			this._fileNamePartProviderContainerMock
				.Setup(mock => mock.GetProvider(fieldDescriptor))
				.Returns(firstFieldProviderMock.Object);
		}
	}
}
