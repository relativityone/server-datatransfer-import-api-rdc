// -----------------------------------------------------------------------------------------------------
// <copyright file="FileEncodingConverterTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.Export.NUnit
{
    using System;
    using System.Text;
    using System.Threading;

    using global::NUnit.Framework;

    using Moq;

	using Relativity.Export.VolumeManagerV2.Download.EncodingHelpers;
	using Relativity.Import.Export.Io;
    using Relativity.Logging;

    [TestFixture]
	public class FileEncodingConverterTests
	{
		private FileEncodingConverter _instance;

		private Mock<IFileEncodingRewrite> _encodingRewrite;
		private Mock<IFile> _fileHelper;

		[SetUp]
		public void SetUp()
		{
			_encodingRewrite = new Mock<IFileEncodingRewrite>();
			_fileHelper = new Mock<IFile>();
			_instance = new FileEncodingConverter(_encodingRewrite.Object, _fileHelper.Object, new NullLogger());
		}

		[Test]
		public void ItShouldConvertFile()
		{
			const string filePath = "file_path";
			Encoding sourceEncoding = Encoding.Unicode;
			Encoding destinationEncoding = Encoding.UTF8;

			// ACT
			_instance.Convert(filePath, sourceEncoding, destinationEncoding, CancellationToken.None);

			// ASSERT
			_encodingRewrite.Verify(x => x.RewriteFile(filePath, GetTempFilePath(filePath), sourceEncoding, destinationEncoding, CancellationToken.None));
			_fileHelper.Verify(x => x.Delete(filePath));
			_fileHelper.Verify(x => x.Move(GetTempFilePath(filePath), filePath));
		}

		[Test]
		public void ItShouldCleanAfterExceptionIsThrown()
		{
			const string filePath = "file_path";
			Encoding sourceEncoding = Encoding.Unicode;
			Encoding destinationEncoding = Encoding.UTF8;

			_encodingRewrite.Setup(x => x.RewriteFile(filePath, GetTempFilePath(filePath), sourceEncoding, destinationEncoding, CancellationToken.None)).Throws<Exception>();
			_fileHelper.Setup(x => x.Exists(GetTempFilePath(filePath))).Returns(true);

			// ACT & ASSERT
			Assert.Throws<Exception>(() => _instance.Convert(filePath, sourceEncoding, destinationEncoding, CancellationToken.None));

			_fileHelper.Verify(x => x.Delete(GetTempFilePath(filePath)));
		}

		private static string GetTempFilePath(string filePath)
		{
			return $"{filePath}.tmp";
		}
	}
}