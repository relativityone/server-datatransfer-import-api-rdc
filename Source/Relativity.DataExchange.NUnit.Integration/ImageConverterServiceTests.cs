// -----------------------------------------------------------------------------------------------------
// <copyright file="ImageConverterServiceTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// <summary>
//   Represents <see cref="ImageConverterServiceTests"/> tests.
// </summary>
// -----------------------------------------------------------------------------------------------------
namespace Relativity.DataExchange.NUnit.Integration
{
	using System;
	using System.Collections;
	using System.Drawing;
	using System.Drawing.Imaging;
	using System.Reflection;
	using global::NUnit.Framework;

	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.Media;
	using Relativity.DataExchange.TestFramework;
	using Relativity.Testing.Identification;

	[TestFixture]
	[TestLevel.L1]
	[Feature.DataTransfer.RelativityDesktopClient.Export]
	public class ImageConverterServiceTests
	{
		private const string MultiTiffImageFileName = "MultiTiffImage";

		/// <summary>
		/// This const specify flag/propertytag where we get info on compression method.
		/// The returned value can be one of: .
		/// 1: no compression
		/// 2: CCITT Group 3
		/// 3: Facsimile - compatible CCITT Group 3
		/// 4: CCITT Group 4(T.6)
		/// 5: LZW.
		/// </summary>
		private const int PropertyTagCompression = 0x0103;

		private const int CcIttCompressionValue = 4;
		private const int LzwCompressionValue = 5;

		private readonly ImageConverterService _subjectUnderTest = new ImageConverterService(new FileSystemWrap());
		private PathWrap _pathWrap;
		private FileWrap _fieWrap;

		public static IEnumerable ImageFileNames
		{
			get
			{
				yield return new TestCaseData(CcIttCompressionValue, new string[] { "AZIPPER_0011374.TIF", "AZIPPER_0011374_01.TIF" }).WithId("B426B1EC-E8D4-48A5-9F25-BC2D75ACBC1F");
				yield return new TestCaseData(LzwCompressionValue, new string[] { "AZIPPER_0011111.jpg" }).WithId("5ACC0658-A56B-4A4B-9138-FAD34F70F740");
			}
		}

		[OneTimeSetUp]
		public void Init()
		{
			this._pathWrap = new PathWrap();
			this._fieWrap = new FileWrap(this._pathWrap);
		}

		[TestCaseSource(nameof(ImageFileNames))]
		[System.Diagnostics.CodeAnalysis.SuppressMessage(
			"Microsoft.Design",
			"CA1062:Validate arguments of public methods",
			Justification = "This is just a test case source data.")]
		public void ItShouldConvertImageToMultiTiffImageCompressedByCcITt(int expectedCompressionType, string[] imageSource)
		{
			// This test checks if images are being converted correctly to multi-tiff image with using CCITT Group 4 compression method or LZW.
			System.Collections.Generic.List<string> imageList = new global::System.Collections.Generic.List<string>();
			foreach (var image in imageSource)
			{
				imageList.Add(ResourceFileHelper.GetResourceFilePath("Media", image));
			}

			// Arrange
			string path = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

			string fullFilePath = this._pathWrap.Combine(path, GenerateTiffFileName(MultiTiffImageFileName));

			// Act
			this._subjectUnderTest.ConvertTiffsToMultiPageTiff(imageList, fullFilePath);

			// Assert
			using (Image outputTiffImage = Image.FromFile(fullFilePath))
			{
				var compressionPropTag = outputTiffImage.GetPropertyItem(PropertyTagCompression);
				var pageCount = outputTiffImage.GetFrameCount(FrameDimension.Page);

				// We expect that image is compressed with CCITT Group 4 method or LZW
				Assert.That(compressionPropTag.Value[0], Is.EqualTo(expectedCompressionType));

				// We expect that image is consists of two pages
				Assert.That(pageCount, Is.EqualTo(imageList.Count));
			}

			this._fieWrap.Delete(fullFilePath);
		}

		private static string GenerateTiffFileName(string name)
		{
			return $"{name}_{Guid.NewGuid().ToString()}.tif";
		}
	}
}
