using System.Collections.Generic;
using System.Linq;
using kCura.WinEDDS.Core.Import.Errors;
using NUnit.Framework;

namespace kCura.WinEDDS.Core.NUnit.Import.Errors
{
	[TestFixture]
	public class ErrorMessageContainerTests
	{
		private ErrorMessageContainer _instance;

		[SetUp]
		public void SetUp()
		{
			_instance = new ErrorMessageContainer();
		}

		[Test]
		public void ItShouldReturnSortedErrors()
		{
			var lineErrors = GetTestData();

			// ACT
			foreach (var lineError in lineErrors)
			{
				_instance.WriteError(lineError);
			}

			var result = _instance.GetAllErrors();

			// ASSERT
			CollectionAssert.AreEqual(result.Select(x => x.LineNumber), lineErrors.OrderBy(x => x.LineNumber).Select(x => x.LineNumber));
		}

		private static List<LineError> GetTestData()
		{
			var clientError1 = new LineError
			{
				LineNumber = 1,
				ErrorType = ErrorType.client
			};
			var clientError2 = new LineError
			{
				LineNumber = 5,
				ErrorType = ErrorType.client
			};
			var clientError3 = new LineError
			{
				LineNumber = 9,
				ErrorType = ErrorType.client
			};
			var serverError1 = new LineError
			{
				LineNumber = 2,
				ErrorType = ErrorType.server
			};
			var serverError2 = new LineError
			{
				LineNumber = 4,
				ErrorType = ErrorType.server
			};
			var serverError3 = new LineError
			{
				LineNumber = 8,
				ErrorType = ErrorType.server
			};

			var lineErrors = new List<LineError>
			{
				clientError1,
				clientError2,
				clientError3,
				serverError1,
				serverError2,
				serverError3
			};
			return lineErrors;
		}
	}
}