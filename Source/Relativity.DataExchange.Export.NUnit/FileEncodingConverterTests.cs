// -----------------------------------------------------------------------------------------------------
// <copyright file="FileEncodingConverterTests.cs" company="Relativity ODA LLC">
//   © Relativity All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------------------------------------

namespace Relativity.DataExchange.Export.NUnit
{
	using System;
	using System.Text;
	using System.Threading;

	using global::NUnit.Framework;

	using Moq;

	using Relativity.DataExchange.Export.VolumeManagerV2.Download.EncodingHelpers;
	using Relativity.DataExchange.Io;
	using Relativity.DataExchange.TestFramework;

	[TestFixture]
	public class FileEncodingConverterTests
	{
		private FileEncodingConverter _instance;

		private Mock<IFileEncodingRewrite> _encodingRewrite;
		private Mock<IFile> _fileHelper;

		[SetUp]
		public void SetUp()
		{
			this._encodingRewrite = new Mock<IFileEncodingRewrite>();
			this._fileHelper = new Mock<IFile>();
			this._instance = new FileEncodingConverter(this._encodingRewrite.Object, this._fileHelper.Object, new TestNullLogger());
		}

		[Test]
		public void ItShouldConvertFile()
		{
			const string filePath = "file_path";
			Encoding sourceEncoding = Encoding.Unicode;
			Encoding destinationEncoding = Encoding.UTF8;

			// ACT
			this._instance.Convert(filePath, sourceEncoding, destinationEncoding, CancellationToken.None);

			// ASSERT
			this._encodingRewrite.Verify(x => x.RewriteFile(filePath, GetTempFilePath(filePath), sourceEncoding, destinationEncoding, CancellationToken.None));
			this._fileHelper.Verify(x => x.Delete(filePath));
			this._fileHelper.Verify(x => x.Move(GetTempFilePath(filePath), filePath));
		}

		[Test]
		public void ItShouldCleanAfterExceptionIsThrown()
		{
			const string filePath = "file_path";
			Encoding sourceEncoding = Encoding.Unicode;
			Encoding destinationEncoding = Encoding.UTF8;

			this._encodingRewrite.Setup(x => x.RewriteFile(filePath, GetTempFilePath(filePath), sourceEncoding, destinationEncoding, CancellationToken.None)).Throws<Exception>();
			this._fileHelper.Setup(x => x.Exists(GetTempFilePath(filePath))).Returns(true);

			// ACT & ASSERT
			Assert.Throws<Exception>(() => this._instance.Convert(filePath, sourceEncoding, destinationEncoding, CancellationToken.None));

			this._fileHelper.Verify(x => x.Delete(GetTempFilePath(filePath)));
		}

		private static string GetTempFilePath(string filePath)
		{
			return $"{filePath}.tmp";
		}
	}
}