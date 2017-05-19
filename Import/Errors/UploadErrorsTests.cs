using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Core.Import;
using kCura.WinEDDS.Core.Import.Errors;
using Moq;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Import.Errors
{
	[TestFixture]
	public class UploadErrorsTests
	{
		private UploadErrors _instance;

		private Mock<IErrorContainer> _errorContainer;

		[SetUp]
		public void SetUp()
		{
			_errorContainer = new Mock<IErrorContainer>();

			_instance = new UploadErrors(_errorContainer.Object);
		}

		[Test]
		public void ItShouldAddLineErrorForEachUploadError()
		{
			var uploadResults = new Dictionary<FileMetadata, UploadResult>
			{
				{new FileMetadata {FileGuid = "1"}, new UploadResult {Success = false}},
				{new FileMetadata {FileGuid = "2"}, new UploadResult {Success = true}},
				{new FileMetadata {FileGuid = "3"}, new UploadResult {Success = false}}
			};

			// ACT
			_instance.HandleUploadErrors(uploadResults);

			// ASSERT
			_errorContainer.Verify(x => x.WriteError(It.IsAny<LineError>()), Times.Exactly(uploadResults.Count(x => !x.Value.Success)));
		}

		[Test]
		public void ItShouldAddLineErrorWithProperData()
		{
			var uploadResults = new Dictionary<FileMetadata, UploadResult>
			{
				{
					new FileMetadata
					{
						FileGuid = "1",
						LineNumber = 123
					},
					new UploadResult
					{
						Success = false,
						ErrorMessage = "error_message_992"
					}
				}
			};

			// ACT
			_instance.HandleUploadErrors(uploadResults);

			// ASSERT
			_errorContainer.Verify(x => x.WriteError(It.Is<LineError>(y => VerifyLineError(y, uploadResults.First()))));
		}

		private bool VerifyLineError(LineError lineError, KeyValuePair<FileMetadata, UploadResult> uploadResult)
		{
			return lineError.LineNumber == uploadResult.Key.LineNumber
					&& lineError.ErrorType == ErrorType.client
					&& lineError.Message == uploadResult.Value.ErrorMessage;
		}
	}
}