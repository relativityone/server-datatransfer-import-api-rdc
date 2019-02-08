using System;
using System.Text;
using System.Threading;
using kCura.WinEDDS.Core.Export.VolumeManagerV2.Download.EncodingHelpers;
using Moq;
using NUnit.Framework;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Export.VolumeManagerV2.Download.EncodingHelpers
{
	[TestFixture]
	public class FileEncodingConverterTests
	{
		private FileEncodingConverter _instance;

		private Mock<IFileEncodingRewrite> _encodingRewrite;
		private Mock<IFileHelper> _fileHelper;

		[SetUp]
		public void SetUp()
		{
			_encodingRewrite = new Mock<IFileEncodingRewrite>();
			_fileHelper = new Mock<IFileHelper>();
			_instance = new FileEncodingConverter(_encodingRewrite.Object, _fileHelper.Object, new NullLogger());
		}

		[Test]
		public void ItShouldConvertFile()
		{
			const string filePath = "file_path";
			Encoding sourceEncoding = Encoding.Unicode;
			Encoding destinationEncoding = Encoding.UTF8;

			//ACT
			_instance.Convert(filePath, sourceEncoding, destinationEncoding, CancellationToken.None);

			//ASSERT
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

			//ACT & ASSERT
			Assert.Throws<Exception>(() => _instance.Convert(filePath, sourceEncoding, destinationEncoding, CancellationToken.None));

			_fileHelper.Verify(x => x.Delete(GetTempFilePath(filePath)));
		}

		private static string GetTempFilePath(string filePath)
		{
			return $"{filePath}.tmp";
		}
	}
}