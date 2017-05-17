using kCura.WinEDDS.Core.Import.Errors;
using Moq;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Import.Errors
{
	[TestFixture]
	public class ClientErrorLineContainerTests
	{
		private ClientErrorLineContainer _instance;

		[SetUp]
		public void SetUp()
		{
			var pathHelper = new Mock<IPathHelper>();

			_instance = new ClientErrorLineContainer(pathHelper.Object);
		}

		[Test]
		[TestCase(new[] {1, 2, 3}, new[] {1, 2, 3})]
		[TestCase(new[] {3, 2, 1}, new[] {1, 2, 3})]
		[TestCase(new[] {2, 2, 3, 1, 3, 1, 2, 1, 1}, new[] {1, 2, 3})]
		public void ItShouldReturnDistinctOrderedErrorLines(int[] input, int[] expectedResult)
		{
			// ACT
			foreach (var i in input)
			{
				_instance.WriteError(new LineError
				{
					LineNumber = i,
					ErrorType = ErrorType.client
				});
			}

			var result = _instance.GetClientErrorLines();

			// ASSERT
			CollectionAssert.AreEqual(result, expectedResult);
		}

		[Test]
		public void ItShouldReturnOnlyClientErrors()
		{
			var clientError = new LineError
			{
				LineNumber = 1,
				ErrorType = ErrorType.client
			};

			var serverError = new LineError
			{
				LineNumber = 2,
				ErrorType = ErrorType.server
			};

			// ACT
			_instance.WriteError(clientError);
			_instance.WriteError(serverError);

			var result = _instance.GetClientErrorLines();

			// ASSERT
			CollectionAssert.AreEqual(result, new[] {clientError.LineNumber});
		}
	}
}