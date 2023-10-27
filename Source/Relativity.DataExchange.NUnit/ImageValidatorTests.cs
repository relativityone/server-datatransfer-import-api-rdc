// -----------------------------------------------------------------------------------------------------
// <copyright file="ImageValidatorTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;
	using System.IO;

	using global::NUnit.Framework;

	using kCura.WinEDDS;

	using Moq;

	using Relativity.DataExchange;
	using Relativity.DataExchange.TestFramework;

	[TestFixture]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	public class ImageValidatorTests
	{
		private readonly ITiffValidator tiffValidator = new TiffValidator();
		private readonly IFileInspector fileInspector = new FileInspector();

		private TempDirectory2 tempDirectory;
		private ImageValidator imageValidator;

		[SetUp]
		public void Setup()
		{
			this.tempDirectory = new TempDirectory2();
			this.imageValidator = new ImageValidator();
		}

		[TearDown]
		public void Teardown()
		{
			this.tempDirectory.Dispose();
		}

		[Test]
		[TestCase("w3c_home.jpg")]
		[TestCase("w3c_home_2.jpg")]
		[TestCase("w3c_home_256.jpg")]
		[TestCase("w3c_home_gray.jpg")]
		[Category(TestCategories.Framework)]
		public void ShouldValidateAndIdentifyTheJpegImage(string fileName)
		{
			string file = ResourceFileHelper.GetResourceFilePath("Jpeg", fileName);
			var result = this.imageValidator.IsImageValid(file, this.tiffValidator, this.fileInspector);
			Assert.That(result.IsValid, Is.EqualTo(true));
			Assert.That(result.Message, Is.EqualTo($"The JPEG image file {file} is valid"));
		}

		[Test]
		[TestCase("CCITT_1.TIF")]
		[TestCase("CCITT_2.TIF")]
		[TestCase("CCITT_3.TIF")]
		[TestCase("CCITT_4.TIF")]
		[TestCase("CCITT_5.TIF")]
		[TestCase("CCITT_6.TIF")]
		[TestCase("CCITT_7.TIF")]
		[TestCase("G4.TIF")]
		[TestCase("G4S.TIF")]
		[Category(TestCategories.Framework)]
		public void ShouldValidateAndIdentifyTheTiffImage(string fileName)
		{
			string file = ResourceFileHelper.GetResourceFilePath("Tiff", fileName);
			var result = this.imageValidator.IsImageValid(file, this.tiffValidator, this.fileInspector);
			Assert.That(result.IsValid, Is.EqualTo(true));
			Assert.That(result.Message, Is.EqualTo($"The TIFF image file {file} is valid"));
		}

		[Test]
		[TestCase("G31D.TIF")]
		[TestCase("G31DS.TIF")]
		[TestCase("G32D.TIF")]
		[TestCase("G32DS.TIF")]
		[Category(TestCategories.Framework)]
		public void ShouldValidateWhenTheTiffEncodingIsNotSupported(string fileName)
		{
			string file = ResourceFileHelper.GetResourceFilePath("Tiff", fileName);
			var result = this.imageValidator.IsImageValid(file, this.tiffValidator, this.fileInspector);
			Assert.That(result.IsValid, Is.EqualTo(false));
			Assert.That(result.Message, Is.EqualTo($"The TIFF image file {file} is not encoded in CCITT T.6"));
		}

		[Test]
		[TestCase("GMARBLES.TIF", 8)]
		[TestCase("MARBIBM.TIF", 24)]
		[TestCase("MARBLES.TIF", 24)]
		[TestCase("XING_T24.TIF", 24)]
		[TestCase("FLAG_T24.TIF", 24)]
		[Category(TestCategories.Framework)]
		public void ShouldValidateWhenTheTiffHeaderBitCountIsGreaterThanOne(string fileName, int bits)
		{
			string file = ResourceFileHelper.GetResourceFilePath("Tiff", fileName);
			var result = this.imageValidator.IsImageValid(file, this.tiffValidator, this.fileInspector);
			Assert.That(result.IsValid, Is.EqualTo(false));
			Assert.That(result.Message, Is.EqualTo($"The TIFF image file {file} is {bits} bits. Only 1-bit TIFFs supported"));
		}

		[Test]
		[TestCase("multi2.tif")]
		[TestCase("multi3.tif")]
		[TestCase("multi4.tif")]
		[Category(TestCategories.Framework)]
		public void ShouldValidateWhenTheTiffPageCountIsGreaterThanOne(string fileName)
		{
			string file = ResourceFileHelper.GetResourceFilePath("MultiTiff", fileName);
			var result = this.imageValidator.IsImageValid(file, this.tiffValidator, this.fileInspector);
			Assert.That(result.IsValid, Is.EqualTo(false));
			Assert.That(result.Message, Is.EqualTo($"The TIFF image file {file} is a Multi Page TIFF and its not supported"));
		}

		[Test]
		[TestCase("w3c_home.png")]
		[TestCase("w3c_home_2.png")]
		[TestCase("w3c_home_256.png")]
		[TestCase("w3c_home_gray.png")]
		[Category(TestCategories.Framework)]
		public void ShouldValidateWhenThePngImageFormatIsNotSupported(string fileName)
		{
			string file = ResourceFileHelper.GetResourceFilePath("Png", fileName);
			var result = this.imageValidator.IsImageValid(file, this.tiffValidator, this.fileInspector);
			Assert.That(result.IsValid, Is.EqualTo(false));
			Assert.That(result.Message, Is.EqualTo($"File {file} must be TIFF or JPEG"));
		}

		[Test]
		[Category(TestCategories.Framework)]
		public void ShouldValidateAndIdentifyWhenFilePathIsNull()
		{
			var result = this.imageValidator.IsImageValid(null, this.tiffValidator, this.fileInspector);
			Assert.That(result.IsValid, Is.EqualTo(false));
			Assert.That(result.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: filePath"));
		}

		[Test]
		[Category(TestCategories.Framework)]
		public void ShouldValidateAndIdentifyWhenFilePathIsEmpty()
		{
			var result = this.imageValidator.IsImageValid(string.Empty, this.tiffValidator, this.fileInspector);
			Assert.That(result.IsValid, Is.EqualTo(false));
			Assert.That(result.Message, Is.EqualTo("filePath argument is empty"));
		}

		[Test]
		[Category(TestCategories.Framework)]
		public void ShouldValidateAndIdentifyWhenTiffValidatorIsNull()
		{
			var result = this.imageValidator.IsImageValid("test", null, this.fileInspector);
			Assert.That(result.IsValid, Is.EqualTo(false));
			Assert.That(result.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: tiffValidator"));
		}

		[Test]
		[Category(TestCategories.Framework)]
		public void ShouldValidateWhenFileInspectorIsNull()
		{
			var result = this.imageValidator.IsImageValid("test", this.tiffValidator, null);
			Assert.That(result.IsValid, Is.EqualTo(false));
			Assert.That(result.Message, Is.EqualTo("Value cannot be null.\r\nParameter name: fileInspector"));
		}

		[Test]
		[Category(TestCategories.Framework)]
		public void ShouldValidateWhenTheImageFileDoesNotExist()
		{
			string file = ResourceFileHelper.GetResourceFilePath("Png", $"{Guid.NewGuid()}.png");
			var result = this.imageValidator.IsImageValid(file, this.tiffValidator, this.fileInspector);
			Assert.That(result.IsValid, Is.EqualTo(false));
			Assert.That(result.Message, Is.EqualTo($"File {file} doesn't exist"));
		}

		[Test]
		[Category(TestCategories.Framework)]
		public void ShouldValidateAndIdentifyTheEmptyFile()
		{
			string file = RandomHelper.NextBinaryFile(0, 0, this.tempDirectory.Directory);
			var result = this.imageValidator.IsImageValid(file, this.tiffValidator, this.fileInspector);
			Assert.That(result.IsValid, Is.EqualTo(false));
			Assert.That(result.Message, Is.EqualTo($"File {file} is empty"));
		}
	}
}