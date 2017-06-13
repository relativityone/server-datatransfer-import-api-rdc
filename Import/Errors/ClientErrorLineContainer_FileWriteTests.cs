using System.Collections.Generic;
using System.IO;
using System.Linq;
using kCura.WinEDDS.Core.Import.Errors;
using Moq;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Import.Errors
{
	[TestFixture]
	public class ClientErrorLineContainer_FileWriteTests
	{
		private const string _FILE_NAME = "temp_file.txt";

		private ClientErrorLineContainer _instance;

		[SetUp]
		public void SetUp()
		{
			var pathHelper = new Mock<IPathHelper>();
			pathHelper.Setup(x => x.GetTempFileName()).Returns(_FILE_NAME);

			_instance = new ClientErrorLineContainer(pathHelper.Object);
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
			var errors = new List<int>
			{
				1,
				2,
				5,
				3,
				4,
				4
			};

			foreach (var error in errors)
			{
				_instance.WriteError(new LineError
				{
					LineNumber = error
				});
			}

			// ACT
			_instance.WriteErrorsToTempFile();

			// ASSERT
			Assert.That(File.Exists(_FILE_NAME));
			var expected = errors.Distinct().OrderBy(x => x);
			var actual = File.ReadAllLines(_FILE_NAME).Select(int.Parse).ToList();
			CollectionAssert.AreEqual(expected, actual);
		}

		[Test]
		public void ItShouldReturnEmptyStringWhenWritingEmptyErrorList()
		{
			// ACT
			var filePath = _instance.WriteErrorsToTempFile();

			// ASSERT
			Assert.That(filePath, Is.Empty);
		}
	}
}