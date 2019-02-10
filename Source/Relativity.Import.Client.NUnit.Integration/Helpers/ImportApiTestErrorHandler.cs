using System.Collections.Generic;
using System.Linq;
using kCura.Relativity.DataReaderClient;
using kCura.Relativity.ImportAPI.IntegrationTests.Tests;

namespace kCura.Relativity.ImportAPI.IntegrationTests.Helpers
{
	internal static class ImportApiTestErrorHandler
	{
		public static void Subscribe(IImportNotifier importApiJob)
		{
			importApiJob.OnComplete += JobOnOnComplete;
			importApiJob.OnFatalException += JobOnOnFatalException;
		}

		private static void JobOnOnComplete(JobReport jobreport)
		{
			if (jobreport.FatalException != null)
			{
				throw jobreport.FatalException;
			}

			if (jobreport.ErrorRowCount > 0)
			{
				IEnumerable<string> errors = jobreport.ErrorRows.Select(x => $"{x.Identifier} - {x.Message}");
				throw new ImportApiTestException(string.Join("\n", errors));
			}
		}

		private static void JobOnOnFatalException(JobReport jobreport)
		{
			throw jobreport.FatalException;
		}
	}
}
