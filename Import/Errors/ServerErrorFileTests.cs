using System;
using System.IO;
using kCura.Utility;
using kCura.WinEDDS.Core.Import.Errors;
using kCura.WinEDDS.Core.Import.Status;
using Moq;
using NUnit.Framework;
using File = System.IO.File;

namespace kCura.WinEDDS.Core.NUnit.Import.Errors
{
	[TestFixture]
	public class ServerErrorFileTests
	{
		private string _filePath;

		private ServerErrorFile _instance;

		private Mock<IErrorContainer> _errorContainer;
		private Mock<IImportStatusManager> _importStatusManager;

		[SetUp]
		public void SetUp()
		{
			_filePath = Path.GetTempFileName();

			_errorContainer = new Mock<IErrorContainer>();
			_importStatusManager = new Mock<IImportStatusManager>();

			_instance = new ServerErrorFile(_errorContainer.Object, _importStatusManager.Object);
		}

		[TearDown]
		public void TearDown()
		{
			if (File.Exists(_filePath))
			{
				File.Delete(_filePath);
			}
		}

		[Test]
		public void ItShouldRaiseFatalErrorForEmptyReader()
		{
			// ACT
			_instance.HandleServerErrors(null);

			// ASSERT
			_importStatusManager.Verify(x => x.RaiseFatalErrorImportEvent(_instance, "There was an error while attempting to retrieve the errors from the server.", -1,
				It.IsAny<Exception>()), Times.Once);
		}

		[Test]
		public void ItShouldWriteError()
		{
			int lineNumber = 556;
			string message = "msg_887";
			string identifier = "";
			
			File.WriteAllText(_filePath, $"{lineNumber},{message},{identifier}{Environment.NewLine}");

			// ACT
			_instance.HandleServerErrors(new GenericCsvReader(_filePath, false));

			// ASSERT
			_errorContainer.Verify(x => x.WriteError(It.Is<LineError>(y => y.LineNumber == lineNumber && y.Message == message)), Times.Once);
		}
	}
}