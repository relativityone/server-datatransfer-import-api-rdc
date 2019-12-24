// -----------------------------------------------------------------------------------------------------
// <copyright file="FreeImageServiceTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="FreeImageService"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.NUnit
{
	using System;
	using System.IO;

	using global::NUnit.Framework;

	using Relativity.DataExchange;
	using Relativity.DataExchange.Media;
	using Relativity.DataExchange.TestFramework;

	/// <summary>
	/// Represents <see cref="FreeImageService"/> tests.
	/// </summary>
	[TestFixture]
	[System.Diagnostics.CodeAnalysis.SuppressMessage(
		"Microsoft.Design",
		"CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable",
		Justification = "The test class handles the disposal.")]
	public class FreeImageServiceTests
	{
		private TempDirectory2 tempDirectory;
		private FreeImageService service;

		[SetUp]
		public void Setup()
		{
			this.tempDirectory = new TempDirectory2();
			this.service = new FreeImageService();
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
			this.service.Validate(file);
			ImageFormat format = this.service.Identify(file);
			Assert.That(format, Is.EqualTo(ImageFormat.Jpeg));
		}

		[Test]
		[TestCase("CCITT_1.TIF")]
		[TestCase("CCITT_2.TIF")]
		[TestCase("CCITT_3.TIF")]
		[TestCase("CCITT_4.TIF")]
		[TestCase("CCITT_5.TIF")]
		[TestCase("CCITT_6.TIF")]
		[TestCase("CCITT_7.TIF")]
		[Category(TestCategories.Framework)]
		public void ShouldValidateAndIdentifyTheTiffImage(string fileName)
		{
			string file = ResourceFileHelper.GetResourceFilePath("Tiff", fileName);
			this.service.Validate(file);
			ImageFormat format = this.service.Identify(file);
			Assert.That(format, Is.EqualTo(ImageFormat.Tiff));
		}

		[Test]
		[TestCase("G31D.TIF")]
		[TestCase("G31DS.TIF")]
		[TestCase("G32D.TIF")]
		[TestCase("G32DS.TIF")]
		[TestCase("G4.TIF")]
		[TestCase("G4S.TIF")]
		[Category(TestCategories.Framework)]
		public void ShouldThrowWhenTheTiffEncodingIsNotSupported(string fileName)
		{
			string file = ResourceFileHelper.GetResourceFilePath("Tiff", fileName);
			ImageFormat format = this.service.Identify(file);
			Assert.That(format, Is.EqualTo(ImageFormat.Tiff));
			ImageValidationException exception = Assert.Throws<ImageValidationException>(() => this.service.Validate(file));
			Assert.That(exception.Message, Contains.Substring("is encoded"));
		}

		[Test]
		[TestCase("GMARBLES.TIF")]
		[TestCase("MARBIBM.TIF")]
		[TestCase("MARBLES.TIF")]
		[TestCase("XING_T24.TIF")]
		[TestCase("FLAG_T24.TIF")]
		[Category(TestCategories.Framework)]
		public void ShouldThrowWhenTheTiffHeaderBitCountIsGreaterThanOne(string fileName)
		{
			string file = ResourceFileHelper.GetResourceFilePath("Tiff", fileName);
			ImageFormat format = this.service.Identify(file);
			Assert.That(format, Is.EqualTo(ImageFormat.Tiff));
			ImageValidationException exception = Assert.Throws<ImageValidationException>(() => this.service.Validate(file));
			Assert.That(exception.Message, Contains.Substring("bits"));
		}

		[Test]
		[TestCase("w3c_home.png")]
		[TestCase("w3c_home_2.png")]
		[TestCase("w3c_home_256.png")]
		[TestCase("w3c_home_gray.png")]
		[Category(TestCategories.Framework)]
		public void ShouldThrowWhenThePngImageFormatIsNotSupported(string fileName)
		{
			string file = ResourceFileHelper.GetResourceFilePath("Png", fileName);
			ImageFormat format = this.service.Identify(file);
			Assert.That(format, Is.EqualTo(ImageFormat.Png));
			ImageValidationException exception = Assert.Throws<ImageValidationException>(() => this.service.Validate(file));
			Assert.That(exception.Message, Contains.Substring("isn't a valid TIFF or JPEG"));
		}

		[Test]
		[TestCase(null)]
		[TestCase("")]
		[Category(TestCategories.Framework)]
		public void ShouldThrowWhenTheFileIsNullOrEmpty(string file)
		{
			Assert.Throws<System.ArgumentNullException>(() => this.service.Validate(file));
		}

		[Test]
		[Category(TestCategories.Framework)]
		public void ShouldThrowWhenTheImageFileDoesNotExist()
		{
			string file = ResourceFileHelper.GetResourceFilePath("Png", $"{Guid.NewGuid()}.png");
			Assert.Throws<FileNotFoundException>(() => this.service.Validate(file));
		}

		[Test]
		[Category(TestCategories.Framework)]
		public void ShouldThrowWhenTheImageFileIsEmptyFile()
		{
			string file = RandomHelper.NextBinaryFile(0, 0, this.tempDirectory.Directory);
			ImageFormat format = this.service.Identify(file);
			Assert.That(format, Is.EqualTo(ImageFormat.Unknown));
			Assert.Throws<ImageValidationException>(() => this.service.Validate(file));
		}
	}
}