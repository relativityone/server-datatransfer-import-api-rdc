using System.Collections.Generic;
using System.IO;
using System.Linq;
using kCura.WinEDDS.Core.Import.Errors;
using Moq;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Import.Errors
{
	[TestFixture]
	public class AllErrorsContainer_FileWriteTests
	{
		private const string _FILE_NAME = "temp_file.txt";

		private AllErrorsContainer _instance;

		[SetUp]
		public void SetUp()
		{
			var pathHelper = new Mock<IPathHelper>();
			pathHelper.Setup(x => x.GetTempFileName()).Returns(_FILE_NAME);

			_instance = new AllErrorsContainer(pathHelper.Object);
		}

		[TearDown]
		public void TearDown()
		{
			if (File.Exists(_FILE_NAME))
			{
				File.Delete(_FILE_NAME);
			}
		}

		[Test]
		public void ItShouldWriteErrorToTempFile()
		{
			var lineErrors = new List<LineError>
			{
				new LineError
				{
					LineNumber = 2,
					Message = "msg_2",
					ErrorType = ErrorType.server
				},
				new LineError
				{
					LineNumber = 1,
					Message = "msg_1",
					ErrorType = ErrorType.client
				}
			};

			foreach (var lineError in lineErrors)
			{
				_instance.WriteError(lineError);
			}

			// ACT
			_instance.WriteErrorsToTempFile();

			// ASSERT
			Assert.That(File.Exists(_FILE_NAME));

			var expected = new []
			{
				"\"1\",\"msg_1\",\"\",\"client\"",
				"\"2\",\"msg_2\",\"\",\"server\""
			};
			var actual = File.ReadAllLines(_FILE_NAME).ToList();
			CollectionAssert.AreEqual(expected, actual);
		}
	}
}