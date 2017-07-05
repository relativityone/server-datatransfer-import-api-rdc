
using kCura.WinEDDS.Core.Import;
using kCura.WinEDDS.Core.Import.Errors;
using kCura.WinEDDS.Core.Import.Status;
using Moq;
using Relativity.Logging;

namespace kCura.WinEDDS.Core.NUnit.Helpers
{
	public static class ImportExceptionHandlerExecFactory
	{
		public static ImportExceptionHandlerExec Create(IImportMetadata importMetadata = null)
		{
			if (importMetadata == null)
			{
				importMetadata = new Mock<IImportMetadata>().Object;
			}
			return new ImportExceptionHandlerExec(new Mock<IImportStatusManager>().Object, importMetadata, new Mock<IErrorContainer>().Object,
				new Mock<ILog>().Object);
		}
	}
}
